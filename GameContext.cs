using Microsoft.EntityFrameworkCore;

namespace Connect4_API.Models;

public class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options) { }

    public DbSet<Game>Game { get; set; } = null!;
}
