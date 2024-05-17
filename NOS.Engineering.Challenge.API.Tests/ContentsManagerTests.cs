using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.API.Tests
{
    public class ContentsManagerTests
    {
        private readonly Mock<IDatabase<Content, ContentDto>> _databaseMock;
        private readonly Mock<ILogger<ContentsManager>> _loggerMock;
        private readonly ContentsManager _manager;

        public ContentsManagerTests()
        {
            _databaseMock = new Mock<IDatabase<Content, ContentDto>>();
            _loggerMock = new Mock<ILogger<ContentsManager>>();
            _manager = new ContentsManager(_databaseMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetManyContents_ShouldReturnContents()
        {
            // Arrange
            var contents = new List<Content>
        {
            new Content(Guid.NewGuid(), "Title1", "SubTitle1", "Description1", "ImageUrl1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre1" }),
            new Content(Guid.NewGuid(), "Title2", "SubTitle2", "Description2", "ImageUrl2", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre2" })
        };

            _databaseMock.Setup(db => db.ReadAll()).ReturnsAsync(contents);

            // Act
            var result = await _manager.GetManyContents();

            // Assert
            Assert.Equal(contents, result);
        }

        [Fact]
        public async Task CreateContent_ShouldReturnCreatedContent()
        {
            // Arrange
            var contentDto = new ContentDto("Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });
            var content = new Content(Guid.NewGuid(), "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });

            _databaseMock.Setup(db => db.Create(contentDto)).ReturnsAsync(content);

            // Act
            var result = await _manager.CreateContent(contentDto);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task GetContent_ShouldReturnContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });

            _databaseMock.Setup(db => db.Read(contentId)).ReturnsAsync(content);

            // Act
            var result = await _manager.GetContent(contentId);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task UpdateContent_ShouldReturnUpdatedContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var contentDto = new ContentDto("Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });

            _databaseMock.Setup(db => db.Update(contentId, contentDto)).ReturnsAsync(content);

            // Act
            var result = await _manager.UpdateContent(contentId, contentDto);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task DeleteContent_ShouldReturnDeletedContentId()
        {
            // Arrange
            var contentId = Guid.NewGuid();

            _databaseMock.Setup(db => db.Delete(contentId)).ReturnsAsync(contentId);

            // Act
            var result = await _manager.DeleteContent(contentId);

            // Assert
            Assert.Equal(contentId, result);
        }

        [Fact]
        public async Task AddGenres_ShouldReturnUpdatedContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Genre1", "Genre2" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });

            _databaseMock.Setup(db => db.Read(contentId)).ReturnsAsync(content);
            _databaseMock.Setup(db => db.Update(contentId, It.IsAny<ContentDto>())).ReturnsAsync(content);

            // Act
            var result = await _manager.AddGenres(contentId, genres);

            // Assert
            Assert.Equal(content, result);
        }

        [Fact]
        public async Task RemoveGenres_ShouldReturnUpdatedContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Genre1", "Genre2" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre1", "Genre2", "Genre3" });

            _databaseMock.Setup(db => db.Read(contentId)).ReturnsAsync(content);
            _databaseMock.Setup(db => db.Update(contentId, It.IsAny<ContentDto>())).ReturnsAsync(content);

            // Act
            var result = await _manager.RemoveGenres(contentId, genres);

            // Assert
            Assert.Equal(content, result);
        }
    }
}
