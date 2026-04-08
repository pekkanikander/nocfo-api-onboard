namespace rec NocfoApi.Types

///* `INITIAL` - INITIAL
///* `PENDING` - PENDING
///* `RUNNING` - RUNNING
///* `SUCCESS` - SUCCESS
///* `FAILURE` - FAILURE
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type AlgoStatusEnum =
    | [<CompiledName "INITIAL">] INITIAL
    | [<CompiledName "PENDING">] PENDING
    | [<CompiledName "RUNNING">] RUNNING
    | [<CompiledName "SUCCESS">] SUCCESS
    | [<CompiledName "FAILURE">] FAILURE
    member this.Format() =
        match this with
        | INITIAL -> "INITIAL"
        | PENDING -> "PENDING"
        | RUNNING -> "RUNNING"
        | SUCCESS -> "SUCCESS"
        | FAILURE -> "FAILURE"

///* `PENDING` - Odottaa
///* `IN_PROGRESS` - Kesken
///* `COMPLETED` - Valmis
///* `CANCELLED` - Peruttu
///* `FAILED` - Epäonnistunut
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type AnalysisStatusEnum =
    | [<CompiledName "PENDING">] PENDING
    | [<CompiledName "IN_PROGRESS">] IN_PROGRESS
    | [<CompiledName "COMPLETED">] COMPLETED
    | [<CompiledName "CANCELLED">] CANCELLED
    | [<CompiledName "FAILED">] FAILED
    member this.Format() =
        match this with
        | PENDING -> "PENDING"
        | IN_PROGRESS -> "IN_PROGRESS"
        | COMPLETED -> "COMPLETED"
        | CANCELLED -> "CANCELLED"
        | FAILED -> "FAILED"

///* `ATTACHMENT_TYPE` - Liitteen tyyppi
///* `CONTACT_NAME` - Kontaktin nimi
///* `CURRENCY_CODE` - Valuutta
///* `INVOICE_DATE` - Laskun päivämäärä
///* `INVOICE_DUE_DATE` - Laskun eräpäivä
///* `PAYMENT_REF` - Viitenumero
///* `RECEIPT_DATE` - Kuitin päivämäärä
///* `TOTAL_AMOUNT` - Summa
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type AttachmentAnalysisValueTypeEnum =
    | [<CompiledName "ATTACHMENT_TYPE">] ATTACHMENT_TYPE
    | [<CompiledName "CONTACT_NAME">] CONTACT_NAME
    | [<CompiledName "CURRENCY_CODE">] CURRENCY_CODE
    | [<CompiledName "INVOICE_DATE">] INVOICE_DATE
    | [<CompiledName "INVOICE_DUE_DATE">] INVOICE_DUE_DATE
    | [<CompiledName "PAYMENT_REF">] PAYMENT_REF
    | [<CompiledName "RECEIPT_DATE">] RECEIPT_DATE
    | [<CompiledName "TOTAL_AMOUNT">] TOTAL_AMOUNT
    member this.Format() =
        match this with
        | ATTACHMENT_TYPE -> "ATTACHMENT_TYPE"
        | CONTACT_NAME -> "CONTACT_NAME"
        | CURRENCY_CODE -> "CURRENCY_CODE"
        | INVOICE_DATE -> "INVOICE_DATE"
        | INVOICE_DUE_DATE -> "INVOICE_DUE_DATE"
        | PAYMENT_REF -> "PAYMENT_REF"
        | RECEIPT_DATE -> "RECEIPT_DATE"
        | TOTAL_AMOUNT -> "TOTAL_AMOUNT"

///* `NOT_STARTED` - Ei aloitettu
///* `EMAIL_SENT` - Sähköposti lähetetty
///* `SENT_TO_KRAVIA` - Rekisteröity Kravialle
///* `FAILED` - Epäonnistunut
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type AutomaticDebtCollectionStatusEnum =
    | [<CompiledName "NOT_STARTED">] NOT_STARTED
    | [<CompiledName "EMAIL_SENT">] EMAIL_SENT
    | [<CompiledName "SENT_TO_KRAVIA">] SENT_TO_KRAVIA
    | [<CompiledName "FAILED">] FAILED
    member this.Format() =
        match this with
        | NOT_STARTED -> "NOT_STARTED"
        | EMAIL_SENT -> "EMAIL_SENT"
        | SENT_TO_KRAVIA -> "SENT_TO_KRAVIA"
        | FAILED -> "FAILED"

///* `y_tunnus` - Y_TUNNUS
///* `vat_code` - VAT_CODE
///* `steuernummer` - STEUERNUMMER
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type BusinessIdentifierTypeEnum =
    | [<CompiledName "y_tunnus">] Y_tunnus
    | [<CompiledName "vat_code">] Vat_code
    | [<CompiledName "steuernummer">] Steuernummer
    member this.Format() =
        match this with
        | Y_tunnus -> "y_tunnus"
        | Vat_code -> "vat_code"
        | Steuernummer -> "steuernummer"

///* `UNSET` - Ei asetettu
///* `PERSON` - Yksityishenkilö
///* `BUSINESS` - Yritys
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type ContactTypeEnum =
    | [<CompiledName "UNSET">] UNSET
    | [<CompiledName "PERSON">] PERSON
    | [<CompiledName "BUSINESS">] BUSINESS
    member this.Format() =
        match this with
        | UNSET -> "UNSET"
        | PERSON -> "PERSON"
        | BUSINESS -> "BUSINESS"

[<RequireQualifiedAccess>]
type DefaultVatCodeEnum =
    | DefaultVatCodeEnum1 = 1
    | DefaultVatCodeEnum2 = 2
    | DefaultVatCodeEnum3 = 3
    | DefaultVatCodeEnum4 = 4
    | DefaultVatCodeEnum5 = 5
    | DefaultVatCodeEnum6 = 6
    | DefaultVatCodeEnum7 = 7
    | DefaultVatCodeEnum8 = 8
    | DefaultVatCodeEnum9 = 9
    | DefaultVatCodeEnum15 = 15
    | DefaultVatCodeEnum14 = 14
    | DefaultVatCodeEnum10 = 10
    | DefaultVatCodeEnum11 = 11
    | DefaultVatCodeEnum12 = 12
    | DefaultVatCodeEnum13 = 13

[<RequireQualifiedAccess>]
type DefaultVatPeriodEnum =
    | ``DefaultVatPeriodEnum-1`` = -1
    | DefaultVatPeriodEnum0 = 0
    | DefaultVatPeriodEnum1 = 1
    | DefaultVatPeriodEnum2 = 2

///* `standard` - STANDARD
///* `reduced_a` - REDUCED_A
///* `reduced_b` - REDUCED_B
///* `zero` - ZERO
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type DefaultVatRateLabelEnum =
    | [<CompiledName "standard">] Standard
    | [<CompiledName "reduced_a">] Reduced_a
    | [<CompiledName "reduced_b">] Reduced_b
    | [<CompiledName "zero">] Zero
    member this.Format() =
        match this with
        | Standard -> "standard"
        | Reduced_a -> "reduced_a"
        | Reduced_b -> "reduced_b"
        | Zero -> "zero"

///* `EMAIL` - Email
///* `EINVOICE` - Einvoice
///* `ELASKU` - Elasku
///* `PAPER` - Paper
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type DeliveryMethodEnum =
    | [<CompiledName "EMAIL">] EMAIL
    | [<CompiledName "EINVOICE">] EINVOICE
    | [<CompiledName "ELASKU">] ELASKU
    | [<CompiledName "PAPER">] PAPER
    member this.Format() =
        match this with
        | EMAIL -> "EMAIL"
        | EINVOICE -> "EINVOICE"
        | ELASKU -> "ELASKU"
        | PAPER -> "PAPER"

///* `FI_OY` - Osakeyhtiö (Oy)
///* `FI_OYJ` - Julkinen osakeyhtiö (Oyj)
///* `FI_TMI` - Toiminimi (Tmi)
///* `FI_KY` - Kommandiittiyhtiö (Ky)
///* `FI_OK` - Osuuskunta (Osk)
///* `FI_AY` - Avoin yhtiö (AY)
///* `FI_AS_OY` - Asunto-osakeyhtiö (As Oy)
///* `FI_KOY` - Kiinteistöosakeyhtiö (Koy)
///* `FI_NY` - NYT-yritys
///* `FI_YHD` - Yhdistys (ry)
///* `DE_UG` - Saksalainen yrittäjäosakeyhtiö (UG)
///* `DEMO` - Demo-yritys
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type FormEnum =
    | [<CompiledName "FI_OY">] FI_OY
    | [<CompiledName "FI_OYJ">] FI_OYJ
    | [<CompiledName "FI_TMI">] FI_TMI
    | [<CompiledName "FI_KY">] FI_KY
    | [<CompiledName "FI_OK">] FI_OK
    | [<CompiledName "FI_AY">] FI_AY
    | [<CompiledName "FI_AS_OY">] FI_AS_OY
    | [<CompiledName "FI_KOY">] FI_KOY
    | [<CompiledName "FI_NY">] FI_NY
    | [<CompiledName "FI_YHD">] FI_YHD
    | [<CompiledName "DE_UG">] DE_UG
    | [<CompiledName "DEMO">] DEMO
    member this.Format() =
        match this with
        | FI_OY -> "FI_OY"
        | FI_OYJ -> "FI_OYJ"
        | FI_TMI -> "FI_TMI"
        | FI_KY -> "FI_KY"
        | FI_OK -> "FI_OK"
        | FI_AY -> "FI_AY"
        | FI_AS_OY -> "FI_AS_OY"
        | FI_KOY -> "FI_KOY"
        | FI_NY -> "FI_NY"
        | FI_YHD -> "FI_YHD"
        | DE_UG -> "DE_UG"
        | DEMO -> "DEMO"

///* `B` - BALANCE_SHEET
///* `I` - INCOME_STATEMENT
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type HeaderListTypeEnum =
    | [<CompiledName "B">] B
    | [<CompiledName "I">] I
    member this.Format() =
        match this with
        | B -> "B"
        | I -> "I"

///* `APIX` - Apix
///* `EMAIL` - Email
///* `UPLOAD` - Lataus
///* `VAT_PURCHASE_INVOICE` - ALV-ostolasku
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type ImportSourceEnum =
    | [<CompiledName "APIX">] APIX
    | [<CompiledName "EMAIL">] EMAIL
    | [<CompiledName "UPLOAD">] UPLOAD
    | [<CompiledName "VAT_PURCHASE_INVOICE">] VAT_PURCHASE_INVOICE
    member this.Format() =
        match this with
        | APIX -> "APIX"
        | EMAIL -> "EMAIL"
        | UPLOAD -> "UPLOAD"
        | VAT_PURCHASE_INVOICE -> "VAT_PURCHASE_INVOICE"

///* `OPEN` - Avoin
///* `ACCEPTED` - Hyväksytty
///* `REJECTED` - Hylätty
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type InvitationStatusEnum =
    | [<CompiledName "OPEN">] OPEN
    | [<CompiledName "ACCEPTED">] ACCEPTED
    | [<CompiledName "REJECTED">] REJECTED
    member this.Format() =
        match this with
        | OPEN -> "OPEN"
        | ACCEPTED -> "ACCEPTED"
        | REJECTED -> "REJECTED"

///* `DRAFT` - Luonnos
///* `ACCEPTED` - Hyväksytty
///* `PAID` - Maksettu
///* `CREDIT_LOSS` - Luottotappio
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type InvoiceStatusEnum =
    | [<CompiledName "DRAFT">] DRAFT
    | [<CompiledName "ACCEPTED">] ACCEPTED
    | [<CompiledName "PAID">] PAID
    | [<CompiledName "CREDIT_LOSS">] CREDIT_LOSS
    member this.Format() =
        match this with
        | DRAFT -> "DRAFT"
        | ACCEPTED -> "ACCEPTED"
        | PAID -> "PAID"
        | CREDIT_LOSS -> "CREDIT_LOSS"

///* `fi` - Suomi
///* `en` - Englanti
///* `sv` - Ruotsi
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type InvoicingLanguageEnum =
    | [<CompiledName "fi">] Fi
    | [<CompiledName "en">] En
    | [<CompiledName "sv">] Sv
    member this.Format() =
        match this with
        | Fi -> "fi"
        | En -> "en"
        | Sv -> "sv"

///* `fi` - Suomi
///* `sv` - Ruotsi
///* `en` - Englanti
///* `de` - Saksa
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type LanguageEnum =
    | [<CompiledName "fi">] Fi
    | [<CompiledName "sv">] Sv
    | [<CompiledName "en">] En
    | [<CompiledName "de">] De
    member this.Format() =
        match this with
        | Fi -> "fi"
        | Sv -> "sv"
        | En -> "en"
        | De -> "de"

///* `admin` - Pääkäyttäjä
///* `read_only` - Lukuoikeus
///* `bookkeeping` - Kirjanpito
///* `invoicing` - Laskutus
///* `custom` - Omavalinta
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type PermissionGroupEnum =
    | [<CompiledName "admin">] Admin
    | [<CompiledName "read_only">] Read_only
    | [<CompiledName "bookkeeping">] Bookkeeping
    | [<CompiledName "invoicing">] Invoicing
    | [<CompiledName "custom">] Custom
    member this.Format() =
        match this with
        | Admin -> "admin"
        | Read_only -> "read_only"
        | Bookkeeping -> "bookkeeping"
        | Invoicing -> "invoicing"
        | Custom -> "custom"

///* `admin` - Pääkäyttäjä
///* `bookkeeping_admin` - Kirjanpidon pääkäyttäjä
///* `bookkeeping_editor` - Kirjanpidon muokkaus
///* `bookkeeping_attachment` - Kirjanpidon liitteet
///* `accounts_editor` - Tilien muokkaus
///* `files_editor` - Tiedostojen muokkaus
///* `invoicing_admin` - Laskutuksen pääkäyttäjä
///* `invoicing_editor` - Laskujen ja tuotteiden muokkaus
///* `contact_editor` - Kontaktien muokkaus
///* `reporting_admin` - Raportoinnin pääkäyttäjä
///* `reporting_editor` - Raportoinnin muokkaus
///* `salary` - Palkat
///* `integration_admin` - Yhteydet
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type PermissionsEnum =
    | [<CompiledName "admin">] Admin
    | [<CompiledName "bookkeeping_admin">] Bookkeeping_admin
    | [<CompiledName "bookkeeping_editor">] Bookkeeping_editor
    | [<CompiledName "bookkeeping_attachment">] Bookkeeping_attachment
    | [<CompiledName "accounts_editor">] Accounts_editor
    | [<CompiledName "files_editor">] Files_editor
    | [<CompiledName "invoicing_admin">] Invoicing_admin
    | [<CompiledName "invoicing_editor">] Invoicing_editor
    | [<CompiledName "contact_editor">] Contact_editor
    | [<CompiledName "reporting_admin">] Reporting_admin
    | [<CompiledName "reporting_editor">] Reporting_editor
    | [<CompiledName "salary">] Salary
    | [<CompiledName "integration_admin">] Integration_admin
    member this.Format() =
        match this with
        | Admin -> "admin"
        | Bookkeeping_admin -> "bookkeeping_admin"
        | Bookkeeping_editor -> "bookkeeping_editor"
        | Bookkeeping_attachment -> "bookkeeping_attachment"
        | Accounts_editor -> "accounts_editor"
        | Files_editor -> "files_editor"
        | Invoicing_admin -> "invoicing_admin"
        | Invoicing_editor -> "invoicing_editor"
        | Contact_editor -> "contact_editor"
        | Reporting_admin -> "reporting_admin"
        | Reporting_editor -> "reporting_editor"
        | Salary -> "salary"
        | Integration_admin -> "integration_admin"

[<RequireQualifiedAccess>]
type ProductVatCodeEnum =
    | ProductVatCodeEnum1 = 1
    | ProductVatCodeEnum3 = 3
    | ProductVatCodeEnum4 = 4
    | ProductVatCodeEnum5 = 5
    | ProductVatCodeEnum6 = 6
    | ProductVatCodeEnum10 = 10
    | ProductVatCodeEnum12 = 12
    | ProductVatCodeEnum13 = 13

///* `KEEP` - Keep
///* `ROLL` - Roll
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type RecurrenceReferenceMethodEnum =
    | [<CompiledName "KEEP">] KEEP
    | [<CompiledName "ROLL">] ROLL
    member this.Format() =
        match this with
        | KEEP -> "KEEP"
        | ROLL -> "ROLL"

///* `ACCRUAL` - ACCRUAL
///* `SETTLEMENT` - SETTLEMENT
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type RoleEnum =
    | [<CompiledName "ACCRUAL">] ACCRUAL
    | [<CompiledName "SETTLEMENT">] SETTLEMENT
    member this.Format() =
        match this with
        | ACCRUAL -> "ACCRUAL"
        | SETTLEMENT -> "SETTLEMENT"

///* `ACCRUAL_PAIR` - ACCRUAL_PAIR
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type Type0a6Enum =
    | [<CompiledName "ACCRUAL_PAIR">] ACCRUAL_PAIR
    member this.Format() =
        match this with
        | ACCRUAL_PAIR -> "ACCRUAL_PAIR"

[<RequireQualifiedAccess>]
type Type110Enum =
    | Type110Enum0 = 0
    | Type110Enum3 = 3
    | Type110Enum4 = 4
    | Type110Enum6 = 6
    | Type110Enum7 = 7

///* `ASS` - Vastaavaa
///* `ASS_DEP` - Poistokelpoinen omaisuus
///* `ASS_VAT` - Arvonlisäverosaatava
///* `ASS_REC` - Siirtosaamiset
///* `ASS_PAY` - Pankkitili / käteisvarat
///* `ASS_DUE` - Myyntisaatavat
///* `LIA` - Vastattavaa
///* `LIA_EQU` - Oma pääoma
///* `LIA_PRE` - Edellisten tilikausien voitto
///* `LIA_DUE` - Ostovelat
///* `LIA_DEB` - Velat
///* `LIA_ACC` - Siirtovelat
///* `LIA_VAT` - Arvonlisäverovelka
///* `REV` - Tulot
///* `REV_SAL` - Liikevaihtotulot (myynti)
///* `REV_NO` - Verottomat tulot
///* `EXP` - Menot
///* `EXP_DEP` - Poistot
///* `EXP_NO` - Vähennyskelvottomat menot
///* `EXP_50` - Puoliksi vähennyskelpoiset menot
///* `EXP_TAX` - Verotili
///* `EXP_TAX_PRE` - Ennakkoverot
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type Type92dEnum =
    | [<CompiledName "ASS">] ASS
    | [<CompiledName "ASS_DEP">] ASS_DEP
    | [<CompiledName "ASS_VAT">] ASS_VAT
    | [<CompiledName "ASS_REC">] ASS_REC
    | [<CompiledName "ASS_PAY">] ASS_PAY
    | [<CompiledName "ASS_DUE">] ASS_DUE
    | [<CompiledName "LIA">] LIA
    | [<CompiledName "LIA_EQU">] LIA_EQU
    | [<CompiledName "LIA_PRE">] LIA_PRE
    | [<CompiledName "LIA_DUE">] LIA_DUE
    | [<CompiledName "LIA_DEB">] LIA_DEB
    | [<CompiledName "LIA_ACC">] LIA_ACC
    | [<CompiledName "LIA_VAT">] LIA_VAT
    | [<CompiledName "REV">] REV
    | [<CompiledName "REV_SAL">] REV_SAL
    | [<CompiledName "REV_NO">] REV_NO
    | [<CompiledName "EXP">] EXP
    | [<CompiledName "EXP_DEP">] EXP_DEP
    | [<CompiledName "EXP_NO">] EXP_NO
    | [<CompiledName "EXP_50">] EXP_50
    | [<CompiledName "EXP_TAX">] EXP_TAX
    | [<CompiledName "EXP_TAX_PRE">] EXP_TAX_PRE
    member this.Format() =
        match this with
        | ASS -> "ASS"
        | ASS_DEP -> "ASS_DEP"
        | ASS_VAT -> "ASS_VAT"
        | ASS_REC -> "ASS_REC"
        | ASS_PAY -> "ASS_PAY"
        | ASS_DUE -> "ASS_DUE"
        | LIA -> "LIA"
        | LIA_EQU -> "LIA_EQU"
        | LIA_PRE -> "LIA_PRE"
        | LIA_DUE -> "LIA_DUE"
        | LIA_DEB -> "LIA_DEB"
        | LIA_ACC -> "LIA_ACC"
        | LIA_VAT -> "LIA_VAT"
        | REV -> "REV"
        | REV_SAL -> "REV_SAL"
        | REV_NO -> "REV_NO"
        | EXP -> "EXP"
        | EXP_DEP -> "EXP_DEP"
        | EXP_NO -> "EXP_NO"
        | EXP_50 -> "EXP_50"
        | EXP_TAX -> "EXP_TAX"
        | EXP_TAX_PRE -> "EXP_TAX_PRE"

[<RequireQualifiedAccess>]
type VatCode585Enum =
    | VatCode585Enum1 = 1
    | VatCode585Enum2 = 2
    | VatCode585Enum3 = 3
    | VatCode585Enum4 = 4
    | VatCode585Enum5 = 5
    | VatCode585Enum6 = 6
    | VatCode585Enum7 = 7
    | VatCode585Enum8 = 8
    | VatCode585Enum9 = 9
    | VatCode585Enum15 = 15
    | VatCode585Enum14 = 14
    | VatCode585Enum10 = 10
    | VatCode585Enum11 = 11
    | VatCode585Enum12 = 12
    | VatCode585Enum13 = 13

[<RequireQualifiedAccess>]
type VatMethodEnum =
    | VatMethodEnum0 = 0
    | VatMethodEnum1 = 1

///* `NET` - ALV netotus
///* `SPLIT` - ALV jaettu kirjaus
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type VatPostingMethodEnum =
    | [<CompiledName "NET">] NET
    | [<CompiledName "SPLIT">] SPLIT
    member this.Format() =
        match this with
        | NET -> "NET"
        | SPLIT -> "SPLIT"

///* `DEFAULT` - ALV kirjanpidon päivälle
///* `DE_CASH_BASIS` - ALV maksupäivänä (DE maksu)
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type VatReportingMethodEnum =
    | [<CompiledName "DEFAULT">] DEFAULT
    | [<CompiledName "DE_CASH_BASIS">] DE_CASH_BASIS
    member this.Format() =
        match this with
        | DEFAULT -> "DEFAULT"
        | DE_CASH_BASIS -> "DE_CASH_BASIS"

type Nametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of Nametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): Nametranslations = { key = key; value = value }

type Account =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      number: string
      padded_number: int
      name: string
      ///List of translations. Each item maps one language code to the translated text.
      name_translations: list<Nametranslations>
      ///Optional header ID for account grouping in countries/setups that use headers. Field may be omitted when the business has no header models.
      header_id: Option<int>
      header_path: list<string>
      description: Option<string>
      ///* `ASS` - Vastaavaa
      ///* `ASS_DEP` - Poistokelpoinen omaisuus
      ///* `ASS_VAT` - Arvonlisäverosaatava
      ///* `ASS_REC` - Siirtosaamiset
      ///* `ASS_PAY` - Pankkitili / käteisvarat
      ///* `ASS_DUE` - Myyntisaatavat
      ///* `LIA` - Vastattavaa
      ///* `LIA_EQU` - Oma pääoma
      ///* `LIA_PRE` - Edellisten tilikausien voitto
      ///* `LIA_DUE` - Ostovelat
      ///* `LIA_DEB` - Velat
      ///* `LIA_ACC` - Siirtovelat
      ///* `LIA_VAT` - Arvonlisäverovelka
      ///* `REV` - Tulot
      ///* `REV_SAL` - Liikevaihtotulot (myynti)
      ///* `REV_NO` - Verottomat tulot
      ///* `EXP` - Menot
      ///* `EXP_DEP` - Poistot
      ///* `EXP_NO` - Vähennyskelvottomat menot
      ///* `EXP_50` - Puoliksi vähennyskelpoiset menot
      ///* `EXP_TAX` - Verotili
      ///* `EXP_TAX_PRE` - Ennakkoverot
      ``type``: Option<Type92dEnum>
      default_vat_code: Option<Newtonsoft.Json.Linq.JToken>
      default_vat_rate: float
      ///* `standard` - STANDARD
      ///* `reduced_a` - REDUCED_A
      ///* `reduced_b` - REDUCED_B
      ///* `zero` - ZERO
      default_vat_rate_label: Option<DefaultVatRateLabelEnum>
      opening_balance: Option<float32>
      is_shown: bool
      balance: float32
      is_used: bool }
    ///Creates an instance of Account with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          number: string,
                          padded_number: int,
                          name: string,
                          name_translations: list<Nametranslations>,
                          header_path: list<string>,
                          default_vat_rate: float,
                          is_shown: bool,
                          balance: float32,
                          is_used: bool): Account =
        { id = id
          created_at = created_at
          updated_at = updated_at
          number = number
          padded_number = padded_number
          name = name
          name_translations = name_translations
          header_id = None
          header_path = header_path
          description = None
          ``type`` = None
          default_vat_code = None
          default_vat_rate = default_vat_rate
          default_vat_rate_label = None
          opening_balance = None
          is_shown = is_shown
          balance = balance
          is_used = is_used }

type AccountListNametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of AccountListNametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): AccountListNametranslations = { key = key; value = value }

type AccountList =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      number: string
      padded_number: int
      name: string
      ///List of translations. Each item maps one language code to the translated text.
      name_translations: list<AccountListNametranslations>
      ///Optional header ID for account grouping in countries/setups that use headers. Field may be omitted when the business has no header models.
      header_id: Option<int>
      header_path: list<string>
      description: Option<string>
      ///* `ASS` - Vastaavaa
      ///* `ASS_DEP` - Poistokelpoinen omaisuus
      ///* `ASS_VAT` - Arvonlisäverosaatava
      ///* `ASS_REC` - Siirtosaamiset
      ///* `ASS_PAY` - Pankkitili / käteisvarat
      ///* `ASS_DUE` - Myyntisaatavat
      ///* `LIA` - Vastattavaa
      ///* `LIA_EQU` - Oma pääoma
      ///* `LIA_PRE` - Edellisten tilikausien voitto
      ///* `LIA_DUE` - Ostovelat
      ///* `LIA_DEB` - Velat
      ///* `LIA_ACC` - Siirtovelat
      ///* `LIA_VAT` - Arvonlisäverovelka
      ///* `REV` - Tulot
      ///* `REV_SAL` - Liikevaihtotulot (myynti)
      ///* `REV_NO` - Verottomat tulot
      ///* `EXP` - Menot
      ///* `EXP_DEP` - Poistot
      ///* `EXP_NO` - Vähennyskelvottomat menot
      ///* `EXP_50` - Puoliksi vähennyskelpoiset menot
      ///* `EXP_TAX` - Verotili
      ///* `EXP_TAX_PRE` - Ennakkoverot
      ``type``: Option<Type92dEnum>
      default_vat_code: Option<Newtonsoft.Json.Linq.JToken>
      default_vat_rate: float
      ///* `standard` - STANDARD
      ///* `reduced_a` - REDUCED_A
      ///* `reduced_b` - REDUCED_B
      ///* `zero` - ZERO
      default_vat_rate_label: Option<DefaultVatRateLabelEnum>
      opening_balance: Option<float32>
      is_shown: bool }
    ///Creates an instance of AccountList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          number: string,
                          padded_number: int,
                          name: string,
                          name_translations: list<AccountListNametranslations>,
                          header_path: list<string>,
                          default_vat_rate: float,
                          is_shown: bool): AccountList =
        { id = id
          created_at = created_at
          updated_at = updated_at
          number = number
          padded_number = padded_number
          name = name
          name_translations = name_translations
          header_id = None
          header_path = header_path
          description = None
          ``type`` = None
          default_vat_code = None
          default_vat_rate = default_vat_rate
          default_vat_rate_label = None
          opening_balance = None
          is_shown = is_shown }

type AccountListRequestNametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of AccountListRequestNametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): AccountListRequestNametranslations = { key = key; value = value }

type AccountListRequest =
    { number: string
      ///List of translations. Each item maps one language code to the translated text.
      name_translations: list<AccountListRequestNametranslations>
      ///Optional header ID for account grouping in countries/setups that use headers. Field may be omitted when the business has no header models.
      header_id: Option<int>
      description: Option<string>
      ///* `ASS` - Vastaavaa
      ///* `ASS_DEP` - Poistokelpoinen omaisuus
      ///* `ASS_VAT` - Arvonlisäverosaatava
      ///* `ASS_REC` - Siirtosaamiset
      ///* `ASS_PAY` - Pankkitili / käteisvarat
      ///* `ASS_DUE` - Myyntisaatavat
      ///* `LIA` - Vastattavaa
      ///* `LIA_EQU` - Oma pääoma
      ///* `LIA_PRE` - Edellisten tilikausien voitto
      ///* `LIA_DUE` - Ostovelat
      ///* `LIA_DEB` - Velat
      ///* `LIA_ACC` - Siirtovelat
      ///* `LIA_VAT` - Arvonlisäverovelka
      ///* `REV` - Tulot
      ///* `REV_SAL` - Liikevaihtotulot (myynti)
      ///* `REV_NO` - Verottomat tulot
      ///* `EXP` - Menot
      ///* `EXP_DEP` - Poistot
      ///* `EXP_NO` - Vähennyskelvottomat menot
      ///* `EXP_50` - Puoliksi vähennyskelpoiset menot
      ///* `EXP_TAX` - Verotili
      ///* `EXP_TAX_PRE` - Ennakkoverot
      ``type``: Option<Type92dEnum>
      default_vat_code: Option<Newtonsoft.Json.Linq.JToken>
      ///* `standard` - STANDARD
      ///* `reduced_a` - REDUCED_A
      ///* `reduced_b` - REDUCED_B
      ///* `zero` - ZERO
      default_vat_rate_label: Option<DefaultVatRateLabelEnum>
      opening_balance: Option<float32> }
    ///Creates an instance of AccountListRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (number: string, name_translations: list<AccountListRequestNametranslations>): AccountListRequest =
        { number = number
          name_translations = name_translations
          header_id = None
          description = None
          ``type`` = None
          default_vat_code = None
          default_vat_rate_label = None
          opening_balance = None }

type AccountRequestNametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of AccountRequestNametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): AccountRequestNametranslations = { key = key; value = value }

type AccountRequest =
    { number: string
      ///List of translations. Each item maps one language code to the translated text.
      name_translations: list<AccountRequestNametranslations>
      ///Optional header ID for account grouping in countries/setups that use headers. Field may be omitted when the business has no header models.
      header_id: Option<int>
      description: Option<string>
      ///* `ASS` - Vastaavaa
      ///* `ASS_DEP` - Poistokelpoinen omaisuus
      ///* `ASS_VAT` - Arvonlisäverosaatava
      ///* `ASS_REC` - Siirtosaamiset
      ///* `ASS_PAY` - Pankkitili / käteisvarat
      ///* `ASS_DUE` - Myyntisaatavat
      ///* `LIA` - Vastattavaa
      ///* `LIA_EQU` - Oma pääoma
      ///* `LIA_PRE` - Edellisten tilikausien voitto
      ///* `LIA_DUE` - Ostovelat
      ///* `LIA_DEB` - Velat
      ///* `LIA_ACC` - Siirtovelat
      ///* `LIA_VAT` - Arvonlisäverovelka
      ///* `REV` - Tulot
      ///* `REV_SAL` - Liikevaihtotulot (myynti)
      ///* `REV_NO` - Verottomat tulot
      ///* `EXP` - Menot
      ///* `EXP_DEP` - Poistot
      ///* `EXP_NO` - Vähennyskelvottomat menot
      ///* `EXP_50` - Puoliksi vähennyskelpoiset menot
      ///* `EXP_TAX` - Verotili
      ///* `EXP_TAX_PRE` - Ennakkoverot
      ``type``: Option<Type92dEnum>
      default_vat_code: Option<Newtonsoft.Json.Linq.JToken>
      ///* `standard` - STANDARD
      ///* `reduced_a` - REDUCED_A
      ///* `reduced_b` - REDUCED_B
      ///* `zero` - ZERO
      default_vat_rate_label: Option<DefaultVatRateLabelEnum>
      opening_balance: Option<float32> }
    ///Creates an instance of AccountRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (number: string, name_translations: list<AccountRequestNametranslations>): AccountRequest =
        { number = number
          name_translations = name_translations
          header_id = None
          description = None
          ``type`` = None
          default_vat_code = None
          default_vat_rate_label = None
          opening_balance = None }

type AccountingReportJsonResponse =
    { ///Internal report type identifier that tells which report payload was generated.
      report_type: string
      ///Start date used for the report range when applicable. Null for point-in-time reports.
      date_from: Option<string>
      ///End date used for the report range when applicable. For point-in-time reports, this may match the reporting date or be null.
      date_to: Option<string>
      ///Column labels for accounting-style reports.
      labels: list<string>
      ///Normalized report rows. Each row value array aligns with `labels`; rows can include nested account-level drill-down data.
      rows: list<ReportRowSchema> }
    ///Creates an instance of AccountingReportJsonResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (report_type: string, labels: list<string>, rows: list<ReportRowSchema>): AccountingReportJsonResponse =
        { report_type = report_type
          date_from = None
          date_to = None
          labels = labels
          rows = rows }

type AttachmentAnalysis =
    { values: list<AttachmentAnalysisValue>
      analysis_data: Newtonsoft.Json.Linq.JToken }
    ///Creates an instance of AttachmentAnalysis with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (values: list<AttachmentAnalysisValue>, analysis_data: Newtonsoft.Json.Linq.JToken): AttachmentAnalysis =
        { values = values
          analysis_data = analysis_data }

type AttachmentAnalysisValue =
    { ///* `ATTACHMENT_TYPE` - Liitteen tyyppi
      ///* `CONTACT_NAME` - Kontaktin nimi
      ///* `CURRENCY_CODE` - Valuutta
      ///* `INVOICE_DATE` - Laskun päivämäärä
      ///* `INVOICE_DUE_DATE` - Laskun eräpäivä
      ///* `PAYMENT_REF` - Viitenumero
      ///* `RECEIPT_DATE` - Kuitin päivämäärä
      ///* `TOTAL_AMOUNT` - Summa
      ``type``: AttachmentAnalysisValueTypeEnum
      value: Option<string> }
    ///Creates an instance of AttachmentAnalysisValue with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (``type``: AttachmentAnalysisValueTypeEnum, value: Option<string>): AttachmentAnalysisValue =
        { ``type`` = ``type``; value = value }

type AttachmentAnalysisValueRequest =
    { ///* `ATTACHMENT_TYPE` - Liitteen tyyppi
      ///* `CONTACT_NAME` - Kontaktin nimi
      ///* `CURRENCY_CODE` - Valuutta
      ///* `INVOICE_DATE` - Laskun päivämäärä
      ///* `INVOICE_DUE_DATE` - Laskun eräpäivä
      ///* `PAYMENT_REF` - Viitenumero
      ///* `RECEIPT_DATE` - Kuitin päivämäärä
      ///* `TOTAL_AMOUNT` - Summa
      ``type``: AttachmentAnalysisValueTypeEnum }
    ///Creates an instance of AttachmentAnalysisValueRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (``type``: AttachmentAnalysisValueTypeEnum): AttachmentAnalysisValueRequest =
        { ``type`` = ``type`` }

type AttachmentInstance =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      file: string
      name: string
      ``type``: string
      preview: Option<Newtonsoft.Json.Linq.JToken>
      folder_id: Option<int>
      blurhash: Option<string>
      blurhash_w: Option<int>
      blurhash_h: Option<int>
      is_deletable: bool
      ///Workaround: Hawaii cannot emit multipart form-data for complex arrays, so this field is downgraded to a plain string placeholder.
      analysis_results: Option<string>
      ///* `PENDING` - Odottaa
      ///* `IN_PROGRESS` - Kesken
      ///* `COMPLETED` - Valmis
      ///* `CANCELLED` - Peruttu
      ///* `FAILED` - Epäonnistunut
      analysis_status: Option<AnalysisStatusEnum> }
    ///Creates an instance of AttachmentInstance with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          file: string,
                          name: string,
                          ``type``: string,
                          preview: Option<Newtonsoft.Json.Linq.JToken>,
                          blurhash: Option<string>,
                          blurhash_w: Option<int>,
                          blurhash_h: Option<int>,
                          is_deletable: bool,
                          analysis_results: Option<string>): AttachmentInstance =
        { id = id
          created_at = created_at
          updated_at = updated_at
          file = file
          name = name
          ``type`` = ``type``
          preview = preview
          folder_id = None
          blurhash = blurhash
          blurhash_w = blurhash_w
          blurhash_h = blurhash_h
          is_deletable = is_deletable
          analysis_results = analysis_results
          analysis_status = None }

type AttachmentInstanceRequest =
    { name: string
      folder_id: Option<int>
      ///* `PENDING` - Odottaa
      ///* `IN_PROGRESS` - Kesken
      ///* `COMPLETED` - Valmis
      ///* `CANCELLED` - Peruttu
      ///* `FAILED` - Epäonnistunut
      analysis_status: Option<AnalysisStatusEnum> }
    ///Creates an instance of AttachmentInstanceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string): AttachmentInstanceRequest =
        { name = name
          folder_id = None
          analysis_status = None }

type AttachmentList =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      name: string
      ``type``: string
      file: string
      preview: Option<Newtonsoft.Json.Linq.JToken>
      folder_id: Option<int>
      blurhash: Option<string>
      blurhash_w: Option<int>
      blurhash_h: Option<int>
      ///* `PENDING` - Odottaa
      ///* `IN_PROGRESS` - Kesken
      ///* `COMPLETED` - Valmis
      ///* `CANCELLED` - Peruttu
      ///* `FAILED` - Epäonnistunut
      analysis_status: Option<AnalysisStatusEnum> }
    ///Creates an instance of AttachmentList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          name: string,
                          ``type``: string,
                          file: string,
                          preview: Option<Newtonsoft.Json.Linq.JToken>,
                          blurhash: Option<string>,
                          blurhash_w: Option<int>,
                          blurhash_h: Option<int>): AttachmentList =
        { id = id
          created_at = created_at
          updated_at = updated_at
          name = name
          ``type`` = ``type``
          file = file
          preview = preview
          folder_id = None
          blurhash = blurhash
          blurhash_w = blurhash_w
          blurhash_h = blurhash_h
          analysis_status = None }

type AttachmentPreview =
    { image: string
      blurhash: Option<string>
      blurhash_w: Option<int>
      blurhash_h: Option<int> }
    ///Creates an instance of AttachmentPreview with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (image: string, blurhash: Option<string>, blurhash_w: Option<int>, blurhash_h: Option<int>): AttachmentPreview =
        { image = image
          blurhash = blurhash
          blurhash_w = blurhash_w
          blurhash_h = blurhash_h }

type Blueprint =
    { ///Shortcut for debit-side payment account (for example bank account). If set, keep `debet_entries` empty.
      debet_account_id: Option<int>
      ///Debit entries generated from this blueprint. Must be empty when `debet_account_id` is used. See `bookkeeping_blueprint_retrieve` for canonical blueprint rules.
      debet_entries: Option<list<BlueprintEntry>>
      ///Shortcut for credit-side payment account (for example bank account). If set, keep `credit_entries` empty.
      credit_account_id: Option<int>
      ///Credit entries generated from this blueprint. Must be empty when `credit_account_id` is used. See `bookkeeping_blueprint_retrieve` for canonical blueprint rules.
      credit_entries: Option<list<BlueprintEntry>>
      ///Optional extra expense entries used mainly in sales blueprints for indirect costs (for example overhead). See `bookkeeping_blueprint_retrieve` for canonical blueprint rules.
      expense_entries: Option<list<BlueprintEntry>> }
    ///Creates an instance of Blueprint with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): Blueprint =
        { debet_account_id = None
          debet_entries = None
          credit_account_id = None
          credit_entries = None
          expense_entries = None }

type BlueprintEntry =
    { ///The account ID associated with this entry.
      account_id: Option<int>
      ///Description of the entry, providing additional context or details.
      description: Option<string>
      ///VAT code for the entry. See `constants_vat_codes_retrieve` with `business_slug` for business-specific value semantics and tags.
      vat_code: Option<int>
      ///VAT rate percentage for this entry (for example 0, 10, 24).
      vat_rate: Option<float>
      ///VAT method for this entry: `0` (gross posting) or `1` (net posting).
      ///* `0` - Bruttokirjaus
      ///* `1` - Nettokirjaus
      vat_method: Option<Newtonsoft.Json.Linq.JToken>
      ///The amount of this entry, in the smallest currency unit (e.g., cents).
      amount: Option<float32> }
    ///Creates an instance of BlueprintEntry with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (account_id: Option<int>): BlueprintEntry =
        { account_id = account_id
          description = None
          vat_code = None
          vat_rate = None
          vat_method = None
          amount = None }

type BlueprintEntryRequest =
    { ///The account ID associated with this entry.
      account_id: Option<int>
      ///Description of the entry, providing additional context or details.
      description: Option<string>
      ///VAT code for the entry. See `constants_vat_codes_retrieve` with `business_slug` for business-specific value semantics and tags.
      vat_code: Option<int>
      ///VAT rate percentage for this entry (for example 0, 10, 24).
      vat_rate: Option<float>
      ///VAT method for this entry: `0` (gross posting) or `1` (net posting).
      ///* `0` - Bruttokirjaus
      ///* `1` - Nettokirjaus
      vat_method: Option<Newtonsoft.Json.Linq.JToken>
      ///The amount of this entry, in the smallest currency unit (e.g., cents).
      amount: Option<float32> }
    ///Creates an instance of BlueprintEntryRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (account_id: Option<int>): BlueprintEntryRequest =
        { account_id = account_id
          description = None
          vat_code = None
          vat_rate = None
          vat_method = None
          amount = None }

type BlueprintRequest =
    { ///Shortcut for debit-side payment account (for example bank account). If set, keep `debet_entries` empty.
      debet_account_id: Option<int>
      ///Debit entries generated from this blueprint. Must be empty when `debet_account_id` is used. See `bookkeeping_blueprint_retrieve` for canonical blueprint rules.
      debet_entries: Option<list<BlueprintEntryRequest>>
      ///Shortcut for credit-side payment account (for example bank account). If set, keep `credit_entries` empty.
      credit_account_id: Option<int>
      ///Credit entries generated from this blueprint. Must be empty when `credit_account_id` is used. See `bookkeeping_blueprint_retrieve` for canonical blueprint rules.
      credit_entries: Option<list<BlueprintEntryRequest>>
      ///Optional extra expense entries used mainly in sales blueprints for indirect costs (for example overhead). See `bookkeeping_blueprint_retrieve` for canonical blueprint rules.
      expense_entries: Option<list<BlueprintEntryRequest>> }
    ///Creates an instance of BlueprintRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): BlueprintRequest =
        { debet_account_id = None
          debet_entries = None
          credit_account_id = None
          credit_entries = None
          expense_entries = None }

type Business =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      logo: Option<string>
      name: string
      slug: Option<string>
      business_id: Option<string>
      country: string
      country_config: Newtonsoft.Json.Linq.JToken
      period_id: Option<int>
      is_billable: bool
      vat_period_id: Option<int>
      ///* `NET` - ALV netotus
      ///* `SPLIT` - ALV jaettu kirjaus
      vat_posting_method: Option<VatPostingMethodEnum>
      ///* `DEFAULT` - ALV kirjanpidon päivälle
      ///* `DE_CASH_BASIS` - ALV maksupäivänä (DE maksu)
      vat_reporting_method: Option<VatReportingMethodEnum>
      ///* `FI_OY` - Osakeyhtiö (Oy)
      ///* `FI_OYJ` - Julkinen osakeyhtiö (Oyj)
      ///* `FI_TMI` - Toiminimi (Tmi)
      ///* `FI_KY` - Kommandiittiyhtiö (Ky)
      ///* `FI_OK` - Osuuskunta (Osk)
      ///* `FI_AY` - Avoin yhtiö (AY)
      ///* `FI_AS_OY` - Asunto-osakeyhtiö (As Oy)
      ///* `FI_KOY` - Kiinteistöosakeyhtiö (Koy)
      ///* `FI_NY` - NYT-yritys
      ///* `FI_YHD` - Yhdistys (ry)
      ///* `DE_UG` - Saksalainen yrittäjäosakeyhtiö (UG)
      ///* `DEMO` - Demo-yritys
      form: Option<FormEnum>
      default_vat_period: Option<Newtonsoft.Json.Linq.JToken>
      invoicing_default_penalty_interest: Option<float>
      invoicing_default_payment_condition_days: Option<int>
      invoicing_default_email_subject: Option<string>
      invoicing_default_email_content: Option<string>
      invoicing_vat_exemption_reason: Option<string>
      has_accrual_based_entries_for_invoicing: Option<bool>
      owner_name: string
      owner_email: string
      has_history_before_nocfo: Option<bool>
      fiscal_period_needs_review: Option<bool>
      vat_period_needs_review: Option<bool>
      contact_phone: Option<string>
      invoicing_email: Option<string>
      invoicing_iban: Option<string>
      invoicing_tax_code: Option<string>
      invoicing_bic: Option<string>
      invoicing_contact: Option<string>
      invoicing_street: Option<string>
      invoicing_city: Option<string>
      invoicing_postal_code: Option<string>
      invoicing_country: Option<string>
      has_business_address: bool
      business_street: Option<string>
      business_city: Option<string>
      business_postal_code: Option<string>
      business_country: Option<string>
      account_yle_tax: Option<int>
      account_tax_deferrals: Option<int>
      account_income_tax_rec: Option<int>
      account_income_tax_lia: Option<int>
      account_previous_profit: Option<int>
      account_vat_receivables: Option<int>
      account_vat_liabilities: Option<int>
      account_vat_accrual_adjustment: Option<int>
      account_trade_receivables: Option<int>
      account_payables: Option<int>
      ///Currently active subscription plan
      subscription_plan: string
      ///Where the current subscription plan is controlled from
      subscription_source: string
      stripe_customer_id: Option<string>
      stripe_subscription_id: Option<string>
      stripe_plan_status: Option<string>
      ///Billing interval from Stripe: 'month' or 'year'.
      stripe_billing_interval: Option<string>
      can_invoice: bool
      onboarding_updated_invoicing_settings: Option<bool>
      onboarding_created_invoicing_contact: Option<bool>
      onboarding_created_invoicing_product: Option<bool>
      onboarding_registered_to_kravia: Option<bool>
      einvoicing_enabled: Option<bool>
      einvoicing_address: Option<string>
      einvoicing_operator: Option<string>
      apix_unique_id: Option<string>
      apix_transfer_id: Option<string>
      apix_transfer_key: Option<string>
      holvi_bookkeeping_api_enabled: Option<bool>
      holvi_nocfo_connection_enabled: Option<bool>
      salaxy_enabled: bool
      salaxy_account_id: Option<string>
      demo_days_left: int
      verohallinto_latest_reporter_full_name: Option<string>
      verohallinto_latest_reporter_phone_number: Option<string>
      is_eligible_for_einvoicing: bool
      has_tags: bool
      identifiers: list<BusinessIdentifier>
      is_new_invoicer: bool
      is_automatic_debt_collection_enabled: Option<bool> }
    ///Creates an instance of Business with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          logo: Option<string>,
                          name: string,
                          country: string,
                          country_config: Newtonsoft.Json.Linq.JToken,
                          period_id: Option<int>,
                          is_billable: bool,
                          vat_period_id: Option<int>,
                          owner_name: string,
                          owner_email: string,
                          invoicing_tax_code: Option<string>,
                          has_business_address: bool,
                          subscription_plan: string,
                          subscription_source: string,
                          stripe_customer_id: Option<string>,
                          stripe_subscription_id: Option<string>,
                          stripe_plan_status: Option<string>,
                          stripe_billing_interval: Option<string>,
                          can_invoice: bool,
                          einvoicing_address: Option<string>,
                          einvoicing_operator: Option<string>,
                          apix_unique_id: Option<string>,
                          apix_transfer_id: Option<string>,
                          apix_transfer_key: Option<string>,
                          salaxy_enabled: bool,
                          salaxy_account_id: Option<string>,
                          demo_days_left: int,
                          is_eligible_for_einvoicing: bool,
                          has_tags: bool,
                          identifiers: list<BusinessIdentifier>,
                          is_new_invoicer: bool): Business =
        { id = id
          created_at = created_at
          updated_at = updated_at
          logo = logo
          name = name
          slug = None
          business_id = None
          country = country
          country_config = country_config
          period_id = period_id
          is_billable = is_billable
          vat_period_id = vat_period_id
          vat_posting_method = None
          vat_reporting_method = None
          form = None
          default_vat_period = None
          invoicing_default_penalty_interest = None
          invoicing_default_payment_condition_days = None
          invoicing_default_email_subject = None
          invoicing_default_email_content = None
          invoicing_vat_exemption_reason = None
          has_accrual_based_entries_for_invoicing = None
          owner_name = owner_name
          owner_email = owner_email
          has_history_before_nocfo = None
          fiscal_period_needs_review = None
          vat_period_needs_review = None
          contact_phone = None
          invoicing_email = None
          invoicing_iban = None
          invoicing_tax_code = invoicing_tax_code
          invoicing_bic = None
          invoicing_contact = None
          invoicing_street = None
          invoicing_city = None
          invoicing_postal_code = None
          invoicing_country = None
          has_business_address = has_business_address
          business_street = None
          business_city = None
          business_postal_code = None
          business_country = None
          account_yle_tax = None
          account_tax_deferrals = None
          account_income_tax_rec = None
          account_income_tax_lia = None
          account_previous_profit = None
          account_vat_receivables = None
          account_vat_liabilities = None
          account_vat_accrual_adjustment = None
          account_trade_receivables = None
          account_payables = None
          subscription_plan = subscription_plan
          subscription_source = subscription_source
          stripe_customer_id = stripe_customer_id
          stripe_subscription_id = stripe_subscription_id
          stripe_plan_status = stripe_plan_status
          stripe_billing_interval = stripe_billing_interval
          can_invoice = can_invoice
          onboarding_updated_invoicing_settings = None
          onboarding_created_invoicing_contact = None
          onboarding_created_invoicing_product = None
          onboarding_registered_to_kravia = None
          einvoicing_enabled = None
          einvoicing_address = einvoicing_address
          einvoicing_operator = einvoicing_operator
          apix_unique_id = apix_unique_id
          apix_transfer_id = apix_transfer_id
          apix_transfer_key = apix_transfer_key
          holvi_bookkeeping_api_enabled = None
          holvi_nocfo_connection_enabled = None
          salaxy_enabled = salaxy_enabled
          salaxy_account_id = salaxy_account_id
          demo_days_left = demo_days_left
          verohallinto_latest_reporter_full_name = None
          verohallinto_latest_reporter_phone_number = None
          is_eligible_for_einvoicing = is_eligible_for_einvoicing
          has_tags = has_tags
          identifiers = identifiers
          is_new_invoicer = is_new_invoicer
          is_automatic_debt_collection_enabled = None }

type BusinessIdentifier =
    { id: int
      ///Identifier type enum. Allowed values: `y_tunnus`, `vat_code`, `steuernummer`.
      ///* `y_tunnus` - Y_TUNNUS
      ///* `vat_code` - VAT_CODE
      ///* `steuernummer` - STEUERNUMMER
      ``type``: Newtonsoft.Json.Linq.JToken
      ///Identifier value string validated by country-specific rules.
      value: string }
    ///Creates an instance of BusinessIdentifier with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int, ``type``: Newtonsoft.Json.Linq.JToken, value: string): BusinessIdentifier =
        { id = id
          ``type`` = ``type``
          value = value }

type BusinessIdentifierRequest =
    { ///Identifier type enum. Allowed values: `y_tunnus`, `vat_code`, `steuernummer`.
      ///* `y_tunnus` - Y_TUNNUS
      ///* `vat_code` - VAT_CODE
      ///* `steuernummer` - STEUERNUMMER
      ``type``: Newtonsoft.Json.Linq.JToken
      ///Identifier value string validated by country-specific rules.
      value: string }
    ///Creates an instance of BusinessIdentifierRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (``type``: Newtonsoft.Json.Linq.JToken, value: string): BusinessIdentifierRequest =
        { ``type`` = ``type``; value = value }

type BusinessPermission =
    { ///Permission identifier.
      id: string
      ///Localized permission name.
      name: string
      ///Permission description.
      explanation: string }
    ///Creates an instance of BusinessPermission with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: string, name: string, explanation: string): BusinessPermission =
        { id = id
          name = name
          explanation = explanation }

type BusinessPermissionsResponse =
    { ///Selected business slug.
      business_slug: string
      ///Granted permission identifiers for the current user in this business.
      granted_permission_ids: list<string>
      ///Detailed granted permissions for the current user in this business.
      granted_permissions: list<BusinessPermission> }
    ///Creates an instance of BusinessPermissionsResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (business_slug: string,
                          granted_permission_ids: list<string>,
                          granted_permissions: list<BusinessPermission>): BusinessPermissionsResponse =
        { business_slug = business_slug
          granted_permission_ids = granted_permission_ids
          granted_permissions = granted_permissions }

type BusinessRequest =
    { name: string
      slug: Option<string>
      business_id: Option<string>
      ///* `NET` - ALV netotus
      ///* `SPLIT` - ALV jaettu kirjaus
      vat_posting_method: Option<VatPostingMethodEnum>
      ///* `DEFAULT` - ALV kirjanpidon päivälle
      ///* `DE_CASH_BASIS` - ALV maksupäivänä (DE maksu)
      vat_reporting_method: Option<VatReportingMethodEnum>
      ///* `FI_OY` - Osakeyhtiö (Oy)
      ///* `FI_OYJ` - Julkinen osakeyhtiö (Oyj)
      ///* `FI_TMI` - Toiminimi (Tmi)
      ///* `FI_KY` - Kommandiittiyhtiö (Ky)
      ///* `FI_OK` - Osuuskunta (Osk)
      ///* `FI_AY` - Avoin yhtiö (AY)
      ///* `FI_AS_OY` - Asunto-osakeyhtiö (As Oy)
      ///* `FI_KOY` - Kiinteistöosakeyhtiö (Koy)
      ///* `FI_NY` - NYT-yritys
      ///* `FI_YHD` - Yhdistys (ry)
      ///* `DE_UG` - Saksalainen yrittäjäosakeyhtiö (UG)
      ///* `DEMO` - Demo-yritys
      form: Option<FormEnum>
      default_vat_period: Option<Newtonsoft.Json.Linq.JToken>
      invoicing_default_penalty_interest: Option<float>
      invoicing_default_payment_condition_days: Option<int>
      invoicing_default_email_subject: Option<string>
      invoicing_default_email_content: Option<string>
      invoicing_vat_exemption_reason: Option<string>
      has_accrual_based_entries_for_invoicing: Option<bool>
      has_history_before_nocfo: Option<bool>
      fiscal_period_needs_review: Option<bool>
      vat_period_needs_review: Option<bool>
      contact_phone: Option<string>
      invoicing_email: Option<string>
      invoicing_iban: Option<string>
      invoicing_bic: Option<string>
      invoicing_contact: Option<string>
      invoicing_street: Option<string>
      invoicing_city: Option<string>
      invoicing_postal_code: Option<string>
      invoicing_country: Option<string>
      business_street: Option<string>
      business_city: Option<string>
      business_postal_code: Option<string>
      business_country: Option<string>
      account_yle_tax: Option<int>
      account_tax_deferrals: Option<int>
      account_income_tax_rec: Option<int>
      account_income_tax_lia: Option<int>
      account_previous_profit: Option<int>
      account_vat_receivables: Option<int>
      account_vat_liabilities: Option<int>
      account_vat_accrual_adjustment: Option<int>
      account_trade_receivables: Option<int>
      account_payables: Option<int>
      onboarding_updated_invoicing_settings: Option<bool>
      onboarding_created_invoicing_contact: Option<bool>
      onboarding_created_invoicing_product: Option<bool>
      onboarding_registered_to_kravia: Option<bool>
      einvoicing_enabled: Option<bool>
      holvi_bookkeeping_api_enabled: Option<bool>
      holvi_nocfo_connection_enabled: Option<bool>
      verohallinto_latest_reporter_full_name: Option<string>
      verohallinto_latest_reporter_phone_number: Option<string>
      is_automatic_debt_collection_enabled: Option<bool> }
    ///Creates an instance of BusinessRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string): BusinessRequest =
        { name = name
          slug = None
          business_id = None
          vat_posting_method = None
          vat_reporting_method = None
          form = None
          default_vat_period = None
          invoicing_default_penalty_interest = None
          invoicing_default_payment_condition_days = None
          invoicing_default_email_subject = None
          invoicing_default_email_content = None
          invoicing_vat_exemption_reason = None
          has_accrual_based_entries_for_invoicing = None
          has_history_before_nocfo = None
          fiscal_period_needs_review = None
          vat_period_needs_review = None
          contact_phone = None
          invoicing_email = None
          invoicing_iban = None
          invoicing_bic = None
          invoicing_contact = None
          invoicing_street = None
          invoicing_city = None
          invoicing_postal_code = None
          invoicing_country = None
          business_street = None
          business_city = None
          business_postal_code = None
          business_country = None
          account_yle_tax = None
          account_tax_deferrals = None
          account_income_tax_rec = None
          account_income_tax_lia = None
          account_previous_profit = None
          account_vat_receivables = None
          account_vat_liabilities = None
          account_vat_accrual_adjustment = None
          account_trade_receivables = None
          account_payables = None
          onboarding_updated_invoicing_settings = None
          onboarding_created_invoicing_contact = None
          onboarding_created_invoicing_product = None
          onboarding_registered_to_kravia = None
          einvoicing_enabled = None
          holvi_bookkeeping_api_enabled = None
          holvi_nocfo_connection_enabled = None
          verohallinto_latest_reporter_full_name = None
          verohallinto_latest_reporter_phone_number = None
          is_automatic_debt_collection_enabled = None }

type BusinessUsership =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      user: int
      business: int
      business_id: string
      permissions: NocfoPermission }
    ///Creates an instance of BusinessUsership with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          user: int,
                          business: int,
                          business_id: string,
                          permissions: NocfoPermission): BusinessUsership =
        { id = id
          created_at = created_at
          updated_at = updated_at
          user = user
          business = business
          business_id = business_id
          permissions = permissions }

type BusinessUsershipRequest =
    { user: int
      permissions: NocfoPermissionRequest }
    ///Creates an instance of BusinessUsershipRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (user: int, permissions: NocfoPermissionRequest): BusinessUsershipRequest =
        { user = user
          permissions = permissions }

type Contact =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      customer_id: Option<string>
      ///* `UNSET` - Ei asetettu
      ///* `PERSON` - Yksityishenkilö
      ///* `BUSINESS` - Yritys
      ``type``: Option<ContactTypeEnum>
      name: string
      name_aliases: Option<list<string>>
      contact_business_id: Option<string>
      notes: Option<string>
      phone_number: Option<string>
      is_invoicing_enabled: Option<bool>
      invoicing_email: Option<string>
      invoicing_einvoice_address: Option<string>
      invoicing_einvoice_operator: Option<string>
      invoicing_tax_code: Option<string>
      invoicing_street: Option<string>
      invoicing_city: Option<string>
      invoicing_postal_code: Option<string>
      invoicing_country: Option<string>
      ///* `fi` - Suomi
      ///* `en` - Englanti
      ///* `sv` - Ruotsi
      invoicing_language: Option<InvoicingLanguageEnum>
      can_be_invoiced: bool
      can_be_invoiced_via_email: bool
      can_be_invoiced_via_einvoice: bool }
    ///Creates an instance of Contact with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          name: string,
                          can_be_invoiced: bool,
                          can_be_invoiced_via_email: bool,
                          can_be_invoiced_via_einvoice: bool): Contact =
        { id = id
          created_at = created_at
          updated_at = updated_at
          customer_id = None
          ``type`` = None
          name = name
          name_aliases = None
          contact_business_id = None
          notes = None
          phone_number = None
          is_invoicing_enabled = None
          invoicing_email = None
          invoicing_einvoice_address = None
          invoicing_einvoice_operator = None
          invoicing_tax_code = None
          invoicing_street = None
          invoicing_city = None
          invoicing_postal_code = None
          invoicing_country = None
          invoicing_language = None
          can_be_invoiced = can_be_invoiced
          can_be_invoiced_via_email = can_be_invoiced_via_email
          can_be_invoiced_via_einvoice = can_be_invoiced_via_einvoice }

type ContactRequest =
    { customer_id: Option<string>
      ///* `UNSET` - Ei asetettu
      ///* `PERSON` - Yksityishenkilö
      ///* `BUSINESS` - Yritys
      ``type``: Option<ContactTypeEnum>
      name: string
      name_aliases: Option<list<string>>
      contact_business_id: Option<string>
      notes: Option<string>
      phone_number: Option<string>
      is_invoicing_enabled: Option<bool>
      invoicing_email: Option<string>
      invoicing_einvoice_address: Option<string>
      invoicing_einvoice_operator: Option<string>
      invoicing_tax_code: Option<string>
      invoicing_street: Option<string>
      invoicing_city: Option<string>
      invoicing_postal_code: Option<string>
      invoicing_country: Option<string>
      ///* `fi` - Suomi
      ///* `en` - Englanti
      ///* `sv` - Ruotsi
      invoicing_language: Option<InvoicingLanguageEnum> }
    ///Creates an instance of ContactRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string): ContactRequest =
        { customer_id = None
          ``type`` = None
          name = name
          name_aliases = None
          contact_business_id = None
          notes = None
          phone_number = None
          is_invoicing_enabled = None
          invoicing_email = None
          invoicing_einvoice_address = None
          invoicing_einvoice_operator = None
          invoicing_tax_code = None
          invoicing_street = None
          invoicing_city = None
          invoicing_postal_code = None
          invoicing_country = None
          invoicing_language = None }

type CreditorInfo =
    { name: string
      iban: string
      bic: Option<string> }
    ///Creates an instance of CreditorInfo with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, iban: string): CreditorInfo = { name = name; iban = iban; bic = None }

type CreditorInfoRequest =
    { name: string
      iban: string
      bic: Option<string> }
    ///Creates an instance of CreditorInfoRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, iban: string): CreditorInfoRequest = { name = name; iban = iban; bic = None }

type CurrentUser =
    { id: int
      email: Option<string>
      first_name: Option<string>
      last_name: Option<string>
      last_login: Option<System.DateTimeOffset>
      date_joined: System.DateTimeOffset
      tos_accepted: bool
      saltedge_tos_accepted_at: Option<System.DateTimeOffset>
      last_csat: Option<System.DateTimeOffset>
      invitations: list<Invitation>
      notifications: Newtonsoft.Json.Linq.JToken
      avatar_url: Option<string>
      userships: list<BusinessUsership>
      show_get_started_info: Option<bool>
      language: Option<Newtonsoft.Json.Linq.JToken>
      last_active_business_id: Option<string>
      last_active_business_slug: Option<string>
      subscription_tier: string
      has_tos_acceptance_record: bool }
    ///Creates an instance of CurrentUser with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          last_login: Option<System.DateTimeOffset>,
                          date_joined: System.DateTimeOffset,
                          tos_accepted: bool,
                          saltedge_tos_accepted_at: Option<System.DateTimeOffset>,
                          last_csat: Option<System.DateTimeOffset>,
                          invitations: list<Invitation>,
                          notifications: Newtonsoft.Json.Linq.JToken,
                          avatar_url: Option<string>,
                          userships: list<BusinessUsership>,
                          last_active_business_id: Option<string>,
                          last_active_business_slug: Option<string>,
                          subscription_tier: string,
                          has_tos_acceptance_record: bool): CurrentUser =
        { id = id
          email = None
          first_name = None
          last_name = None
          last_login = last_login
          date_joined = date_joined
          tos_accepted = tos_accepted
          saltedge_tos_accepted_at = saltedge_tos_accepted_at
          last_csat = last_csat
          invitations = invitations
          notifications = notifications
          avatar_url = avatar_url
          userships = userships
          show_get_started_info = None
          language = None
          last_active_business_id = last_active_business_id
          last_active_business_slug = last_active_business_slug
          subscription_tier = subscription_tier
          has_tos_acceptance_record = has_tos_acceptance_record }

type CurrentUserRequest =
    { email: Option<string>
      first_name: Option<string>
      last_name: Option<string>
      show_get_started_info: Option<bool>
      language: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of CurrentUserRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): CurrentUserRequest =
        { email = None
          first_name = None
          last_name = None
          show_get_started_info = None
          language = None }

type DateRangeReportColumnSchemaRequest =
    { ///Optional column label shown in report output.
      name: Option<string>
      ///Start date for range-based reports (YYYY-MM-DD).
      date_from: string
      ///End date for range-based reports (YYYY-MM-DD).
      date_to: string }
    ///Creates an instance of DateRangeReportColumnSchemaRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (date_from: string, date_to: string): DateRangeReportColumnSchemaRequest =
        { name = None
          date_from = date_from
          date_to = date_to }

type DateRangeTypedReportRequestSchemaRequest =
    { ///When true, include account-level drill-down rows for supported reports.
      extend_accounts: Option<bool>
      ///When true and column count is small enough, add comparison columns.
      append_comparison_columns: Option<bool>
      ///Optional tag filters. Only entries/documents matching these tags are included in report calculations.
      tag_ids: Option<list<int>>
      ///Columns for period reports. Use `date_from` and `date_to` for each column.
      columns: list<DateRangeReportColumnSchemaRequest> }
    ///Creates an instance of DateRangeTypedReportRequestSchemaRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (columns: list<DateRangeReportColumnSchemaRequest>): DateRangeTypedReportRequestSchemaRequest =
        { extend_accounts = None
          append_comparison_columns = None
          tag_ids = None
          columns = columns }

type DocumentAttachmentIdsRequest =
    { ///List of attachment IDs to attach or detach from the document.
      attachment_ids: list<int> }
    ///Creates an instance of DocumentAttachmentIdsRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (attachment_ids: list<int>): DocumentAttachmentIdsRequest = { attachment_ids = attachment_ids }

type DocumentImport =
    { id: int
      ///Unique identifier attached to logs. For debugging purposes.
      trace_id: Option<System.Guid>
      ///* `INITIAL` - INITIAL
      ///* `PENDING` - PENDING
      ///* `RUNNING` - RUNNING
      ///* `SUCCESS` - SUCCESS
      ///* `FAILURE` - FAILURE
      algo_status: Option<AlgoStatusEnum>
      data_source: Option<string>
      identifier: Option<string>
      date: string
      amount: float32
      description: Option<string>
      contact_hint: Option<string>
      balance_after_transaction: Option<float32>
      reference_number: Option<string>
      payment_account_id: Option<int> }
    ///Creates an instance of DocumentImport with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          trace_id: Option<System.Guid>,
                          data_source: Option<string>,
                          date: string,
                          amount: float32,
                          balance_after_transaction: Option<float32>,
                          payment_account_id: Option<int>): DocumentImport =
        { id = id
          trace_id = trace_id
          algo_status = None
          data_source = data_source
          identifier = None
          date = date
          amount = amount
          description = None
          contact_hint = None
          balance_after_transaction = balance_after_transaction
          reference_number = None
          payment_account_id = payment_account_id }

type DocumentImportRequest =
    { ///* `INITIAL` - INITIAL
      ///* `PENDING` - PENDING
      ///* `RUNNING` - RUNNING
      ///* `SUCCESS` - SUCCESS
      ///* `FAILURE` - FAILURE
      algo_status: Option<AlgoStatusEnum>
      identifier: Option<string>
      date: string
      amount: float32
      description: Option<string>
      contact_hint: Option<string>
      reference_number: Option<string> }
    ///Creates an instance of DocumentImportRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (date: string, amount: float32): DocumentImportRequest =
        { algo_status = None
          identifier = None
          date = date
          amount = amount
          description = None
          contact_hint = None
          reference_number = None }

type DocumentInstance =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      ///The document number is an automatically generated number that identifies the document in the accounting system.
      number: Option<string>
      ///Indicates whether the document is flagged for review or attention.
      is_flagged: Option<bool>
      ///Indicates whether the document is locked and cannot be modified.
      is_locked: bool
      ///Indicates whether the document is a draft. Draft documents are not yet finalized.
      is_draft: Option<bool>
      ///The date of the document. This is the date when the document was created or issued.
      date: Option<string>
      ///A brief description of the document.
      description: Option<string>
      ///Contact ID associated with this document.
      contact_id: Option<int>
      ///Contact name associated with this document.
      contact_name: Option<string>
      ///Accounting period ID to which this document belongs.
      period: Option<string>
      ///Total balance of the document. This value represents the net effect on accounts of type "bank account." Note that this value is typically zero if no money has yet moved, for example, when recording accounts receivable. Once money is received or paid via a bank account, the amount will be shown in this field.
      balance: float32
      ///IDs of attachments associated with this document.
      attachment_ids: Option<list<int>>
      ///The type of the document.
      ///* `0` - Normaali kirjaus
      ///* `3` - Arvonlisäverokirjaus
      ///* `4` - Poisto
      ///* `6` - YLE-vero
      ///* `7` - Tulovero
      ``type``: Option<Newtonsoft.Json.Linq.JToken>
      ///Accounting blueprint used to generate document entries. Whenever this payload changes, entries are recalculated from it. See `bookkeeping_blueprint_retrieve` for full field-level guidance.
      blueprint: Option<Newtonsoft.Json.Linq.JToken>
      ///Read-only blueprint classification derived from blueprint content. Possible values: `SALES`, `PURCHASE`, `MANUAL`.
      blueprint_type: string
      ///Import data e.g. CSV or bank transaction data associated with this document.
      import_data: Option<Newtonsoft.Json.Linq.JToken>
      ///Deprecated: Indicates whether the description of this document was generated by AI.
      has_ai_generated_description: bool
      ///UI rows for entries in this document, used for rendering the entry UI.
      entry_ui_rows: list<EntryUIRows>
      ///Last time the user has visited this document's activity feed.
      audit_trail_last_seen_at: Option<System.DateTimeOffset>
      ///Indicates whether there are unseen actions in the activity feed for this document.
      audit_trail_has_unseen_actions: Option<bool>
      ///Last time the user was mentioned in the activity feed of this document.
      audit_trail_last_mention_at: Option<System.DateTimeOffset>
      ///Last time the user has commented in the activity feed of this document.
      audit_trail_last_comment_at: Option<System.DateTimeOffset>
      ///List of tag IDs associated with this document.
      tag_ids: Option<list<int>>
      ///Combined list of incoming and outgoing document relations.
      relations: Option<list<DocumentRelation>>
      has_accrual_pair_due_mismatch: Option<bool>
      shortcuts: list<Shortcut> }
    ///Creates an instance of DocumentInstance with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          number: Option<string>,
                          is_locked: bool,
                          contact_name: Option<string>,
                          period: Option<string>,
                          balance: float32,
                          blueprint_type: string,
                          import_data: Option<Newtonsoft.Json.Linq.JToken>,
                          has_ai_generated_description: bool,
                          entry_ui_rows: list<EntryUIRows>,
                          audit_trail_last_seen_at: Option<System.DateTimeOffset>,
                          audit_trail_has_unseen_actions: Option<bool>,
                          audit_trail_last_mention_at: Option<System.DateTimeOffset>,
                          audit_trail_last_comment_at: Option<System.DateTimeOffset>,
                          relations: Option<list<DocumentRelation>>,
                          has_accrual_pair_due_mismatch: Option<bool>,
                          shortcuts: list<Shortcut>): DocumentInstance =
        { id = id
          created_at = created_at
          updated_at = updated_at
          number = number
          is_flagged = None
          is_locked = is_locked
          is_draft = None
          date = None
          description = None
          contact_id = None
          contact_name = contact_name
          period = period
          balance = balance
          attachment_ids = None
          ``type`` = None
          blueprint = None
          blueprint_type = blueprint_type
          import_data = import_data
          has_ai_generated_description = has_ai_generated_description
          entry_ui_rows = entry_ui_rows
          audit_trail_last_seen_at = audit_trail_last_seen_at
          audit_trail_has_unseen_actions = audit_trail_has_unseen_actions
          audit_trail_last_mention_at = audit_trail_last_mention_at
          audit_trail_last_comment_at = audit_trail_last_comment_at
          tag_ids = None
          relations = relations
          has_accrual_pair_due_mismatch = has_accrual_pair_due_mismatch
          shortcuts = shortcuts }

type DocumentInstanceRequest =
    { ///Indicates whether the document is flagged for review or attention.
      is_flagged: Option<bool>
      ///Indicates whether the document is a draft. Draft documents are not yet finalized.
      is_draft: Option<bool>
      ///The date of the document. This is the date when the document was created or issued.
      date: Option<string>
      ///A brief description of the document.
      description: Option<string>
      ///Contact ID associated with this document.
      contact_id: Option<int>
      ///IDs of attachments associated with this document.
      attachment_ids: Option<list<int>>
      ///The type of the document.
      ///* `0` - Normaali kirjaus
      ///* `3` - Arvonlisäverokirjaus
      ///* `4` - Poisto
      ///* `6` - YLE-vero
      ///* `7` - Tulovero
      ``type``: Option<Newtonsoft.Json.Linq.JToken>
      ///Accounting blueprint used to generate document entries. Whenever this payload changes, entries are recalculated from it. See `bookkeeping_blueprint_retrieve` for full field-level guidance.
      blueprint: Option<Newtonsoft.Json.Linq.JToken>
      ///List of tag IDs associated with this document.
      tag_ids: Option<list<int>> }
    ///Creates an instance of DocumentInstanceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): DocumentInstanceRequest =
        { is_flagged = None
          is_draft = None
          date = None
          description = None
          contact_id = None
          attachment_ids = None
          ``type`` = None
          blueprint = None
          tag_ids = None }

type DocumentList =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      ///The document number is an automatically generated number that identifies the document in the accounting system.
      number: Option<string>
      ///Indicates whether the document is flagged for review or attention.
      is_flagged: Option<bool>
      ///Indicates whether the document is locked and cannot be modified.
      is_locked: bool
      ///Indicates whether the document is a draft. Draft documents are not yet finalized.
      is_draft: Option<bool>
      ///The date of the document. This is the date when the document was created or issued.
      date: Option<string>
      ///A brief description of the document.
      description: Option<string>
      ///Contact ID associated with this document.
      contact_id: Option<int>
      ///Contact name associated with this document.
      contact_name: Option<string>
      ///Accounting period ID to which this document belongs.
      period: Option<string>
      ///Total balance of the document. This value represents the net effect on accounts of type "bank account." Note that this value is typically zero if no money has yet moved, for example, when recording accounts receivable. Once money is received or paid via a bank account, the amount will be shown in this field.
      balance: float32
      ///IDs of attachments associated with this document.
      attachment_ids: Option<list<int>>
      ///The type of the document.
      ///* `0` - Normaali kirjaus
      ///* `3` - Arvonlisäverokirjaus
      ///* `4` - Poisto
      ///* `6` - YLE-vero
      ///* `7` - Tulovero
      ``type``: Option<Newtonsoft.Json.Linq.JToken>
      ///Accounting blueprint used to generate document entries. Whenever this payload changes, entries are recalculated from it. See `bookkeeping_blueprint_retrieve` for full field-level guidance.
      blueprint: Option<Newtonsoft.Json.Linq.JToken>
      ///Read-only blueprint classification derived from blueprint content. Possible values: `SALES`, `PURCHASE`, `MANUAL`.
      blueprint_type: string
      ///Import data e.g. CSV or bank transaction data associated with this document.
      import_data: Option<Newtonsoft.Json.Linq.JToken>
      ///Deprecated: Indicates whether the description of this document was generated by AI.
      has_ai_generated_description: bool
      ///UI rows for entries in this document, used for rendering the entry UI.
      entry_ui_rows: list<EntryUIRows>
      ///Last time the user has visited this document's activity feed.
      audit_trail_last_seen_at: Option<System.DateTimeOffset>
      ///Indicates whether there are unseen actions in the activity feed for this document.
      audit_trail_has_unseen_actions: Option<bool>
      ///Last time the user was mentioned in the activity feed of this document.
      audit_trail_last_mention_at: Option<System.DateTimeOffset>
      ///Last time the user has commented in the activity feed of this document.
      audit_trail_last_comment_at: Option<System.DateTimeOffset>
      ///List of tag IDs associated with this document.
      tag_ids: Option<list<int>>
      ///Combined list of incoming and outgoing document relations.
      relations: Option<list<DocumentRelation>>
      has_accrual_pair_due_mismatch: Option<bool> }
    ///Creates an instance of DocumentList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          number: Option<string>,
                          is_locked: bool,
                          contact_name: Option<string>,
                          period: Option<string>,
                          balance: float32,
                          blueprint_type: string,
                          import_data: Option<Newtonsoft.Json.Linq.JToken>,
                          has_ai_generated_description: bool,
                          entry_ui_rows: list<EntryUIRows>,
                          audit_trail_last_seen_at: Option<System.DateTimeOffset>,
                          audit_trail_has_unseen_actions: Option<bool>,
                          audit_trail_last_mention_at: Option<System.DateTimeOffset>,
                          audit_trail_last_comment_at: Option<System.DateTimeOffset>,
                          relations: Option<list<DocumentRelation>>,
                          has_accrual_pair_due_mismatch: Option<bool>): DocumentList =
        { id = id
          created_at = created_at
          updated_at = updated_at
          number = number
          is_flagged = None
          is_locked = is_locked
          is_draft = None
          date = None
          description = None
          contact_id = None
          contact_name = contact_name
          period = period
          balance = balance
          attachment_ids = None
          ``type`` = None
          blueprint = None
          blueprint_type = blueprint_type
          import_data = import_data
          has_ai_generated_description = has_ai_generated_description
          entry_ui_rows = entry_ui_rows
          audit_trail_last_seen_at = audit_trail_last_seen_at
          audit_trail_has_unseen_actions = audit_trail_has_unseen_actions
          audit_trail_last_mention_at = audit_trail_last_mention_at
          audit_trail_last_comment_at = audit_trail_last_comment_at
          tag_ids = None
          relations = relations
          has_accrual_pair_due_mismatch = has_accrual_pair_due_mismatch }

type DocumentListRequest =
    { ///Indicates whether the document is flagged for review or attention.
      is_flagged: Option<bool>
      ///Indicates whether the document is a draft. Draft documents are not yet finalized.
      is_draft: Option<bool>
      ///The date of the document. This is the date when the document was created or issued.
      date: Option<string>
      ///A brief description of the document.
      description: Option<string>
      ///Contact ID associated with this document.
      contact_id: Option<int>
      ///IDs of attachments associated with this document.
      attachment_ids: Option<list<int>>
      ///The type of the document.
      ///* `0` - Normaali kirjaus
      ///* `3` - Arvonlisäverokirjaus
      ///* `4` - Poisto
      ///* `6` - YLE-vero
      ///* `7` - Tulovero
      ``type``: Option<Newtonsoft.Json.Linq.JToken>
      ///Accounting blueprint used to generate document entries. Whenever this payload changes, entries are recalculated from it. See `bookkeeping_blueprint_retrieve` for full field-level guidance.
      blueprint: Option<Newtonsoft.Json.Linq.JToken>
      ///List of tag IDs associated with this document.
      tag_ids: Option<list<int>> }
    ///Creates an instance of DocumentListRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): DocumentListRequest =
        { is_flagged = None
          is_draft = None
          date = None
          description = None
          contact_id = None
          attachment_ids = None
          ``type`` = None
          blueprint = None
          tag_ids = None }

type DocumentRelation =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      ///The related document of this relation.
      related_document: Option<int>
      ///Description of the related (target) document.
      description: string
      ///Semantic role of the current document within this relation.
      ///* `ACCRUAL` - ACCRUAL
      ///* `SETTLEMENT` - SETTLEMENT
      role: Option<Newtonsoft.Json.Linq.JToken>
      ///Type of relation that links the documents.
      ///* `ACCRUAL_PAIR` - ACCRUAL_PAIR
      ``type``: Newtonsoft.Json.Linq.JToken }
    ///Creates an instance of DocumentRelation with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          description: string,
                          ``type``: Newtonsoft.Json.Linq.JToken): DocumentRelation =
        { id = id
          created_at = created_at
          updated_at = updated_at
          related_document = None
          description = description
          role = None
          ``type`` = ``type`` }

type DocumentRelationRequest =
    { ///The related document of this relation.
      related_document: Option<int>
      ///Semantic role of the current document within this relation.
      ///* `ACCRUAL` - ACCRUAL
      ///* `SETTLEMENT` - SETTLEMENT
      role: Option<Newtonsoft.Json.Linq.JToken>
      ///Type of relation that links the documents.
      ///* `ACCRUAL_PAIR` - ACCRUAL_PAIR
      ``type``: Newtonsoft.Json.Linq.JToken }
    ///Creates an instance of DocumentRelationRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (``type``: Newtonsoft.Json.Linq.JToken): DocumentRelationRequest =
        { related_document = None
          role = None
          ``type`` = ``type`` }

type DocumentRelationSuggestion =
    { related_document: int
      ///* `ACCRUAL` - ACCRUAL
      ///* `SETTLEMENT` - SETTLEMENT
      role: RoleEnum
      ///* `ACCRUAL_PAIR` - ACCRUAL_PAIR
      ``type``: Type0a6Enum
      reason: string
      score: int }
    ///Creates an instance of DocumentRelationSuggestion with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (related_document: int, role: RoleEnum, ``type``: Type0a6Enum, reason: string, score: int): DocumentRelationSuggestion =
        { related_document = related_document
          role = role
          ``type`` = ``type``
          reason = reason
          score = score }

type DocumentRelationSuggestionPreviewRequest =
    { date: Option<string>
      contact_id: Option<int>
      blueprint: Option<BlueprintRequest>
      exclude_document_ids: Option<list<int>> }
    ///Creates an instance of DocumentRelationSuggestionPreviewRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): DocumentRelationSuggestionPreviewRequest =
        { date = None
          contact_id = None
          blueprint = None
          exclude_document_ids = None }

type Entry =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      ///The account ID associated with this entry.
      account: Option<int>
      ///The name of the account associated with this entry.
      account_name: string
      ///The number of the account associated with this entry.
      account_number: string
      amount: Option<float32>
      ///Indicates whether this entry is a debit (True) or credit (False).
      is_debet: Option<bool>
      ///Description of the entry, providing additional context or details.
      description: Option<string>
      ///The VAT code of this entry.
      ///* `1` - Kotimaan verollinen myynti
      ///* `2` - Kotimaan verollinen osto
      ///* `3` - Veroton
      ///* `4` - Nollaverokannan myynti
      ///* `5` - Tavaroiden myynti EU-maihin
      ///* `6` - Palveluiden myynti EU-maihin
      ///* `7` - Tavaroiden ostot EU-maista
      ///* `8` - Palveluiden ostot EU-maista
      ///* `9` - Tavaroiden maahantuonti EU:n ulkopuolelta
      ///* `15` - Tavaroiden maahantuonti EU:n ulkopuolelta
      ///* `14` - Palveluostot EU:n ulkopuolelta
      ///* `10` - Rakennuspalveluiden myynti
      ///* `11` - Rakennuspalveluiden osto
      ///* `12` - Palvelumyynnit EU:n ulkopuolelle
      ///* `13` - Tavaroiden myynti EU:n ulkopuolelle
      vat_code: Option<Newtonsoft.Json.Linq.JToken>
      ///The VAT rate of this entry.
      vat_rate: Option<string> }
    ///Creates an instance of Entry with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          account_name: string,
                          account_number: string): Entry =
        { id = id
          created_at = created_at
          updated_at = updated_at
          account = None
          account_name = account_name
          account_number = account_number
          amount = None
          is_debet = None
          description = None
          vat_code = None
          vat_rate = None }

type EntryUIRows =
    { account_name: string
      account_number: string
      vat_code: int
      vat_rate: string
      is_debet: bool
      amount: float32 }
    ///Creates an instance of EntryUIRows with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (account_name: string,
                          account_number: string,
                          vat_code: int,
                          vat_rate: string,
                          is_debet: bool,
                          amount: float32): EntryUIRows =
        { account_name = account_name
          account_number = account_number
          vat_code = vat_code
          vat_rate = vat_rate
          is_debet = is_debet
          amount = amount }

type FileUploadJsonRequestRequest =
    { name: string
      ///File payload as base64-encoded content.
      file: string
      ``type``: Option<string>
      folder_id: Option<int> }
    ///Creates an instance of FileUploadJsonRequestRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, file: string): FileUploadJsonRequestRequest =
        { name = name
          file = file
          ``type`` = None
          folder_id = None }

type FileUploadRequestRequest =
    { name: string
      file: string
      ``type``: Option<string>
      folder_id: Option<int> }
    ///Creates an instance of FileUploadRequestRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, file: string): FileUploadRequestRequest =
        { name = name
          file = file
          ``type`` = None
          folder_id = None }

type GetJwtTokenRequestRequest =
    { business_slug: Option<string> }
    ///Creates an instance of GetJwtTokenRequestRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): GetJwtTokenRequestRequest = { business_slug = None }

type GetJwtTokenResponse =
    { token: string }
    ///Creates an instance of GetJwtTokenResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (token: string): GetJwtTokenResponse = { token = token }

type HeaderListNametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of HeaderListNametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): HeaderListNametranslations = { key = key; value = value }

type HeaderList =
    { id: int
      name: string
      ///List of translations. Each item maps one language code to the translated text.
      name_translations: list<HeaderListNametranslations>
      ///* `B` - BALANCE_SHEET
      ///* `I` - INCOME_STATEMENT
      ``type``: HeaderListTypeEnum
      parent_id: Option<int>
      parent_ids: list<int>
      level: Option<int> }
    ///Creates an instance of HeaderList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          name: string,
                          name_translations: list<HeaderListNametranslations>,
                          ``type``: HeaderListTypeEnum,
                          parent_id: Option<int>,
                          parent_ids: list<int>,
                          level: Option<int>): HeaderList =
        { id = id
          name = name
          name_translations = name_translations
          ``type`` = ``type``
          parent_id = parent_id
          parent_ids = parent_ids
          level = level }

type HeaderListRequestNametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of HeaderListRequestNametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): HeaderListRequestNametranslations = { key = key; value = value }

type HeaderListRequest =
    { ///List of translations. Each item maps one language code to the translated text.
      name_translations: list<HeaderListRequestNametranslations>
      ///* `B` - BALANCE_SHEET
      ///* `I` - INCOME_STATEMENT
      ``type``: HeaderListTypeEnum }
    ///Creates an instance of HeaderListRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name_translations: list<HeaderListRequestNametranslations>, ``type``: HeaderListTypeEnum): HeaderListRequest =
        { name_translations = name_translations
          ``type`` = ``type`` }

type Identifier =
    { ///Human-readable business identifier key label.
      key: string
      ///Business identifier type key.
      ``type``: string
      ///Business identifier value.
      value: string }
    ///Creates an instance of Identifier with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, ``type``: string, value: string): Identifier =
        { key = key
          ``type`` = ``type``
          value = value }

type Invitation =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      email: string
      ///* `OPEN` - Avoin
      ///* `ACCEPTED` - Hyväksytty
      ///* `REJECTED` - Hylätty
      status: Option<InvitationStatusEnum>
      ///        Business specific permissions for this invitation.
      ///        If no permission group is provided, this can be
      ///        used as a fallback for custom permissions.
      permissions: Option<list<PermissionsEnum>>
      ///The permission group for the invitation.
      ///* `admin` - Pääkäyttäjä
      ///* `read_only` - Lukuoikeus
      ///* `bookkeeping` - Kirjanpito
      ///* `invoicing` - Laskutus
      ///* `custom` - Omavalinta
      permission_group: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of Invitation with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int, created_at: System.DateTimeOffset, updated_at: System.DateTimeOffset, email: string): Invitation =
        { id = id
          created_at = created_at
          updated_at = updated_at
          email = email
          status = None
          permissions = None
          permission_group = None }

type InvitationRequest =
    { email: string
      ///* `OPEN` - Avoin
      ///* `ACCEPTED` - Hyväksytty
      ///* `REJECTED` - Hylätty
      status: Option<InvitationStatusEnum>
      ///        Business specific permissions for this invitation.
      ///        If no permission group is provided, this can be
      ///        used as a fallback for custom permissions.
      permissions: Option<list<PermissionsEnum>>
      ///The permission group for the invitation.
      ///* `admin` - Pääkäyttäjä
      ///* `read_only` - Lukuoikeus
      ///* `bookkeeping` - Kirjanpito
      ///* `invoicing` - Laskutus
      ///* `custom` - Omavalinta
      permission_group: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of InvitationRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (email: string): InvitationRequest =
        { email = email
          status = None
          permissions = None
          permission_group = None }

type Invoice =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      friendly_name: string
      receiver: int
      receiver_info: Newtonsoft.Json.Linq.JToken
      contact_person: Option<string>
      invoice_number: Option<int>
      invoicing_date: string
      payment_condition_days: int
      due_date: string
      reference: Option<string>
      penalty_interest: Option<float>
      currency: string
      vat_country: string
      description: Option<string>
      is_credit_note_for: Option<int>
      credit_notes: list<int>
      pdf: Newtonsoft.Json.Linq.JToken
      rows: Option<list<InvoiceRow>>
      ///Invoice status enum. Values: DRAFT, ACCEPTED, PAID, CREDIT_LOSS.
      ///* `DRAFT` - Luonnos
      ///* `ACCEPTED` - Hyväksytty
      ///* `PAID` - Maksettu
      ///* `CREDIT_LOSS` - Luottotappio
      status: Option<Newtonsoft.Json.Linq.JToken>
      total_vat_amount: float32
      total_amount: float32
      is_editable: bool
      is_active_recurrence_template: Option<bool>
      is_recurrence_active: bool
      is_recurrence_end_exceeded: Option<bool>
      ///Tells if invoice was created from recurring invoice template
      ///or if invoice has some fulfilled recurrences. Property is mostly
      ///used in UI to show icon for recurring invoices.
      is_recurrence_applied: bool
      is_past_due: bool
      delivery_method: Option<Newtonsoft.Json.Linq.JToken>
      is_sendable: bool
      last_delivery_at: Option<System.DateTimeOffset>
      recurrence_rule: Option<Newtonsoft.Json.Linq.JToken>
      ///Invoice that this invoice is based on
      recurrence_parent_id: Option<int>
      ///Method for calculating the reference
      ///* `KEEP` - Keep
      ///* `ROLL` - Roll
      recurrence_reference_method: Option<Newtonsoft.Json.Linq.JToken>
      recurrence_email_subject: Option<string>
      recurrence_email_content: Option<string>
      recurrence_end: Option<string>
      next_recurrences: list<string>
      is_registered_to_kravia: bool
      earliest_debt_collection_date: string
      attachments: Option<list<int>>
      settlement_date: Option<string>
      ///Viiteemme
      seller_reference: Option<string>
      ///Viiteenne
      buyer_reference: Option<string>
      bank_transaction: Option<Newtonsoft.Json.Linq.JToken>
      payment_date: Option<string>
      document: Option<Newtonsoft.Json.Linq.JToken>
      is_automatic_debt_collection_enabled: Option<bool>
      automatic_debt_collection_status: Newtonsoft.Json.Linq.JToken
      external_status_messages: list<InvoiceExternalStatusMessage> }
    ///Creates an instance of Invoice with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          friendly_name: string,
                          receiver: int,
                          receiver_info: Newtonsoft.Json.Linq.JToken,
                          invoice_number: Option<int>,
                          invoicing_date: string,
                          payment_condition_days: int,
                          due_date: string,
                          currency: string,
                          vat_country: string,
                          credit_notes: list<int>,
                          pdf: Newtonsoft.Json.Linq.JToken,
                          total_vat_amount: float32,
                          total_amount: float32,
                          is_editable: bool,
                          is_recurrence_active: bool,
                          is_recurrence_end_exceeded: Option<bool>,
                          is_recurrence_applied: bool,
                          is_past_due: bool,
                          is_sendable: bool,
                          recurrence_parent_id: Option<int>,
                          next_recurrences: list<string>,
                          is_registered_to_kravia: bool,
                          earliest_debt_collection_date: string,
                          bank_transaction: Option<Newtonsoft.Json.Linq.JToken>,
                          payment_date: Option<string>,
                          document: Option<Newtonsoft.Json.Linq.JToken>,
                          automatic_debt_collection_status: Newtonsoft.Json.Linq.JToken,
                          external_status_messages: list<InvoiceExternalStatusMessage>): Invoice =
        { id = id
          created_at = created_at
          updated_at = updated_at
          friendly_name = friendly_name
          receiver = receiver
          receiver_info = receiver_info
          contact_person = None
          invoice_number = invoice_number
          invoicing_date = invoicing_date
          payment_condition_days = payment_condition_days
          due_date = due_date
          reference = None
          penalty_interest = None
          currency = currency
          vat_country = vat_country
          description = None
          is_credit_note_for = None
          credit_notes = credit_notes
          pdf = pdf
          rows = None
          status = None
          total_vat_amount = total_vat_amount
          total_amount = total_amount
          is_editable = is_editable
          is_active_recurrence_template = None
          is_recurrence_active = is_recurrence_active
          is_recurrence_end_exceeded = is_recurrence_end_exceeded
          is_recurrence_applied = is_recurrence_applied
          is_past_due = is_past_due
          delivery_method = None
          is_sendable = is_sendable
          last_delivery_at = None
          recurrence_rule = None
          recurrence_parent_id = recurrence_parent_id
          recurrence_reference_method = None
          recurrence_email_subject = None
          recurrence_email_content = None
          recurrence_end = None
          next_recurrences = next_recurrences
          is_registered_to_kravia = is_registered_to_kravia
          earliest_debt_collection_date = earliest_debt_collection_date
          attachments = None
          settlement_date = None
          seller_reference = None
          buyer_reference = None
          bank_transaction = bank_transaction
          payment_date = payment_date
          document = document
          is_automatic_debt_collection_enabled = None
          automatic_debt_collection_status = automatic_debt_collection_status
          external_status_messages = external_status_messages }

type InvoiceExternalStatusMessage =
    { id: int
      message: string }
    ///Creates an instance of InvoiceExternalStatusMessage with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int, message: string): InvoiceExternalStatusMessage = { id = id; message = message }

type InvoiceExternalStatusMessageRequest =
    { message: string }
    ///Creates an instance of InvoiceExternalStatusMessageRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (message: string): InvoiceExternalStatusMessageRequest = { message = message }

type InvoiceRequest =
    { receiver: int
      contact_person: Option<string>
      invoicing_date: string
      payment_condition_days: int
      reference: Option<string>
      penalty_interest: Option<float>
      description: Option<string>
      is_credit_note_for: Option<int>
      rows: Option<list<InvoiceRowRequest>>
      ///Invoice status enum. Values: DRAFT, ACCEPTED, PAID, CREDIT_LOSS.
      ///* `DRAFT` - Luonnos
      ///* `ACCEPTED` - Hyväksytty
      ///* `PAID` - Maksettu
      ///* `CREDIT_LOSS` - Luottotappio
      status: Option<Newtonsoft.Json.Linq.JToken>
      is_active_recurrence_template: Option<bool>
      delivery_method: Option<Newtonsoft.Json.Linq.JToken>
      last_delivery_at: Option<System.DateTimeOffset>
      recurrence_rule: Option<Newtonsoft.Json.Linq.JToken>
      ///Method for calculating the reference
      ///* `KEEP` - Keep
      ///* `ROLL` - Roll
      recurrence_reference_method: Option<Newtonsoft.Json.Linq.JToken>
      recurrence_email_subject: Option<string>
      recurrence_email_content: Option<string>
      recurrence_end: Option<string>
      attachments: Option<list<int>>
      settlement_date: Option<string>
      ///Viiteemme
      seller_reference: Option<string>
      ///Viiteenne
      buyer_reference: Option<string>
      is_automatic_debt_collection_enabled: Option<bool> }
    ///Creates an instance of InvoiceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (receiver: int, invoicing_date: string, payment_condition_days: int): InvoiceRequest =
        { receiver = receiver
          contact_person = None
          invoicing_date = invoicing_date
          payment_condition_days = payment_condition_days
          reference = None
          penalty_interest = None
          description = None
          is_credit_note_for = None
          rows = None
          status = None
          is_active_recurrence_template = None
          delivery_method = None
          last_delivery_at = None
          recurrence_rule = None
          recurrence_reference_method = None
          recurrence_email_subject = None
          recurrence_email_content = None
          recurrence_end = None
          attachments = None
          settlement_date = None
          seller_reference = None
          buyer_reference = None
          is_automatic_debt_collection_enabled = None }

type InvoiceRow =
    { id: int
      code: Option<string>
      name: string
      unit: string
      amount: float32
      vat_rate: float
      vat_code: int
      product: Option<int>
      product_count: float
      description: Option<string>
      total_vat_amount: float32
      total_amount: float32
      account_id: Option<int> }
    ///Creates an instance of InvoiceRow with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          name: string,
                          unit: string,
                          amount: float32,
                          vat_rate: float,
                          vat_code: int,
                          product_count: float,
                          total_vat_amount: float32,
                          total_amount: float32): InvoiceRow =
        { id = id
          code = None
          name = name
          unit = unit
          amount = amount
          vat_rate = vat_rate
          vat_code = vat_code
          product = None
          product_count = product_count
          description = None
          total_vat_amount = total_vat_amount
          total_amount = total_amount
          account_id = None }

type InvoiceRowRequest =
    { code: Option<string>
      name: string
      unit: string
      amount: float32
      vat_rate: float
      vat_code: int
      product: Option<int>
      product_count: float
      description: Option<string>
      account_id: Option<int> }
    ///Creates an instance of InvoiceRowRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string,
                          unit: string,
                          amount: float32,
                          vat_rate: float,
                          vat_code: int,
                          product_count: float): InvoiceRowRequest =
        { code = None
          name = name
          unit = unit
          amount = amount
          vat_rate = vat_rate
          vat_code = vat_code
          product = None
          product_count = product_count
          description = None
          account_id = None }

type JournalJsonResponse =
    { ///Internal report type identifier that tells which report payload was generated.
      report_type: string
      ///Start date used for the report range when applicable. Null for point-in-time reports.
      date_from: Option<string>
      ///End date used for the report range when applicable. For point-in-time reports, this may match the reporting date or be null.
      date_to: Option<string>
      ///Journal grouped by source document/voucher with entry lines and totals.
      documents: list<JournalReportDocumentSchema>
      ///Report-level totals keyed by metric name. Common keys include debet and credit.
      totals: Option<Map<string, float>> }
    ///Creates an instance of JournalJsonResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (report_type: string, documents: list<JournalReportDocumentSchema>): JournalJsonResponse =
        { report_type = report_type
          date_from = None
          date_to = None
          documents = documents
          totals = None }

type JournalReportDocumentSchema =
    { ///Document/journal voucher number for this journal block.
      number: string
      ///Posting date for the journal document when available.
      date: Option<string>
      ///Journal lines that belong to this document.
      entries: list<JournalReportEntrySchema>
      ///Per-document totals keyed by metric name. Common keys include debet and credit.
      totals: Option<Map<string, float>> }
    ///Creates an instance of JournalReportDocumentSchema with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (number: string, entries: list<JournalReportEntrySchema>): JournalReportDocumentSchema =
        { number = number
          date = None
          entries = entries
          totals = None }

type JournalReportEntrySchema =
    { ///Account number used in this journal entry line.
      account_number: string
      ///Account name used in this journal entry line.
      account_name: string
      ///Debit amount for this journal line.
      debet: float
      ///Credit amount for this journal line.
      credit: float
      ///Optional description for this journal entry line.
      description: Option<string> }
    ///Creates an instance of JournalReportEntrySchema with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (account_number: string, account_name: string, debet: float, credit: float): JournalReportEntrySchema =
        { account_number = account_number
          account_name = account_name
          debet = debet
          credit = credit
          description = None }

type LedgerJsonResponse =
    { ///Internal report type identifier that tells which report payload was generated.
      report_type: string
      ///Start date used for the report range when applicable. Null for point-in-time reports.
      date_from: Option<string>
      ///End date used for the report range when applicable. For point-in-time reports, this may match the reporting date or be null.
      date_to: Option<string>
      ///Ledger grouped by account. Each item contains opening balance, entries, and optional per-account totals.
      accounts: list<LedgerReportAccountSchema>
      ///Report-level totals keyed by metric name. Common keys include debet, credit, and balance_csum.
      totals: Option<Map<string, float>> }
    ///Creates an instance of LedgerJsonResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (report_type: string, accounts: list<LedgerReportAccountSchema>): LedgerJsonResponse =
        { report_type = report_type
          date_from = None
          date_to = None
          accounts = accounts
          totals = None }

type LedgerReportAccountSchema =
    { ///Account number for this ledger account section.
      number: string
      ///Account name for this ledger account section.
      name: string
      ///Opening balance of the account before the listed period entries.
      opening_balance: float
      ///Ledger entries belonging to this account within the requested period.
      entries: list<LedgerReportEntrySchema>
      ///Per-account totals keyed by metric name. Common keys include debet, credit, and balance_csum.
      totals: Option<Map<string, float>> }
    ///Creates an instance of LedgerReportAccountSchema with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (number: string, name: string, opening_balance: float, entries: list<LedgerReportEntrySchema>): LedgerReportAccountSchema =
        { number = number
          name = name
          opening_balance = opening_balance
          entries = entries
          totals = None }

type LedgerReportEntrySchema =
    { ///Entry/document number for this ledger line.
      number: string
      ///Posting date of the ledger entry.
      date: string
      ///Debit amount for this ledger entry line.
      debet: float
      ///Credit amount for this ledger entry line.
      credit: float
      ///Running cumulative balance after applying this entry.
      balance_csum: float
      ///Optional free-text description for this ledger entry.
      description: Option<string> }
    ///Creates an instance of LedgerReportEntrySchema with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (number: string, date: string, debet: float, credit: float, balance_csum: float): LedgerReportEntrySchema =
        { number = number
          date = date
          debet = debet
          credit = credit
          balance_csum = balance_csum
          description = None }

type MCPBusinessSummary =
    { ///Business slug.
      slug: string
      ///Business name.
      name: string
      ///Human-readable business form name.
      form_name: Option<string>
      ///Business identifiers available for this business.
      identifiers: list<Identifier>
      ///Granted permission identifiers for the current user in this business.
      granted_permission_ids: list<string>
      ///Detailed granted permissions for the current user in this business.
      granted_permissions: list<BusinessPermission> }
    ///Creates an instance of MCPBusinessSummary with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (slug: string,
                          name: string,
                          form_name: Option<string>,
                          identifiers: list<Identifier>,
                          granted_permission_ids: list<string>,
                          granted_permissions: list<BusinessPermission>): MCPBusinessSummary =
        { slug = slug
          name = name
          form_name = form_name
          identifiers = identifiers
          granted_permission_ids = granted_permission_ids
          granted_permissions = granted_permissions }

type NocfoPermission =
    { admin: PermissionValue
      bookkeeping_admin: PermissionValue
      bookkeeping_editor: PermissionValue
      bookkeeping_attachment: PermissionValue
      accounts_editor: PermissionValue
      files_editor: PermissionValue
      invoicing_admin: PermissionValue
      invoicing_editor: PermissionValue
      contact_editor: PermissionValue
      reporting_admin: PermissionValue
      reporting_editor: PermissionValue
      salary: PermissionValue
      integration_admin: PermissionValue }
    ///Creates an instance of NocfoPermission with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (admin: PermissionValue,
                          bookkeeping_admin: PermissionValue,
                          bookkeeping_editor: PermissionValue,
                          bookkeeping_attachment: PermissionValue,
                          accounts_editor: PermissionValue,
                          files_editor: PermissionValue,
                          invoicing_admin: PermissionValue,
                          invoicing_editor: PermissionValue,
                          contact_editor: PermissionValue,
                          reporting_admin: PermissionValue,
                          reporting_editor: PermissionValue,
                          salary: PermissionValue,
                          integration_admin: PermissionValue): NocfoPermission =
        { admin = admin
          bookkeeping_admin = bookkeeping_admin
          bookkeeping_editor = bookkeeping_editor
          bookkeeping_attachment = bookkeeping_attachment
          accounts_editor = accounts_editor
          files_editor = files_editor
          invoicing_admin = invoicing_admin
          invoicing_editor = invoicing_editor
          contact_editor = contact_editor
          reporting_admin = reporting_admin
          reporting_editor = reporting_editor
          salary = salary
          integration_admin = integration_admin }

type NocfoPermissionRequest =
    { admin: PermissionValueRequest
      bookkeeping_admin: PermissionValueRequest
      bookkeeping_editor: PermissionValueRequest
      bookkeeping_attachment: PermissionValueRequest
      accounts_editor: PermissionValueRequest
      files_editor: PermissionValueRequest
      invoicing_admin: PermissionValueRequest
      invoicing_editor: PermissionValueRequest
      contact_editor: PermissionValueRequest
      reporting_admin: PermissionValueRequest
      reporting_editor: PermissionValueRequest
      salary: PermissionValueRequest
      integration_admin: PermissionValueRequest }
    ///Creates an instance of NocfoPermissionRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (admin: PermissionValueRequest,
                          bookkeeping_admin: PermissionValueRequest,
                          bookkeeping_editor: PermissionValueRequest,
                          bookkeeping_attachment: PermissionValueRequest,
                          accounts_editor: PermissionValueRequest,
                          files_editor: PermissionValueRequest,
                          invoicing_admin: PermissionValueRequest,
                          invoicing_editor: PermissionValueRequest,
                          contact_editor: PermissionValueRequest,
                          reporting_admin: PermissionValueRequest,
                          reporting_editor: PermissionValueRequest,
                          salary: PermissionValueRequest,
                          integration_admin: PermissionValueRequest): NocfoPermissionRequest =
        { admin = admin
          bookkeeping_admin = bookkeeping_admin
          bookkeeping_editor = bookkeeping_editor
          bookkeeping_attachment = bookkeeping_attachment
          accounts_editor = accounts_editor
          files_editor = files_editor
          invoicing_admin = invoicing_admin
          invoicing_editor = invoicing_editor
          contact_editor = contact_editor
          reporting_admin = reporting_admin
          reporting_editor = reporting_editor
          salary = salary
          integration_admin = integration_admin }

type PaginatedAccountListList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<AccountList> }
    ///Creates an instance of PaginatedAccountListList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<AccountList>): PaginatedAccountListList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedAttachmentListList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<AttachmentList> }
    ///Creates an instance of PaginatedAttachmentListList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<AttachmentList>): PaginatedAttachmentListList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedBusinessIdentifierList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<BusinessIdentifier> }
    ///Creates an instance of PaginatedBusinessIdentifierList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<BusinessIdentifier>): PaginatedBusinessIdentifierList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedBusinessList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Business> }
    ///Creates an instance of PaginatedBusinessList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Business>): PaginatedBusinessList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedContactList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Contact> }
    ///Creates an instance of PaginatedContactList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Contact>): PaginatedContactList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedDocumentListList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<DocumentList> }
    ///Creates an instance of PaginatedDocumentListList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<DocumentList>): PaginatedDocumentListList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedDocumentRelationList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<DocumentRelation> }
    ///Creates an instance of PaginatedDocumentRelationList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<DocumentRelation>): PaginatedDocumentRelationList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedEntryList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Entry> }
    ///Creates an instance of PaginatedEntryList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Entry>): PaginatedEntryList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedHeaderListList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<HeaderList> }
    ///Creates an instance of PaginatedHeaderListList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<HeaderList>): PaginatedHeaderListList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedInvoiceList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Invoice> }
    ///Creates an instance of PaginatedInvoiceList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Invoice>): PaginatedInvoiceList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedPeriodList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Period> }
    ///Creates an instance of PaginatedPeriodList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Period>): PaginatedPeriodList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedProductList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Product> }
    ///Creates an instance of PaginatedProductList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Product>): PaginatedProductList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedPurchaseInvoiceList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<PurchaseInvoice> }
    ///Creates an instance of PaginatedPurchaseInvoiceList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<PurchaseInvoice>): PaginatedPurchaseInvoiceList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedTagList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<Tag> }
    ///Creates an instance of PaginatedTagList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<Tag>): PaginatedTagList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PaginatedVatPeriodList =
    { ///Total number of items in the collection
      count: int
      ///Next page number
      next: Option<int>
      ///Previous page number
      prev: Option<int>
      ///Number of items on the current page
      size: int
      results: list<VatPeriod> }
    ///Creates an instance of PaginatedVatPeriodList with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (count: int, size: int, results: list<VatPeriod>): PaginatedVatPeriodList =
        { count = count
          next = None
          prev = None
          size = size
          results = results }

type PatchedAccountRequestNametranslations =
    { ///Language code for this translation
      key: string
      ///Translated text in the language defined by `key`.
      value: string }
    ///Creates an instance of PatchedAccountRequestNametranslations with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, value: string): PatchedAccountRequestNametranslations =
        { key = key; value = value }

type PatchedAccountRequest =
    { number: Option<string>
      ///List of translations. Each item maps one language code to the translated text.
      name_translations: Option<list<PatchedAccountRequestNametranslations>>
      ///Optional header ID for account grouping in countries/setups that use headers. Field may be omitted when the business has no header models.
      header_id: Option<int>
      description: Option<string>
      ///* `ASS` - Vastaavaa
      ///* `ASS_DEP` - Poistokelpoinen omaisuus
      ///* `ASS_VAT` - Arvonlisäverosaatava
      ///* `ASS_REC` - Siirtosaamiset
      ///* `ASS_PAY` - Pankkitili / käteisvarat
      ///* `ASS_DUE` - Myyntisaatavat
      ///* `LIA` - Vastattavaa
      ///* `LIA_EQU` - Oma pääoma
      ///* `LIA_PRE` - Edellisten tilikausien voitto
      ///* `LIA_DUE` - Ostovelat
      ///* `LIA_DEB` - Velat
      ///* `LIA_ACC` - Siirtovelat
      ///* `LIA_VAT` - Arvonlisäverovelka
      ///* `REV` - Tulot
      ///* `REV_SAL` - Liikevaihtotulot (myynti)
      ///* `REV_NO` - Verottomat tulot
      ///* `EXP` - Menot
      ///* `EXP_DEP` - Poistot
      ///* `EXP_NO` - Vähennyskelvottomat menot
      ///* `EXP_50` - Puoliksi vähennyskelpoiset menot
      ///* `EXP_TAX` - Verotili
      ///* `EXP_TAX_PRE` - Ennakkoverot
      ``type``: Option<Type92dEnum>
      default_vat_code: Option<Newtonsoft.Json.Linq.JToken>
      ///* `standard` - STANDARD
      ///* `reduced_a` - REDUCED_A
      ///* `reduced_b` - REDUCED_B
      ///* `zero` - ZERO
      default_vat_rate_label: Option<DefaultVatRateLabelEnum>
      opening_balance: Option<float32> }
    ///Creates an instance of PatchedAccountRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedAccountRequest =
        { number = None
          name_translations = None
          header_id = None
          description = None
          ``type`` = None
          default_vat_code = None
          default_vat_rate_label = None
          opening_balance = None }

type PatchedAttachmentInstanceRequest =
    { name: Option<string>
      folder_id: Option<int>
      ///* `PENDING` - Odottaa
      ///* `IN_PROGRESS` - Kesken
      ///* `COMPLETED` - Valmis
      ///* `CANCELLED` - Peruttu
      ///* `FAILED` - Epäonnistunut
      analysis_status: Option<AnalysisStatusEnum> }
    ///Creates an instance of PatchedAttachmentInstanceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedAttachmentInstanceRequest =
        { name = None
          folder_id = None
          analysis_status = None }

type PatchedBusinessIdentifierRequest =
    { ///Identifier type enum. Allowed values: `y_tunnus`, `vat_code`, `steuernummer`.
      ///* `y_tunnus` - Y_TUNNUS
      ///* `vat_code` - VAT_CODE
      ///* `steuernummer` - STEUERNUMMER
      ``type``: Option<Newtonsoft.Json.Linq.JToken>
      ///Identifier value string validated by country-specific rules.
      value: Option<string> }
    ///Creates an instance of PatchedBusinessIdentifierRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedBusinessIdentifierRequest = { ``type`` = None; value = None }

type PatchedBusinessRequest =
    { name: Option<string>
      slug: Option<string>
      business_id: Option<string>
      ///* `NET` - ALV netotus
      ///* `SPLIT` - ALV jaettu kirjaus
      vat_posting_method: Option<VatPostingMethodEnum>
      ///* `DEFAULT` - ALV kirjanpidon päivälle
      ///* `DE_CASH_BASIS` - ALV maksupäivänä (DE maksu)
      vat_reporting_method: Option<VatReportingMethodEnum>
      ///* `FI_OY` - Osakeyhtiö (Oy)
      ///* `FI_OYJ` - Julkinen osakeyhtiö (Oyj)
      ///* `FI_TMI` - Toiminimi (Tmi)
      ///* `FI_KY` - Kommandiittiyhtiö (Ky)
      ///* `FI_OK` - Osuuskunta (Osk)
      ///* `FI_AY` - Avoin yhtiö (AY)
      ///* `FI_AS_OY` - Asunto-osakeyhtiö (As Oy)
      ///* `FI_KOY` - Kiinteistöosakeyhtiö (Koy)
      ///* `FI_NY` - NYT-yritys
      ///* `FI_YHD` - Yhdistys (ry)
      ///* `DE_UG` - Saksalainen yrittäjäosakeyhtiö (UG)
      ///* `DEMO` - Demo-yritys
      form: Option<FormEnum>
      default_vat_period: Option<Newtonsoft.Json.Linq.JToken>
      invoicing_default_penalty_interest: Option<float>
      invoicing_default_payment_condition_days: Option<int>
      invoicing_default_email_subject: Option<string>
      invoicing_default_email_content: Option<string>
      invoicing_vat_exemption_reason: Option<string>
      has_accrual_based_entries_for_invoicing: Option<bool>
      has_history_before_nocfo: Option<bool>
      fiscal_period_needs_review: Option<bool>
      vat_period_needs_review: Option<bool>
      contact_phone: Option<string>
      invoicing_email: Option<string>
      invoicing_iban: Option<string>
      invoicing_bic: Option<string>
      invoicing_contact: Option<string>
      invoicing_street: Option<string>
      invoicing_city: Option<string>
      invoicing_postal_code: Option<string>
      invoicing_country: Option<string>
      business_street: Option<string>
      business_city: Option<string>
      business_postal_code: Option<string>
      business_country: Option<string>
      account_yle_tax: Option<int>
      account_tax_deferrals: Option<int>
      account_income_tax_rec: Option<int>
      account_income_tax_lia: Option<int>
      account_previous_profit: Option<int>
      account_vat_receivables: Option<int>
      account_vat_liabilities: Option<int>
      account_vat_accrual_adjustment: Option<int>
      account_trade_receivables: Option<int>
      account_payables: Option<int>
      onboarding_updated_invoicing_settings: Option<bool>
      onboarding_created_invoicing_contact: Option<bool>
      onboarding_created_invoicing_product: Option<bool>
      onboarding_registered_to_kravia: Option<bool>
      einvoicing_enabled: Option<bool>
      holvi_bookkeeping_api_enabled: Option<bool>
      holvi_nocfo_connection_enabled: Option<bool>
      verohallinto_latest_reporter_full_name: Option<string>
      verohallinto_latest_reporter_phone_number: Option<string>
      is_automatic_debt_collection_enabled: Option<bool> }
    ///Creates an instance of PatchedBusinessRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedBusinessRequest =
        { name = None
          slug = None
          business_id = None
          vat_posting_method = None
          vat_reporting_method = None
          form = None
          default_vat_period = None
          invoicing_default_penalty_interest = None
          invoicing_default_payment_condition_days = None
          invoicing_default_email_subject = None
          invoicing_default_email_content = None
          invoicing_vat_exemption_reason = None
          has_accrual_based_entries_for_invoicing = None
          has_history_before_nocfo = None
          fiscal_period_needs_review = None
          vat_period_needs_review = None
          contact_phone = None
          invoicing_email = None
          invoicing_iban = None
          invoicing_bic = None
          invoicing_contact = None
          invoicing_street = None
          invoicing_city = None
          invoicing_postal_code = None
          invoicing_country = None
          business_street = None
          business_city = None
          business_postal_code = None
          business_country = None
          account_yle_tax = None
          account_tax_deferrals = None
          account_income_tax_rec = None
          account_income_tax_lia = None
          account_previous_profit = None
          account_vat_receivables = None
          account_vat_liabilities = None
          account_vat_accrual_adjustment = None
          account_trade_receivables = None
          account_payables = None
          onboarding_updated_invoicing_settings = None
          onboarding_created_invoicing_contact = None
          onboarding_created_invoicing_product = None
          onboarding_registered_to_kravia = None
          einvoicing_enabled = None
          holvi_bookkeeping_api_enabled = None
          holvi_nocfo_connection_enabled = None
          verohallinto_latest_reporter_full_name = None
          verohallinto_latest_reporter_phone_number = None
          is_automatic_debt_collection_enabled = None }

type PatchedContactRequest =
    { customer_id: Option<string>
      ///* `UNSET` - Ei asetettu
      ///* `PERSON` - Yksityishenkilö
      ///* `BUSINESS` - Yritys
      ``type``: Option<ContactTypeEnum>
      name: Option<string>
      name_aliases: Option<list<string>>
      contact_business_id: Option<string>
      notes: Option<string>
      phone_number: Option<string>
      is_invoicing_enabled: Option<bool>
      invoicing_email: Option<string>
      invoicing_einvoice_address: Option<string>
      invoicing_einvoice_operator: Option<string>
      invoicing_tax_code: Option<string>
      invoicing_street: Option<string>
      invoicing_city: Option<string>
      invoicing_postal_code: Option<string>
      invoicing_country: Option<string>
      ///* `fi` - Suomi
      ///* `en` - Englanti
      ///* `sv` - Ruotsi
      invoicing_language: Option<InvoicingLanguageEnum> }
    ///Creates an instance of PatchedContactRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedContactRequest =
        { customer_id = None
          ``type`` = None
          name = None
          name_aliases = None
          contact_business_id = None
          notes = None
          phone_number = None
          is_invoicing_enabled = None
          invoicing_email = None
          invoicing_einvoice_address = None
          invoicing_einvoice_operator = None
          invoicing_tax_code = None
          invoicing_street = None
          invoicing_city = None
          invoicing_postal_code = None
          invoicing_country = None
          invoicing_language = None }

type PatchedCurrentUserRequest =
    { email: Option<string>
      first_name: Option<string>
      last_name: Option<string>
      show_get_started_info: Option<bool>
      language: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of PatchedCurrentUserRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedCurrentUserRequest =
        { email = None
          first_name = None
          last_name = None
          show_get_started_info = None
          language = None }

type PatchedDocumentInstanceRequest =
    { ///Indicates whether the document is flagged for review or attention.
      is_flagged: Option<bool>
      ///Indicates whether the document is a draft. Draft documents are not yet finalized.
      is_draft: Option<bool>
      ///The date of the document. This is the date when the document was created or issued.
      date: Option<string>
      ///A brief description of the document.
      description: Option<string>
      ///Contact ID associated with this document.
      contact_id: Option<int>
      ///IDs of attachments associated with this document.
      attachment_ids: Option<list<int>>
      ///The type of the document.
      ///* `0` - Normaali kirjaus
      ///* `3` - Arvonlisäverokirjaus
      ///* `4` - Poisto
      ///* `6` - YLE-vero
      ///* `7` - Tulovero
      ``type``: Option<Newtonsoft.Json.Linq.JToken>
      ///Accounting blueprint used to generate document entries. Whenever this payload changes, entries are recalculated from it. See `bookkeeping_blueprint_retrieve` for full field-level guidance.
      blueprint: Option<Newtonsoft.Json.Linq.JToken>
      ///List of tag IDs associated with this document.
      tag_ids: Option<list<int>> }
    ///Creates an instance of PatchedDocumentInstanceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedDocumentInstanceRequest =
        { is_flagged = None
          is_draft = None
          date = None
          description = None
          contact_id = None
          attachment_ids = None
          ``type`` = None
          blueprint = None
          tag_ids = None }

type PatchedDocumentRelationRequest =
    { ///The related document of this relation.
      related_document: Option<int>
      ///Semantic role of the current document within this relation.
      ///* `ACCRUAL` - ACCRUAL
      ///* `SETTLEMENT` - SETTLEMENT
      role: Option<Newtonsoft.Json.Linq.JToken>
      ///Type of relation that links the documents.
      ///* `ACCRUAL_PAIR` - ACCRUAL_PAIR
      ``type``: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of PatchedDocumentRelationRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedDocumentRelationRequest =
        { related_document = None
          role = None
          ``type`` = None }

type PatchedInvoiceRequest =
    { receiver: Option<int>
      contact_person: Option<string>
      invoicing_date: Option<string>
      payment_condition_days: Option<int>
      reference: Option<string>
      penalty_interest: Option<float>
      description: Option<string>
      is_credit_note_for: Option<int>
      rows: Option<list<InvoiceRowRequest>>
      ///Invoice status enum. Values: DRAFT, ACCEPTED, PAID, CREDIT_LOSS.
      ///* `DRAFT` - Luonnos
      ///* `ACCEPTED` - Hyväksytty
      ///* `PAID` - Maksettu
      ///* `CREDIT_LOSS` - Luottotappio
      status: Option<Newtonsoft.Json.Linq.JToken>
      is_active_recurrence_template: Option<bool>
      delivery_method: Option<Newtonsoft.Json.Linq.JToken>
      last_delivery_at: Option<System.DateTimeOffset>
      recurrence_rule: Option<Newtonsoft.Json.Linq.JToken>
      ///Method for calculating the reference
      ///* `KEEP` - Keep
      ///* `ROLL` - Roll
      recurrence_reference_method: Option<Newtonsoft.Json.Linq.JToken>
      recurrence_email_subject: Option<string>
      recurrence_email_content: Option<string>
      recurrence_end: Option<string>
      attachments: Option<list<int>>
      settlement_date: Option<string>
      ///Viiteemme
      seller_reference: Option<string>
      ///Viiteenne
      buyer_reference: Option<string>
      is_automatic_debt_collection_enabled: Option<bool> }
    ///Creates an instance of PatchedInvoiceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedInvoiceRequest =
        { receiver = None
          contact_person = None
          invoicing_date = None
          payment_condition_days = None
          reference = None
          penalty_interest = None
          description = None
          is_credit_note_for = None
          rows = None
          status = None
          is_active_recurrence_template = None
          delivery_method = None
          last_delivery_at = None
          recurrence_rule = None
          recurrence_reference_method = None
          recurrence_email_subject = None
          recurrence_email_content = None
          recurrence_end = None
          attachments = None
          settlement_date = None
          seller_reference = None
          buyer_reference = None
          is_automatic_debt_collection_enabled = None }

type PatchedPeriodRequest =
    { ///Accounting period start date (inclusive).
      start_date: Option<string>
      ///Accounting period end date (inclusive).
      end_date: Option<string> }
    ///Creates an instance of PatchedPeriodRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedPeriodRequest = { start_date = None; end_date = None }

type PatchedProductRequest =
    { code: Option<string>
      name: Option<string>
      unit: Option<string>
      amount: Option<float32>
      vat_rate: Option<float>
      vat_code: Option<Newtonsoft.Json.Linq.JToken>
      is_vat_inclusive: Option<bool>
      account_id: Option<int>
      description: Option<string> }
    ///Creates an instance of PatchedProductRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedProductRequest =
        { code = None
          name = None
          unit = None
          amount = None
          vat_rate = None
          vat_code = None
          is_vat_inclusive = None
          account_id = None
          description = None }

type PatchedPurchaseInvoiceRequest =
    { invoice_number: Option<string>
      invoicing_date: Option<string>
      due_date: Option<string>
      sender_vat: Option<string>
      sender_name: Option<string>
      sender_bank_account: Option<string>
      sender_bank_bic: Option<string>
      receiver_vat: Option<string>
      receiver_name: Option<string>
      reference: Option<string>
      message: Option<string>
      original_attachment: Option<Newtonsoft.Json.Linq.JToken>
      amount: Option<float32>
      currency: Option<string>
      is_paid: Option<bool>
      ///Tells the source where the invoice was imported from.
      ///* `APIX` - Apix
      ///* `EMAIL` - Email
      ///* `UPLOAD` - Lataus
      ///* `VAT_PURCHASE_INVOICE` - ALV-ostolasku
      import_source: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of PatchedPurchaseInvoiceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedPurchaseInvoiceRequest =
        { invoice_number = None
          invoicing_date = None
          due_date = None
          sender_vat = None
          sender_name = None
          sender_bank_account = None
          sender_bank_bic = None
          receiver_vat = None
          receiver_name = None
          reference = None
          message = None
          original_attachment = None
          amount = None
          currency = None
          is_paid = None
          import_source = None }

type PatchedTagRequest =
    { name: Option<string>
      description: Option<string>
      color: Option<string> }
    ///Creates an instance of PatchedTagRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedTagRequest =
        { name = None
          description = None
          color = None }

type PatchedVatPeriodRequest =
    { ///VAT period start date (inclusive).
      start_date: Option<string>
      ///VAT cadence value used by this VAT period.
      period: Option<int> }
    ///Creates an instance of PatchedVatPeriodRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PatchedVatPeriodRequest = { start_date = None; period = None }

type PaymentAmount =
    { amount: float32
      currency: string }
    ///Creates an instance of PaymentAmount with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (amount: float32, currency: string): PaymentAmount = { amount = amount; currency = currency }

type PaymentAmountRequest =
    { amount: float32
      currency: string }
    ///Creates an instance of PaymentAmountRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (amount: float32, currency: string): PaymentAmountRequest =
        { amount = amount; currency = currency }

type PaymentDetails =
    { requested_execution_date: string
      holvi_account: string
      creditor: CreditorInfo
      amount: PaymentAmount
      reference: Option<string>
      message: Option<string> }
    ///Creates an instance of PaymentDetails with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (requested_execution_date: string,
                          holvi_account: string,
                          creditor: CreditorInfo,
                          amount: PaymentAmount): PaymentDetails =
        { requested_execution_date = requested_execution_date
          holvi_account = holvi_account
          creditor = creditor
          amount = amount
          reference = None
          message = None }

type PaymentDetailsRequest =
    { requested_execution_date: string
      holvi_account: string
      creditor: CreditorInfoRequest
      amount: PaymentAmountRequest
      reference: Option<string>
      message: Option<string> }
    ///Creates an instance of PaymentDetailsRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (requested_execution_date: string,
                          holvi_account: string,
                          creditor: CreditorInfoRequest,
                          amount: PaymentAmountRequest): PaymentDetailsRequest =
        { requested_execution_date = requested_execution_date
          holvi_account = holvi_account
          creditor = creditor
          amount = amount
          reference = None
          message = None }

type PaymentInstruction =
    { id: System.Guid
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      statuses: list<PaymentInstructionStatus> }
    ///Creates an instance of PaymentInstruction with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: System.Guid, created_at: System.DateTimeOffset, statuses: list<PaymentInstructionStatus>): PaymentInstruction =
        { id = id
          created_at = created_at
          statuses = statuses }

type PaymentInstructionRequest =
    { payment_details: Newtonsoft.Json.Linq.JToken }
    ///Creates an instance of PaymentInstructionRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (payment_details: Newtonsoft.Json.Linq.JToken): PaymentInstructionRequest =
        { payment_details = payment_details }

type PaymentInstructionStatus =
    { id: int
      message_id: string
      ///Status of the payment instruction
      status: Option<string>
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset }
    ///Creates an instance of PaymentInstructionStatus with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int, message_id: string, created_at: System.DateTimeOffset): PaymentInstructionStatus =
        { id = id
          message_id = message_id
          status = None
          created_at = created_at }

type PaymentInstructionStatusRequest =
    { message_id: string
      ///Status of the payment instruction
      status: Option<string> }
    ///Creates an instance of PaymentInstructionStatusRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (message_id: string): PaymentInstructionStatusRequest =
        { message_id = message_id
          status = None }

type Period =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      ///Accounting period start date (inclusive).
      start_date: Option<string>
      ///Accounting period end date (inclusive).
      end_date: Option<string>
      ///True when the accounting period start date is in the past.
      has_began: bool
      ///True when the accounting period end date is in the past.
      has_ended: bool
      ///Whether accounting period checks have been marked as ensured.
      is_ensured: bool
      ///Whether depreciation entries have been recorded for this accounting period.
      is_depreciations_recorded: bool
      ///Whether depreciation checks have been marked as ensured.
      is_depreciations_ensured: bool
      ///Whether deferral entries have been recorded for this accounting period.
      is_deferrals_recorded: bool
      ///Whether period tax entries have been recorded.
      is_taxed: bool
      ///Whether deductions have been processed for this accounting period.
      is_deducted: bool
      ///Whether documents in this accounting period are locked.
      is_locked: bool
      ///Whether this accounting period has been reported/closed.
      is_reported: bool }
    ///Creates an instance of Period with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          has_began: bool,
                          has_ended: bool,
                          is_ensured: bool,
                          is_depreciations_recorded: bool,
                          is_depreciations_ensured: bool,
                          is_deferrals_recorded: bool,
                          is_taxed: bool,
                          is_deducted: bool,
                          is_locked: bool,
                          is_reported: bool): Period =
        { id = id
          created_at = created_at
          updated_at = updated_at
          start_date = None
          end_date = None
          has_began = has_began
          has_ended = has_ended
          is_ensured = is_ensured
          is_depreciations_recorded = is_depreciations_recorded
          is_depreciations_ensured = is_depreciations_ensured
          is_deferrals_recorded = is_deferrals_recorded
          is_taxed = is_taxed
          is_deducted = is_deducted
          is_locked = is_locked
          is_reported = is_reported }

type PeriodRequest =
    { ///Accounting period start date (inclusive).
      start_date: Option<string>
      ///Accounting period end date (inclusive).
      end_date: Option<string> }
    ///Creates an instance of PeriodRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): PeriodRequest = { start_date = None; end_date = None }

type PermissionValue =
    { name: string
      value: bool
      explanation: string }
    ///Creates an instance of PermissionValue with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, value: bool, explanation: string): PermissionValue =
        { name = name
          value = value
          explanation = explanation }

type PermissionValueRequest =
    { name: string
      value: bool
      explanation: string }
    ///Creates an instance of PermissionValueRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, value: bool, explanation: string): PermissionValueRequest =
        { name = name
          value = value
          explanation = explanation }

type PointInTimeReportColumnSchemaRequest =
    { ///Optional column label shown in report output.
      name: Option<string>
      ///Point-in-time date for balance-sheet style reports (YYYY-MM-DD).
      date_at: string }
    ///Creates an instance of PointInTimeReportColumnSchemaRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (date_at: string): PointInTimeReportColumnSchemaRequest = { name = None; date_at = date_at }

type PointInTimeTypedReportRequestSchemaRequest =
    { ///When true, include account-level drill-down rows for supported reports.
      extend_accounts: Option<bool>
      ///When true and column count is small enough, add comparison columns.
      append_comparison_columns: Option<bool>
      ///Optional tag filters. Only entries/documents matching these tags are included in report calculations.
      tag_ids: Option<list<int>>
      ///Columns for point-in-time reports. Use `date_at` for each column.
      columns: list<PointInTimeReportColumnSchemaRequest> }
    ///Creates an instance of PointInTimeTypedReportRequestSchemaRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (columns: list<PointInTimeReportColumnSchemaRequest>): PointInTimeTypedReportRequestSchemaRequest =
        { extend_accounts = None
          append_comparison_columns = None
          tag_ids = None
          columns = columns }

type Product =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      code: Option<string>
      name: string
      unit: string
      amount: float32
      vat_exclusive_amount: float32
      vat_inclusive_amount: float32
      vat_amount: float32
      vat_rate: Option<float>
      vat_code: Option<Newtonsoft.Json.Linq.JToken>
      is_vat_inclusive: Option<bool>
      account_id: Option<int>
      description: Option<string> }
    ///Creates an instance of Product with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          name: string,
                          unit: string,
                          amount: float32,
                          vat_exclusive_amount: float32,
                          vat_inclusive_amount: float32,
                          vat_amount: float32): Product =
        { id = id
          created_at = created_at
          updated_at = updated_at
          code = None
          name = name
          unit = unit
          amount = amount
          vat_exclusive_amount = vat_exclusive_amount
          vat_inclusive_amount = vat_inclusive_amount
          vat_amount = vat_amount
          vat_rate = None
          vat_code = None
          is_vat_inclusive = None
          account_id = None
          description = None }

type ProductRequest =
    { code: Option<string>
      name: string
      unit: string
      amount: float32
      vat_rate: Option<float>
      vat_code: Option<Newtonsoft.Json.Linq.JToken>
      is_vat_inclusive: Option<bool>
      account_id: Option<int>
      description: Option<string> }
    ///Creates an instance of ProductRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string, unit: string, amount: float32): ProductRequest =
        { code = None
          name = name
          unit = unit
          amount = amount
          vat_rate = None
          vat_code = None
          is_vat_inclusive = None
          account_id = None
          description = None }

type PurchaseInvoice =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      invoice_number: Option<string>
      invoicing_date: string
      due_date: Option<string>
      sender_vat: Option<string>
      sender_name: Option<string>
      sender_bank_account: Option<string>
      sender_bank_bic: Option<string>
      receiver_vat: Option<string>
      receiver_name: Option<string>
      reference: Option<string>
      message: Option<string>
      original_invoice_xml: Option<string>
      original_attachment: Option<Newtonsoft.Json.Linq.JToken>
      amount: float32
      currency: Option<string>
      virtuaaliviivakoodi: Option<string>
      is_paid: Option<bool>
      is_past_due: string
      attachments: list<int>
      ///Tells the source where the invoice was imported from.
      ///* `APIX` - Apix
      ///* `EMAIL` - Email
      ///* `UPLOAD` - Lataus
      ///* `VAT_PURCHASE_INVOICE` - ALV-ostolasku
      import_source: Newtonsoft.Json.Linq.JToken
      payment: Option<Newtonsoft.Json.Linq.JToken>
      pis_payment: Option<Newtonsoft.Json.Linq.JObject>
      ///The date when the invoice was set to paid.
      payment_date: Option<string>
      bank_transaction: Option<Newtonsoft.Json.Linq.JToken>
      document: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of PurchaseInvoice with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          invoicing_date: string,
                          original_invoice_xml: Option<string>,
                          amount: float32,
                          virtuaaliviivakoodi: Option<string>,
                          is_past_due: string,
                          attachments: list<int>,
                          import_source: Newtonsoft.Json.Linq.JToken,
                          payment: Option<Newtonsoft.Json.Linq.JToken>,
                          pis_payment: Option<Newtonsoft.Json.Linq.JObject>,
                          payment_date: Option<string>,
                          bank_transaction: Option<Newtonsoft.Json.Linq.JToken>,
                          document: Option<Newtonsoft.Json.Linq.JToken>): PurchaseInvoice =
        { id = id
          created_at = created_at
          updated_at = updated_at
          invoice_number = None
          invoicing_date = invoicing_date
          due_date = None
          sender_vat = None
          sender_name = None
          sender_bank_account = None
          sender_bank_bic = None
          receiver_vat = None
          receiver_name = None
          reference = None
          message = None
          original_invoice_xml = original_invoice_xml
          original_attachment = None
          amount = amount
          currency = None
          virtuaaliviivakoodi = virtuaaliviivakoodi
          is_paid = None
          is_past_due = is_past_due
          attachments = attachments
          import_source = import_source
          payment = payment
          pis_payment = pis_payment
          payment_date = payment_date
          bank_transaction = bank_transaction
          document = document }

type PurchaseInvoiceRequest =
    { invoice_number: Option<string>
      invoicing_date: string
      due_date: Option<string>
      sender_vat: Option<string>
      sender_name: Option<string>
      sender_bank_account: Option<string>
      sender_bank_bic: Option<string>
      receiver_vat: Option<string>
      receiver_name: Option<string>
      reference: Option<string>
      message: Option<string>
      original_attachment: Option<Newtonsoft.Json.Linq.JToken>
      amount: float32
      currency: Option<string>
      is_paid: Option<bool>
      ///Tells the source where the invoice was imported from.
      ///* `APIX` - Apix
      ///* `EMAIL` - Email
      ///* `UPLOAD` - Lataus
      ///* `VAT_PURCHASE_INVOICE` - ALV-ostolasku
      import_source: Newtonsoft.Json.Linq.JToken }
    ///Creates an instance of PurchaseInvoiceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (invoicing_date: string, amount: float32, import_source: Newtonsoft.Json.Linq.JToken): PurchaseInvoiceRequest =
        { invoice_number = None
          invoicing_date = invoicing_date
          due_date = None
          sender_vat = None
          sender_name = None
          sender_bank_account = None
          sender_bank_bic = None
          receiver_vat = None
          receiver_name = None
          reference = None
          message = None
          original_attachment = None
          amount = amount
          currency = None
          is_paid = None
          import_source = import_source }

type ReportRowAccountSchema =
    { ///Account number for this account-level drill-down row.
      number: string
      ///Human-readable account name for this drill-down row.
      name: string
      ///Numeric values for this account in the same order as response `labels`.
      values: list<float>
      ///Column labels repeated on this row for self-contained parsing. Label order matches `values`.
      labels: Option<list<string>> }
    ///Creates an instance of ReportRowAccountSchema with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (number: string, name: string, values: list<float>): ReportRowAccountSchema =
        { number = number
          name = name
          values = values
          labels = None }

type ReportRowSchema =
    { ///Row title shown in the report tree (for example Revenue, Expenses).
      name: string
      ///Hierarchy depth of this row. Lower values are higher-level summary rows.
      level: int
      ///True when the row is a calculated summary/subtotal; false for regular rows.
      is_sum_row: bool
      ///Row values in the same order as this output object's `labels`.
      values: list<float>
      ///Column labels repeated on this row for self-contained parsing. Label order matches `values`.
      labels: Option<list<string>>
      ///Optional account-level drill-down rows for this report row. Usually present when `extend_accounts=true` is requested.
      accounts: list<ReportRowAccountSchema> }
    ///Creates an instance of ReportRowSchema with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (name: string,
                          level: int,
                          is_sum_row: bool,
                          values: list<float>,
                          accounts: list<ReportRowAccountSchema>): ReportRowSchema =
        { name = name
          level = level
          is_sum_row = is_sum_row
          values = values
          labels = None
          accounts = accounts }

type SendInvoiceRequest =
    { ///* `EMAIL` - Email
      ///* `EINVOICE` - Einvoice
      ///* `ELASKU` - Elasku
      ///* `PAPER` - Paper
      delivery_method: DeliveryMethodEnum
      data: Option<Newtonsoft.Json.Linq.JToken> }
    ///Creates an instance of SendInvoiceRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (delivery_method: DeliveryMethodEnum): SendInvoiceRequest =
        { delivery_method = delivery_method
          data = None }

type SendInvoiceViaEmailRequest =
    { email_subject: string
      email_content: Option<string> }
    ///Creates an instance of SendInvoiceViaEmailRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (email_subject: string, email_content: Option<string>): SendInvoiceViaEmailRequest =
        { email_subject = email_subject
          email_content = email_content }

type Shortcut =
    { ///Unique key identifying the shortcut.
      key: string
      ///Indicates whether the shortcut is promoted.
      is_promoted: bool }
    ///Creates an instance of Shortcut with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (key: string, is_promoted: bool): Shortcut = { key = key; is_promoted = is_promoted }

type Tag =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      name: Option<string>
      description: Option<string>
      color: Option<string> }
    ///Creates an instance of Tag with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int, created_at: System.DateTimeOffset, updated_at: System.DateTimeOffset): Tag =
        { id = id
          created_at = created_at
          updated_at = updated_at
          name = None
          description = None
          color = None }

type TagRequest =
    { name: Option<string>
      description: Option<string>
      color: Option<string> }
    ///Creates an instance of TagRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): TagRequest =
        { name = None
          description = None
          color = None }

type VatPeriod =
    { id: int
      ///The date and time when the record was created.
      created_at: System.DateTimeOffset
      ///The date and time when the record was last updated.
      updated_at: System.DateTimeOffset
      ///VAT period start date (inclusive).
      start_date: Option<string>
      ///VAT period end date (inclusive).
      end_date: string
      ///Due date for VAT reporting/payment of this VAT period.
      due_date: string
      ///VAT cadence value used by this VAT period.
      period: Option<int>
      ///True when the VAT period start date is in the past.
      has_began: bool
      ///True when the VAT period end date is in the past.
      has_ended: bool
      ///True when the VAT due date has passed.
      has_due_passed: bool
      ///Whether documents in this VAT period are locked.
      is_locked: bool
      ///Whether this VAT period has been reported.
      is_reported: bool }
    ///Creates an instance of VatPeriod with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (id: int,
                          created_at: System.DateTimeOffset,
                          updated_at: System.DateTimeOffset,
                          end_date: string,
                          due_date: string,
                          has_began: bool,
                          has_ended: bool,
                          has_due_passed: bool,
                          is_locked: bool,
                          is_reported: bool): VatPeriod =
        { id = id
          created_at = created_at
          updated_at = updated_at
          start_date = None
          end_date = end_date
          due_date = due_date
          period = None
          has_began = has_began
          has_ended = has_ended
          has_due_passed = has_due_passed
          is_locked = is_locked
          is_reported = is_reported }

type VatPeriodRequest =
    { ///VAT period start date (inclusive).
      start_date: Option<string>
      ///VAT cadence value used by this VAT period.
      period: Option<int> }
    ///Creates an instance of VatPeriodRequest with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (): VatPeriodRequest = { start_date = None; period = None }

type VatRateOption =
    { ///VAT rate label key (standard|reduced_a|reduced_b|zero).
      label: string
      ///VAT rate percentage value.
      rate: string }
    ///Creates an instance of VatRateOption with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (label: string, rate: string): VatRateOption = { label = label; rate = rate }

type VatRatesResponse =
    { ///Date from which this VAT configuration is effective.
      effective_from: string
      ///Default VAT rate label for the selected date.
      default_vat_rate_label: string
      ///Default VAT rate percentage for the selected date.
      default_vat_rate: string
      ///VAT rate options available for the selected date.
      vat_rate_options: list<VatRateOption> }
    ///Creates an instance of VatRatesResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (effective_from: string,
                          default_vat_rate_label: string,
                          default_vat_rate: string,
                          vat_rate_options: list<VatRateOption>): VatRatesResponse =
        { effective_from = effective_from
          default_vat_rate_label = default_vat_rate_label
          default_vat_rate = default_vat_rate
          vat_rate_options = vat_rate_options }

type VatReportJsonResponse =
    { ///Internal report type identifier that tells which report payload was generated.
      report_type: string
      ///Start date used for the report range when applicable. Null for point-in-time reports.
      date_from: Option<string>
      ///End date used for the report range when applicable. For point-in-time reports, this may match the reporting date or be null.
      date_to: Option<string>
      ///DE VAT totals keyed by total name. Values are numeric totals used for ELSTER reporting.
      totals: Option<Map<string, float>>
      ///FI VAT summary payload. Contains numeric totals and nested groupings such as domestic_sales.
      summary: Option<Map<string, string>>
      ///DE ELSTER mapping keyed by ELSTER field code as string (for example, '66', '81'). Values are numeric amounts.
      mapping: Option<Map<string, float>> }
    ///Creates an instance of VatReportJsonResponse with all optional fields initialized to None. The required fields are parameters of this function
    static member Create (report_type: string): VatReportJsonResponse =
        { report_type = report_type
          date_from = None
          date_to = None
          totals = None
          summary = None
          mapping = None }

[<RequireQualifiedAccess>]
type AuthJwtCreate =
    | OK of payload: GetJwtTokenResponse
    ///Validation error
    | BadRequest
    ///Forbidden
    | Forbidden

[<RequireQualifiedAccess>]
type SettingsBusinessesList = OK of payload: PaginatedBusinessList

[<RequireQualifiedAccess>]
type SettingsBusinessCreate = Created of payload: Business

[<RequireQualifiedAccess>]
type SettingsBusinessRetrieve = OK of payload: Business

[<RequireQualifiedAccess>]
type SettingsBusinessReplace = OK of payload: Business

[<RequireQualifiedAccess>]
type SettingsBusinessUpdate = OK of payload: Business

[<RequireQualifiedAccess>]
type SettingsBusinessDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type BookkeepingAccountsList = OK of payload: PaginatedAccountListList

[<RequireQualifiedAccess>]
type BookkeepingAccountCreate = Created of payload: AccountList

[<RequireQualifiedAccess>]
type BookkeepingAccountRetrieve = OK of payload: Account

[<RequireQualifiedAccess>]
type BookkeepingAccountReplace = OK of payload: Account

[<RequireQualifiedAccess>]
type BookkeepingAccountUpdate = OK of payload: Account

[<RequireQualifiedAccess>]
type BookkeepingAccountDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type BookkeepingAccountHide = | OK

[<RequireQualifiedAccess>]
type BookkeepingAccountShow = | OK

[<RequireQualifiedAccess>]
type ConstantsBusinessIdentifiersRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type V1BusinessConstantsBusinessInvoicingCountriesRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type V1BusinessConstantsCountriesRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type ConstantsEinvoiceOperatorsRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type V1BusinessConstantsPermissionGroupsRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type V1BusinessConstantsPermissionsRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type ConstantsVatCodesRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type V1BusinessConstantsVatPostingMethodsRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type ConstantsVatRatesRetrieve = OK of payload: VatRatesResponse

[<RequireQualifiedAccess>]
type V1BusinessConstantsVatReportingMethodsRetrieve =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingContactsList = OK of payload: PaginatedContactList

[<RequireQualifiedAccess>]
type InvoicingContactCreate = Created of payload: Contact

[<RequireQualifiedAccess>]
type InvoicingContactRetrieve = OK of payload: Contact

[<RequireQualifiedAccess>]
type InvoicingContactReplace = OK of payload: Contact

[<RequireQualifiedAccess>]
type InvoicingContactUpdate = OK of payload: Contact

[<RequireQualifiedAccess>]
type InvoicingContactDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type BookkeepingDocumentsList = OK of payload: PaginatedDocumentListList

[<RequireQualifiedAccess>]
type BookkeepingDocumentCreate = Created of payload: DocumentList

[<RequireQualifiedAccess>]
type BookkeepingDocumentRetrieve = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentReplace = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentUpdate = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type BookkeepingDocumentAttachFiles = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type V1BusinessDocumentActionCopyCreate = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentDetachFiles = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentFlag = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentLock = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type V1BusinessDocumentActionSetFilesCreate = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentUnflag = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentUnlock = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingEntriesList = OK of payload: PaginatedEntryList

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationsList = OK of payload: PaginatedDocumentRelationList

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationCreate = Created of payload: DocumentRelation

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationRetrieve = OK of payload: DocumentRelation

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationReplace = OK of payload: DocumentRelation

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationUpdate = OK of payload: DocumentRelation

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationSuggestionsList = OK of payload: DocumentRelationSuggestion

[<RequireQualifiedAccess>]
type V1BusinessDocumentSuggestAttachmentsRetrieve = OK of payload: DocumentInstance

[<RequireQualifiedAccess>]
type BookkeepingDocumentRelationSuggestionsPreview = OK of payload: list<DocumentRelationSuggestion>

[<RequireQualifiedAccess>]
type BookkeepingFileUpload = OK of payload: AttachmentInstance

[<RequireQualifiedAccess>]
type BookkeepingFilesList = OK of payload: PaginatedAttachmentListList

[<RequireQualifiedAccess>]
type BookkeepingFileRetrieve = OK of payload: AttachmentInstance

[<RequireQualifiedAccess>]
type BookkeepingFileReplace = OK of payload: AttachmentInstance

[<RequireQualifiedAccess>]
type BookkeepingFileUpdate = OK of payload: AttachmentInstance

[<RequireQualifiedAccess>]
type BookkeepingFileDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type BookkeepingHeadersList = OK of payload: PaginatedHeaderListList

[<RequireQualifiedAccess>]
type BookkeepingHeaderCreate = Created of payload: HeaderList

[<RequireQualifiedAccess>]
type BookkeepingHeaderRetrieve = OK of payload: HeaderList

[<RequireQualifiedAccess>]
type SettingsBusinessIdentifiersList = OK of payload: PaginatedBusinessIdentifierList

[<RequireQualifiedAccess>]
type SettingsBusinessIdentifierCreate = Created of payload: BusinessIdentifier

[<RequireQualifiedAccess>]
type SettingsBusinessIdentifierRetrieve = OK of payload: BusinessIdentifier

[<RequireQualifiedAccess>]
type SettingsBusinessIdentifierReplace = OK of payload: BusinessIdentifier

[<RequireQualifiedAccess>]
type SettingsBusinessIdentifierUpdate = OK of payload: BusinessIdentifier

[<RequireQualifiedAccess>]
type SettingsBusinessIdentifierDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type SettingsBusinessPermissionsRetrieve = OK of payload: BusinessPermissionsResponse

[<RequireQualifiedAccess>]
type ReportingAccountingPeriodsList = OK of payload: PaginatedPeriodList

[<RequireQualifiedAccess>]
type ReportingAccountingPeriodCreate = Created of payload: Period

[<RequireQualifiedAccess>]
type ReportingAccountingPeriodRetrieve = OK of payload: Period

[<RequireQualifiedAccess>]
type ReportingAccountingPeriodReplace = OK of payload: Period

[<RequireQualifiedAccess>]
type ReportingAccountingPeriodUpdate = OK of payload: Period

[<RequireQualifiedAccess>]
type ReportingAccountingPeriodDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type ReportingBalanceSheetRetrieve = OK of payload: AccountingReportJsonResponse

[<RequireQualifiedAccess>]
type ReportingBalanceSheetShortRetrieve = OK of payload: AccountingReportJsonResponse

[<RequireQualifiedAccess>]
type ReportingEquityChangesRetrieve = OK of payload: AccountingReportJsonResponse

[<RequireQualifiedAccess>]
type ReportingIncomeStatementRetrieve = OK of payload: AccountingReportJsonResponse

[<RequireQualifiedAccess>]
type ReportingIncomeStatementShortRetrieve = OK of payload: AccountingReportJsonResponse

[<RequireQualifiedAccess>]
type ReportingJournalRetrieve = OK of payload: JournalJsonResponse

[<RequireQualifiedAccess>]
type ReportingLedgerRetrieve = OK of payload: LedgerJsonResponse

[<RequireQualifiedAccess>]
type ReportingVatRetrieve = OK of payload: VatReportJsonResponse

[<RequireQualifiedAccess>]
type BookkeepingTagsList = OK of payload: PaginatedTagList

[<RequireQualifiedAccess>]
type BookkeepingTagCreate = Created of payload: Tag

[<RequireQualifiedAccess>]
type BookkeepingTagRetrieve = OK of payload: Tag

[<RequireQualifiedAccess>]
type BookkeepingTagReplace = OK of payload: Tag

[<RequireQualifiedAccess>]
type BookkeepingTagUpdate = OK of payload: Tag

[<RequireQualifiedAccess>]
type BookkeepingTagDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type ReportingVatPeriodsList = OK of payload: PaginatedVatPeriodList

[<RequireQualifiedAccess>]
type ReportingVatPeriodCreate = Created of payload: VatPeriod

[<RequireQualifiedAccess>]
type ReportingVatPeriodRetrieve = OK of payload: VatPeriod

[<RequireQualifiedAccess>]
type ReportingVatPeriodReplace = OK of payload: VatPeriod

[<RequireQualifiedAccess>]
type ReportingVatPeriodUpdate = OK of payload: VatPeriod

[<RequireQualifiedAccess>]
type ReportingVatPeriodDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type InvoicingSalesInvoicesList = OK of payload: PaginatedInvoiceList

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceCreate = Created of payload: Invoice

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceRetrieve = OK of payload: Invoice

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceReplace = OK of payload: Invoice

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceUpdate = OK of payload: Invoice

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceAccept =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceMarkCreditLoss =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceRecurrenceDisable =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceMarkPaid =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceMarkUnpaid =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceDeliveryMethods =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceSend =
    ///No response body
    | OK

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceStatusMessageCreate = Created of payload: InvoiceExternalStatusMessage

[<RequireQualifiedAccess>]
type InvoicingSalesInvoiceStatusMessageDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type InvoicingProductsList = OK of payload: PaginatedProductList

[<RequireQualifiedAccess>]
type InvoicingProductCreate = Created of payload: Product

[<RequireQualifiedAccess>]
type InvoicingProductRetrieve = OK of payload: Product

[<RequireQualifiedAccess>]
type InvoicingProductReplace = OK of payload: Product

[<RequireQualifiedAccess>]
type InvoicingProductUpdate = OK of payload: Product

[<RequireQualifiedAccess>]
type InvoicingProductDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type InvoicingPurchaseInvoicesList = OK of payload: PaginatedPurchaseInvoiceList

[<RequireQualifiedAccess>]
type InvoicingPurchaseInvoiceRetrieve = OK of payload: PurchaseInvoice

[<RequireQualifiedAccess>]
type InvoicingPurchaseInvoiceReplace = OK of payload: PurchaseInvoice

[<RequireQualifiedAccess>]
type InvoicingPurchaseInvoiceUpdate = OK of payload: PurchaseInvoice

[<RequireQualifiedAccess>]
type InvoicingPurchaseInvoiceDelete =
    ///No response body
    | NoContent

[<RequireQualifiedAccess>]
type IdentityUserRetrieve = OK of payload: CurrentUser

[<RequireQualifiedAccess>]
type IdentityUserReplace = OK of payload: CurrentUser

[<RequireQualifiedAccess>]
type IdentityUserUpdate = OK of payload: CurrentUser
