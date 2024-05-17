using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Xunit;

namespace NOS.Engineering.Challenge.API.Tests
{
    public class ContentControllerTests
    {
        private readonly Mock<IContentsManager> _contentsManagerMock;
        private readonly Mock<ILogger<ContentController>> _loggerMock;
        private readonly ContentController _controller;

        public ContentControllerTests()
        {
            _contentsManagerMock = new Mock<IContentsManager>();
            _loggerMock = new Mock<ILogger<ContentController>>();
            _controller = new ContentController(_contentsManagerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetManyContents_ShouldReturnOkResultWithContents()
        {
            // Arrange
            var contents = new List<Content>
        {
            new Content(Guid.NewGuid(), "Title1", "SubTitle1", "Description1", "ImageUrl1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre1" }),
            new Content(Guid.NewGuid(), "Title2", "SubTitle2", "Description2", "ImageUrl2", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre2" })
        };

            _contentsManagerMock.Setup(manager => manager.GetManyContents()).ReturnsAsync(contents);

            // Act
            var result = await _controller.GetManyContents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(contents, okResult.Value);
        }

        [Fact]
        public async Task GetManyContents_ShouldReturnNotFoundWhenNoContents()
        {
            // Arrange
            _contentsManagerMock.Setup(manager => manager.GetManyContents()).ReturnsAsync(new List<Content>());

            // Act
            var result = await _controller.GetManyContents();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetContent_ShouldReturnOkResultWithContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });

            _contentsManagerMock.Setup(manager => manager.GetContent(contentId)).ReturnsAsync(content);

            // Act
            var result = await _controller.GetContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }

        [Fact]
        public async Task GetContent_ShouldReturnNotFoundWhenContentDoesNotExist()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            _contentsManagerMock.Setup(manager => manager.GetContent(contentId)).ReturnsAsync((Content?)null);

            // Act
            var result = await _controller.GetContent(contentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateContent_ShouldReturnOkResultWithCreatedContent()
        {
            // Arrange
            var contentInput = new ContentInput
            {
                Title = "Title",
                SubTitle = "SubTitle",
                Description = "Description",
                ImageUrl = "ImageUrl",
                Duration = 120,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2)
            };
            var contentDto = contentInput.ToDto();
            var content = new Content(Guid.NewGuid(), contentInput.Title, contentInput.SubTitle, contentInput.Description, contentInput.ImageUrl, contentInput.Duration.Value, contentInput.StartTime.Value, contentInput.EndTime.Value, new List<string>());

            _contentsManagerMock.Setup(manager => manager.CreateContent(contentDto)).ReturnsAsync(content);

            // Act
            var result = await _controller.CreateContent(contentInput);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }

        [Fact]
        public async Task UpdateContent_ShouldReturnOkResultWithUpdatedContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var contentInput = new ContentInput
            {
                Title = "Title",
                SubTitle = "SubTitle",
                Description = "Description",
                ImageUrl = "ImageUrl",
                Duration = 120,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(2)
            };
            var contentDto = contentInput.ToDto();
            var content = new Content(contentId, contentInput.Title, contentInput.SubTitle, contentInput.Description, contentInput.ImageUrl, contentInput.Duration.Value, contentInput.StartTime.Value, contentInput.EndTime.Value, new List<string>());

            _contentsManagerMock.Setup(manager => manager.UpdateContent(contentId, contentDto)).ReturnsAsync(content);

            // Act
            var result = await _controller.UpdateContent(contentId, contentInput);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }

        [Fact]
        public async Task DeleteContent_ShouldReturnOkResultWithDeletedContentId()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            _contentsManagerMock.Setup(manager => manager.DeleteContent(contentId)).ReturnsAsync(contentId);

            // Act
            var result = await _controller.DeleteContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(contentId, okResult.Value);
        }

        [Fact]
        public async Task AddGenres_ShouldReturnOkResultWithUpdatedContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Genre1", "Genre2" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre" });

            _contentsManagerMock.Setup(manager => manager.AddGenres(contentId, genres)).ReturnsAsync(content);

            // Act
            var result = await _controller.AddGenres(contentId, genres);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }

        [Fact]
        public async Task RemoveGenres_ShouldReturnOkResultWithUpdatedContent()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Genre1", "Genre2" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Genre1", "Genre2", "Genre3" });

            _contentsManagerMock.Setup(manager => manager.RemoveGenres(contentId, genres)).ReturnsAsync(content);

            // Act
            var result = await _controller.RemoveGenres(contentId, genres);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }
    }
}
