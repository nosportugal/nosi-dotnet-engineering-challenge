using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;
using Xunit;

namespace NOS.Engineering.Challenge.Tests
{
    public class CacheContentDatabaseTests
    {
        private readonly Mock<IDatabase<Content?, ContentDto>> _mockDatabase;
        private readonly IMemoryCache _cache;
        private readonly CacheContentDatabase _cacheContentDatabase;

        public CacheContentDatabaseTests()
        {
            _mockDatabase = new Mock<IDatabase<Content?, ContentDto>>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacheContentDatabase = new CacheContentDatabase(_mockDatabase.Object, _cache);
        }

        [Fact]
        public async Task Create_AddsContentToCache()
        {
            // Arrange
            var contentDto = new ContentDto(
                "Title",
                "SubTitle",
                "Description",
                "ImageUrl",
                120,
                DateTime.Now,
                DateTime.Now.AddHours(2),
                new List<string>());

            var content = new Content(
                Guid.NewGuid(),
                "Title",
                "SubTitle",
                "Description",
                "ImageUrl",
                120,
                DateTime.Now,
                DateTime.Now.AddHours(2),
                new List<string>());

            _mockDatabase.Setup(db => db.Create(contentDto)).ReturnsAsync(content);

            // Act
            var result = await _cacheContentDatabase.Create(contentDto);

            // Assert
            Assert.Equal(content, result);
            Assert.True(_cache.TryGetValue(content.Id, out Content? cachedContent));
            Assert.Equal(content, cachedContent);
        }

        [Fact]
        public async Task Read_GetsContentFromCache()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var content = new Content(
                contentId,
                "Title",
                "SubTitle",
                "Description",
                "ImageUrl",
                120,
                DateTime.Now,
                DateTime.Now.AddHours(2),
                new List<string>());

            _cache.Set(contentId, content);

            // Act
            var result = await _cacheContentDatabase.Read(contentId);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task Read_GetsContentFromDatabase_WhenNotInCache()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var content = new Content(
                contentId,
                "Title",
                "SubTitle",
                "Description",
                "ImageUrl",
                120,
                DateTime.Now,
                DateTime.Now.AddHours(2),
                new List<string>());

            _mockDatabase.Setup(db => db.Read(contentId)).ReturnsAsync(content);

            // Act
            var result = await _cacheContentDatabase.Read(contentId);

            // Assert
            Assert.Equal(content, result);
            Assert.True(_cache.TryGetValue(contentId, out Content? cachedContent));
            Assert.Equal(content, cachedContent);
        }

        // Additional tests...
    }
}
