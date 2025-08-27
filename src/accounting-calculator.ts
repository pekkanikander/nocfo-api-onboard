// Simple Accounting Balance Calculator
// Demonstrates double-entry bookkeeping logic

import { Account, AccountType } from '../types/index.js';

/**
 * Simple accounting balance calculator
 * Demonstrates basic double-entry bookkeeping principles
 */
export class AccountingCalculator {

  /**
   * Calculate total assets (sum of all ASS* accounts)
   */
  static calculateTotalAssets(accounts: Account[]): number {
    return accounts
      .filter(account => account.type.startsWith('ASS'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total liabilities (sum of all LIA* accounts EXCEPT equity accounts)
   */
  static calculateTotalLiabilities(accounts: Account[]): number {
    return accounts
      .filter(account => account.type.startsWith('LIA') &&
                        account.type !== 'LIA_EQU' &&
                        account.type !== 'LIA_PRE')
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total equity (sum of LIA_EQU and LIA_PRE accounts)
   */
  static calculateTotalEquity(accounts: Account[]): number {
    return accounts
      .filter(account => account.type === 'LIA_EQU' || account.type === 'LIA_PRE')
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total revenue (sum of all REV* accounts)
   */
  static calculateTotalRevenue(accounts: Account[]): number {
    return accounts
      .filter(account => account.type.startsWith('REV'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate total expenses (sum of all EXP* accounts)
   */
  static calculateTotalExpenses(accounts: Account[]): number {
    return accounts
      .filter(account => account.type.startsWith('EXP'))
      .reduce((sum, account) => sum + account.balance, 0);
  }

  /**
   * Calculate net income (Revenue - Expenses)
   */
  static calculateNetIncome(accounts: Account[]): number {
    const revenue = this.calculateTotalRevenue(accounts);
    const expenses = this.calculateTotalExpenses(accounts);
    return revenue - expenses;
  }

  /**
   * Verify the fundamental accounting equation: Assets = Liabilities + Equity
   */
  static verifyAccountingEquation(accounts: Account[]): {
    assets: number;
    liabilities: number;
    equity: number;
    difference: number;
    isBalanced: boolean;
  } {
    const assets = this.calculateTotalAssets(accounts);
    const liabilities = this.calculateTotalLiabilities(accounts);
    const equity = this.calculateTotalEquity(accounts);

    const difference = assets - (liabilities + equity);
    const isBalanced = Math.abs(difference) < 0.01; // Allow for small rounding errors

    return {
      assets,
      liabilities,
      equity,
      difference,
      isBalanced
    };
  }

  /**
   * Get accounts grouped by type for analysis
   */
  static groupAccountsByType(accounts: Account[]): Record<AccountType, Account[]> {
    const grouped: Partial<Record<AccountType, Account[]>> = {};

    accounts.forEach(account => {
      if (!grouped[account.type]) {
        grouped[account.type] = [];
      }
      grouped[account.type]!.push(account);
    });

    return grouped as Record<AccountType, Account[]>;
  }

  /**
   * Calculate working capital (Current Assets - Current Liabilities)
   * Note: This is a simplified version - in practice you'd need to identify
   * which accounts are "current" vs "long-term"
   */
  static calculateWorkingCapital(accounts: Account[]): number {
    // For this example, we'll consider ASS_PAY and ASS_DUE as current assets
    // and LIA_DUE as current liabilities
    const currentAssets = accounts
      .filter(account => account.type === 'ASS_PAY' || account.type === 'ASS_DUE')
      .reduce((sum, account) => sum + account.balance, 0);

    const currentLiabilities = accounts
      .filter(account => account.type === 'LIA_DUE')
      .reduce((sum, account) => sum + account.balance, 0);

    return currentAssets - currentLiabilities;
  }

  /**
   * Generate a simple balance sheet summary
   */
  static generateBalanceSheetSummary(accounts: Account[]): string {
    const equation = this.verifyAccountingEquation(accounts);
    const workingCapital = this.calculateWorkingCapital(accounts);

    return `
=== BALANCE SHEET SUMMARY ===
Assets: ${equation.assets.toFixed(2)}
Liabilities: ${equation.liabilities.toFixed(2)}
Equity: ${equation.equity.toFixed(2)}
Working Capital: ${workingCapital.toFixed(2)}

Fundamental Equation Check:
Assets = Liabilities + Equity
${equation.assets.toFixed(2)} = ${(equation.liabilities + equation.equity).toFixed(2)}
Difference: ${equation.difference.toFixed(2)}
Balanced: ${equation.isBalanced ? '✅ YES' : '❌ NO'}
    `.trim();
  }
}
