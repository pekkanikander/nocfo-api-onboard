#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"
#r "bin/Debug/net10.0/hawaii-client.dll"

open Nocfo
open NocfoApi.Types

let emptyPatch = PatchedAccountRequest.Create()
let hasChanges = PatchShape<Account, PatchedAccountRequest>.HasChanges emptyPatch
printfn "PatchShape<Account,PatchedAccountRequest> initialised; empty patch has changes = %b" hasChanges
