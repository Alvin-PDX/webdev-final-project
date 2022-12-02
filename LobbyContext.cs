using Microsoft.EntityFrameworkCore;
namespace Connect4_API.Models;

public class LobbyContext : DbContext
{ 
    public LobbyContext(DbContextOptions<LobbyContext> options) : base(options) { }

    public DbSet<Lobby>Lobby { get; set; } = null!;
}
