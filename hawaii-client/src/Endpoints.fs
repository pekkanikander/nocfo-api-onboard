namespace NocfoClient

module Endpoints =
    let businessBySlug (slug: string) = $"/business/{slug}/"
    let businessList (page: int)      = $"/business/?page_size=100&page={page}"

    let accountsBySlugPage (slug: string) (page: int) =
        $"/business/{slug}/account/?page_size=100&page={page}"
    let accountById (slug: string) (id: string) =
        $"/business/{slug}/account/{id}/"

    let documentsBySlugPage (slug: string) (page: int) =
        $"/business/{slug}/document/?page_size=100&page={page}"
