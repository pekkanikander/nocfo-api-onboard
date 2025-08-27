// Test our TypeScript interfaces and types
import { Account, AccountType, VatCode, VatRateLabel } from '../types/index.js';

// Sample account data that matches our interface
const sampleAccount: Account = {
  id: 1001,
  number: "1000",
  padded_number: 1000,
  name: "Bank Account",
  name_translations: {
    "fi": "Pankkitili",
    "en": "Bank Account"
  },
  header_path: ["Assets", "Current Assets", "Cash and Cash Equivalents"],
  description: "Main business bank account",
  type: "ASS_PAY", // Bank account type
  default_vat_code: 3, // Tax-free
  default_vat_rate: 0.0,
  default_vat_rate_label: "zero",
  opening_balance: 10000.00,
  balance: 12500.50,
  is_shown: true,
  is_used: true,
  created_at: "2024-01-01T00:00:00Z",
  updated_at: "2024-01-15T12:30:00Z"
};

// Test function to verify our interface
function testAccountInterface() {
  console.log("🧪 Testing Account interface...");

  // Test basic properties
  console.log(`Account: ${sampleAccount.number} - ${sampleAccount.name}`);
  console.log(`Type: ${sampleAccount.type}`);
  console.log(`Balance: ${sampleAccount.balance}`);
  console.log(`VAT Code: ${sampleAccount.default_vat_code}`);
  console.log(`VAT Rate: ${sampleAccount.default_vat_rate}`);

  // Test type safety
  const validTypes: AccountType[] = ["ASS", "ASS_PAY", "LIA", "REV", "EXP"];
  console.log("Valid account types:", validTypes);

  const validVatCodes: VatCode[] = [1, 3, 10, 14];
  console.log("Valid VAT codes:", validVatCodes);

  const validVatLabels: VatRateLabel[] = ["standard", "reduced_a", "reduced_b", "zero"];
  console.log("Valid VAT rate labels:", validVatLabels);

  console.log("✅ Account interface test passed!");
}

// Run the test
testAccountInterface();
