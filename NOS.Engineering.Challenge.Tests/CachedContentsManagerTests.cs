using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using System.Text;
using System.Text.Json;

namespace NOS.Engineering.Challenge.Tests
{
    public class CachedContentsManagerTests
    {
        private readonly Mock<IContentsManager> _mockContentsManager;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ILogger<CachedContentsManager>> _mockLogger;
        private readonly CachedContentsManager _cachedContentsManager;

        public CachedContentsManagerTests()
        {
            _mockContentsManager = new Mock<IContentsManager>();
            _mockCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<CachedContentsManager>>();
            _cachedContentsManager = new CachedContentsManager(_mockContentsManager.Object, _mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetManyContents_ReturnsContentsFromCache_WhenAvailable()
        {
            try
            {
                // Arrange
                var contents = new List<Content> 
                {
                    new Content( Guid.NewGuid(), "O Colecionador de Quadrinhos", "Uma Jornada Nerd",
                                 "Um jovem nerd embarca em uma jornada épica em sua história em quadrinhos favorita.",
                                 "comic_collector.jpg", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>{"Aventura", "Comédia", "Drama"}),
                    new Content( Guid.NewGuid(), "Os Exploradores da Terra Nerd", "Uma Aventura Nerd pela História",
                                 "Um grupo de nerds viaja no tempo para explorar grandes momentos da história da humanidade.",
                                 "nerd_explorers.jpg", 180, DateTime.Now, DateTime.Now.AddHours(3), new List<string>{"Aventura", "Ficção Científica", "Fantasia", "História"})
                };
                var cachedContentsJson = JsonSerializer.Serialize(contents);

                _mockCache.Setup(cache => cache.GetAsync("AllContents", default)).ReturnsAsync(Encoding.UTF8.GetBytes(cachedContentsJson));

                // Act
                var result = await _cachedContentsManager.GetManyContents();

                // Assert
                var expectedResultCount = contents.Count;
                var actualResultCount = result.ToList().Count;
                Assert.Equal(expectedResultCount, actualResultCount);
                _mockContentsManager.Verify(c => c.GetManyContents(), Times.Never);  
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
