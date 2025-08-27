// Simple NOCFO API Client with Stream Abstractions
// Minimal approach: only what we need for display + slug for account fetching

import { Business, BusinessListItem, PaginatedBusinessList } from '../types/index.js';
import { Account } from '../types/index.js';
import { Stream, Stream as StreamFactory } from './stream-abstraction.js';

/**
 * Basic configuration for the NOCFO API client
 */
export interface NocfoApiConfig {
  baseUrl: string;
  token: string;
  maxConcurrentRequests?: number; // Default: 3
}

/**
 * Account fetching result with business context
 */
export interface BusinessAccounts {
  business: Business;
  accounts: Stream<Account>;
  accountCount: number;
  totalAssets: number;
  totalLiabilities: number;
  totalEquity: number;
  isBalanced: boolean;
}

/**
 * Simple NOCFO API client
 * Focuses on minimal data + stream abstractions
 */
export class NocfoApiClient {
  private config: NocfoApiConfig;

  constructor(config: NocfoApiConfig) {
    this.config = {
      maxConcurrentRequests: 3, // Conservative default
      ...config
    };
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
   * Fetch accounts for a specific business
   * Returns a stream of accounts
   */
  async getAccounts(businessSlug: string): Promise<Stream<Account>> {
    const url = `${this.config.baseUrl}/v1/business/${businessSlug}/account/`;

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
      const accounts = data.results || [];

      // Return as a stream
      return StreamFactory.fromArray(accounts);
    } catch (error) {
      console.error(`Error fetching accounts for business ${businessSlug}:`, error);
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

  /**
   * Stream businesses with accounts - configurable parallelism
   * This is the main method for accounting operations
   */
  async *getBusinessesWithAccountsStream(
    maxConcurrent: number = this.config.maxConcurrentRequests!
  ): AsyncGenerator<BusinessAccounts> {
    const businesses = await this.getBusinesses();

    if (maxConcurrent <= 1) {
      // Sequential processing
      for (const business of businesses) {
        const accounts = await this.getAccounts(business.slug);
        const accountArray = await accounts.toArray();
        yield this.createBusinessAccounts(business, accounts, accountArray);
      }
    } else {
      // Parallel processing with controlled concurrency
      const chunks = this.chunkArray(businesses, maxConcurrent);

      for (const chunk of chunks) {
        const promises = chunk.map(async (business) => {
          const accounts = await this.getAccounts(business.slug);
          const accountArray = await accounts.toArray();
          return this.createBusinessAccounts(business, accounts, accountArray);
        });

        const results = await Promise.all(promises);
        for (const result of results) {
          yield result;
        }
      }
    }
  }

  /**
   * Helper: Create BusinessAccounts object with calculated totals
   */
  private createBusinessAccounts(
    business: Business,
    accounts: Stream<Account>,
    accountArray: Account[]
  ): BusinessAccounts {
    const totalAssets = accountArray
      .filter(acc => acc.type.startsWith('ASS'))
      .reduce((sum, acc) => sum + acc.balance, 0);

    const totalLiabilities = accountArray
      .filter(acc => acc.type.startsWith('LIA') &&
                    acc.type !== 'LIA_EQU' &&
                    acc.type !== 'LIA_PRE')
      .reduce((sum, acc) => sum + acc.balance, 0);

    const totalEquity = accountArray
      .filter(acc => acc.type.startsWith('LIA_EQU') || acc.type.startsWith('LIA_PRE'))
      .reduce((sum, acc) => sum + acc.balance, 0);

    const isBalanced = Math.abs(totalAssets - (totalLiabilities + totalEquity)) < 0.01;

    return {
      business,
      accounts,
      accountCount: accountArray.length,
      totalAssets,
      totalLiabilities,
      totalEquity,
      isBalanced
    };
  }

  /**
   * Helper: Split array into chunks for parallel processing
   */
  private chunkArray<T>(array: T[], chunkSize: number): T[][] {
    const chunks: T[][] = [];
    for (let i = 0; i < array.length; i += chunkSize) {
      chunks.push(array.slice(i, i + chunkSize));
    }
    return chunks;
  }
}
