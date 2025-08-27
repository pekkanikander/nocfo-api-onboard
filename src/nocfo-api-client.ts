// Simple NOCFO API Client with Stream Abstractions
// Minimal approach: only what we need for display + slug for account fetching

import { Business, BusinessListItem, PaginatedBusinessList } from '../types/index.js';

/**
 * Basic configuration for the NOCFO API client
 */
export interface NocfoApiConfig {
  baseUrl: string;
  token: string;
}

/**
 * Simple NOCFO API client
 * Focuses on minimal data + stream abstractions
 */
export class NocfoApiClient {
  private config: NocfoApiConfig;

  constructor(config: NocfoApiConfig) {
    this.config = config;
  }

  /**
   * Fetch business list - minimal info only
   * Returns just what we need: ID, name, slug, business_id, form
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

      // Extract only the fields we need from the API response
      return (data.results || []).map(item => ({
        id: item.id,
        slug: item.slug,
        name: item.name,
        business_id: item.business_id,
        form: item.form
      }));
    } catch (error) {
      console.error('Error fetching businesses:', error);
      throw error;
    }
  }

  /**
   * Stream businesses one at a time
   * Useful for processing large numbers of businesses
   */
  async *getBusinessesStream(): AsyncGenerator<Business> {
    const businesses = await this.getBusinesses();

    for (const business of businesses) {
      yield business;
    }
  }

  /**
   * Stream businesses with optional parallel processing
   * This is what we'll use for account fetching later
   */
  async *getBusinessesStreamWithProcessor<T>(
    processor: (business: Business) => Promise<T>
  ): AsyncGenerator<T> {
    const businesses = await this.getBusinesses();

    // For now, process sequentially
    // Later we can add parallel processing if needed
    for (const business of businesses) {
      const result = await processor(business);
      yield result;
    }
  }
}
