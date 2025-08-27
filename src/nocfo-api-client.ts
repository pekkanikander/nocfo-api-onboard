// Basic NOCFO API Client
// Step 1: Simple client that can fetch businesses

import { Business, BusinessListItem, PaginatedBusinessList } from '../types/index.js';

/**
 * Basic configuration for the NOCFO API client
 */
export interface NocfoApiConfig {
  baseUrl: string;
  token: string;
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

      const data: PaginatedBusinessList = await response.json();

      // The API returns a paginated structure with results array
      if (data.results && Array.isArray(data.results)) {
        // For now, we'll fetch full details for each business
        // Later we can optimize this with batch operations
        const fullBusinesses = await Promise.all(
          data.results.map(businessItem => this.getBusiness(businessItem.slug))
        );
        return fullBusinesses;
      } else {
        // Fallback: if the API returns businesses directly
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

      const business: Business = await response.json();
      return business;
    } catch (error) {
      console.error(`Error fetching business ${slug}:`, error);
      throw error;
    }
  }

  /**
   * Get just the business list without full details
   * This is more efficient when you only need basic info
   */
  async getBusinessList(): Promise<BusinessListItem[]> {
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

      const data: PaginatedBusinessList = await response.json();
      return data.results || [];
    } catch (error) {
      console.error('Error fetching business list:', error);
      throw error;
    }
  }
}
