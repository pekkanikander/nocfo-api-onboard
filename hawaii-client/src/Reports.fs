namespace Nocfo.Domain

open Nocfo.Domain
open NocfoClient

module Reports =
  let addToTotals (totals: AccountClassTotals) (account: Account) : Async<Result<AccountClassTotals, DomainError>> =
    Account.hydrate account
    |> AsyncResult.bind (fun hydrated ->
      match hydrated with
      | Full full ->
        let cls = Account.classify full
        match cls with
        | Some cls -> Ok (Map.change cls (fun old -> Some ((defaultArg old 0M) + (decimal full.balance))) totals)
        | None     -> Error (DomainError.Unexpected "Account type is required")
      | Partial _  -> Error (DomainError.Unexpected "Account is not hydrated")
    )
