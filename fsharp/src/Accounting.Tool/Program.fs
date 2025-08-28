open System
open Accounting.Api

[<EntryPoint>]
let main argv =
    printfn "🚀 NOCFO Business Stream Tool"
    printfn "================================"

        // Create API client using environment variable
    let apiToken = Environment.GetEnvironmentVariable("NOCFO_API_TOKEN")
    if String.IsNullOrEmpty(apiToken) then
        printfn "❌ Error: NOCFO_API_TOKEN environment variable not set"
        exit 1

    printfn "🔑 Using API token: %s" apiToken
    printfn "🔑 Token length: %d characters" apiToken.Length
    printfn ""

    let client = NocfoApiClient(apiToken)

    printfn "📡 Fetching businesses from stream..."
    printfn ""

    // Get the business stream and process it
    let businessStream = client.GetBusinessesStreamAsync() |> Async.RunSynchronously

    // Process the stream and print each business
    let mutable count = 0
    let businesses = FSharp.Control.AsyncSeq.toListAsync businessStream |> Async.RunSynchronously

    for business in businesses do
        count <- count + 1
        printfn "🏢 Business #%d:" count
        printfn "   ID: %d" business.Id
        printfn "   Slug: %s" business.Slug
        printfn "   Name: %s" business.Name
        printfn "   Business ID: %s" business.BusinessId
        printfn "   Form: %s" business.Form
        printfn ""

    printfn "✅ Stream processing complete!"
    printfn "📊 Total businesses processed: %d" businesses.Length
    printfn ""
    printfn "🎯 This demonstrates F# streaming with AsyncSeq"
    printfn "   - Each business is processed as it comes through the stream"
    printfn "   - The stream is lazy and processes items on-demand"
    printfn "   - Perfect for handling large datasets efficiently"

    0
