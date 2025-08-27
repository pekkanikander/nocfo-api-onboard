// Test our basic NOCFO API client
import { NocfoApiClient } from '../src/nocfo-api-client.js';

// Configuration - we'll get these from environment variables later
const config = {
  baseUrl: 'https://api-tst.nocfo.io',
  token: process.env.NOCFO_API_TOKEN || ''
};

// Simple test function
async function testApiClient() {
  console.log("🧪 Testing NOCFO API Client...\n");

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

    // Test 1: Get all businesses
    console.log("\n📋 Test 1: Fetching businesses...");
    const businesses = await client.getBusinesses();

    console.log(`✅ Successfully fetched ${businesses.length} businesses:`);
    businesses.forEach((business, index) => {
      console.log(`  ${index + 1}. ${business.name} (slug: ${business.slug})`);
    });

    // Test 2: Get details for the first business
    if (businesses.length > 0) {
      console.log("\n📋 Test 2: Fetching details for first business...");
      const firstBusiness = await client.getBusiness(businesses[0].slug);
      console.log(`✅ Business details:`, JSON.stringify(firstBusiness, null, 2));
    }

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
