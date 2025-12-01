using System.Net.Http.Json;
using HackerNewsBestNStories.Api.Stories.Cache;

namespace HackerNewsBestNStories.Tests;

public class HackerNewsBestNStoriesApiClient()
{
    private static readonly HttpClient HttpClient = new ()
    {
        BaseAddress = new Uri("http://localhost:5143/")
    };

    public async Task<IReadOnlyCollection<StoryCacheEntry>> GetBestStoriesAsync(
        int? numberOfStories = null,
        CancellationToken cancellationToken = default)
    {
        var query = numberOfStories.HasValue ? $"?numberOfStories={numberOfStories.Value}" : "";
        var response = await HttpClient.GetFromJsonAsync<IReadOnlyCollection<StoryCacheEntry>>(
            $"stories{query}", 
            cancellationToken);

        return response ?? [];
    }
}