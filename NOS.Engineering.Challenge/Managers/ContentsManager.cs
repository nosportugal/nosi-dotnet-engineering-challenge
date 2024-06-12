using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Managers
{
    public class ContentsManager : IContentsManager
    {
        private readonly IDatabase<Content?, ContentDto> _database;
        private readonly ILogger<ContentsManager> _logger;

        public ContentsManager(IDatabase<Content?, ContentDto> database, ILogger<ContentsManager> logger)
        {
            _database = database;
            _logger = logger;
        }

        public Task<IEnumerable<Content?>> GetManyContents() =>
            LogExecution(() => _database.ReadAll(), "retrieving multiple contents");

        public Task<Content?> CreateContent(ContentDto content) =>
            LogExecution(() => _database.Create(content), "creating new content");

        public Task<Content?> GetContent(Guid id) =>
            LogExecution(() => _database.Read(id), $"retrieving content with ID: {id}");

        public Task<Content?> UpdateContent(Guid id, ContentDto content) =>
            LogExecution(() => _database.Update(id, content), $"updating content with ID: {id}");

        public Task<Guid> DeleteContent(Guid id) =>
            LogExecution(() => _database.Delete(id), $"deleting content with ID: {id}");

        public async Task<Content?> AddGenre(Guid id, IEnumerable<string> genres)
        {
            var content = await GetContentWithLogging(id);
            if (content == null) return null;

            var newGenres = genres.Except(content.GenreList).ToList();
            if (!newGenres.Any())
            {
                _logger.LogWarning($"No new genres found to add to content with ID: {id}.");
                return null;
            }

            var updatedContentDto = new ContentDto(content.Title, content.SubTitle, content.Description,
                                                    content.ImageUrl, content.Duration, content.StartTime,
                                                    content.EndTime, content.GenreList.Concat(newGenres));

            return await _database.Update(id, updatedContentDto);
        }

        public async Task<Content?> RemoveGenre(Guid id, IEnumerable<string> genres)
        {
            var content = await GetContentWithLogging(id);
            if (content == null) return null;

            var genresToRemove = genres.Intersect(content.GenreList).ToList();
            if (!genresToRemove.Any())
            {
                _logger.LogWarning($"No genres found to remove from content with ID: {id}.");
                return null;
            }

            var updatedContentDto = new ContentDto(content.Title, content.SubTitle, content.Description,
                                                    content.ImageUrl, content.Duration, content.StartTime,
                                                    content.EndTime, content.GenreList.Except(genresToRemove));

            return await _database.Update(id, updatedContentDto);
        }

        private Task<Content?> GetContentWithLogging(Guid id) =>
            LogExecution(() => _database.Read(id), $"retrieving content with ID: {id}");

        private async Task<T> LogExecution<T>(Func<Task<T>> func, string action)
        {
            _logger.LogInformation($"Starting {action}...");
            try
            {
                var result = await func();
                _logger.LogInformation($"Successfully completed {action}.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while {action}.");
                throw;
            }
        }
    }
}
