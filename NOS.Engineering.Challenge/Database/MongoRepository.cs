using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database
{
    public class MongoRepository : IDatabase<Content?, ContentDto>
    {
        private readonly IMongoCollection<Content> _collection;
        private readonly ILogger<MongoRepository> _logger;

        public MongoRepository(MongoDbContext dbContext, ILogger<MongoRepository> logger)
        {
            _collection = dbContext.Contents;
            _logger = logger;
        }

        public async Task<IEnumerable<Content?>> ReadAll() =>
            await LogExecution(async () =>
            {
                var documents = await _collection.Find(_ => true).ToListAsync();
                return documents.Select(content => (Content?)content);
            }, "retrieving multiple contents");

        public async Task<Content?> Create(ContentDto content) =>
            await LogExecution(async () =>
            {
                var newContent = new Content(
                                                Guid.NewGuid(),
                                                content.Title,
                                                content.SubTitle,
                                                content.Description,
                                                content.ImageUrl,
                                                content.Duration ?? 0,
                                                content.StartTime ?? DateTime.UtcNow,
                                                content.EndTime ?? DateTime.UtcNow,
                                                content.GenreList
                                            );
                await _collection.InsertOneAsync(newContent);
                return newContent;
            }, "creating new content");

        public async Task<Content?> Read(Guid id) =>
            await LogExecution(async () =>
            {
                var content = await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
                return content;
            }, $"retrieving content with ID: {id}");

        public async Task<Content?> Update(Guid id, ContentDto content) =>
            await LogExecution(async () =>
            {
                var updatedContent = await _collection.FindOneAndUpdateAsync(
                    Builders<Content>.Filter.Eq(c => c.Id, id),
                    Builders<Content>.Update
                        .Set(c => c.Title, content.Title)
                        .Set(c => c.SubTitle, content.SubTitle)
                        .Set(c => c.Description, content.Description)
                        .Set(c => c.ImageUrl, content.ImageUrl)
                        .Set(c => c.Duration, content.Duration)
                        .Set(c => c.StartTime, content.StartTime)
                        .Set(c => c.EndTime, content.EndTime)
                        .Set(c => c.GenreList, content.GenreList.ToList()),
                    //.Set(c => c.LastUpdated, DateTime.UtcNow),
                    new FindOneAndUpdateOptions<Content> { ReturnDocument = ReturnDocument.After }
                );
                return updatedContent;
            }, $"updating content with ID: {id}");

        public async Task<Guid> Delete(Guid id) =>
            await LogExecution(async () =>
            {
                var result = await _collection.DeleteOneAsync(c => c.Id == id);
                return result.DeletedCount > 0 ? id : Guid.Empty;
            }, $"deleting content with ID: {id}");

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
