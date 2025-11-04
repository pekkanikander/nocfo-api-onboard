// Account Type Definitions for NOCFO API
// Based on OpenAPI schema analysis

/**
 * Account types in the Finnish accounting system
 * These represent the chart of accounts structure
 */
export type AccountType =
  // Assets (Vastaavaa)
  | "ASS"           // General assets
  | "ASS_DEP"       // Depreciable assets (Poistokelpoinen omaisuus)
  | "ASS_VAT"       // VAT receivables (Arvonlisäverosaatava)
  | "ASS_REC"       // Trade receivables (Siirtosaamiset)
  | "ASS_PAY"       // Bank account / Cash (Pankkitili / käteisvarat)
  | "ASS_DUE"       // Sales receivables (Myyntisaatavat)

  // Liabilities (Vastattavaa)
  | "LIA"           // General liabilities
  | "LIA_EQU"       // Equity (Oma pääoma)
  | "LIA_PRE"       // Previous periods' profit (Edellisten tilikausien voitto)
  | "LIA_DUE"       // Trade payables (Ostovelat)
  | "LIA_DEB"       // Debts (Velat)
  | "LIA_ACC"       // Accrued liabilities (Siirtovelat)
  | "LIA_VAT"       // VAT liability (Arvonlisäverovelka)

  // Revenue (Tulot)
  | "REV"           // General revenue
  | "REV_SAL"       // Sales revenue (Liikevaihtotulot / myynti)
  | "REV_NO"        // Tax-free revenue (Verottomat tulot)

  // Expenses (Menot)
  | "EXP"           // General expenses
  | "EXP_DEP"       // Depreciation (Poistot)
  | "EXP_NO"        // Non-deductible expenses (Vähennyskelvottomat menot)
  | "EXP_50"        // Half-deductible expenses (Puoliksi vähennyskelpoiset menot)
  | "EXP_TAX"       // Tax account (Verotili)
  | "EXP_TAX_PRE";  // Prepaid taxes (Ennakkoverot)

/**
 * VAT codes for different types of transactions
 */
export type VatCode =
  | 1   // Domestic taxable sales (Kotimaan verollinen myynti)
  | 2   // Domestic taxable purchases (Kotimaan verollinen osto)
  | 3   // Tax-free (Veroton)
  | 4   // Zero-rate sales (Nollaverokannan myynti)
  | 5   // Goods sales to EU (Tavaroiden myynti EU-maihin)
  | 6   // Service sales to EU (Palveluiden myynti EU-maihin)
  | 7   // Goods purchases from EU (Tavaroiden ostot EU-maista)
  | 8   // Service purchases from EU (Palveluiden ostot EU-maista)
  | 9   // Goods import from outside EU (Tavaroiden maahantuonti EU:n ulkopuolelta)
  | 10  // Construction service sales (Rakennuspalveluiden myynti)
  | 11  // Construction service purchases (Rakennuspalveluiden osto)
  | 12  // Service sales outside EU (Palvelumyynnit EU:n ulkopuolelle)
  | 13  // Goods sales outside EU (Tavaroiden myynti EU:n ulkopuolelle)
  | 14; // Service purchases outside EU (Palveluostot EU:n ulkopuolelta)

/**
 * VAT rate labels for different tax categories
 */
export type VatRateLabel =
  | "standard"    // Standard rate (typically 25.5% in Finland)
  | "reduced_a"   // Reduced rate A (typically 14%)
  | "reduced_b"   // Reduced rate B (typically 10%)
  | "zero";       // Zero rate (0%)

/**
 * Main Account interface representing a chart of accounts entry
 */
export interface Account {
  // Core identifiers
  id: number;
  number: string;                    // Account number (e.g., "1000")
  padded_number: number;             // Numeric version for sorting

  // Names and descriptions
  name: string;                      // Account name
  name_translations: Record<string, string>; // Multi-language support
  header_path: string[];             // Hierarchical path (e.g., ["Assets", "Current Assets"])
  description?: string | null;       // Optional description

  // Account classification
  type: AccountType;                 // Account type from enum

  // VAT defaults
  default_vat_code: VatCode;         // Default VAT code for this account
  default_vat_rate: number;          // Default VAT rate (read-only)
  default_vat_rate_label: VatRateLabel; // Default VAT rate category

  // Balance information
  opening_balance: number;           // Opening balance for the period
  balance: number;                   // Current balance (read-only)

  // UI and usage flags
  is_shown: boolean;                 // Whether to show in UI
  is_used: boolean;                  // Whether account has transactions

  // Timestamps
  created_at: string;                // ISO date-time string
  updated_at: string;                // ISO date-time string
}

/**
 * Account list response wrapper
 */
export interface AccountList {
  id: number;
  number: string;
  name: string;
  type: AccountType;
  balance: number;
  is_shown: boolean;
  is_used: boolean;
  created_at: string;
  updated_at: string;
}

/**
 * Paginated account list response
 */
export interface PaginatedAccountList {
  count: number;                     // Total number of accounts
  next?: string | null;              // Next page URL
  previous?: string | null;          // Previous page URL
  results: AccountList[];            // Account list for current page
}
