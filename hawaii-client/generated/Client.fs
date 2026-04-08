namespace rec NocfoApi

open System.Net
open System.Net.Http
open System.Text
open System.Threading
open NocfoApi.Types
open NocfoApi.Http

///Get help and discuss in our [Slack community](https://nocfocommunity.slack.com) 👉 [**Click here to join**](https://join.slack.com/t/nocfocommunity/shared_invite/zt-3b7i1rcfp-4JmhWVTqGxzireppzCwA)
///### Welcome!
///Welcome to the NoCFO API Documentation! The **NoCFO API** provides secure, programmatic access to the core functionalities of the NoCFO platform, enabling seamless integration with your own applications and workflows.
///With these endpoints, you can interact with a variety of features including **bookkeeping**, **invoicing**, and other financial operations.
///Our API is built to be reliable, scalable, and developer-friendly, with consistent request/response structures and clear authentication methods. Whether you are building new integrations, automating repetitive tasks, or syncing data between systems, the NoCFO API offers the tools you need.
///### Available Resources
///We provide both **testing** and **production** environments so you can safely develop and validate your integration before going live.
///- **Testing environment**: [https://api-tst.nocfo.io/docs](https://api-tst.nocfo.io/docs)
///Use this environment for development and testing purposes. Data here is isolated and does not affect production systems.
///- **Production environment**: [https://api-prd.nocfo.io/docs](https://api-prd.nocfo.io/docs)
///Use this environment for live, real-world operations once your integration is fully tested.
///### Authentication
///The NoCFO API uses **token-based authentication** to ensure secure access.
///To authenticate requests, you must include a valid **Personal Access Token (PAT)** in the `Authorization` header:
///```http
///Authorization: Token &amp;lt;your_token_here&amp;gt;
///```
///#### Key points to note:
///- Tokens are user-specific. Any action performed with a token will be attributed to the user who created it.
///- Keep your token secure — treat it like a password. Do not share it or expose it in public repositories.
///- If a token is compromised, it should be revoked immediately from your account settings.
///#### Obtaining Access Tokens
///You can generate and manage your access tokens directly in the NoCFO web application under Account Settings.
///- Testing environment: https://login-tst.nocfo.io/auth/tokens/
///- Production environment: https://login.nocfo.io/auth/tokens/
///### Developer Support and Community
///Join our [NoCFO Slack Community](https://nocfocommunity.slack.com) to connect with other developers, share best practices, and get direct support from our team.
///In the Slack workspace, you can:
///- Ask questions about API usage and integrations
///- Get quick feedback and troubleshooting help
///- Share your use cases and solutions with others
///[**Click here to join**](https://join.slack.com/t/nocfocommunity/shared_invite/zt-3b7i1rcfp-4JmhWVTqGxzireppzCwA)
type NocfoApiClient(httpClient: HttpClient) =
    ///<summary>
    ///Returns a short-lived JWT (1 hour) for the authenticated user. If a `business_slug` is provided, the token is scoped to that business.
    ///Use the returned token in the `Authorization` header:
    ///`Authorization: Token &amp;lt;jwt&amp;gt;`
    ///Business scoping: when provided, the `business_slug` is embedded in the JWT. The token is only valid for the business whose slug was requested, and requests must match that slug in the business context.
    ///</summary>
    member this.AuthJwtCreate(body: GetJwtTokenRequestRequest, ?cancellationToken: CancellationToken) =
        async {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, content) = OpenApiHttp.postAsync httpClient "/auth/jwt/" requestParts cancellationToken

            match int status with
            | 200 -> return AuthJwtCreate.OK(Serializer.deserialize content)
            | 400 -> return AuthJwtCreate.BadRequest
            | _ -> return AuthJwtCreate.Forbidden
        }

    ///<summary>
    ///List businesses. Supports search by business name, slug, and identifier value.
    ///</summary>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="search">A search term.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessesList
        (
            ?page: int,
            ?pageSize: int,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, content) = OpenApiHttp.getAsync httpClient "/v1/business/" requestParts cancellationToken
            return SettingsBusinessesList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a new business
    ///</summary>
    member this.SettingsBusinessCreate(body: BusinessRequest, ?cancellationToken: CancellationToken) =
        async {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, content) = OpenApiHttp.postAsync httpClient "/v1/business/" requestParts cancellationToken
            return SettingsBusinessCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve the currently selected business (`business_slug`). Use this for business settings and metadata.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/" requestParts cancellationToken

            return SettingsBusinessRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace the currently selected business (`business_slug`, full body). Use this for business settings and metadata updates.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessReplace
        (
            businessSlug: string,
            body: BusinessRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync httpClient "/v1/business/{business_slug}/" requestParts cancellationToken

            return SettingsBusinessReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update the currently selected business (`business_slug`). Use this for business settings and metadata updates.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessUpdate
        (
            businessSlug: string,
            body: PatchedBusinessRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync httpClient "/v1/business/{business_slug}/" requestParts cancellationToken

            return SettingsBusinessUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete the currently selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessDelete(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync httpClient "/v1/business/{business_slug}/" requestParts cancellationToken

            return SettingsBusinessDelete.NoContent
        }

    ///<summary>
    ///List accounts for the selected business. Supports search by account number/name and filtering by type, usage, visibility, and tags.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="isShown">If set to true, it will return accounts that are selected to be shown in the UI.</param>
    ///<param name="isUsed">If set to true, it will return accounts that have at least one entry.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="search">A search term.</param>
    ///<param name="tags">Filter accounts by tags. Account will be shown if it has at least one associated document with the specified tag.</param>
    ///<param name="type">
    ///Filter accounts by account type enum values (for example `ASS_PAY`, `LIA_DUE`, `REV_SAL`, `EXP`).
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
    ///</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountsList
        (
            businessSlug: string,
            ?isShown: bool,
            ?isUsed: bool,
            ?page: int,
            ?pageSize: int,
            ?search: string,
            ?tags: float,
            ?``type``: list<string>,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if isShown.IsSome then
                      RequestPart.query ("is_shown", isShown.Value)
                  if isUsed.IsSome then
                      RequestPart.query ("is_used", isUsed.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if tags.IsSome then
                      RequestPart.query ("tags", tags.Value)
                  if ``type``.IsSome then
                      RequestPart.query ("type", ``type``.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/account/" requestParts cancellationToken

            return BookkeepingAccountsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create an account for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountCreate
        (
            businessSlug: string,
            body: AccountListRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/business/{business_slug}/account/" requestParts cancellationToken

            return BookkeepingAccountCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one account by ID. This endpoint can access hidden accounts when an explicit ID is provided.
    ///</summary>
    ///<param name="accountId">Identifier of the target account. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountRetrieve
        (
            accountId: int,
            businessSlug: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/account/{account_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingAccountRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one account by ID (full body).
    ///</summary>
    ///<param name="accountId">Identifier of the target account. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountReplace
        (
            accountId: int,
            businessSlug: string,
            body: AccountRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/account/{account_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingAccountReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one account by ID.
    ///</summary>
    ///<param name="accountId">Identifier of the target account. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountUpdate
        (
            accountId: int,
            businessSlug: string,
            body: PatchedAccountRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/account/{account_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingAccountUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one account by ID.
    ///</summary>
    ///<param name="accountId">Identifier of the target account. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountDelete(accountId: int, businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/account/{account_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingAccountDelete.NoContent
        }

    ///<summary>
    ///Mark one account as hidden by ID.
    ///</summary>
    ///<param name="accountId">Identifier of the target account. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountHide(accountId: int, businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/account/{account_id}/hide/"
                    requestParts
                    cancellationToken

            return BookkeepingAccountHide.OK
        }

    ///<summary>
    ///Mark one account as shown by ID.
    ///</summary>
    ///<param name="accountId">Identifier of the target account. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingAccountShow(accountId: int, businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("account_id", accountId)
                  RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/account/{account_id}/show/"
                    requestParts
                    cancellationToken

            return BookkeepingAccountShow.OK
        }

    ///<summary>
    ///Retrieve country-specific business identifier requirements for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.ConstantsBusinessIdentifiersRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/business_identifiers/"
                    requestParts
                    cancellationToken

            return ConstantsBusinessIdentifiersRetrieve.OK
        }

    member this.V1BusinessConstantsBusinessInvoicingCountriesRetrieve
        (
            businessSlug: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/business_invoicing_countries/"
                    requestParts
                    cancellationToken

            return V1BusinessConstantsBusinessInvoicingCountriesRetrieve.OK
        }

    member this.V1BusinessConstantsCountriesRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/countries/"
                    requestParts
                    cancellationToken

            return V1BusinessConstantsCountriesRetrieve.OK
        }

    ///<summary>
    ///Retrieve supported e-invoice operators.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.ConstantsEinvoiceOperatorsRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/einvoice_operators/"
                    requestParts
                    cancellationToken

            return ConstantsEinvoiceOperatorsRetrieve.OK
        }

    member this.V1BusinessConstantsPermissionGroupsRetrieve
        (
            businessSlug: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/permission_groups/"
                    requestParts
                    cancellationToken

            return V1BusinessConstantsPermissionGroupsRetrieve.OK
        }

    member this.V1BusinessConstantsPermissionsRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/permissions/"
                    requestParts
                    cancellationToken

            return V1BusinessConstantsPermissionsRetrieve.OK
        }

    ///<summary>
    ///Retrieve VAT codes available for the selected business country configuration.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.ConstantsVatCodesRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/vat_codes/"
                    requestParts
                    cancellationToken

            return ConstantsVatCodesRetrieve.OK
        }

    member this.V1BusinessConstantsVatPostingMethodsRetrieve
        (
            businessSlug: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/vat_posting_methods/"
                    requestParts
                    cancellationToken

            return V1BusinessConstantsVatPostingMethodsRetrieve.OK
        }

    ///<summary>
    ///Retrieve VAT rate configuration effective for a specific date in the selected business country configuration.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="dateAt">Effective date for VAT rate lookup, format YYYY-MM-DD (for example 2026-01-31).</param>
    ///<param name="cancellationToken"></param>
    member this.ConstantsVatRatesRetrieve(businessSlug: string, dateAt: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.query ("date_at", dateAt) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/vat_rates/"
                    requestParts
                    cancellationToken

            return ConstantsVatRatesRetrieve.OK(Serializer.deserialize content)
        }

    member this.V1BusinessConstantsVatReportingMethodsRetrieve
        (
            businessSlug: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/constants/vat_reporting_methods/"
                    requestParts
                    cancellationToken

            return V1BusinessConstantsVatReportingMethodsRetrieve.OK
        }

    ///<summary>
    ///List contacts for the selected business. Supports search and filters such as invoicing-enabled, exact name, and excluded contact ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="excludeId">Exclude contact by ID.</param>
    ///<param name="isInvoicingEnabled">Filter contacts by whether they have invoicing enabled.</param>
    ///<param name="nameIexact">Filter contacts by exact name match (case insensitive).</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="search">A search term.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingContactsList
        (
            businessSlug: string,
            ?excludeId: int,
            ?isInvoicingEnabled: bool,
            ?nameIexact: string,
            ?page: int,
            ?pageSize: int,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if excludeId.IsSome then
                      RequestPart.query ("exclude_id", excludeId.Value)
                  if isInvoicingEnabled.IsSome then
                      RequestPart.query ("is_invoicing_enabled", isInvoicingEnabled.Value)
                  if nameIexact.IsSome then
                      RequestPart.query ("name_iexact", nameIexact.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/contacts/" requestParts cancellationToken

            return InvoicingContactsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a contact for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingContactCreate
        (
            businessSlug: string,
            body: ContactRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/business/{business_slug}/contacts/" requestParts cancellationToken

            return InvoicingContactCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one contact by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="contactId">Identifier of the target contact. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingContactRetrieve(businessSlug: string, contactId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("contact_id", contactId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/contacts/{contact_id}/"
                    requestParts
                    cancellationToken

            return InvoicingContactRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one contact by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="contactId">Identifier of the target contact. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingContactReplace
        (
            businessSlug: string,
            contactId: int,
            body: ContactRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("contact_id", contactId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/contacts/{contact_id}/"
                    requestParts
                    cancellationToken

            return InvoicingContactReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one contact by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="contactId">Identifier of the target contact. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingContactUpdate
        (
            businessSlug: string,
            contactId: int,
            body: PatchedContactRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("contact_id", contactId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/contacts/{contact_id}/"
                    requestParts
                    cancellationToken

            return InvoicingContactUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one contact by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="contactId">Identifier of the target contact. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingContactDelete(businessSlug: string, contactId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("contact_id", contactId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/contacts/{contact_id}/"
                    requestParts
                    cancellationToken

            return InvoicingContactDelete.NoContent
        }

    ///<summary>
    ///List accounting documents for the selected business. Supports search and extensive filtering (including account type, date range, draft/flag state, tags, and attachments).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="account">Filter documents by account ID.</param>
    ///<param name="accountType">Filter documents that include at least one entry matching the provided account type. The parameter can be repeated, e.g. account_type=ASS_DUE&amp;account_type=LIA_DUE. Values must be valid `AccountType` enum values.</param>
    ///<param name="contact">Filter documents by contact ID.</param>
    ///<param name="dateFrom">Filter documents from this date onwards. Date is inclusive.</param>
    ///<param name="dateTo">Filter documents up to this date. Date is inclusive.</param>
    ///<param name="expense">Filter documents that have expense entries.</param>
    ///<param name="file">Filter documents by attachment ID.</param>
    ///<param name="hasAttachments">Filter documents that have attachments.</param>
    ///<param name="hasRule">Filter documents that have an accounting rule.</param>
    ///<param name="income">Filter documents that have income entries.</param>
    ///<param name="isDraft">Filter documents by draft status.</param>
    ///<param name="isFlagged">Filter documents by flagged status.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="rule">Filter documents by accounting rule ID.</param>
    ///<param name="search">A search term.</param>
    ///<param name="tagged">Filter documents by user ID tagged in comments.</param>
    ///<param name="tags">Filter documents by tag ID.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentsList
        (
            businessSlug: string,
            ?account: float,
            ?accountType: string,
            ?contact: float,
            ?dateFrom: string,
            ?dateTo: string,
            ?expense: bool,
            ?file: int,
            ?hasAttachments: bool,
            ?hasRule: bool,
            ?income: bool,
            ?isDraft: bool,
            ?isFlagged: bool,
            ?page: int,
            ?pageSize: int,
            ?rule: float,
            ?search: string,
            ?tagged: float,
            ?tags: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if account.IsSome then
                      RequestPart.query ("account", account.Value)
                  if accountType.IsSome then
                      RequestPart.query ("account_type", accountType.Value)
                  if contact.IsSome then
                      RequestPart.query ("contact", contact.Value)
                  if dateFrom.IsSome then
                      RequestPart.query ("date_from", dateFrom.Value)
                  if dateTo.IsSome then
                      RequestPart.query ("date_to", dateTo.Value)
                  if expense.IsSome then
                      RequestPart.query ("expense", expense.Value)
                  if file.IsSome then
                      RequestPart.query ("file", file.Value)
                  if hasAttachments.IsSome then
                      RequestPart.query ("has_attachments", hasAttachments.Value)
                  if hasRule.IsSome then
                      RequestPart.query ("has_rule", hasRule.Value)
                  if income.IsSome then
                      RequestPart.query ("income", income.Value)
                  if isDraft.IsSome then
                      RequestPart.query ("is_draft", isDraft.Value)
                  if isFlagged.IsSome then
                      RequestPart.query ("is_flagged", isFlagged.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if rule.IsSome then
                      RequestPart.query ("rule", rule.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if tagged.IsSome then
                      RequestPart.query ("tagged", tagged.Value)
                  if tags.IsSome then
                      RequestPart.query ("tags", tags.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/document/" requestParts cancellationToken

            return BookkeepingDocumentsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create an accounting document for the selected business. On create, entries are generated from `blueprint`.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentCreate
        (
            businessSlug: string,
            body: DocumentListRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/business/{business_slug}/document/" requestParts cancellationToken

            return BookkeepingDocumentCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one accounting document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRetrieve
        (
            businessSlug: string,
            documentId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one accounting document by ID (full document body). Entries are recalculated from the current blueprint payload.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentReplace
        (
            businessSlug: string,
            documentId: int,
            body: DocumentInstanceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one accounting document by ID. On update, entries are recalculated from the current blueprint payload.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentUpdate
        (
            businessSlug: string,
            documentId: int,
            body: PatchedDocumentInstanceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one accounting document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentDelete
        (
            businessSlug: string,
            documentId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentDelete.NoContent
        }

    ///<summary>
    ///Attach files to one document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentAttachFiles
        (
            businessSlug: string,
            documentId: int,
            body: DocumentAttachmentIdsRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/attach_files/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentAttachFiles.OK(Serializer.deserialize content)
        }

    member this.V1BusinessDocumentActionCopyCreate
        (
            businessSlug: string,
            documentId: int,
            body: DocumentInstanceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/copy/"
                    requestParts
                    cancellationToken

            return V1BusinessDocumentActionCopyCreate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Detach files from one document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentDetachFiles
        (
            businessSlug: string,
            documentId: int,
            body: DocumentAttachmentIdsRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/detach_files/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentDetachFiles.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Flag one document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentFlag(businessSlug: string, documentId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/flag/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentFlag.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Lock one document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentLock(businessSlug: string, documentId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/lock/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentLock.OK(Serializer.deserialize content)
        }

    member this.V1BusinessDocumentActionSetFilesCreate
        (
            businessSlug: string,
            documentId: int,
            body: DocumentInstanceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/set_files/"
                    requestParts
                    cancellationToken

            return V1BusinessDocumentActionSetFilesCreate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Unflag one document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentUnflag
        (
            businessSlug: string,
            documentId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/unflag/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentUnflag.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Unlock one document by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentUnlock
        (
            businessSlug: string,
            documentId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/action/unlock/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentUnlock.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List entries for a single document. Entries are typically generated and maintained through the document `blueprint`, so update the document when you need to change accounting logic.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingEntriesList
        (
            businessSlug: string,
            documentId: int,
            ?page: int,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/entry/"
                    requestParts
                    cancellationToken

            return BookkeepingEntriesList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List all relations belonging to a document. Each relation links the document to another document with a role and type (e.g. accrual/settlement pair).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationsList
        (
            businessSlug: string,
            documentId: int,
            ?page: int,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a document relation between two documents in the same business (source and target, with roles and relation type).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationCreate
        (
            businessSlug: string,
            documentId: int,
            body: DocumentRelationRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one document relation by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="relationId">Identifier of the target relation. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationRetrieve
        (
            businessSlug: string,
            documentId: int,
            relationId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.path ("relation_id", relationId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/{relation_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one document relation by ID (full update).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="relationId">Identifier of the target relation. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationReplace
        (
            businessSlug: string,
            documentId: int,
            relationId: int,
            body: DocumentRelationRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.path ("relation_id", relationId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/{relation_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Partially update one document relation by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="relationId">Identifier of the target relation. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationUpdate
        (
            businessSlug: string,
            documentId: int,
            relationId: int,
            body: PatchedDocumentRelationRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.path ("relation_id", relationId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/{relation_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one document relation by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="relationId">Identifier of the target relation. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationDelete
        (
            businessSlug: string,
            documentId: int,
            relationId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId)
                  RequestPart.path ("relation_id", relationId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/{relation_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationDelete.NoContent
        }

    ///<summary>
    ///List suggested document relations for a document, based on business rules and context (for example matching accrual/settlement pairs).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="documentId">Identifier of the target document. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationSuggestionsList
        (
            businessSlug: string,
            documentId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/relation/suggestions/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationSuggestionsList.OK(Serializer.deserialize content)
        }

    member this.V1BusinessDocumentSuggestAttachmentsRetrieve
        (
            businessSlug: string,
            documentId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("document_id", documentId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/document/{document_id}/suggest_attachments/"
                    requestParts
                    cancellationToken

            return V1BusinessDocumentSuggestAttachmentsRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Preview document relation suggestions from a draft blueprint and optional filters (date, contact, excluded document IDs) without persisting a document.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingDocumentRelationSuggestionsPreview
        (
            businessSlug: string,
            body: DocumentRelationSuggestionPreviewRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/document/relation/suggestions/preview/"
                    requestParts
                    cancellationToken

            return BookkeepingDocumentRelationSuggestionsPreview.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Upload a file using multipart form data. Accepted fields: `file` (required), `name` (required), `type` (optional MIME type), and `folder_id` (optional).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingFileUpload
        (
            businessSlug: string,
            body: FileUploadJsonRequestRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/file_upload/"
                    requestParts
                    cancellationToken

            return BookkeepingFileUpload.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List files for the selected business. Supports search and filtering by usage, folder, root-level files, and tags.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="folder"></param>
    ///<param name="isUsed"></param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="root"></param>
    ///<param name="search">A search term.</param>
    ///<param name="tags"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingFilesList
        (
            businessSlug: string,
            ?folder: float,
            ?isUsed: bool,
            ?page: int,
            ?pageSize: int,
            ?root: bool,
            ?search: string,
            ?tags: float,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if folder.IsSome then
                      RequestPart.query ("folder", folder.Value)
                  if isUsed.IsSome then
                      RequestPart.query ("is_used", isUsed.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if root.IsSome then
                      RequestPart.query ("root", root.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if tags.IsSome then
                      RequestPart.query ("tags", tags.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/files/" requestParts cancellationToken

            return BookkeepingFilesList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one file by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="fileId">Identifier of the target file. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingFileRetrieve(businessSlug: string, fileId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("file_id", fileId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/files/{file_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingFileRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one file's metadata by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="fileId">Identifier of the target file. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingFileReplace
        (
            businessSlug: string,
            fileId: int,
            body: AttachmentInstanceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("file_id", fileId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/files/{file_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingFileReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one file's metadata by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="fileId">Identifier of the target file. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingFileUpdate
        (
            businessSlug: string,
            fileId: int,
            body: PatchedAttachmentInstanceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("file_id", fileId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/files/{file_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingFileUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one file by ID. Delete requires lock-safe permissions.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="fileId">Identifier of the target file. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingFileDelete(businessSlug: string, fileId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("file_id", fileId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/files/{file_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingFileDelete.NoContent
        }

    ///<summary>
    ///List account headers (account groups) for the selected business. Supports name search and returns hierarchy metadata (parent and level).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="search">A search term.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingHeadersList
        (
            businessSlug: string,
            ?page: int,
            ?pageSize: int,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/header/" requestParts cancellationToken

            return BookkeepingHeadersList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create an account header (account group) for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingHeaderCreate
        (
            businessSlug: string,
            body: HeaderListRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/business/{business_slug}/header/" requestParts cancellationToken

            return BookkeepingHeaderCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one account header by ID. This endpoint is read-only; use header list/create for creation.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="headerId">Identifier of the target header. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingHeaderRetrieve(businessSlug: string, headerId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("header_id", headerId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/header/{header_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingHeaderRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List or upsert business identifiers for the selected business. Identifier `type` uses enum `y_tunnus`, `vat_code`, or `steuernummer`.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessIdentifiersList
        (
            businessSlug: string,
            ?page: int,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/identifiers/"
                    requestParts
                    cancellationToken

            return SettingsBusinessIdentifiersList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List or upsert business identifiers for the selected business. Identifier `type` uses enum `y_tunnus`, `vat_code`, or `steuernummer`.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessIdentifierCreate
        (
            businessSlug: string,
            body: BusinessIdentifierRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/identifiers/"
                    requestParts
                    cancellationToken

            return SettingsBusinessIdentifierCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one business identifier by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="identifierId">Identifier of the target identifier. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessIdentifierRetrieve
        (
            businessSlug: string,
            identifierId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("identifier_id", identifierId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/identifiers/{identifier_id}/"
                    requestParts
                    cancellationToken

            return SettingsBusinessIdentifierRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one business identifier by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="identifierId">Identifier of the target identifier. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessIdentifierReplace
        (
            businessSlug: string,
            identifierId: int,
            body: BusinessIdentifierRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("identifier_id", identifierId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/identifiers/{identifier_id}/"
                    requestParts
                    cancellationToken

            return SettingsBusinessIdentifierReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one business identifier by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="identifierId">Identifier of the target identifier. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessIdentifierUpdate
        (
            businessSlug: string,
            identifierId: int,
            body: PatchedBusinessIdentifierRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("identifier_id", identifierId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/identifiers/{identifier_id}/"
                    requestParts
                    cancellationToken

            return SettingsBusinessIdentifierUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one business identifier by ID. Deletion is blocked for country-required identifier types.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="identifierId">Identifier of the target identifier. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessIdentifierDelete
        (
            businessSlug: string,
            identifierId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("identifier_id", identifierId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/identifiers/{identifier_id}/"
                    requestParts
                    cancellationToken

            return SettingsBusinessIdentifierDelete.NoContent
        }

    ///<summary>
    ///Retrieve current user (`me`) permissions for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="cancellationToken"></param>
    member this.SettingsBusinessPermissionsRetrieve(businessSlug: string, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/me/permissions/"
                    requestParts
                    cancellationToken

            return SettingsBusinessPermissionsRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List accounting periods for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="cancellationToken"></param>
    member this.ReportingAccountingPeriodsList
        (
            businessSlug: string,
            ?page: int,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/period/" requestParts cancellationToken

            return ReportingAccountingPeriodsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a new accounting period for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingAccountingPeriodCreate
        (
            businessSlug: string,
            body: PeriodRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/business/{business_slug}/period/" requestParts cancellationToken

            return ReportingAccountingPeriodCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one accounting period (`period_id`) for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="periodId">Identifier of the target period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.ReportingAccountingPeriodRetrieve
        (
            businessSlug: string,
            periodId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("period_id", periodId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/period/{period_id}/"
                    requestParts
                    cancellationToken

            return ReportingAccountingPeriodRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one accounting period (`period_id`) for the selected business (`business_slug`) using a full payload.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="periodId">Identifier of the target period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingAccountingPeriodReplace
        (
            businessSlug: string,
            periodId: int,
            body: PeriodRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("period_id", periodId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/period/{period_id}/"
                    requestParts
                    cancellationToken

            return ReportingAccountingPeriodReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one accounting period (`period_id`) for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="periodId">Identifier of the target period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingAccountingPeriodUpdate
        (
            businessSlug: string,
            periodId: int,
            body: PatchedPeriodRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("period_id", periodId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/period/{period_id}/"
                    requestParts
                    cancellationToken

            return ReportingAccountingPeriodUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one accounting period (`period_id`) for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="periodId">Identifier of the target period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.ReportingAccountingPeriodDelete
        (
            businessSlug: string,
            periodId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("period_id", periodId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/period/{period_id}/"
                    requestParts
                    cancellationToken

            return ReportingAccountingPeriodDelete.NoContent
        }

    ///<summary>
    ///Generate balance sheet report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingBalanceSheetRetrieve
        (
            businessSlug: string,
            body: PointInTimeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/balance-sheet/"
                    requestParts
                    cancellationToken

            return ReportingBalanceSheetRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate short balance sheet report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingBalanceSheetShortRetrieve
        (
            businessSlug: string,
            body: PointInTimeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/balance-sheet-short/"
                    requestParts
                    cancellationToken

            return ReportingBalanceSheetShortRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate equity changes report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingEquityChangesRetrieve
        (
            businessSlug: string,
            body: DateRangeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/equity-changes/"
                    requestParts
                    cancellationToken

            return ReportingEquityChangesRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate income statement report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingIncomeStatementRetrieve
        (
            businessSlug: string,
            body: DateRangeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/income-statement/"
                    requestParts
                    cancellationToken

            return ReportingIncomeStatementRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate short income statement report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingIncomeStatementShortRetrieve
        (
            businessSlug: string,
            body: DateRangeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/income-statement-short/"
                    requestParts
                    cancellationToken

            return ReportingIncomeStatementShortRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate journal report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingJournalRetrieve
        (
            businessSlug: string,
            body: DateRangeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/journal/"
                    requestParts
                    cancellationToken

            return ReportingJournalRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate ledger report data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingLedgerRetrieve
        (
            businessSlug: string,
            body: DateRangeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/ledger/"
                    requestParts
                    cancellationToken

            return ReportingLedgerRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Generate VAT statement data as JSON using typed report request filters and columns.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatRetrieve
        (
            businessSlug: string,
            body: DateRangeTypedReportRequestSchemaRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/report/vat-report/"
                    requestParts
                    cancellationToken

            return ReportingVatRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///List tags for the selected business. Tags can be used to group documents/events and to filter report calculations.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingTagsList
        (
            businessSlug: string,
            ?page: int,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/business/{business_slug}/tags/" requestParts cancellationToken

            return BookkeepingTagsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a tag for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingTagCreate(businessSlug: string, body: TagRequest, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/business/{business_slug}/tags/" requestParts cancellationToken

            return BookkeepingTagCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one tag by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="tagId">Identifier of the target tag. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingTagRetrieve(businessSlug: string, tagId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("tag_id", tagId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/tags/{tag_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingTagRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one tag by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="tagId">Identifier of the target tag. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingTagReplace
        (
            businessSlug: string,
            tagId: int,
            body: TagRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("tag_id", tagId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/tags/{tag_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingTagReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one tag by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="tagId">Identifier of the target tag. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingTagUpdate
        (
            businessSlug: string,
            tagId: int,
            body: PatchedTagRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("tag_id", tagId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/tags/{tag_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingTagUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one tag by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="tagId">Identifier of the target tag. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.BookkeepingTagDelete(businessSlug: string, tagId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("tag_id", tagId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/tags/{tag_id}/"
                    requestParts
                    cancellationToken

            return BookkeepingTagDelete.NoContent
        }

    ///<summary>
    ///List VAT periods for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatPeriodsList
        (
            businessSlug: string,
            ?page: int,
            ?pageSize: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/vat_period/"
                    requestParts
                    cancellationToken

            return ReportingVatPeriodsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a new VAT period for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatPeriodCreate
        (
            businessSlug: string,
            body: VatPeriodRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/business/{business_slug}/vat_period/"
                    requestParts
                    cancellationToken

            return ReportingVatPeriodCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one VAT period (`vat_period_id`) for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="vatPeriodId">Identifier of the target vat period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatPeriodRetrieve
        (
            businessSlug: string,
            vatPeriodId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("vat_period_id", vatPeriodId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/business/{business_slug}/vat_period/{vat_period_id}/"
                    requestParts
                    cancellationToken

            return ReportingVatPeriodRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one VAT period (`vat_period_id`) for the selected business (`business_slug`) using a full payload.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="vatPeriodId">Identifier of the target vat period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatPeriodReplace
        (
            businessSlug: string,
            vatPeriodId: int,
            body: VatPeriodRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("vat_period_id", vatPeriodId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/business/{business_slug}/vat_period/{vat_period_id}/"
                    requestParts
                    cancellationToken

            return ReportingVatPeriodReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one VAT period (`vat_period_id`) for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="vatPeriodId">Identifier of the target vat period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatPeriodUpdate
        (
            businessSlug: string,
            vatPeriodId: int,
            body: PatchedVatPeriodRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("vat_period_id", vatPeriodId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/business/{business_slug}/vat_period/{vat_period_id}/"
                    requestParts
                    cancellationToken

            return ReportingVatPeriodUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one VAT period (`vat_period_id`) for the selected business (`business_slug`).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="vatPeriodId">Identifier of the target vat period. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.ReportingVatPeriodDelete
        (
            businessSlug: string,
            vatPeriodId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("vat_period_id", vatPeriodId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/business/{business_slug}/vat_period/{vat_period_id}/"
                    requestParts
                    cancellationToken

            return ReportingVatPeriodDelete.NoContent
        }

    ///<summary>
    ///List sales invoices for the selected business. For sync/integration use cases, status-based filters support both current status (`status`) and status transition history windows (`status_changed_to` + `status_changed_at_gte/lte`). When available, invoice payloads also include related bank transaction/document linkage.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="createdAtGte">Filter invoices by creation date from this date onwards (inclusive).</param>
    ///<param name="createdAtLte">Filter invoices by creation date up to this date (inclusive).</param>
    ///<param name="dueDateGte">Filter invoices from this due date onwards (inclusive).</param>
    ///<param name="dueDateLte">Filter invoices up to this due date (inclusive).</param>
    ///<param name="invoicingDateGte">Filter invoices from this invoicing date onwards (inclusive).</param>
    ///<param name="invoicingDateLte">Filter invoices up to this invoicing date (inclusive).</param>
    ///<param name="isCreditNote">Filter invoices based on whether they are credit notes.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="search">A search term.</param>
    ///<param name="status">Filter invoices by current status. Supports repeated query params (status=PAID&amp;status=ACCEPTED) or comma-separated values (status=PAID,ACCEPTED). Allowed values: DRAFT, ACCEPTED, PAID, CREDIT_LOSS.</param>
    ///<param name="statusChangedAtGte">Filter status transition history from this date onwards (inclusive). Requires status_changed_to.</param>
    ///<param name="statusChangedAtLte">Filter status transition history up to this date (inclusive). Requires status_changed_to.</param>
    ///<param name="statusChangedTo">Filter invoices that have transitioned to this status within the status_changed_at window.</param>
    ///<param name="updatedAtGte">Filter invoices by last update date from this date onwards (inclusive).</param>
    ///<param name="updatedAtLte">Filter invoices by last update date up to this date (inclusive).</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoicesList
        (
            businessSlug: string,
            ?createdAtGte: string,
            ?createdAtLte: string,
            ?dueDateGte: string,
            ?dueDateLte: string,
            ?invoicingDateGte: string,
            ?invoicingDateLte: string,
            ?isCreditNote: bool,
            ?page: int,
            ?pageSize: int,
            ?search: string,
            ?status: string,
            ?statusChangedAtGte: string,
            ?statusChangedAtLte: string,
            ?statusChangedTo: string,
            ?updatedAtGte: string,
            ?updatedAtLte: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if createdAtGte.IsSome then
                      RequestPart.query ("created_at_gte", createdAtGte.Value)
                  if createdAtLte.IsSome then
                      RequestPart.query ("created_at_lte", createdAtLte.Value)
                  if dueDateGte.IsSome then
                      RequestPart.query ("due_date_gte", dueDateGte.Value)
                  if dueDateLte.IsSome then
                      RequestPart.query ("due_date_lte", dueDateLte.Value)
                  if invoicingDateGte.IsSome then
                      RequestPart.query ("invoicing_date_gte", invoicingDateGte.Value)
                  if invoicingDateLte.IsSome then
                      RequestPart.query ("invoicing_date_lte", invoicingDateLte.Value)
                  if isCreditNote.IsSome then
                      RequestPart.query ("is_credit_note", isCreditNote.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if status.IsSome then
                      RequestPart.query ("status", status.Value)
                  if statusChangedAtGte.IsSome then
                      RequestPart.query ("status_changed_at_gte", statusChangedAtGte.Value)
                  if statusChangedAtLte.IsSome then
                      RequestPart.query ("status_changed_at_lte", statusChangedAtLte.Value)
                  if statusChangedTo.IsSome then
                      RequestPart.query ("status_changed_to", statusChangedTo.Value)
                  if updatedAtGte.IsSome then
                      RequestPart.query ("updated_at_gte", updatedAtGte.Value)
                  if updatedAtLte.IsSome then
                      RequestPart.query ("updated_at_lte", updatedAtLte.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/invoicing/{business_slug}/invoice/" requestParts cancellationToken

            return InvoicingSalesInvoicesList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create a sales invoice for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceCreate
        (
            businessSlug: string,
            body: InvoiceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/invoicing/{business_slug}/invoice/" requestParts cancellationToken

            return InvoicingSalesInvoiceCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one sales invoice by ID. When an invoice is linked to imported banking data, related bank transaction and document details are included in the response.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceRetrieve
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one sales invoice by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceReplace
        (
            businessSlug: string,
            invoiceId: int,
            body: InvoiceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one sales invoice by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceUpdate
        (
            businessSlug: string,
            invoiceId: int,
            body: PatchedInvoiceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one sales invoice by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceDelete
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceDelete.NoContent
        }

    ///<summary>
    ///Transition invoice status from draft to accepted and assign invoice numbering/reference.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceAccept
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/actions/accept/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceAccept.OK
        }

    ///<summary>
    ///Set invoice status to `CREDIT_LOSS`.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceMarkCreditLoss
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/actions/credit_loss/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceMarkCreditLoss.OK
        }

    ///<summary>
    ///Disable future recurrence generation for this invoice template.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceRecurrenceDisable
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/actions/disable_recurrence/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceRecurrenceDisable.OK
        }

    ///<summary>
    ///Set invoice status to `PAID` and update payment date to current timestamp.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceMarkPaid
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/actions/paid/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceMarkPaid.OK
        }

    ///<summary>
    ///Set invoice status back to `ACCEPTED` (unpaid state).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceMarkUnpaid
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/actions/unpaid/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceMarkUnpaid.OK
        }

    ///<summary>
    ///List available delivery methods for this invoice (for example email).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceDeliveryMethods
        (
            businessSlug: string,
            invoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/delivery_methods/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceDeliveryMethods.OK
        }

    ///<summary>
    ///Send the invoice using the selected delivery method. This action can trigger external delivery channels such as email/e-invoice.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceSend
        (
            businessSlug: string,
            invoiceId: int,
            body: SendInvoiceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/send/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceSend.OK
        }

    ///<summary>
    ///Add a custom status message shown in NoCFO UI, typically used to surface progress from external processing systems.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceStatusMessageCreate
        (
            businessSlug: string,
            invoiceId: int,
            body: InvoiceExternalStatusMessageRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/status_messages/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceStatusMessageCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one external status message by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="invoiceId">Identifier of the target invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="messageId">Identifier of the target message. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingSalesInvoiceStatusMessageDelete
        (
            businessSlug: string,
            invoiceId: int,
            messageId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("invoice_id", invoiceId)
                  RequestPart.path ("message_id", messageId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/invoice/{invoice_id}/status_messages/{message_id}/"
                    requestParts
                    cancellationToken

            return InvoicingSalesInvoiceStatusMessageDelete.NoContent
        }

    ///<summary>
    ///List invoicing products for the selected business. Products are reusable invoice row templates and support code/name search.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="search">A search term.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingProductsList
        (
            businessSlug: string,
            ?page: int,
            ?pageSize: int,
            ?search: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync httpClient "/v1/invoicing/{business_slug}/product/" requestParts cancellationToken

            return InvoicingProductsList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Create an invoicing product for the selected business.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingProductCreate
        (
            businessSlug: string,
            body: ProductRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.postAsync httpClient "/v1/invoicing/{business_slug}/product/" requestParts cancellationToken

            return InvoicingProductCreate.Created(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one product by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="productId">Identifier of the target product. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingProductRetrieve(businessSlug: string, productId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("product_id", productId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/product/{product_id}/"
                    requestParts
                    cancellationToken

            return InvoicingProductRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one product by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="productId">Identifier of the target product. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingProductReplace
        (
            businessSlug: string,
            productId: int,
            body: ProductRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("product_id", productId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/product/{product_id}/"
                    requestParts
                    cancellationToken

            return InvoicingProductReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one product by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="productId">Identifier of the target product. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingProductUpdate
        (
            businessSlug: string,
            productId: int,
            body: PatchedProductRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("product_id", productId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/product/{product_id}/"
                    requestParts
                    cancellationToken

            return InvoicingProductUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one product by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="productId">Identifier of the target product. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingProductDelete(businessSlug: string, productId: int, ?cancellationToken: CancellationToken) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("product_id", productId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/product/{product_id}/"
                    requestParts
                    cancellationToken

            return InvoicingProductDelete.NoContent
        }

    ///<summary>
    ///List purchase invoices for the selected business. Supports date-window filtering (`invoicing_date_*`, `due_date_*`, `created_at_*`, `updated_at_*`, `payment_date_*`), paid-state filtering (`is_paid`), import source filtering (`import_source`) and search.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="createdAtGte">Filter purchase invoices by creation date from this date onwards (inclusive).</param>
    ///<param name="createdAtLte">Filter purchase invoices by creation date up to this date (inclusive).</param>
    ///<param name="dueDateGte">Filter purchase invoices from this due date onwards (inclusive).</param>
    ///<param name="dueDateLte">Filter purchase invoices up to this due date (inclusive).</param>
    ///<param name="importSource">Filter purchase invoices by import source.</param>
    ///<param name="invoicingDateGte">Filter purchase invoices from this invoicing date onwards (inclusive).</param>
    ///<param name="invoicingDateLte">Filter purchase invoices up to this invoicing date (inclusive).</param>
    ///<param name="isPaid">Filter purchase invoices by paid status.</param>
    ///<param name="page">A page number within the paginated result set.</param>
    ///<param name="pageSize">Number of results to return per page.</param>
    ///<param name="paymentDateGte">Filter purchase invoices by payment date from this date onwards (inclusive).</param>
    ///<param name="paymentDateLte">Filter purchase invoices by payment date up to this date (inclusive).</param>
    ///<param name="search">A search term.</param>
    ///<param name="updatedAtGte">Filter purchase invoices by last update date from this date onwards (inclusive).</param>
    ///<param name="updatedAtLte">Filter purchase invoices by last update date up to this date (inclusive).</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingPurchaseInvoicesList
        (
            businessSlug: string,
            ?createdAtGte: string,
            ?createdAtLte: string,
            ?dueDateGte: string,
            ?dueDateLte: string,
            ?importSource: string,
            ?invoicingDateGte: string,
            ?invoicingDateLte: string,
            ?isPaid: bool,
            ?page: int,
            ?pageSize: int,
            ?paymentDateGte: string,
            ?paymentDateLte: string,
            ?search: string,
            ?updatedAtGte: string,
            ?updatedAtLte: string,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  if createdAtGte.IsSome then
                      RequestPart.query ("created_at_gte", createdAtGte.Value)
                  if createdAtLte.IsSome then
                      RequestPart.query ("created_at_lte", createdAtLte.Value)
                  if dueDateGte.IsSome then
                      RequestPart.query ("due_date_gte", dueDateGte.Value)
                  if dueDateLte.IsSome then
                      RequestPart.query ("due_date_lte", dueDateLte.Value)
                  if importSource.IsSome then
                      RequestPart.query ("import_source", importSource.Value)
                  if invoicingDateGte.IsSome then
                      RequestPart.query ("invoicing_date_gte", invoicingDateGte.Value)
                  if invoicingDateLte.IsSome then
                      RequestPart.query ("invoicing_date_lte", invoicingDateLte.Value)
                  if isPaid.IsSome then
                      RequestPart.query ("is_paid", isPaid.Value)
                  if page.IsSome then
                      RequestPart.query ("page", page.Value)
                  if pageSize.IsSome then
                      RequestPart.query ("page_size", pageSize.Value)
                  if paymentDateGte.IsSome then
                      RequestPart.query ("payment_date_gte", paymentDateGte.Value)
                  if paymentDateLte.IsSome then
                      RequestPart.query ("payment_date_lte", paymentDateLte.Value)
                  if search.IsSome then
                      RequestPart.query ("search", search.Value)
                  if updatedAtGte.IsSome then
                      RequestPart.query ("updated_at_gte", updatedAtGte.Value)
                  if updatedAtLte.IsSome then
                      RequestPart.query ("updated_at_lte", updatedAtLte.Value) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/purchase_invoice/"
                    requestParts
                    cancellationToken

            return InvoicingPurchaseInvoicesList.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Retrieve one purchase invoice by ID. When linked to imported banking data, related bank transaction and document details are included.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="purchaseInvoiceId">Identifier of the target purchase invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingPurchaseInvoiceRetrieve
        (
            businessSlug: string,
            purchaseInvoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("purchase_invoice_id", purchaseInvoiceId) ]

            let! (status, content) =
                OpenApiHttp.getAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/purchase_invoice/{purchase_invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingPurchaseInvoiceRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace one purchase invoice by ID (full body).
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="purchaseInvoiceId">Identifier of the target purchase invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingPurchaseInvoiceReplace
        (
            businessSlug: string,
            purchaseInvoiceId: int,
            body: PurchaseInvoiceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("purchase_invoice_id", purchaseInvoiceId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.putAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/purchase_invoice/{purchase_invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingPurchaseInvoiceReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update one purchase invoice by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="purchaseInvoiceId">Identifier of the target purchase invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="body"></param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingPurchaseInvoiceUpdate
        (
            businessSlug: string,
            purchaseInvoiceId: int,
            body: PatchedPurchaseInvoiceRequest,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("purchase_invoice_id", purchaseInvoiceId)
                  RequestPart.jsonContent body ]

            let! (status, content) =
                OpenApiHttp.patchAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/purchase_invoice/{purchase_invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingPurchaseInvoiceUpdate.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Delete one purchase invoice by ID.
    ///</summary>
    ///<param name="businessSlug">Business slug that scopes this request. If you do not have one yet, call `common_accessible_businesses_list` and pick a slug from the response.</param>
    ///<param name="purchaseInvoiceId">Identifier of the target purchase invoice. Use the corresponding list endpoint to discover valid IDs.</param>
    ///<param name="cancellationToken"></param>
    member this.InvoicingPurchaseInvoiceDelete
        (
            businessSlug: string,
            purchaseInvoiceId: int,
            ?cancellationToken: CancellationToken
        ) =
        async {
            let requestParts =
                [ RequestPart.path ("business_slug", businessSlug)
                  RequestPart.path ("purchase_invoice_id", purchaseInvoiceId) ]

            let! (status, content) =
                OpenApiHttp.deleteAsync
                    httpClient
                    "/v1/invoicing/{business_slug}/purchase_invoice/{purchase_invoice_id}/"
                    requestParts
                    cancellationToken

            return InvoicingPurchaseInvoiceDelete.NoContent
        }

    ///<summary>
    ///Retrieve the currently authenticated user profile. This endpoint is always scoped to the caller and does not accept a user ID.
    ///</summary>
    member this.IdentityUserRetrieve(?cancellationToken: CancellationToken) =
        async {
            let requestParts = []
            let! (status, content) = OpenApiHttp.getAsync httpClient "/v1/user/" requestParts cancellationToken
            return IdentityUserRetrieve.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Replace the currently authenticated user profile (full body). This endpoint is always scoped to the caller and does not accept a user ID.
    ///</summary>
    member this.IdentityUserReplace(body: CurrentUserRequest, ?cancellationToken: CancellationToken) =
        async {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, content) = OpenApiHttp.putAsync httpClient "/v1/user/" requestParts cancellationToken
            return IdentityUserReplace.OK(Serializer.deserialize content)
        }

    ///<summary>
    ///Update the currently authenticated user profile. This endpoint is always scoped to the caller and does not accept a user ID.
    ///</summary>
    member this.IdentityUserUpdate(body: PatchedCurrentUserRequest, ?cancellationToken: CancellationToken) =
        async {
            let requestParts = [ RequestPart.jsonContent body ]
            let! (status, content) = OpenApiHttp.patchAsync httpClient "/v1/user/" requestParts cancellationToken
            return IdentityUserUpdate.OK(Serializer.deserialize content)
        }
