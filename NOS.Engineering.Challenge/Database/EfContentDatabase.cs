using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Data
{
    public class EfContentDatabase : IDatabase<Content?, ContentDto>
    {
        private readonly ContentDbContext _context;

        public EfContentDatabase(ContentDbContext context)
        {
            _context = context;
        }

        public async Task<Content?> Create(ContentDto contentDto)
        {
            var content = new Content(
                Guid.NewGuid(),
                contentDto.Title ?? throw new ArgumentNullException(nameof(contentDto.Title)),
                contentDto.SubTitle ?? throw new ArgumentNullException(nameof(contentDto.SubTitle)),
                contentDto.Description ?? throw new ArgumentNullException(nameof(contentDto.Description)),
                contentDto.ImageUrl ?? throw new ArgumentNullException(nameof(contentDto.ImageUrl)),
                contentDto.Duration ?? throw new ArgumentNullException(nameof(contentDto.Duration)),
                contentDto.StartTime ?? throw new ArgumentNullException(nameof(contentDto.StartTime)),
                contentDto.EndTime ?? throw new ArgumentNullException(nameof(contentDto.EndTime)),
                contentDto.GenreList ?? throw new ArgumentNullException(nameof(contentDto.GenreList))
            );

            _context.Contents.Add(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<Content?> Read(Guid id)
        {
            return await _context.Contents.FindAsync(id);
        }

        public async Task<IEnumerable<Content?>> ReadAll()
        {
            return await _context.Contents.ToListAsync();
        }

        public async Task<Content?> Update(Guid id, ContentDto contentDto)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null)
            {
                return null;
            }

            var updatedContent = new Content(
                id,
                contentDto.Title ?? content.Title,
                contentDto.SubTitle ?? content.SubTitle,
                contentDto.Description ?? content.Description,
                contentDto.ImageUrl ?? content.ImageUrl,
                contentDto.Duration ?? content.Duration,
                contentDto.StartTime ?? content.StartTime,
                contentDto.EndTime ?? content.EndTime,
                contentDto.GenreList ?? content.GenreList
            );

            _context.Entry(content).CurrentValues.SetValues(updatedContent);
            await _context.SaveChangesAsync();
            return updatedContent;
        }

        public async Task<Guid> Delete(Guid id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content != null)
            {
                _context.Contents.Remove(content);
                await _context.SaveChangesAsync();
            }
            return id;
        }
    }
}
