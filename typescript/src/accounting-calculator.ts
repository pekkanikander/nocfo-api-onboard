// Stream-Ready Accounting Calculator
// Works with any stream implementation: arrays, async generators, Effect streams, etc.

import { Account } from '../types/index.js';
import { Stream } from './stream-abstraction.js';

/**
 * Stream-ready accounting calculator that works with any stream implementation
 * Truly type-agnostic: works with arrays, async generators, Effect streams, etc.
 */
export class AccountingCalculator {
  /**
   * Calculate total assets from any stream of accounts
   * Works with arrays, streams, async generators, Effect streams, etc.
   */
  static async calculateTotalAssets(accounts: Stream<Account>): Promise<number> {
    return accounts
      .filter(account => account.type.startsWith('ASS'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total liabilities from any stream of accounts
   * Excludes equity accounts (LIA_EQU, LIA_PRE)
   */
  static async calculateTotalLiabilities(accounts: Stream<Account>): Promise<number> {
    return accounts
      .filter(account => account.type.startsWith('LIA') &&
                        account.type !== 'LIA_EQU' &&
                        account.type !== 'LIA_PRE')
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total equity from any stream of accounts
   * Includes equity accounts (LIA_EQU, LIA_PRE)
   */
  static async calculateTotalEquity(accounts: Stream<Account>): Promise<number> {
    return accounts
      .filter(account => account.type.startsWith('LIA_EQU') || account.type.startsWith('LIA_PRE'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Verify the fundamental accounting equation: Assets = Liabilities + Equity
   * Works with any stream of accounts
   */
  static async verifyAccountingEquation(accounts: Stream<Account>): Promise<{
    assets: number;
    liabilities: number;
    equity: number;
    difference: number;
    isBalanced: boolean;
  }> {
    const [assets, liabilities, equity] = await Promise.all([
      this.calculateTotalAssets(accounts),
      this.calculateTotalLiabilities(accounts),
      this.calculateTotalEquity(accounts)
    ]);

    const difference = assets - (liabilities + equity);
    const isBalanced = Math.abs(difference) < 0.01; // Allow for small rounding errors

    return { assets, liabilities, equity, difference, isBalanced };
  }

  /**
   * Calculate total revenue from any stream of accounts
   */
  static async calculateTotalRevenue(accounts: Stream<Account>): Promise<number> {
    return accounts
      .filter(account => account.type.startsWith('REV'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total expenses from any stream of accounts
   */
  static async calculateTotalExpenses(accounts: Stream<Account>): Promise<number> {
    return accounts
      .filter(account => account.type.startsWith('EXP'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate net income: Revenue - Expenses
   */
  static async calculateNetIncome(accounts: Stream<Account>): Promise<number> {
    const [revenue, expenses] = await Promise.all([
      this.calculateTotalRevenue(accounts),
      this.calculateTotalExpenses(accounts)
    ]);
    return revenue - expenses;
  }

  /**
   * Get account summary by type from any stream
   * Returns a map of account types to their total balances
   */
  static async getAccountSummaryByType(accounts: Stream<Account>): Promise<Map<string, number>> {
    const summary = new Map<string, number>();

    await accounts.forEach(account => {
      const currentTotal = summary.get(account.type) || 0;
      summary.set(account.type, currentTotal + account.balance);
    });

    return summary;
  }

  /**
   * Filter accounts by type from any stream
   * Returns a new stream with accounts matching the type pattern
   */
  static filterAccountsByType(accounts: Stream<Account>, typePattern: string): Stream<Account> {
    return accounts.filter(account => account.type.startsWith(typePattern));
  }

  /**
   * Count accounts by type from any stream
   * Returns a map of account types to their counts
   */
  static async countAccountsByType(accounts: Stream<Account>): Promise<Map<string, number>> {
    const counts = new Map<string, number>();

    await accounts.forEach(account => {
      const currentCount = counts.get(account.type) || 0;
      counts.set(account.type, currentCount + 1);
    });

    return counts;
  }

  /**
   * Get all accounts as an array (useful for debugging/testing)
   */
  static async getAccountsArray(accounts: Stream<Account>): Promise<Account[]> {
    return accounts.toArray();
  }

  /**
   * Process accounts in chunks (useful for large datasets)
   */
  static async processAccountsInChunks<T>(
    accounts: Stream<Account>,
    chunkSize: number,
    processor: (chunk: Account[]) => Promise<T>
  ): Promise<T[]> {
    const allAccounts = await accounts.toArray();
    const results: T[] = [];

    for (let i = 0; i < allAccounts.length; i += chunkSize) {
      const chunk = allAccounts.slice(i, i + chunkSize);
      const result = await processor(chunk);
      results.push(result);
    }

    return results;
  }
}
