using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class CacheContentDatabase : IDatabase<Content?, ContentDto>
    {
        private readonly IDatabase<Content?, ContentDto> _innerDatabase;
        private readonly IMemoryCache _cache;

        public CacheContentDatabase(IDatabase<Content?, ContentDto> innerDatabase, IMemoryCache cache)
        {
            _innerDatabase = innerDatabase;
            _cache = cache;
        }

        public async Task<Content?> Create(ContentDto contentDto)
        {
            var content = await _innerDatabase.Create(contentDto);
            if (content != null)
            {
                _cache.Set(content.Id, content);
            }
            return content;
        }

        public async Task<Content?> Read(Guid id)
        {
            if (_cache.TryGetValue(id, out Content? content))
            {
                return content;
            }

            content = await _innerDatabase.Read(id);
            if (content != null)
            {
                _cache.Set(id, content);
            }

            return content;
        }

        public async Task<IEnumerable<Content?>> ReadAll()
        {
            // Caching the whole collection might not be ideal depending on size,
            // consider implementing cache based on individual items if necessary.
            var cacheKey = "AllContents";
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Content?> contents))
            {
                return contents;
            }

            contents = await _innerDatabase.ReadAll();
            _cache.Set(cacheKey, contents);

            return contents;
        }

        public async Task<Content?> Update(Guid id, ContentDto contentDto)
        {
            var content = await _innerDatabase.Update(id, contentDto);
            if (content != null)
            {
                _cache.Set(content.Id, content);
            }
            return content;
        }

        public async Task<Guid> Delete(Guid id)
        {
            var deletedId = await _innerDatabase.Delete(id);
            if (deletedId != Guid.Empty)
            {
                _cache.Remove(id);
            }
            return deletedId;
        }
    }
}
