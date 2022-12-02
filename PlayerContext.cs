using Microsoft.EntityFrameworkCore;
using Connect4_API.Models;
namespace Connect4_API.Models;

public class PlayerContext : DbContext
{ 
    public PlayerContext(DbContextOptions<PlayerContext> options) : base(options) { }

    public DbSet<Player> Player { get; set; } = default!;
}

