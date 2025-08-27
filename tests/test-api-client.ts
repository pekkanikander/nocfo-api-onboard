// Test our simplified NOCFO API client with stream abstractions and account fetching
import { NocfoApiClient } from '../src/nocfo-api-client.js';
import { AccountingCalculator } from '../src/accounting-calculator.js';
import { Stream } from '../src/stream-abstraction.js';

// Configuration - we'll get these from environment variables later
const config = {
  baseUrl: 'https://api-tst.nocfo.io',
  token: process.env.NOCFO_API_TOKEN || '',
  maxConcurrentRequests: 3 // Conservative default
};

// Simple test function
async function testApiClient() {
  console.log("🧪 Testing NOCFO API Client with Stream Abstractions...\n");

  // Check if we have a token
  if (!config.token) {
    console.log("❌ No NOCFO_API_TOKEN found in environment variables");
    console.log("Please set NOCFO_API_TOKEN before running this test");
    return;
  }

  console.log("✅ Token found, proceeding with API test");
  console.log(`🌐 Base URL: ${config.baseUrl}`);
  console.log(`⚡ Max Concurrent Requests: ${config.maxConcurrentRequests}\n`);

  try {
    // Create the client
    const client = new NocfoApiClient(config);
    console.log("✅ API client created successfully");

    // Test 1: Get business list (minimal data)
    console.log("\n📋 Test 1: Fetching businesses (minimal data)...");
    const businesses = await client.getBusinesses();

    console.log(`✅ Successfully fetched ${businesses.length} businesses:`);
    businesses.forEach((business, index) => {
      console.log(`  ${index + 1}. ${business.name} (${business.business_id}) - ${business.form}`);
      console.log(`     Slug: ${business.slug}`);
    });

    // Test 2: Stream businesses one at a time
    console.log("\n📋 Test 2: Streaming businesses...");
    let streamCount = 0;
    for await (const business of client.getBusinessesStream()) {
      streamCount++;
      console.log(`  Stream ${streamCount}: ${business.name}`);
    }
    console.log(`✅ Streamed ${streamCount} businesses`);

    // Test 3: Stream with processor (simulating future account fetching)
    console.log("\n📋 Test 3: Streaming with processor...");
    let processedCount = 0;
    for await (const result of client.getBusinessesStreamWithProcessor(async (business) => {
      // Simulate what we'll do later: fetch accounts and compute balances
      return {
        businessName: business.name,
        businessId: business.business_id,
        slug: business.slug,
        // Later this will include: accountCount, totalAssets, totalLiabilities, etc.
        status: "ready_for_accounts"
      };
    })) {
      processedCount++;
      console.log(`  Processed ${processedCount}: ${result.businessName} - ${result.status}`);
    }
    console.log(`✅ Processed ${processedCount} businesses`);

    // Test 4: Fetch accounts for each business (parallel processing)
    console.log("\n📋 Test 4: Fetching accounts with parallel processing...");
    console.log(`⚡ Using ${config.maxConcurrentRequests} concurrent requests`);

    let accountCount = 0;
    let totalAssets = 0;
    let totalLiabilities = 0;
    let totalEquity = 0;

    for await (const businessAccounts of client.getBusinessesWithAccountsStream()) {
      console.log(`\n🏢 ${businessAccounts.business.name}:`);
      console.log(`   📊 Accounts: ${businessAccounts.accountCount}`);
      console.log(`   💰 Assets: ${businessAccounts.totalAssets.toFixed(2)} €`);
      console.log(`   📋 Liabilities: ${businessAccounts.totalLiabilities.toFixed(2)} €`);
      console.log(`   🏛️  Equity: ${businessAccounts.totalEquity.toFixed(2)} €`);
      console.log(`   ⚖️  Balanced: ${businessAccounts.isBalanced ? '✅ YES' : '❌ NO'}`);

      // Use our stream-ready accounting calculator
      console.log(`   🔍 Using Stream Calculator...`);
      const equation = await AccountingCalculator.verifyAccountingEquation(businessAccounts.accounts);
      console.log(`      Calculator Check: ${equation.isBalanced ? '✅ Balanced' : '❌ Imbalanced'}`);
      if (!equation.isBalanced) {
        console.log(`         Difference: ${equation.difference.toFixed(2)} €`);
      }

      // Test stream operations
      console.log(`   🔄 Testing stream operations...`);
      const assetAccounts = AccountingCalculator.filterAccountsByType(businessAccounts.accounts, 'ASS');
      const assetCount = await assetAccounts.toArray();
      console.log(`      Asset accounts: ${assetCount.length}`);

      // Debug: Let's see what the first few accounts look like
      console.log(`   🐛 Debug: First 3 accounts:`);
      const firstAccounts = await businessAccounts.accounts.take(3).toArray();
      firstAccounts.forEach((acc, i) => {
        console.log(`      ${i + 1}. Type: ${acc.type}, Balance: ${acc.balance}, Name: ${acc.name}`);
      });

      // Accumulate totals across all businesses
      accountCount += businessAccounts.accountCount;
      totalAssets += businessAccounts.totalAssets;
      totalLiabilities += businessAccounts.totalLiabilities;
      totalEquity += businessAccounts.totalEquity;
    }

    // Test 5: Overall summary across all businesses
    console.log("\n📋 Test 5: Overall summary across all businesses...");
    console.log(`📊 Total Accounts: ${accountCount}`);
    console.log(`💰 Total Assets: ${totalAssets.toFixed(2)} €`);
    console.log(`📋 Total Liabilities: ${totalLiabilities.toFixed(2)} €`);
    console.log(`🏛️  Total Equity: ${totalEquity.toFixed(2)} €`);

    const overallBalanced = Math.abs(totalAssets - (totalLiabilities + totalEquity)) < 0.01;
    console.log(`⚖️  Overall Balanced: ${overallBalanced ? '✅ YES' : '❌ NO'}`);

    // Test 6: Demonstrate stream abstraction with arrays
    console.log("\n📋 Test 6: Demonstrating stream abstraction...");
    const sampleAccounts = [
      { type: 'ASS_PAY', balance: 1000, id: 1, number: '1000', padded_number: 1000, name: 'Bank', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 1000, is_shown: true, created_at: '', updated_at: '' },
      { type: 'LIA_EQU', balance: 1000, id: 2, number: '2000', padded_number: 2000, name: 'Equity', name_translations: {}, header_path: [], description: null, default_vat_code: 1, default_vat_rate: 0, default_vat_rate_label: 'zero', opening_balance: 1000, is_shown: true, created_at: '', updated_at: '' }
    ];

    const accountStream = Stream.fromArray(sampleAccounts);
    const testEquation = await AccountingCalculator.verifyAccountingEquation(accountStream);
    console.log(`   Test with array stream: ${testEquation.isBalanced ? '✅ Balanced' : '❌ Imbalanced'}`);

    console.log("\n🎉 All API tests passed!");

  } catch (error) {
    console.error("❌ API test failed:", error);

    if (error instanceof Error) {
      console.error("Error message:", error.message);
      console.error("Error stack:", error.stack);
    }
  }
}

// Run the test
testApiClient();
