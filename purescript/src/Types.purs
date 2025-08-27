module Types where

-- Business type definition for NOCFO API
-- Minimal business info: ID for display + slug for account fetching

data Business = Business Int String String String String

-- Field accessors
id :: Business -> Int
id (Business i _ _ _ _) = i

slug :: Business -> String
slug (Business _ s _ _ _) = s

name :: Business -> String
name (Business _ _ n _ _) = n

businessId :: Business -> String
businessId (Business _ _ _ bi _) = bi

form :: Business -> String
form (Business _ _ _ _ f) = f

-- Account types in the Finnish accounting system
-- These represent the chart of accounts structure
data AccountType
  -- Assets (Vastaavaa)
  = ASS           -- General assets
  | ASS_DEP       -- Depreciable assets (Poistokelpoinen omaisuus)
  | ASS_VAT       -- VAT receivables (Arvonlisäverosaatava)
  | ASS_REC       -- Trade receivables (Siirtosaamiset)
  | ASS_PAY       -- Bank account / Cash (Pankkitili / käteisvarat)
  | ASS_DUE       -- Sales receivables (Myyntisaatavat)

  -- Liabilities (Vastattavaa)
  | LIA           -- General liabilities
  | LIA_EQU       -- Equity (Oma pääoma)
  | LIA_PRE       -- Previous periods' profit (Edellisten tilikausien voitto)
  | LIA_DUE       -- Trade payables (Ostovelat)
  | LIA_DEB       -- Debts (Velat)
  | LIA_ACC       -- Accrued liabilities (Siirtovelat)
  | LIA_VAT       -- VAT liability (Arvonlisäverovelka)

  -- Revenue (Tulot)
  | REV           -- General revenue
  | REV_SAL       -- Sales revenue (Liikevaihtotulot / myynti)
  | REV_NO        -- Tax-free revenue (Verottomat tulot)

  -- Expenses (Menot)
  | EXP           -- General expenses
  | EXP_DEP       -- Depreciation (Poistot)
  | EXP_NO        -- Non-deductible expenses (Vähennyskelvottomat menot)
  | EXP_50        -- Half-deductible expenses (Puoliksi vähennyskelpoiset menot)
  | EXP_TAX       -- Tax account (Verotili)
  | EXP_TAX_PRE   -- Prepaid taxes (Ennakkoverot)

-- Main Account interface representing a chart of accounts entry
data Account = Account
  Int           -- id
  String        -- number (Account number e.g., "1000")
  Int           -- padded_number (Numeric version for sorting)
  String        -- name (Account name)
  AccountType   -- type (Account type from enum)
  Int           -- default_vat_code
  Number        -- default_vat_rate
  String        -- default_vat_rate_label
  Number        -- opening_balance
  Number        -- balance (Current balance)
  Boolean       -- is_shown (Whether to show in UI)
  Boolean       -- is_used (Whether account has transactions)
  String        -- created_at (ISO date-time string)
  String        -- updated_at (ISO date-time string)

-- Account field accessors
accountId :: Account -> Int
accountId (Account i _ _ _ _ _ _ _ _ _ _ _ _) = i

accountNumber :: Account -> String
accountNumber (Account _ n _ _ _ _ _ _ _ _ _ _ _) = n

accountPaddedNumber :: Account -> Int
accountPaddedNumber (Account _ _ pn _ _ _ _ _ _ _ _ _ _) = pn

accountName :: Account -> String
accountName (Account _ _ _ nm _ _ _ _ _ _ _ _ _) = nm

accountType :: Account -> AccountType
accountType (Account _ _ _ _ t _ _ _ _ _ _ _ _) = t

accountDefaultVatCode :: Account -> Int
accountDefaultVatCode (Account _ _ _ _ _ vc _ _ _ _ _ _ _) = vc

accountDefaultVatRate :: Account -> Number
accountDefaultVatRate (Account _ _ _ _ _ _ vr _ _ _ _ _ _) = vr

accountDefaultVatRateLabel :: Account -> String
accountDefaultVatRateLabel (Account _ _ _ _ _ vl _ _ _ _ _ _) = vl

accountOpeningBalance :: Account -> Number
accountOpeningBalance (Account _ _ _ _ _ _ _ ob _ _ _ _ _) = ob

accountBalance :: Account -> Number
accountBalance (Account _ _ _ _ _ _ _ _ b _ _ _ _) = b

accountIsShown :: Account -> Boolean
accountIsShown (Account _ _ _ _ _ _ _ _ _ is _ _ _) = is

accountIsUsed :: Account -> Boolean
accountIsUsed (Account _ _ _ _ _ _ _ _ _ _ iu _ _) = iu

accountCreatedAt :: Account -> String
accountCreatedAt (Account _ _ _ _ _ _ _ _ _ _ _ ca _) = ca

accountUpdatedAt :: Account -> String
accountUpdatedAt (Account _ _ _ _ _ _ _ _ _ _ _ _ _ ua) = ua
