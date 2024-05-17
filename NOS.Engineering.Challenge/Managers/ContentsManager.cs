using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;
    private readonly ILogger<ContentsManager> _logger;

    public ContentsManager(IDatabase<Content?, ContentDto> database, ILogger<ContentsManager> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IEnumerable<Content?>> GetManyContents()
    {
        _logger.LogInformation("Getting all contents");
        var contents = await _database.ReadAll().ConfigureAwait(false);
        _logger.LogInformation("Retrieved {Count} contents", contents.Count());
        return contents;
    }

    public async Task<Content?> CreateContent(ContentDto content)
    {
        _logger.LogInformation("Creating content with title {Title}", content.Title);
        var createdContent = await _database.Create(content).ConfigureAwait(false);
        if (createdContent != null)
        {
            _logger.LogInformation("Content with ID {Id} created successfully", createdContent.Id);
        }
        else
        {
            _logger.LogWarning("Failed to create content with title {Title}", content.Title);
        }
        return createdContent;
    }

    public async Task<Content?> GetContent(Guid id)
    {
        _logger.LogInformation("Getting content with ID {Id}", id);
        var content = await _database.Read(id).ConfigureAwait(false);
        if (content == null)
        {
            _logger.LogWarning("Content with ID {Id} not found", id);
        }
        return content;
    }

    public async Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        _logger.LogInformation("Updating content with ID {Id}", id);
        var updatedContent = await _database.Update(id, content).ConfigureAwait(false);
        if (updatedContent != null)
        {
            _logger.LogInformation("Content with ID {Id} updated successfully", id);
        }
        else
        {
            _logger.LogWarning("Failed to update content with ID {Id}", id);
        }
        return updatedContent;
    }

    public async Task<Guid> DeleteContent(Guid id)
    {
        _logger.LogInformation("Deleting content with ID {Id}", id);
        var deletedId = await _database.Delete(id).ConfigureAwait(false);
        if (deletedId == Guid.Empty)
        {
            _logger.LogWarning("Failed to delete content with ID {Id}", id);
        }
        else
        {
            _logger.LogInformation("Content with ID {Id} deleted successfully", id);
        }
        return deletedId;
    }

    public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        _logger.LogInformation("Adding genres to content with ID {Id}", id);
        var content = await _database.Read(id).ConfigureAwait(false);
        if (content == null)
        {
            _logger.LogWarning("Content with ID {Id} not found", id);
            return null;
        }

        content.AddGenres(genres);
        var updatedContent = await _database.Update(id, new ContentDto(content.Title, content.SubTitle, content.Description, content.ImageUrl, content.Duration, content.StartTime, content.EndTime, content.GenreList)).ConfigureAwait(false);

        if (updatedContent != null)
        {
            _logger.LogInformation("Genres added to content with ID {Id}", id);
        }
        else
        {
            _logger.LogWarning("Failed to add genres to content with ID {Id}", id);
        }

        return updatedContent;
    }

    public async Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        _logger.LogInformation("Removing genres from content with ID {Id}", id);
        var content = await _database.Read(id).ConfigureAwait(false);
        if (content == null)
        {
            _logger.LogWarning("Content with ID {Id} not found", id);
            return null;
        }

        content.RemoveGenres(genres);
        var updatedContent = await _database.Update(id, new ContentDto(content.Title, content.SubTitle, content.Description, content.ImageUrl, content.Duration, content.StartTime, content.EndTime, content.GenreList)).ConfigureAwait(false);

        if (updatedContent != null)
        {
            _logger.LogInformation("Genres removed from content with ID {Id}", id);
        }
        else
        {
            _logger.LogWarning("Failed to remove genres from content with ID {Id}", id);
        }

        return updatedContent;
    }

    public async Task<IEnumerable<Content?>> SearchContents(string? title, string? genre)
    {
        _logger.LogInformation("Searching contents with Title: {Title} and Genre: {Genre}", title, genre);
        var contents = await _database.ReadAll().ConfigureAwait(false);

        if (!string.IsNullOrEmpty(title))
        {
            contents = contents.Where(c => c != null && c.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            contents = contents.Where(c => c != null && c.GenreList.Any(g => g.Contains(genre, StringComparison.OrdinalIgnoreCase)));
        }

        return contents;
    }
}