using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using HackerNewsBestNStories.Api.Stories.Cache;

namespace HackerNewsBestNStories.Api.Stories;

[ApiController]
[Route("stories")]
[ApiVersion("1.0")]
public class StoriesApi(
    IStoriesCache storiesCache)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IReadOnlyCollection<StoryCacheEntry>> Get([FromQuery] int? numberOfStories, CancellationToken cancellationToken)
    {
        numberOfStories ??= 10;
        var topStories = await storiesCache.GetBestStories(cancellationToken);

        var storyTasks= topStories.Take(numberOfStories.Value)
            .Select(x => storiesCache.GetStoryById(x, cancellationToken))
            .ToList();
        
        var stories = await Task.WhenAll(storyTasks);
        return stories.OfType<StoryCacheEntry>().ToList();
    }
}
