using Shouldly;

namespace HackerNewsBestNStories.Tests;

[TestFixture]
public class HackerNewsBestNStoriesApiTests
{
    private HackerNewsBestNStoriesApiClient  _apiClient;

    [SetUp]
    public void Setup()
    {
        _apiClient = new HackerNewsBestNStoriesApiClient();
    }

    [Test]
    public async Task should_get_best_100_stories()
    {
        var bestStories = await _apiClient.GetBestStoriesAsync(100);
        bestStories.Count.ShouldBe(100);
        Assert.That(bestStories.Select(x=> x.Score), Is.Ordered.Descending);
        await TestContext.Progress.WriteLineAsync(System.Text.Json.JsonSerializer.Serialize(bestStories));
    }
    
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(1000)]
    [TestCase(10000)]
    public async Task should_handle_many_requests(int totalRequests)
    {
        // TODO prepare NBomber or similar tool for load testing
        var numberOfStories = 200;
        await Parallel.ForEachAsync(Enumerable.Range(1, totalRequests),
            new ParallelOptions { MaxDegreeOfParallelism = 16 },
            async (i, cancellationToken) =>
            {
                var response = await _apiClient.GetBestStoriesAsync(numberOfStories, cancellationToken: cancellationToken);
                response.Count.ShouldBeGreaterThanOrEqualTo(numberOfStories);
            });
    }
}