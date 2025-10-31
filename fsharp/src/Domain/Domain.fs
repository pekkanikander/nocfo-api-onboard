namespace Domain

// Account types in the Finnish accounting system
type AccountType =
    // Assets (Vastaavaa)
    | ASS           // General assets
    | ASS_DEP       // Depreciable assets (Poistokelpoinen omaisuus)
    | ASS_VAT       // VAT receivables (Arvonlisäverosaatava)
    | ASS_REC       // Trade receivables (Siirtosaamiset)
    | ASS_PAY       // Bank account / Cash (Pankkitili / käteisvarat)
    | ASS_DUE       // Sales receivables (Myyntisaatavat)

    // Liabilities (Vastattavaa)
    | LIA           // General liabilities
    | LIA_EQU       // Equity (Oma pääoma)
    | LIA_PRE       // Previous periods' profit (Edellisten tilikausien voitto)
    | LIA_DUE       // Trade payables (Ostovelat)
    | LIA_DEB       // Debts (Velat)
    | LIA_ACC       // Accrued liabilities (Siirtovelat)
    | LIA_VAT       // VAT liability (Arvonlisäverovelka)

    // Revenue (Tulot)
    | REV           // General revenue
    | REV_SAL       // Sales revenue (Liikevaihtotulot / myynti)
    | REV_NO        // Tax-free revenue (Verottomat tulot)

    // Expenses (Menot)
    | EXP           // General expenses
    | EXP_DEP       // Depreciation (Poistot)
    | EXP_NO        // Non-deductible expenses (Vähennyskelvottomat menot)
    | EXP_50        // Half-deductible expenses (Puoliksi vähennyskelpoiset menot)
    | EXP_TAX       // Tax account (Verotili)
    | EXP_TAX_PRE   // Prepaid taxes (Ennakkoverot)

// Business type definition
type Business = {
    Id: int
    Slug: string
    Name: string
    BusinessId: string
    Form: string
}

// Account type definition
type Account = {
    Id: int
    Number: string
    PaddedNumber: int
    Name: string
    AccountType: AccountType
    DefaultVatCode: int
    DefaultVatRate: float
    DefaultVatRateLabel: string
    OpeningBalance: float
    Balance: float
    IsShown: bool
    IsUsed: bool
    CreatedAt: string
    UpdatedAt: string
}

// Balance calculation types
type AccountCategory =
    | Assets
    | Liabilities
    | Revenue
    | Expenses

type CategoryBalance = {
    Category: AccountCategory
    Total: float
    AccountCount: int
    LastUpdated: System.DateTime
}

type BusinessBalance = {
    BusinessSlug: string
    Assets: CategoryBalance
    Liabilities: CategoryBalance
    Revenue: CategoryBalance
    Expenses: CategoryBalance
    IsBalanced: bool
    CalculatedAt: System.DateTime
}

// Factory functions
module Business =
    let create id slug name businessId form = {
        Id = id
        Slug = slug
        Name = name
        BusinessId = businessId
        Form = form
    }

module Account =
    let create id number paddedNumber name accountType defaultVatCode defaultVatRate defaultVatRateLabel openingBalance balance isShown isUsed createdAt updatedAt = {
        Id = id
        Number = number
        PaddedNumber = paddedNumber
        Name = name
        AccountType = accountType
        DefaultVatCode = defaultVatCode
        DefaultVatRate = defaultVatRate
        DefaultVatRateLabel = defaultVatRateLabel
        OpeningBalance = openingBalance
        Balance = balance
        IsShown = isShown
        IsUsed = isUsed
        CreatedAt = createdAt
        UpdatedAt = updatedAt
    }

module AccountType =
    let toCategory accountType =
        match accountType with
        | ASS | ASS_DEP | ASS_VAT | ASS_REC | ASS_PAY | ASS_DUE -> Assets
        | LIA | LIA_EQU | LIA_PRE | LIA_DUE | LIA_DEB | LIA_ACC | LIA_VAT -> Liabilities
        | REV | REV_SAL | REV_NO -> Revenue
        | EXP | EXP_DEP | EXP_NO | EXP_50 | EXP_TAX | EXP_TAX_PRE -> Expenses

module BalanceCalculation =
    let createCategoryBalance category total accountCount =
        { Category = category; Total = total; AccountCount = accountCount; LastUpdated = System.DateTime.UtcNow }

    let createBusinessBalance businessSlug assets liabilities revenue expenses =
        let isBalanced =
            // Basic accounting equation: Assets + Expenses = Liabilities + Revenue
            // This is a simplified version - real accounting is more complex
            abs((assets.Total + expenses.Total) - (liabilities.Total + revenue.Total)) < 0.01

        { BusinessSlug = businessSlug
          Assets = assets
          Liabilities = liabilities
          Revenue = revenue
          Expenses = expenses
          IsBalanced = isBalanced
          CalculatedAt = System.DateTime.UtcNow }

    let updateCategoryBalance (balance: CategoryBalance) (account: Account) =
        { balance with
            Total = balance.Total + account.Balance
            AccountCount = balance.AccountCount + 1
            LastUpdated = System.DateTime.UtcNow }
