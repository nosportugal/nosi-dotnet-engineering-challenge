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

    public Task<IEnumerable<Content?>> GetManyContents()
    {
        var result = _database.ReadAll();
        _logger.LogInformation($"{nameof(GetManyContents)} returned {result.Result.Count()} results");
        return result;
    }

    public Task<Content?> CreateContent(ContentDto content)
    {
        try
        {
            var result = _database.Create(content);
            _logger.LogInformation($"Sucessfully created content with ID: {result.Result.Id}");
            return result;
        }
        catch (Exception ex) {
            _logger.LogError($"{ex}");
            return null;
        }
    }

    public Task<Content?> GetContent(Guid id)
    {
        return _database.Read(id);
    }

    public Task<Content?> UpdateContent(Guid id, ContentDto content)
    {

        var result = _database.Update(id, content);
        _logger.LogInformation($"Sucessfully updated content with ID: {result.Result.Id}");
        return result;
    }

    public Task<Guid> DeleteContent(Guid id)
    {
        var result = _database.Delete(id);
        _logger.LogInformation($"Sucessfully deleted {result} with ID: {result.Result}");
        return result;
    }
}