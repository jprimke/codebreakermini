﻿using CodeBreaker.Data.Configurations;
using CodeBreaker.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeBreaker.Data;

public class CodeBreakerContext : DbContext, ICodeBreakerRepository
{
    private readonly ILogger _logger;

    public CodeBreakerContext(DbContextOptions<CodeBreakerContext> options, ILogger<CodeBreakerContext> logger) : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("GameContainer");
        modelBuilder.ApplyConfiguration(new GameDbConfiguration());
    }

    public DbSet<Game> Games => Set<Game>();

    public async Task CreateGameAsync(Game game) {
        Games.Add(game);
        await SaveChangesAsync();
        _logger.LogInformation($"Created game with id {game.GameId}");
    }

    public Task<Game?> GetGameAsync(Guid gameId, bool withTracking = true) =>
        Games
            .AsTracking(withTracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTrackingWithIdentityResolution)
            .WithPartitionKey(gameId.ToString())
            .SingleOrDefaultAsync(g => g.GameId == gameId);

    public IAsyncEnumerable<Game> GetGamesByDateAsync(DateOnly date)
    {
        DateTime begin = new (date.Year, date.Month, date.Day);
        DateTime end = begin.AddDays(1);
        return Games
            .AsNoTracking()
            .Where(x => x.Start >= begin && x.Start < end)
            .OrderByDescending(x => x.Start)
            .Take(100)
            .AsAsyncEnumerable();
    }

    public async Task UpdateGameAsync(Game game) {
        Games.Update(game);
        await SaveChangesAsync();
        _logger.LogInformation($"Updated game with id {game.GameId}");
    }
}
