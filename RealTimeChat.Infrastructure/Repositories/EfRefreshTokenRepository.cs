using Microsoft.EntityFrameworkCore;
using RealTimeChat.Domain.Entities;
using RealTimeChat.Domain.Repositories;
using RealTimeChat.Infrastructure.Context;
using System;

public class EfRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly RealTimeChatDbContext _context;

    public EfRefreshTokenRepository(RealTimeChatDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(x => x.Token == token && !x.IsRevoked);
    }

    public async Task RevokeAsync(string token)
    {
        var rt = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
        if (rt != null)
        {
            rt.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
