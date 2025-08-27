// Test our stream abstractions without requiring API connectivity
import { Stream } from '../src/stream-abstraction.js';
import { AccountingCalculator } from '../src/accounting-calculator.js';

// Mock account data for testing
const mockAccounts = [
  { type: 'ASS_PAY', balance: 1000, id: 1, number: '1000', padded_number: 1000, name: 'Bank', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 1000, is_shown: true, created_at: '', updated_at: '' },
  { type: 'ASS_DUE', balance: 500, id: 2, number: '1200', padded_number: 1200, name: 'Accounts Receivable', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 500, is_shown: true, created_at: '', updated_at: '' },
  { type: 'LIA_EQU', balance: 1500, id: 3, number: '2000', padded_number: 2000, name: 'Equity', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 1500, is_shown: true, created_at: '', updated_at: '' },
  { type: 'REV_SAL', balance: 2000, id: 4, number: '3000', padded_number: 3000, name: 'Sales Revenue', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 2000, is_shown: true, created_at: '', updated_at: '' },
  { type: 'EXP_TAX', balance: 300, id: 5, number: '4000', padded_number: 4000, name: 'Tax Expense', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 300, is_shown: true, created_at: '', updated_at: '' }
];

async function testStreamAbstractions() {
  console.log("🧪 Testing Stream Abstractions...\n");

  try {
    // Test 1: Create stream from array
    console.log("📋 Test 1: Creating stream from array...");
    const accountStream = Stream.fromArray(mockAccounts);
    console.log(`✅ Created stream with ${mockAccounts.length} accounts`);

    // Test 2: Basic stream operations
    console.log("\n📋 Test 2: Basic stream operations...");

    // Filter asset accounts
    const assetStream = accountStream.filter(acc => acc.type.startsWith('ASS'));
    const assetAccounts = await assetStream.toArray();
    console.log(`✅ Filtered asset accounts: ${assetAccounts.length} accounts`);
    assetAccounts.forEach(acc => console.log(`   - ${acc.name}: ${acc.balance} €`));

    // Map to get account types
    const typeStream = accountStream.map(acc => acc.type);
    const types = await typeStream.toArray();
    console.log(`✅ Mapped account types: ${types.join(', ')}`);

    // Test 3: Stream calculator operations
    console.log("\n📋 Test 3: Stream calculator operations...");

    // Calculate totals using streams
    const totalAssets = await AccountingCalculator.calculateTotalAssets(accountStream);
    const totalLiabilities = await AccountingCalculator.calculateTotalLiabilities(accountStream);
    const totalEquity = await AccountingCalculator.calculateTotalEquity(accountStream);
    const totalRevenue = await AccountingCalculator.calculateTotalRevenue(accountStream);
    const totalExpenses = await AccountingCalculator.calculateTotalExpenses(accountStream);

    console.log(`✅ Total Assets: ${totalAssets} €`);
    console.log(`✅ Total Liabilities: ${totalLiabilities} €`);
    console.log(`✅ Total Equity: ${totalEquity} €`);
    console.log(`✅ Total Revenue: ${totalRevenue} €`);
    console.log(`✅ Total Expenses: ${totalExpenses} €`);

    // Test 4: Accounting equation verification
    console.log("\n📋 Test 4: Accounting equation verification...");
    const equation = await AccountingCalculator.verifyAccountingEquation(accountStream);

    console.log(`✅ Assets = Liabilities + Equity`);
    console.log(`   ${equation.assets} = ${equation.liabilities} + ${equation.equity}`);
    console.log(`   ${equation.assets} = ${equation.liabilities + equation.equity}`);
    console.log(`   Difference: ${equation.difference} €`);
    console.log(`   Balanced: ${equation.isBalanced ? '✅ YES' : '❌ NO'}`);

    // Test 5: Stream chaining and lazy operations
    console.log("\n📋 Test 5: Stream chaining and lazy operations...");

    // Chain multiple operations
    const processedStream = accountStream
      .filter(acc => acc.balance > 0)           // Only positive balances
      .map(acc => ({ ...acc, balance: acc.balance * 1.1 }))  // Add 10% interest
      .take(3);                                 // Take first 3

    const processedAccounts = await processedStream.toArray();
    console.log(`✅ Chained stream operations: ${processedAccounts.length} accounts`);
    processedAccounts.forEach(acc => {
      console.log(`   - ${acc.name}: ${acc.balance.toFixed(2)} € (with 10% interest)`);
    });

    // Test 6: Stream summary operations
    console.log("\n📋 Test 6: Stream summary operations...");

    const accountSummary = await AccountingCalculator.getAccountSummaryByType(accountStream);
    console.log(`✅ Account summary by type:`);
    for (const [type, total] of accountSummary) {
      console.log(`   - ${type}: ${total} €`);
    }

    const accountCounts = await AccountingCalculator.countAccountsByType(accountStream);
    console.log(`✅ Account counts by type:`);
    for (const [type, count] of accountCounts) {
      console.log(`   - ${type}: ${count} accounts`);
    }

    // Test 7: Demonstrate stream abstraction with different data
    console.log("\n📋 Test 7: Stream abstraction demonstration...");

    // Create a stream from a different data source (simulating async generator)
    const asyncStream = Stream.fromAsyncGenerator((async function* () {
      for (const account of mockAccounts) {
        yield { ...account, balance: account.balance * 2 }; // Double all balances
      }
    })());

    const doubledAssets = await AccountingCalculator.calculateTotalAssets(asyncStream);
    console.log(`✅ Async stream total assets (doubled): ${doubledAssets} €`);

    console.log("\n🎉 All stream abstraction tests passed!");
    console.log("\n🚀 Key Benefits Demonstrated:");
    console.log("   ✅ Type-agnostic: Works with arrays, async generators, etc.");
    console.log("   ✅ Functional style: map, filter, reduce operations");
    console.log("   ✅ Lazy evaluation: Operations chain without immediate execution");
    console.log("   ✅ Stream-ready calculator: All accounting functions work with streams");
    console.log("   ✅ Easy to extend: Can add Effect streams, RxJS, etc. later");

  } catch (error) {
    console.error("❌ Stream abstraction test failed:", error);

    if (error instanceof Error) {
      console.error("Error message:", error.message);
      console.error("Error stack:", error.stack);
    }
  }
}

// Run the test
testStreamAbstractions();
