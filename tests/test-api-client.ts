// Test our simplified NOCFO API client with stream abstractions
import { NocfoApiClient } from '../src/nocfo-api-client.js';

// Configuration - we'll get these from environment variables later
const config = {
  baseUrl: 'https://api-tst.nocfo.io',
  token: process.env.NOCFO_API_TOKEN || ''
};

// Simple test function
async function testApiClient() {
  console.log("🧪 Testing Simplified NOCFO API Client...\n");

  // Check if we have a token
  if (!config.token) {
    console.log("❌ No NOCFO_API_TOKEN found in environment variables");
    console.log("Please set NOCFO_API_TOKEN before running this test");
    return;
  }

  console.log("✅ Token found, proceeding with API test");
  console.log(`🌐 Base URL: ${config.baseUrl}\n`);

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
