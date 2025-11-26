#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Fable.Remoting.Json, 2.18.0"
#r "generated/bin/Debug/netstandard2.0/NocfoApi.dll"
#r "bin/Debug/net9.0/hawaii-client.dll"

open Nocfo
open NocfoApi.Types

let descriptors = DeltaShape<Account, PatchedAccount>.Descriptors
printfn "DeltaShape<Account,PatchedAccount> initialised with %d fields." descriptors.Length
