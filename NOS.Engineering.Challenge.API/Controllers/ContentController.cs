using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContentController : Controller
    {
        private readonly IContentsManager _manager;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentsManager manager, ILogger<ContentController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetManyContents()
        {
            var contents = await _manager.GetManyContents().ConfigureAwait(false);

            if (!contents.Any())
                return NotFound();

            return Ok(contents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContent(Guid id)
        {
            var content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
                return NotFound();

            return Ok(content);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] ContentInput content)
        {
            var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

            return createdContent == null ? Problem() : Ok(createdContent);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateContent(Guid id, [FromBody] ContentInput content)
        {
            var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

            return updatedContent == null ? NotFound() : Ok(updatedContent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(Guid id)
        {
            var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
            return Ok(deletedId);
        }

        [HttpPost("{id}/genre")]
        public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genres)
        {
            _logger.LogInformation("Adding genres {Genres} to content with ID {ContentId}", string.Join(", ", genres), id);

            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
            {
                _logger.LogWarning("Content with ID {ContentId} not found", id);
                return NotFound();
            }

            var newGenres = genres.Except(content.GenreList, StringComparer.OrdinalIgnoreCase).ToList();
            if (!newGenres.Any())
            {
                _logger.LogWarning("No new genres to add for content with ID {ContentId}", id);
                return BadRequest("Genres already exist.");
            }

            content.AddGenres(newGenres);
            var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

            _logger.LogInformation("Genres {Genres} added to content with ID {ContentId}", string.Join(", ", newGenres), id);
            return Ok(updatedContent);
        }

        [HttpDelete("{id}/genre")]
        public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genres)
        {
            _logger.LogInformation("Removing genres {Genres} from content with ID {ContentId}", string.Join(", ", genres), id);

            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
            {
                _logger.LogWarning("Content with ID {ContentId} not found", id);
                return NotFound();
            }

            var removedGenres = content.GenreList.Intersect(genres, StringComparer.OrdinalIgnoreCase).ToList();
            if (!removedGenres.Any())
            {
                _logger.LogWarning("No genres found to remove for content with ID {ContentId}", id);
                return NotFound("Genres not found.");
            }

            content.RemoveGenres(genres);
            var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

            _logger.LogInformation("Genres {Genres} removed from content with ID {ContentId}", string.Join(", ", removedGenres), id);
            return Ok(updatedContent);
        }
    }
}
