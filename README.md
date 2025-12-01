### To run unit tests, you need to start HackerNewsBestNStories.API first
 ```bash 
 dotnet run --project HackerNewsBestNStories/HackerNewsBestNStories.API/HackerNewsBestNStories.Api.csproj
 ```
Then Run in another window
 ```bash 
dotnet test HackerNewsBestNStories/HackerNewsBestNStories.Tests/
```

The best approach for testing will be setup running API in other process and mock request to HackerNews API 
using WireMock. In that way we will be able to run tests on CI pipeline without 'hack'.
This approach will also give ability for integration tests which will 'cover' more part of code and mock e.g. 429 or other error code which may return by third party API (HackerNews)

There are a few things which I will change if I have a little more time.
1) Add IDistribatedCache - add shared cache using Redis/KeyDb. This will allow to run multiple API (as a microservices) and do not overload HackerNews. Cache will be refresh each 5min (TBD). To initialize cache, we may consider using pattern called Leader-Election.
2) Error Handling - 500/429 handling or other (401 if by any reason they add auth as requirements)
3) Polly - retry + 429 handling.
4) HttpClient cache - besides Memory cache we may consider adding additional layer caching on Http level.
5) HackerNewsBestNStories API client generation form OpenAPI definition. 
6) Rate limiter for HttpClient


