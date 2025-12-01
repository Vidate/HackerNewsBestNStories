using HackerNewsBestNStories.Api.ApiClient;
using HackerNewsBestNStories.Api.Stories;
using HackerNewsBestNStories.Api.Stories.Cache;

namespace HackerNewsBestNStories.Api.Infrastructure;

public static class HackerNewsConfigurationExtension
{
    public static void RegisterHackerNewsApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HackerNewsApiConfiguration>(configuration.GetSection(nameof(HackerNewsApiConfiguration)));
        
        services.AddHttpClient<IHackerNewsApiClient, HackerNewsApiClient>(client =>
        {
            var hackerNewsApiConfiguration = configuration.GetSection(nameof(HackerNewsApiConfiguration)).Get<HackerNewsApiConfiguration>();
            client.BaseAddress = new Uri(hackerNewsApiConfiguration!.BaseUrl);
        });
    }
    
    public static void RegisterCaches(this IServiceCollection services)
    {
        services.AddMemoryCache();   // TODO add redis (IDistributedCache)
        services.AddSingleton<IStoriesCache, StoriesCache>();   
    }
}