// Minimal Business Type Definitions for NOCFO API
// Only what we need: ID info for display + slug for account fetching

/**
 * Minimal business info - just what we need for display and navigation
 */
export interface Business {
  id: number;                       // Internal NOCFO ID
  slug: string;                     // URL-friendly business identifier (needed for API calls)
  name: string;                     // Business name (for display)
  business_id: string;              // Official business identifier (Y-tunnus, for display)
  form: string;                     // Business form (e.g., "FI_YHD", for display)
}

/**
 * Business list item (same as Business for now - we might not need the distinction)
 */
export interface BusinessListItem extends Business {}

/**
 * Paginated business list response
 */
export interface PaginatedBusinessList {
  count: number;                    // Total number of businesses
  next?: string | null;             // Next page URL
  previous?: string | null;          // Previous page URL
  results: BusinessListItem[];       // Business list for current page
}
