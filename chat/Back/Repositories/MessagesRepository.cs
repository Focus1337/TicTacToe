using Back.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Back.Repositories;

public class MessagesRepository
{
    private readonly AppDbContext _dbContext;

    public MessagesRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<Message>> GetLast(int count) =>
        await _dbContext.Messages
            .OrderBy(m => m.DateTime)
            .Skip(Math.Max(_dbContext.Messages.Count() - count, 0))
            .Take(count)
            .ToListAsync();

    public async Task<bool> AddMessage(Message message)
    {
        _dbContext.Messages.Add(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveAll()
    {
        _dbContext.Messages.RemoveRange(_dbContext.Messages);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}