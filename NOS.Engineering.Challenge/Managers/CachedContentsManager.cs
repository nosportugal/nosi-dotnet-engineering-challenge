using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Managers
{
    public class CachedContentsManager : IContentsManager
    {
        private readonly IContentsManager _innerManager;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedContentsManager> _logger;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedContentsManager(IContentsManager innerManager, IMemoryCache cache, ILogger<CachedContentsManager> logger)
        {
            _innerManager = innerManager;
            _cache = cache;
            _logger = logger;

            // Set cache options (e.g., 1 minute expiration)
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));
        }

        public async Task<IEnumerable<Content?>> GetManyContents()
        {
            const string cacheKey = "GetManyContents";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Content?> contents))
            {
                _logger.LogInformation("Cache miss for GetManyContents");
                contents = await _innerManager.GetManyContents().ConfigureAwait(false);
                _cache.Set(cacheKey, contents, _cacheOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for GetManyContents");
            }
            return contents;
        }

        public async Task<Content?> CreateContent(ContentDto content)
        {
            var createdContent = await _innerManager.CreateContent(content).ConfigureAwait(false);
            if (createdContent != null)
            {
                _cache.Remove("GetManyContents"); // Invalidate cache for list
                _cache.Remove("SearchContents"); // Invalidate cache for search
            }
            return createdContent;
        }

        public async Task<Content?> GetContent(Guid id)
        {
            var cacheKey = $"GetContent_{id}";
            if (!_cache.TryGetValue(cacheKey, out Content? content))
            {
                _logger.LogInformation("Cache miss for GetContent with ID {Id}", id);
                content = await _innerManager.GetContent(id).ConfigureAwait(false);
                if (content != null)
                {
                    _cache.Set(cacheKey, content, _cacheOptions);
                }
            }
            else
            {
                _logger.LogInformation("Cache hit for GetContent with ID {Id}", id);
            }
            return content;
        }

        public async Task<Content?> UpdateContent(Guid id, ContentDto content)
        {
            var updatedContent = await _innerManager.UpdateContent(id, content).ConfigureAwait(false);
            if (updatedContent != null)
            {
                _cache.Remove($"GetContent_{id}"); // Invalidate cache for item
                _cache.Remove("GetManyContents"); // Invalidate cache for list
                _cache.Remove("SearchContents"); // Invalidate cache for search
            }
            return updatedContent;
        }

        public async Task<Guid> DeleteContent(Guid id)
        {
            var deletedId = await _innerManager.DeleteContent(id).ConfigureAwait(false);
            if (deletedId != Guid.Empty)
            {
                _cache.Remove($"GetContent_{id}"); // Invalidate cache for item
                _cache.Remove("GetManyContents"); // Invalidate cache for list
                _cache.Remove("SearchContents"); // Invalidate cache for search
            }
            return deletedId;
        }

        public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
        {
            var updatedContent = await _innerManager.AddGenres(id, genres).ConfigureAwait(false);
            if (updatedContent != null)
            {
                _cache.Remove($"GetContent_{id}"); // Invalidate cache for item
                _cache.Remove("GetManyContents"); // Invalidate cache for list
                _cache.Remove("SearchContents"); // Invalidate cache for search
            }
            return updatedContent;
        }

        public async Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
        {
            var updatedContent = await _innerManager.RemoveGenres(id, genres).ConfigureAwait(false);
            if (updatedContent != null)
            {
                _cache.Remove($"GetContent_{id}"); // Invalidate cache for item
                _cache.Remove("GetManyContents"); // Invalidate cache for list
                _cache.Remove("SearchContents"); // Invalidate cache for search
            }
            return updatedContent;
        }

        public async Task<IEnumerable<Content?>> SearchContents(string? title, string? genre)
        {
            var cacheKey = $"SearchContents_{title}_{genre}";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Content?> contents))
            {
                _logger.LogInformation("Cache miss for SearchContents with Title: {Title} and Genre: {Genre}", title, genre);
                contents = await _innerManager.SearchContents(title, genre).ConfigureAwait(false);
                _cache.Set(cacheKey, contents, _cacheOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for SearchContents with Title: {Title} and Genre: {Genre}", title, genre);
            }
            return contents;
        }
    }
}
