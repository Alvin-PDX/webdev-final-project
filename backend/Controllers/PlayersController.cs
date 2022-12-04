using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Connect4_API.Models;

namespace Connect4_API.Controllers
{
    [Route("api/Player")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly PlayerContext _context;

        public PlayersController(PlayerContext context)
        {
            _context = context;
        }

        // GET: api/Player
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
          if (_context.Player == null)
          {
              return NotFound();
          }
                 
            return await _context.Player.ToListAsync();
        }

        // GET: api/Player/Black
        [HttpGet("Black")]
        public async Task<ActionResult<Player>> GetPlayerBlack()
        {
          if (_context.Player == null)
          {
              return NotFound();
          }

            long id = 2;
            var player = await _context.Player.FindAsync(id);

            if(player == null)
            {
                return NotFound();
            }

            return player;
        }

        // GET: api/Player/Red
        [HttpGet("Red")]
        public async Task<ActionResult<Player>> GetPlayerRed()
        {
          if (_context.Player == null)
          {
              return NotFound();
          }
              
            long id = 1;
            var player = await _context.Player.FindAsync(id);

            if(player == null)
            {
                return NotFound();
            }

            return player;
        }

        // GET: api/Player/Spectator
        [HttpGet("Spectator/{id}")]
        public async Task<ActionResult<Player>> GetPlayerSpectator(long id)
        {
          if (_context.Player == null)
          {
              return NotFound();
          }
              
            var player = await _context.Player.FindAsync(id);

            if(player == null)
            {
                return NotFound();
            }

            return player;
        }

        // PUT: api/Player/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(long id, Player player)
        {
            if (id != player.Id)
            {
                return BadRequest();
            }

            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Player/Post/Red
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Post/Red")]
        public async Task<ActionResult<Player>> PostPlayerRed(Player player)
        {
          if (_context.Player == null)
          {
              return Problem("Entity set 'PlayerContext.Player' is null.");
          }
            player.Id = 1;
            player.PlayerColor = "red";

            _context.Player.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayerRed), new { id = player.Id }, player);
        }

        // POST: api/Player/Post/Black
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Post/Black")]
        public async Task<ActionResult<Player>> PostPlayerBlack(Player player)
        {
          if (_context.Player == null)
          {
              return Problem("Entity set 'PlayerContext.Player' is null.");
          }
            player.Id = 2;
            player.PlayerColor = "black";

            _context.Player.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayerBlack), new { id = player.Id }, player);
        }

        // POST: api/Player/Post/Spectator
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Post/Spectator")]
        public async Task<ActionResult<Player>> PostPlayerSpectator(Player player)
        {
          if (_context.Player == null)
          {
              return Problem("Entity set 'PlayerContext.Player' is null.");
          }

            player.Id = 3;

            while( (_context.Player?.Any(e => e.Id == player.Id)).GetValueOrDefault() )
            {
                player.Id += 1;
            }

            player.PlayerColor = "spectator";

            _context.Player.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayerSpectator), new { id = player.Id }, player);
        }


        // DELETE: api/Player/Red
        [HttpDelete("Delete/Red")]
        public async Task<IActionResult> DeletePlayerRed()
        {
            long id = 1;

            if (_context.Player == null)
            {
                return NotFound();
            }
            var player = await _context.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Player.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Player/Black
        [HttpDelete("Delete/Black")]
        public async Task<IActionResult> DeletePlayerBlack()
        {
            long id = 2;

            if (_context.Player == null)
            {
                return NotFound();
            }
            var player = await _context.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Player.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Player/Spectator/id
        [HttpDelete("Delete/Spectator/{id}")]
        public async Task<IActionResult> DeletePlayerSpectator(long id)
        {
            if(id == 1 || id == 2)
            {
                return Problem("Error. Deleting Invalid Spectator.");
            }

            if (_context.Player == null)
            {
                return NotFound();
            }

            var player = await _context.Player.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Player.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerExists(long id)
        {
            return (_context.Player?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
