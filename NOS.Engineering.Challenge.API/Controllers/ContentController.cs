using System.Net;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;

namespace NOS.Engineering.Challenge.API.Controllers;

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
    [Obsolete("This endpoint is deprecated. Use /api/v1/Content/search instead.")]
    public async Task<IActionResult> GetManyContents()
    {
        _logger.LogInformation("Received request to get all contents");
        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
        {
            _logger.LogWarning("No contents found");
            return NotFound();
        }

        return Ok(contents);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchContents([FromQuery] string? title, [FromQuery] string? genre)
    {
        _logger.LogInformation("Received request to search contents with Title: {Title} and Genre: {Genre}", title, genre);
        var contents = await _manager.SearchContents(title, genre).ConfigureAwait(false);

        if (!contents.Any())
        {
            _logger.LogWarning("No contents found matching the search criteria");
            return NotFound();
        }

        return Ok(contents);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        _logger.LogInformation("Received request to get content with ID {Id}", id);
        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
        {
            _logger.LogWarning("Content with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(content);
    }

    [HttpPost]
    public async Task<IActionResult> CreateContent([FromBody] ContentInput content)
    {
        _logger.LogInformation("Received request to create content with title {Title}", content.Title);
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        if (createdContent == null)
        {
            _logger.LogError("Failed to create content with title {Title}", content.Title);
            return Problem();
        }

        return Ok(createdContent);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(Guid id, [FromBody] ContentInput content)
    {
        _logger.LogInformation("Received request to update content with ID {Id}", id);
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        if (updatedContent == null)
        {
            _logger.LogWarning("Failed to update content with ID {Id}", id);
            return NotFound();
        }

        return Ok(updatedContent);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(Guid id)
    {
        _logger.LogInformation("Received request to delete content with ID {Id}", id);
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);

        if (deletedId == Guid.Empty)
        {
            _logger.LogWarning("Failed to delete content with ID {Id}", id);
        }

        return Ok(deletedId);
    }

    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genres)
    {
        _logger.LogInformation("Received request to add genres to content with ID {Id}", id);
        var content = await _manager.AddGenres(id, genres).ConfigureAwait(false);

        if (content == null)
        {
            _logger.LogWarning("Content with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(content);
    }

    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genres)
    {
        _logger.LogInformation("Received request to remove genres from content with ID {Id}", id);
        var content = await _manager.RemoveGenres(id, genres).ConfigureAwait(false);

        if (content == null)
        {
            _logger.LogWarning("Content with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(content);
    }
}