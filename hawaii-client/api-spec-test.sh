#!/usr/bin/env bash
set -euo pipefail

# ----------------------------------------------------------------------------
# api-spec-test.sh
#
# High-level "spec vs. server" drift signal for OpenAPI.
# Runs Schemathesis (breadth + fuzz) and Dredd (docs examples),
# then emits a one‑page Markdown summary with pass/fail totals and
# coarse issue categories (undocumented, schema/content-type, 5xx).
# ----------------------------------------------------------------------------

SCRIPT_DIR=$(cd "$(dirname "$0")" && pwd)
DEFAULT_SPEC="$SCRIPT_DIR/../api/openapi.json"
DEFAULT_BASE_URL="https://api-tst.nocfo.io"
DEFAULT_OUT_DIR="$SCRIPT_DIR/../api/reports/api-spec"

SPEC="$DEFAULT_SPEC"
BASE_URL="$DEFAULT_BASE_URL"
OUT_DIR="$DEFAULT_OUT_DIR"
MAX_EXAMPLES="100"
SKIP_ST=0
SKIP_DREDD=0
AUTH_TOKEN="${NOCFO_TOKEN}"
HEADERS=()

print_usage() {
  cat <<EOF
Usage: $(basename "$0") [options]

Options:
  -s, --spec FILE            OpenAPI spec file (default: $DEFAULT_SPEC)
  -b, --base-url URL         Base URL of the running API (default: $DEFAULT_BASE_URL)
  -o, --out DIR              Output directory for reports (default: $DEFAULT_OUT_DIR)
      --token TOKEN          Add Authorization: Token TOKEN to requests (default: \$NOCFO_TOKEN)
  -H, --header "K: V"        Add an extra HTTP header (repeatable)
      --skip-st              Skip Schemathesis
      --skip-dredd           Skip Dredd
  -h, --help                 Show this help

Examples:
  $(basename "\$0") -s ./api/openapi.json -b https://api-tst.nocfo.io \
    --token "\$NOCFO_TOKEN" -H "Accept-Language: en" -o ./api/reports/api-spec
EOF
}

# ---------- arg parsing ----------
while [[ $# -gt 0 ]]; do
  case "$1" in
    -s|--spec) SPEC="$2"; shift 2;;
    -b|--base-url) BASE_URL="$2"; shift 2;;
    -o|--out) OUT_DIR="$2"; shift 2;;
    --token) AUTH_TOKEN="$2"; shift 2;;
    -H|--header) HEADERS+=("$2"); shift 2;;
    --skip-st) SKIP_ST=1; shift;;
    --skip-dredd) SKIP_DREDD=1; shift;;
    -h|--help) print_usage; exit 0;;
    *) echo "[ERR] Unknown argument: $1" >&2; print_usage; exit 1;;
  esac
done

mkdir -p "$OUT_DIR/st"

command_exists() { command -v "$1" >/dev/null 2>&1; }

# Prefer installed CLI, fall back to uvx for Schemathesis
if command_exists schemathesis; then
  ST_CMD=(schemathesis)
elif command_exists st; then
  ST_CMD=(st)
elif command_exists uvx; then
  ST_CMD=(uvx schemathesis)
else
  ST_CMD=()
fi

# Prefer installed Dredd, fall back to npx
if command_exists dredd; then
  DREDD_CMD=(dredd)
elif command_exists npx; then
  DREDD_CMD=(npx dredd)
else
  DREDD_CMD=()
fi

# ---------- build header args ----------
ST_HEADER_ARGS=()
DREDD_HEADER_ARGS=()

if [[ -n "$AUTH_TOKEN" ]]; then
  ST_HEADER_ARGS+=(--header "Authorization: Token $AUTH_TOKEN")
  DREDD_HEADER_ARGS+=(--header "Authorization: Token $AUTH_TOKEN")
fi
if [[ ${#HEADERS[@]} -gt 0 ]]; then
  for h in "${HEADERS[@]}"; do
    ST_HEADER_ARGS+=(--header "$h")
    DREDD_HEADER_ARGS+=(--header "$h")
  done
fi

# ---------- run Schemathesis ----------
if [[ $SKIP_ST -eq 0 ]]; then
  if [[ ${#ST_CMD[@]} -eq 0 ]]; then
    echo "[WARN] Schemathesis CLI not found (schemathesis/st/uvx). Skipping."
  else
    echo "[INFO] Running Schemathesis against $BASE_URL with spec $SPEC"
    # See CLI docs: --report junit/har and --report-dir, --report-junit-path available in recent versions
    "${ST_CMD[@]}" run "$SPEC" \
      --url="$BASE_URL" \
      --exclude-checks negative_data_rejection \
      --mode positive \
      --exclude-method TRACE \
      --exclude-method OPTIONS \
      --checks all \
      --continue-on-failure \
      --workers auto \
      --report junit,har \
      --report-dir "$OUT_DIR/st" \
      --output-truncate true \
      "${ST_HEADER_ARGS[@]}"
  fi
fi

SCHEMATHESIS_XML="$(ls -t "$OUT_DIR"/st/junit*.xml 2>/dev/null | head -n1 || true)"
SCHEMATHESIS_HAR="$(ls -t "$OUT_DIR"/st/network*.har 2>/dev/null | head -n1 || true)"


# ---------- run Dredd ----------
DREDD_XML="$OUT_DIR/dredd.xml"
DREDD_LOG="$OUT_DIR/dredd.log"

if [[ $SKIP_DREDD -eq 0 ]]; then
  if [[ ${#DREDD_CMD[@]} -eq 0 ]]; then
    echo "[WARN] Dredd CLI not found (dredd/npx). Skipping."
  else
    echo "[INFO] Running Dredd (examples in spec)"
    "${DREDD_CMD[@]}" "$SPEC" "$BASE_URL" \
      --reporter xunit --output "$DREDD_XML" \
      "${DREDD_HEADER_ARGS[@]}" 2>&1 | tee "$DREDD_LOG" || true
  fi
fi

# ---------- summarize to Markdown ----------
SUMMARY_MD="$OUT_DIR/summary.md"
PYTHON_BIN="python3"
command_exists python3 || PYTHON_BIN="python"

$PYTHON_BIN - "$SCHEMATHESIS_XML" "$DREDD_XML" "$SPEC" "$BASE_URL" "$OUT_DIR" << PY  > "$SUMMARY_MD"
import sys, os, xml.etree.ElementTree as ET, datetime, json

st_xml, dredd_xml, spec, base_url, out_dir = sys.argv[1:]

class Totals:
    def __init__(self):
        self.tests = 0
        self.failures = 0
        self.errors = 0
        self.skipped = 0
        self.failed_cases = []  # (suite, name, message)
        self.categories = {
            'undocumented': 0,
            'schema_or_content': 0,
            'server_error_5xx': 0,
            'other': 0,
        }
    def add_suite(self, ts):
        self.tests += int(ts.get('tests', 0) or 0)
        self.failures += int(ts.get('failures', 0) or 0)
        self.errors += int(ts.get('errors', 0) or 0)
        self.skipped += int(ts.get('skipped', 0) or 0)

    def add_failure(self, suite, name, message, text):
        msg = (message or '') + ' ' + (text or '')
        low = msg.lower()
        if 'undocumented' in low:
            self.categories['undocumented'] += 1
        elif 'content-type' in low or 'content type' in low or 'schema' in low or 'validation' in low or 'invalid' in low:
            self.categories['schema_or_content'] += 1
        elif '5' in low and 'xx' in low or '500' in low or '502' in low or '503' in low:
            self.categories['server_error_5xx'] += 1
        else:
            self.categories['other'] += 1
        self.failed_cases.append((suite, name, msg.strip()))


def parse_junit(path):
    t = Totals()
    if not path or not os.path.isfile(path):
        return t
    try:
        tree = ET.parse(path)
        root = tree.getroot()
        suites = []
        if root.tag == 'testsuite':
            suites = [root]
        else:
            suites = list(root.iter('testsuite'))
        for ts in suites:
            t.add_suite(ts)
            for tc in ts.iter('testcase'):
                # failures
                for f in tc.findall('failure'):
                    t.add_failure(ts.get('name') or '', tc.get('name') or tc.get('classname') or '', f.get('message'), (f.text or ''))
                for f in tc.findall('error'):
                    t.add_failure(ts.get('name') or '', tc.get('name') or tc.get('classname') or '', f.get('message'), (f.text or ''))
        return t
    except Exception as e:
        # best-effort parsing
        return t

st = parse_junit(st_xml)
dredd = parse_junit(dredd_xml)

now = datetime.datetime.utcnow().strftime('%Y-%m-%d %H:%M UTC')

def section(title):
    print(f"\n## {title}\n")

print(f"# API Spec vs Server — Drift Overview\n")
print(f"**Spec:** {spec}\n\n**Base URL:** {base_url}\n\n**Generated:** {now}\n")

section("Schemathesis")
print(f"- Tests: {st.tests}, Failures: {st.failures}, Errors: {st.errors}, Skipped: {st.skipped}")
print("- Issue categories (approximate):")
for k,v in st.categories.items():
    print(f"  - {k.replace('_',' ').title()}: {v}")

section("Dredd")
print(f"- Tests: {dredd.tests}, Failures: {dredd.failures}, Errors: {dredd.errors}, Skipped: {dredd.skipped}")

# Combined signal
section("Combined signal")
failed_endpoints = set()
for (s,n,_) in st.failed_cases + dredd.failed_cases:
    key = (s or '') + '|' + (n or '')
    failed_endpoints.add(key)
print(f"- Unique failing cases (approx.): {len(failed_endpoints)}")

print("\n### Artifacts\n")
if os.path.isfile(st_xml):
    print(f"- Schemathesis JUnit: {st_xml}")
if os.path.isfile(dredd_xml):
    print(f"- Dredd xUnit: {dredd_xml}")
har = os.path.join(out_dir, 'st', 'network.har')
if os.path.isfile(har):
    print(f"- Schemathesis HAR: {har}")
log_st = os.path.join(out_dir, 'st', 'run.log')
if os.path.isfile(log_st):
    print(f"- Schemathesis log: {log_st}")
log_d = os.path.join(out_dir, 'dredd.log')
if os.path.isfile(log_d):
    print(f"- Dredd log: {log_d}")
PY

# ---------- final note ----------
echo ""
echo "[OK] Drift summary written to: $SUMMARY_MD"
echo ""
echo "Tip: To see live validation while you click around, try Prism:" \
     "prism proxy <spec> $BASE_URL --errors"
