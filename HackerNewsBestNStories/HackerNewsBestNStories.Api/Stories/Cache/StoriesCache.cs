using HackerNewsBestNStories.Api.ApiClient;
using HackerNewsBestNStories.Api.ApiClient.DTO;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsBestNStories.Api.Stories.Cache;

public class StoriesCacheKey
{
    public static string GetKey(int storyId)
    {
        return $"story_{storyId}";
    }
}

public class TopStoriesCacheKey
{
    public static readonly TopStoriesCacheKey Instance = new();
}

public interface IStoriesCache
{
    Task<IEnumerable<int>> GetBestStories(CancellationToken cancellationToken);
    Task<StoryCacheEntry?> GetStoryById(int storyId, CancellationToken cancellationToken);
}

public class StoriesCache(IMemoryCache memoryCache, IHackerNewsApiClient hackerNewsApiClient) : IStoriesCache
{
    public async Task<IEnumerable<int>> GetBestStories(CancellationToken cancellationToken)
    {
        if(memoryCache.TryGetValue(TopStoriesCacheKey.Instance, out IEnumerable<int>? value))
        {
            return value ?? [];
        }
        
        var topStoryIds = await hackerNewsApiClient.GetBestStoriesAsync(cancellationToken);
        memoryCache.Set(TopStoriesCacheKey.Instance, topStoryIds, TimeSpan.FromMinutes(5));
    
        return topStoryIds ?? [];
    }

    public async Task<StoryCacheEntry?> GetStoryById(int storyId, CancellationToken cancellationToken)
    {
        var storyKey = StoriesCacheKey.GetKey(storyId);
        if(memoryCache.TryGetValue(storyKey, out StoryCacheEntry? storyCache))
        {
            return storyCache ?? new StoryCacheEntry();
        }
        
        var storyDetails = await hackerNewsApiClient.GetStoryById(storyId, cancellationToken);
        var entryCache = MapToStoryCacheEntry(storyDetails);
        memoryCache.Set(storyKey, entryCache, TimeSpan.FromMinutes(5));
        return entryCache;
    }

    private StoryCacheEntry? MapToStoryCacheEntry(StoryDto? dto)
    {
        if (dto == null)
        {
            return null;
        }
        
        return new StoryCacheEntry
        {
            Title = dto.Title,
            Uri = dto.Url,
            PostedBy = dto.By,
            Time = DateTimeOffset.FromUnixTimeSeconds(dto.Time),
            Score = dto.Score,
            CommentCount = dto.Descendants
        };
    }
}