using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NOS.Engineering.Challenge.API.Tests
{
    public class ContentControllerTests
    {
        private readonly Mock<IContentsManager> _mockManager;
        private readonly Mock<ILogger<ContentController>> _mockLogger;
        private readonly ContentController _controller;

        public ContentControllerTests()
        {
            _mockManager = new Mock<IContentsManager>();
            _mockLogger = new Mock<ILogger<ContentController>>();
            _controller = new ContentController(_mockManager.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetManyContents_ReturnsOk_WhenContentsExist()
        {
            // Arrange
            var contents = new List<Content>
            {
                new Content(Guid.NewGuid(), "Title1", "SubTitle1", "Description1", "ImageUrl1", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>())
            };

            _mockManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);

            // Act
            var result = await _controller.GetManyContents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContents = Assert.IsType<List<Content>>(okResult.Value);
            Assert.NotEmpty(returnedContents);
        }

        [Fact]
        public async Task GetManyContents_ReturnsNotFound_WhenNoContentsExist()
        {
            // Arrange
            var contents = new List<Content>();

            _mockManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);

            // Act
            var result = await _controller.GetManyContents();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetContent_ReturnsOk_WhenContentIsFound()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>());

            _mockManager.Setup(m => m.GetContent(contentId)).ReturnsAsync(content);

            // Act
            var result = await _controller.GetContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContent = Assert.IsType<Content>(okResult.Value);
            Assert.Equal(contentId, returnedContent.Id);
        }

        [Fact]
        public async Task GetContent_ReturnsNotFound_WhenContentIsNotFound()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            _mockManager.Setup(m => m.GetContent(contentId)).ReturnsAsync((Content)null);

            // Act
            var result = await _controller.GetContent(contentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreateContent_ReturnsProblem_WhenCreationFails()
        {
            // Arrange
            var contentInput = new ContentInput { Title = "Title", SubTitle = "SubTitle", Description = "Description", ImageUrl = "ImageUrl", Duration = 120 };
            var contentDto = contentInput.ToDto();

            _mockManager.Setup(m => m.CreateContent(contentDto)).ReturnsAsync((Content)null);

            // Act
            var result = await _controller.CreateContent(contentInput);

            // Assert
        }

        [Fact]
        public async Task UpdateContent_ReturnsNotFound_WhenContentIsNotFound()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var contentInput = new ContentInput { Title = "Title", SubTitle = "SubTitle", Description = "Description", ImageUrl = "ImageUrl", Duration = 120 };

            _mockManager.Setup(m => m.UpdateContent(contentId, contentInput.ToDto())).ReturnsAsync((Content)null);

            // Act
            var result = await _controller.UpdateContent(contentId, contentInput);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteContent_ReturnsOk_WhenContentIsDeleted()
        {
            // Arrange
            var contentId = Guid.NewGuid();

            _mockManager.Setup(m => m.DeleteContent(contentId)).ReturnsAsync(contentId);

            // Act
            var result = await _controller.DeleteContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedId = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(contentId, returnedId);
        }

        [Fact]
        public async Task SearchContents_ReturnsOk_WhenContentsMatchFilters()
        {
            // Arrange
            var titleFilter = "Title";
            var genreFilter = "Action";
            var contents = new List<Content>();

            _mockManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);

            // Act
            var result = await _controller.SearchContents(titleFilter, genreFilter);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SearchContents_ReturnsNotFound_WhenNoContentsMatchFilters()
        {
            // Arrange
            var titleFilter = "Title";
            var genreFilter = "Action";
            var contents = new List<Content>();

            _mockManager.Setup(m => m.GetManyContents()).ReturnsAsync(contents);

            // Act
            var result = await _controller.SearchContents(titleFilter, genreFilter);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
