using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;

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
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

        return createdContent == null ? Problem() : Ok(createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        return Ok(deletedId);
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        try
        {
            // Checks if id exists
            var content = await _manager.GetContent(id);

            if (content == null)
            {
                return NotFound();
            }

            // Checks if provided genres are actually new
            var existingGenres = content.GenreList ?? Enumerable.Empty<string>();    
            var uniqueGenres = genre
                    .Where( g => !existingGenres.Contains(g, StringComparer.OrdinalIgnoreCase))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
        
            if (uniqueGenres.Count == 0)
            {
                return BadRequest("The provided genres are not new");
            }
        
            // Updates the content
            var updatedContent = await _manager.UpdateContent(id, new ContentDto
            (
                title: null,
                subTitle: null,
                description: null,
                imageUrl: null,
                duration: null,
                startTime: null,
                endTime: null,
                genreList: uniqueGenres
            )
            );

            _logger. LogInformation("Genres added succcessfully to {Id}", id);

            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger. LogError(ex, "Error adding genres to {Id}", id);

            return StatusCode(500, "Error occured while adding genres");
        }
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        try
        {
            // Checks if id exists
            var content = await _manager.GetContent(id);

            if (content == null)
            {
                return NotFound();
            }

            // Updates the content
            var updatedGenres = content.GenreList.Except(genre, StringComparer.OrdinalIgnoreCase).ToList();
                
            if(updatedGenres.Count == content.GenreList.Count())
            {
                return BadRequest ("Provided genres not found");
            }
            var updatedContent = await _manager.UpdateContent(id, new ContentDto
            (
                title: null,
                subTitle: null,
                description: null,
                imageUrl: null,
                duration: null,
                startTime: null,
                endTime: null,
                genreList: updatedGenres
            )
            );

            _logger. LogInformation("Genres removed successfully from {Id}", id);

            return Ok(updatedContent);
        }

        catch (Exception ex)
        {
            _logger. LogError(ex, "Error removing genres from {Id}", id);

            return StatusCode(500, "Error occurred while removing genres");
        }

    }


    [HttpGet("search")]
    public async Task<IActionResult> SearchContents([FromQuery] string? title, [FromQuery] string? genre)
    {
        try
        {
            var contents = await _manager.GetManyContents();

            if (!string.IsNullOrEmpty(title))
            {
                contents = contents.Where(c => c.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
                
            }

            if(!string.IsNullOrEmpty(genre))
            {
                contents = contents.Where(c => c.GenreList.Contains(genre, StringComparer.OrdinalIgnoreCase));
            }

            return Ok(contents);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching");

            return StatusCode(500, "Error occurred while searching");
        }
    }
}