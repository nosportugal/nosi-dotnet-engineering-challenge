using Microsoft.EntityFrameworkCore;
using NOS.Engineering.Challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOS.Engineering.Challenge.Database
{
    public class DatabaseService : IDatabase<Content, ContentDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper<Content, ContentDto> _mapper;

        public DatabaseService(ApplicationDbContext context, IMapper<Content, ContentDto> mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Content?> Create(ContentDto item)
        {
            var id = Guid.NewGuid();
            var content = _mapper.Map(id, item);
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

        public async Task<Content?> Update(Guid id, ContentDto item)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null) return null;

            content = _mapper.Patch(content, item);
            _context.Contents.Update(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<Guid> Delete(Guid id)
        {
            var content = await _context.Contents.FindAsync(id);
            if (content == null) return Guid.Empty;

            _context.Contents.Remove(content);
            await _context.SaveChangesAsync();
            return id;
        }
    }

}
