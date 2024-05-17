using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Xunit;

namespace NOS.Engineering.Challenge.API.Tests
{
    public class ContentControllerTests
    {
        private readonly Mock<IContentsManager> _contentsManagerMock;
        private readonly ContentController _controller;

        public ContentControllerTests()
        {
            _contentsManagerMock = new Mock<IContentsManager>();
            _controller = new ContentController(_contentsManagerMock.Object);
        }

        [Fact]
        public async Task AddGenres_ReturnsOkResult_WhenGenresAdded()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Action", "Drama" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), genres);

            _contentsManagerMock.Setup(manager => manager.AddGenres(contentId, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(content);

            // Act
            var result = await _controller.AddGenres(contentId, genres);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }

        [Fact]
        public async Task AddGenres_ReturnsNotFoundResult_WhenContentNotFound()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Action", "Drama" };

            _contentsManagerMock.Setup(manager => manager.AddGenres(contentId, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((Content?)null);

            // Act
            var result = await _controller.AddGenres(contentId, genres);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveGenres_ReturnsOkResult_WhenGenresRemoved()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Action", "Drama" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), genres);

            _contentsManagerMock.Setup(manager => manager.RemoveGenres(contentId, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(content);

            // Act
            var result = await _controller.RemoveGenres(contentId, genres);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(content, okResult.Value);
        }

        [Fact]
        public async Task RemoveGenres_ReturnsNotFoundResult_WhenContentNotFound()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genres = new List<string> { "Action", "Drama" };

            _contentsManagerMock.Setup(manager => manager.RemoveGenres(contentId, It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync((Content?)null);

            // Act
            var result = await _controller.RemoveGenres(contentId, genres);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
