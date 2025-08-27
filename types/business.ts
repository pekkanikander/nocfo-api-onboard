// Business Type Definitions for NOCFO API
// Based on actual API response analysis

/**
 * VAT rate configuration for a business
 */
export interface VatRateConfig {
  effective_from: string;           // Date when this rate configuration becomes effective
  default_vat_rate_label: string;   // Default VAT rate category
  vat_rate_options: Array<{
    label: string;                  // Rate category (standard, reduced_a, reduced_b, zero)
    rate: number;                   // Actual percentage rate
  }>;
  default_vat_rate: number;         // Default VAT rate percentage
}

/**
 * Country-specific configuration for a business
 */
export interface CountryConfig {
  currency: string;                 // Currency code (e.g., "EUR")
  default_language: string;         // Default language (e.g., "fi")
  default_vat_posting_method: string; // VAT posting method (e.g., "NET")
  vat_configs: VatRateConfig[];    // VAT rate configurations over time
  vat_codes: number[];             // Available VAT codes for this country
  feature: {
    salaxy: boolean;                // Salaxy integration enabled
    enable_banking: boolean;        // Banking features enabled
    verohallinto: boolean;          // Finnish Tax Administration integration
  };
}

/**
 * Business identifier (e.g., Y-tunnus)
 */
export interface BusinessIdentifier {
  type: string;                     // Identifier type (e.g., "y_tunnus")
  value: string;                    // Identifier value (e.g., "2999322-9")
}

/**
 * Main Business interface representing a business entity in NOCFO
 */
export interface Business {
  // Core identifiers
  id: number;                       // Internal NOCFO ID
  slug: string;                     // URL-friendly business identifier
  name: string;                     // Business name

  // Timestamps
  created_at: string;               // ISO date-time when business was created
  updated_at: string;               // ISO date-time when business was last updated

  // Business details
  logo?: string | null;             // Logo URL if available
  business_id: string;              // Official business identifier

  // Country and VAT configuration
  country_config: CountryConfig;    // Country-specific settings

  // Accounting periods
  period_id: number;                // Current accounting period ID
  vat_period_id?: number | null;    // Current VAT period ID

  // VAT settings
  vat_posting_method: string;       // VAT posting method (e.g., "NET")
  default_vat_period: number;       // Default VAT reporting period

  // Invoicing defaults
  invoicing_default_penalty_interest: number;      // Default penalty interest rate
  invoicing_default_payment_condition_days: number; // Default payment terms in days

  // Business features
  has_accrual_based_entries_for_invoicing: boolean; // Accrual accounting enabled
  form: string;                     // Business form (e.g., "FI_YHD" for Finnish association)

  // Owner information
  owner_name: string;               // Owner's name
  owner_email: string;              // Owner's email

  // Business history
  has_history_before_nocfo: boolean; // Whether business existed before NOCFO

  // Contact information
  contact_phone?: string | null;    // Business phone number
  invoicing_email?: string | null;  // Invoicing email address

  // Banking information
  invoicing_iban?: string | null;   // IBAN for invoicing
  invoicing_bic?: string | null;    // BIC/SWIFT code
  invoicing_tax_code?: string | null; // Tax code for invoicing

  // Address information
  has_business_address: boolean;    // Whether business address is set
  business_street?: string | null;  // Business street address
  business_city?: string | null;    // Business city
  business_postal_code?: string | null; // Business postal code
  business_country?: string | null; // Business country

  // Invoicing address
  invoicing_street?: string | null; // Invoicing street address
  invoicing_city?: string | null;   // Invoicing city
  invoicing_postal_code?: string | null; // Invoicing postal code
  invoicing_country?: string | null; // Invoicing country
  invoicing_contact?: string | null; // Invoicing contact person

  // Account mappings (important for accounting operations)
  account_yle_tax_id?: number | null;           // YLE tax account ID
  account_tax_deferrals_id?: number | null;     // Tax deferrals account ID
  account_income_tax_rec_id?: number | null;    // Income tax receivables account ID
  account_income_tax_lia_id?: number | null;    // Income tax liabilities account ID
  account_previous_profit_id?: number | null;   // Previous profit account ID
  account_vat_receivables_id?: number | null;   // VAT receivables account ID
  account_vat_liabilities_id?: number | null;   // VAT liabilities account ID
  account_trade_receivables_id?: number | null; // Trade receivables account ID
  account_payables_id?: number | null;          // Trade payables account ID

  // Subscription and billing
  subscription_plan: string;        // Subscription plan (e.g., "FREE")
  subscription_source: string;      // Subscription source (e.g., "STRIPE")
  stripe_customer_id?: string | null; // Stripe customer ID
  stripe_subscription_id?: string | null; // Stripe subscription ID

  // Feature flags
  can_invoice: boolean;             // Whether business can send invoices
  einvoicing_enabled: boolean;      // E-invoicing enabled
  einvoicing_address?: string | null; // E-invoicing address
  einvoicing_operator?: string | null; // E-invoicing operator
  holvi_bookkeeping_api_enabled: boolean; // Holvi API integration
  holvi_nocfo_connection_enabled: boolean; // Holvi-NOCFO connection
  salaxy_enabled: boolean;          // Salaxy integration enabled
  salaxy_account_id?: string | null; // Salaxy account ID

  // Trial and demo information
  trial_progress_percentage: number; // Trial progress percentage
  trial_days_left: number;          // Days left in trial
  is_trialing: boolean;             // Whether business is in trial
  demo_days_left: number;           // Days left in demo (if applicable)

  // Tax administration integration
  verohallinto_latest_reporter_full_name?: string | null; // Latest tax reporter name
  verohallinto_latest_reporter_phone_number?: string | null; // Latest tax reporter phone
  is_eligible_for_einvoicing: boolean; // Whether eligible for e-invoicing

  // Business features
  has_tags: boolean;                // Whether business uses tags
  identifiers: BusinessIdentifier[]; // Business identifiers (Y-tunnus, etc.)

  // Usage statistics
  invoices_sent_this_month: number; // Invoices sent this month
  monthly_invoice_limit?: number | null; // Monthly invoice limit
}

/**
 * Simplified business list item (for when we only need basic info)
 */
export interface BusinessListItem {
  id: number;
  slug: string;
  name: string;
  business_id: string;
  form: string;
  owner_email: string;
  subscription_plan: string;
  is_trialing: boolean;
}

/**
 * Paginated business list response
 */
export interface PaginatedBusinessList {
  count: number;                    // Total number of businesses
  next?: string | null;             // Next page URL
  previous?: string | null;          // Previous page URL
  results: BusinessListItem[];       // Business list for current page
}
