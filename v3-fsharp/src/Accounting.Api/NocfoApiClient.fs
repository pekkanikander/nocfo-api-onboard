

namespace Accounting.Api

open FSharp.Data
open Domain

// JSON types for API responses
type BusinessResponse = JsonProvider<"""
{
  "count": 2,
  "next": null,
  "prev": null,
  "size": 2,
  "results": [
    {
      "id": 436,
      "slug": "2999322-9",
      "name": "Holotropic Breathwork Finland ry",
      "business_id": "2999322-9",
      "form": "FI_YHD"
    }
  ]
}
""">

// Simple API client for NOCFO
type NocfoApiClient(apiToken: string) =
    let baseUrl = "https://api-tst.nocfo.io"
    let headers = [
        "Authorization", $"Token {apiToken}"
        "Accept", "application/json"
        "Content-Type", "application/json"
    ]

    // Fetch businesses from the real API
    member _.GetBusinessesAsync() = async {
        let url = $"{baseUrl}/v1/business/"

        // Debug: print what we're sending
        printfn "🌐 Making request to: %s" url
        printfn "🔑 Authorization header: Token %s" apiToken
        printfn ""

        try
            let! response = Http.AsyncRequestString(
                url,
                headers = headers,
                httpMethod = "GET"
            )

            printfn "📥 Raw response: %s" response
            printfn ""

            let businessData = BusinessResponse.Parse(response)
            printfn "📊 Parsed data: %d businesses found" businessData.Results.Length
            return businessData.Results |> Array.toList
        with
        | ex ->
            printfn "Error fetching businesses: %A" ex
            return []
    }

    // Convert businesses to our domain types
    member this.GetBusinessesStreamAsync() = async {
        let! businesses = this.GetBusinessesAsync()

        // Convert JSON data to domain types
        let domainBusinesses = businesses |> List.map (fun business ->
            Business.create
                business.Id
                business.Slug
                business.Name
                business.BusinessId
                business.Form
        )

        return FSharp.Control.AsyncSeq.ofSeq domainBusinesses
    }
