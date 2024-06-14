using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Models;
using System.Text.Json;

namespace NOS.Engineering.Challenge.Managers
{
    public class CachedContentsManager : IContentsManager
    {
        private readonly IContentsManager _contentsManager;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachedContentsManager> _logger;

        public CachedContentsManager(IContentsManager contentsManager, IDistributedCache cache, ILogger<CachedContentsManager> logger)
        {
            _contentsManager = contentsManager;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<Content?>> GetManyContents()
        {
            var cacheKey = "AllContents";
            var cachedContents = await _cache.GetStringAsync(cacheKey);

            if (cachedContents != null)
            {
                _logger.LogInformation("Retrieving contents from cache.");
                return JsonSerializer.Deserialize<IEnumerable<Content?>>(cachedContents);
            }

            _logger.LogInformation("Retrieving contents from database.");
            var contents = await _contentsManager.GetManyContents();

            _logger.LogInformation("Caching contents.");
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(contents));

            return contents;
        }

        public async Task<Content?> CreateContent(ContentDto content)
        {
            var createdContent = await _contentsManager.CreateContent(content);
            if (createdContent != null)
            {
                _logger.LogInformation($"Content created: {createdContent.Id}");
                await _cache.RemoveAsync("AllContents");
            }
            return createdContent;
        }

        public async Task<Content?> GetContent(Guid id)
        {
            var cacheKey = $"Content_{id}";
            var cachedContent = await _cache.GetStringAsync(cacheKey);

            if (cachedContent != null)
            {
                _logger.LogInformation($"Retrieving content from cache: {id}");
                return JsonSerializer.Deserialize<Content?>(cachedContent);
            }

            _logger.LogInformation($"Retrieving content from database: {id}");
            var content = await _contentsManager.GetContent(id);

            if (content != null)
            {
                _logger.LogInformation($"Caching content: {id}");
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(content));
            }

            return content;
        }

        public async Task<Content?> UpdateContent(Guid id, ContentDto content)
        {
            var updatedContent = await _contentsManager.UpdateContent(id, content);
            if (updatedContent != null)
            {
                _logger.LogInformation($"Content updated: {id}");
                await _cache.RemoveAsync($"Content_{id}");
                await _cache.RemoveAsync("AllContents");
            }
            return updatedContent;
        }

        public async Task<Guid> DeleteContent(Guid id)
        {
            var deletedId = await _contentsManager.DeleteContent(id);
            if (deletedId != Guid.Empty)
            {
                _logger.LogInformation($"Content deleted: {id}");
                await _cache.RemoveAsync($"Content_{id}");
                await _cache.RemoveAsync("AllContents");
            }
            return deletedId;
        }

        public async Task<Content?> AddGenre(Guid id, IEnumerable<string> genres)
        {
            var updatedContent = await _contentsManager.AddGenre(id, genres);
            if (updatedContent != null)
            {
                _logger.LogInformation($"Genre added to content: {id}");
                await _cache.RemoveAsync($"Content_{id}");
                await _cache.RemoveAsync("AllContents");
            }
            return updatedContent;
        }

        public async Task<Content?> RemoveGenre(Guid id, IEnumerable<string> genres)
        {
            var updatedContent = await _contentsManager.RemoveGenre(id, genres);
            if (updatedContent != null)
            {
                _logger.LogInformation($"Genre removed from content: {id}");
                await _cache.RemoveAsync($"Content_{id}");
                await _cache.RemoveAsync("AllContents");
            }
            return updatedContent;
        }
    }
}
