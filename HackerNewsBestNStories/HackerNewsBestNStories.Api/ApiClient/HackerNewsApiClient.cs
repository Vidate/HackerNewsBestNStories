using System.Net;
using System.Text.Json;
using HackerNewsBestNStories.Api.ApiClient.DTO;
using Polly;

namespace HackerNewsBestNStories.Api.ApiClient;

public interface IHackerNewsApiClient
{
    Task<int[]> GetBestStoriesAsync(CancellationToken cancellationToken);
    Task<StoryDto?> GetStoryById(int storyId, CancellationToken cancellationToken);
}

public class HackerNewsApiClient(HttpClient httpClient, ILogger<HackerNewsApiClient> logger) : IHackerNewsApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<int[]> GetBestStoriesAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("v0/beststories.json", cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var storyIds = JsonSerializer.Deserialize<int[]>(json, JsonOptions) ?? [];
        return storyIds;
    }

    public async Task<StoryDto?> GetStoryById(int storyId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"v0/item/{storyId}.json", cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var story = JsonSerializer.Deserialize<StoryDto>(json, JsonOptions) ?? null;
        return story;
    }
}

