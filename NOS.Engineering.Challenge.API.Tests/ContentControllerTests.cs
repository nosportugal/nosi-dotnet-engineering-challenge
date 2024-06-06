using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
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
        public async Task AddGenres_ReturnsOk_WhenGenresAreAdded()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Action", "Drama" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string>());

            _mockManager.Setup(m => m.GetContent(contentId)).ReturnsAsync(content);
            _mockManager.Setup(m => m.UpdateContent(contentId, It.IsAny<ContentDto>())).ReturnsAsync(content);

            // Act
            var result = await _controller.AddGenres(contentId, genres);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContent = Assert.IsType<Content>(okResult.Value);
            Assert.Contains("Action", returnedContent.GenreList);
            Assert.Contains("Drama", returnedContent.GenreList);
        }

        [Fact]
        public async Task RemoveGenres_ReturnsOk_WhenGenresAreRemoved()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Action" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), new List<string> { "Action", "Drama" });

            _mockManager.Setup(m => m.GetContent(contentId)).ReturnsAsync(content);
            _mockManager.Setup(m => m.UpdateContent(contentId, It.IsAny<ContentDto>())).ReturnsAsync(content);

            // Act
            var result = await _controller.RemoveGenres(contentId, genres);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContent = Assert.IsType<Content>(okResult.Value);
            Assert.DoesNotContain("Action", returnedContent.GenreList);
        }

        // Additional tests...
    }
}
