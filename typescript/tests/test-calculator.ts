// Test the Accounting Calculator with Properly Balanced Data
// This version demonstrates the fundamental accounting equation working correctly
import { AccountingCalculator } from '../src/accounting-calculator.js';
import { Account, AccountType } from '../types/index.js';

// Create sample accounts for testing with properly balanced data
// The key insight: Assets = Liabilities + Equity must balance
const sampleAccounts: Account[] = [
  // Assets (Debit balances - increase with debits)
  {
    id: 1001,
    number: "1000",
    padded_number: 1000,
    name: "Bank Account",
    name_translations: { "en": "Bank Account" },
    header_path: ["Assets", "Current Assets"],
    description: "Main business bank account",
    type: "ASS_PAY",
    default_vat_code: 3,
    default_vat_rate: 0.0,
    default_vat_rate_label: "zero",
    opening_balance: 10000.00,
    balance: 15000.00, // Debit balance (asset)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },
  {
    id: 1002,
    number: "1200",
    padded_number: 1200,
    name: "Accounts Receivable",
    name_translations: { "en": "Accounts Receivable" },
    header_path: ["Assets", "Current Assets"],
    description: "Money owed by customers",
    type: "ASS_DUE",
    default_vat_code: 1,
    default_vat_rate: 24.0,
    default_vat_rate_label: "standard",
    opening_balance: 5000.00,
    balance: 8000.00, // Debit balance (asset)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },
  {
    id: 1003,
    number: "1500",
    padded_number: 1500,
    name: "Equipment",
    name_translations: { "en": "Equipment" },
    header_path: ["Assets", "Fixed Assets"],
    description: "Office equipment and furniture",
    type: "ASS_DEP",
    default_vat_code: 2,
    default_vat_rate: 24.0,
    default_vat_rate_label: "standard",
    opening_balance: 20000.00,
    balance: 18000.00, // Debit balance (asset, depreciated)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },

  // Liabilities (Credit balances - increase with credits)
  {
    id: 2001,
    number: "2000",
    padded_number: 2000,
    name: "Accounts Payable",
    name_translations: { "en": "Accounts Payable" },
    header_path: ["Liabilities", "Current Liabilities"],
    description: "Money owed to suppliers",
    type: "LIA_DUE",
    default_vat_code: 2,
    default_vat_rate: 24.0,
    default_vat_rate_label: "standard",
    opening_balance: 3000.00,
    balance: 5000.00, // Credit balance (liability)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },
  {
    id: 2002,
    number: "2500",
    padded_number: 2500,
    name: "VAT Liability",
    name_translations: { "en": "VAT Liability" },
    header_path: ["Liabilities", "Current Liabilities"],
    description: "VAT owed to tax authorities",
    type: "LIA_VAT",
    default_vat_code: 1,
    default_vat_rate: 24.0,
    default_vat_rate_label: "standard",
    opening_balance: 1000.00,
    balance: 1500.00, // Credit balance (liability)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },

  // Equity (Credit balances - increase with credits)
  // This needs to balance: Assets (41000) = Liabilities (6500) + Equity (34500)
  {
    id: 3001,
    number: "3000",
    padded_number: 3000,
    name: "Owner's Equity",
    name_translations: { "en": "Owner's Equity" },
    header_path: ["Equity"],
    description: "Owner's investment in the business",
    type: "LIA_EQU",
    default_vat_code: 3,
    default_vat_rate: 0.0,
    default_vat_rate_label: "zero",
    opening_balance: 25000.00,
    balance: 34500.00, // Credit balance (equity) - adjusted to balance equation
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },

  // Revenue and Expense accounts with zero balances (closed to equity)
  {
    id: 4001,
    number: "4000",
    padded_number: 4000,
    name: "Sales Revenue",
    name_translations: { "en": "Sales Revenue" },
    header_path: ["Revenue"],
    description: "Income from sales (closed to equity)",
    type: "REV_SAL",
    default_vat_code: 1,
    default_vat_rate: 24.0,
    default_vat_rate_label: "standard",
    opening_balance: 0.00,
    balance: 0.00, // Zero balance (closed to equity)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  },
  {
    id: 5001,
    number: "5000",
    padded_number: 5000,
    name: "Cost of Goods Sold",
    name_translations: { "en": "Cost of Goods Sold" },
    header_path: ["Expenses"],
    description: "Direct costs of producing goods (closed to equity)",
    type: "EXP",
    default_vat_code: 2,
    default_vat_rate: 24.0,
    default_vat_rate_label: "standard",
    opening_balance: 0.00,
    balance: 0.00, // Zero balance (closed to equity)
    is_shown: true,
    is_used: true,
    created_at: "2024-01-01T00:00:00Z",
    updated_at: "2024-01-15T12:30:00Z"
  }
];

// Test the calculator with balanced data
function testAccountingCalculatorBalanced() {
  console.log("🧮 Testing Accounting Calculator with Balanced Data...\n");

  // Test individual calculations
  console.log("📊 Individual Calculations:");
  console.log(`Total Assets: ${AccountingCalculator.calculateTotalAssets(sampleAccounts).toFixed(2)}`);
  console.log(`Total Liabilities: ${AccountingCalculator.calculateTotalLiabilities(sampleAccounts).toFixed(2)}`);
  console.log(`Total Equity: ${AccountingCalculator.calculateTotalEquity(sampleAccounts).toFixed(2)}`);
  console.log(`Total Revenue: ${AccountingCalculator.calculateTotalRevenue(sampleAccounts).toFixed(2)}`);
  console.log(`Total Expenses: ${AccountingCalculator.calculateTotalExpenses(sampleAccounts).toFixed(2)}`);
  console.log(`Net Income: ${AccountingCalculator.calculateNetIncome(sampleAccounts).toFixed(2)}`);
  console.log(`Working Capital: ${AccountingCalculator.calculateWorkingCapital(sampleAccounts).toFixed(2)}`);

  console.log("\n" + "=".repeat(50));

  // Test the fundamental accounting equation
  console.log("\n🔍 Fundamental Accounting Equation Check:");
  const equation = AccountingCalculator.verifyAccountingEquation(sampleAccounts);
  console.log(`Assets (${equation.assets.toFixed(2)}) = Liabilities (${equation.liabilities.toFixed(2)}) + Equity (${equation.equity.toFixed(2)})`);
  console.log(`Difference: ${equation.difference.toFixed(2)}`);
  console.log(`Balanced: ${equation.isBalanced ? '✅ YES' : '❌ NO'}`);

  console.log("\n" + "=".repeat(50));

  // Generate balance sheet summary
  console.log("\n📋 Balance Sheet Summary:");
  console.log(AccountingCalculator.generateBalanceSheetSummary(sampleAccounts));

  console.log("\n" + "=".repeat(50));

  // Test grouping by type
  console.log("\n🏷️  Accounts Grouped by Type:");
  const grouped = AccountingCalculator.groupAccountsByType(sampleAccounts);
  Object.entries(grouped).forEach(([type, accounts]) => {
    console.log(`${type}: ${accounts.length} accounts, Total: ${accounts.reduce((sum, acc) => sum + acc.balance, 0).toFixed(2)}`);
  });

  console.log("\n✅ Accounting Calculator test completed!");

  // Explain the accounting principle
  console.log("\n" + "=".repeat(50));
  console.log("\n📚 ACCOUNTING PRINCIPLE EXPLANATION:");
  console.log("In double-entry bookkeeping:");
  console.log("- Assets = Liabilities + Equity (the fundamental equation)");
  console.log("- Assets: 41000.00 (Bank + Receivables + Equipment)");
  console.log("- Liabilities: 6500.00 (Payables + VAT)");
  console.log("- Equity: 34500.00 (Owner's Equity)");
  console.log("- 41000 = 6500 + 34500 ✅ BALANCED!");
  console.log("- Revenue and Expense accounts are temporary accounts");
  console.log("- They get closed to equity at the end of each period");
  console.log("- This is why they show zero balances");
}

// Run the test
testAccountingCalculatorBalanced();
