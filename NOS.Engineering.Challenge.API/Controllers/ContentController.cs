using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    public ContentController(IContentsManager manager, ILogger<ContentController> @object)
    {
        _manager = manager;
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
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(true);

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
    public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genre)
    {
        var content = await _manager.AddGenre(id, genre).ConfigureAwait(false);

        return content == null ? NotFound("Genre already exists or content not found.") : Ok(content);
    }

    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(Guid id, [FromBody] IEnumerable<string> genre)
    {
        var content = await _manager.RemoveGenre(id, genre).ConfigureAwait(false);

        return content == null ? NotFound("Genre not found or content not found.") : Ok(content);
    }

    [HttpGet("Search")]
    public async Task<IActionResult> SearchContents([FromQuery] string title = "", string genre = "")
    {
        var allContents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(title))
            allContents = allContents.Where(c => c.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(genre))
            allContents = allContents.Where(c => c.GenreList.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase)));

        return allContents == null || !allContents.Any() ? NotFound("No content found with the specified filters.") 
                                                         : Ok(allContents);
    }
}