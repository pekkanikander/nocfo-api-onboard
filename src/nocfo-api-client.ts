// Basic NOCFO API Client
// Step 1: Simple client that can fetch businesses

/**
 * Basic configuration for the NOCFO API client
 */
export interface NocfoApiConfig {
  baseUrl: string;
  token: string;
}

/**
 * Business entity from the NOCFO API
 * We'll expand this as we learn more about the API
 */
export interface Business {
  id: number;
  slug: string;
  name: string;
  // Add more fields as we discover them
}

/**
 * Basic NOCFO API client
 * Currently only handles business operations
 */
export class NocfoApiClient {
  private config: NocfoApiConfig;

  constructor(config: NocfoApiConfig) {
    this.config = config;
  }

  /**
   * Fetch all businesses for the authenticated user
   * This is the first step - we need businesses to get their slugs
   */
  async getBusinesses(): Promise<Business[]> {
    const url = `${this.config.baseUrl}/v1/business/`;

    try {
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Authorization': `Token ${this.config.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();

      // For now, we'll assume the API returns a structure like:
      // { results: Business[], count: number, next: string | null, previous: string | null }
      // We'll refine this as we see the actual response
      if (data.results && Array.isArray(data.results)) {
        return data.results;
      } else {
        // If the API returns businesses directly
        return Array.isArray(data) ? data : [];
      }
    } catch (error) {
      console.error('Error fetching businesses:', error);
      throw error;
    }
  }

  /**
   * Get a specific business by slug
   * This will be useful for getting business details
   */
  async getBusiness(slug: string): Promise<Business> {
    const url = `${this.config.baseUrl}/v1/business/${slug}/`;

    try {
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Authorization': `Token ${this.config.token}`,
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const business = await response.json();
      return business;
    } catch (error) {
      console.error(`Error fetching business ${slug}:`, error);
      throw error;
    }
  }
}
