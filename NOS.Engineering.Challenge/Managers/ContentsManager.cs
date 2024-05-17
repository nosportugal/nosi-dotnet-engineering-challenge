using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers;

public class ContentsManager : IContentsManager
{
    private readonly IDatabase<Content?, ContentDto> _database;

    public ContentsManager(IDatabase<Content?, ContentDto> database)
    {
        _database = database;
    }

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        return _database.ReadAll();
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        return _database.Create(content);
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {
        return _database.Update(id, content);
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        return _database.Delete(id);
    }

    public async Task<Content?> AddGenres(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id).ConfigureAwait(false);
        if (content == null) return null;

        content.AddGenres(genres);
        await _database.Update(id, new ContentDto(
            content.Title, 
            content.SubTitle, 
            content.Description, 
            content.ImageUrl, 
            content.Duration, 
            content.StartTime, 
            content.EndTime, 
            content.GenreList)).ConfigureAwait(false);

        return content;
    }

    public async Task<Content?> RemoveGenres(Guid id, IEnumerable<string> genres)
    {
        var content = await _database.Read(id).ConfigureAwait(false);
        if (content == null) return null;

        content.RemoveGenres(genres);
        await _database.Update(id, new ContentDto(
            content.Title, 
            content.SubTitle, 
            content.Description, 
            content.ImageUrl, 
            content.Duration, 
            content.StartTime, 
            content.EndTime, 
            content.GenreList)).ConfigureAwait(false);

        return content;
    }
}