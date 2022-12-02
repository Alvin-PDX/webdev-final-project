using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Connect4_API.Models;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Connect4_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbiesController : ControllerBase
    {
        private readonly LobbyContext _context;

        public LobbiesController(LobbyContext context)
        {
            _context = context;
        }

        // GET: api/Lobbies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lobby>>> GetLobby()
        {
          if (_context.Lobby == null)
          {
              return NotFound();
          }

     
          return await _context.Lobby.ToListAsync();
        }



        // GET: api/Lobbies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lobby>> GetLobby(int id)
        {
          if (_context.Lobby == null)
          {
              return NotFound();
          }
            var lobby = await _context.Lobby.FindAsync(id);

            if (lobby == null)
            {
                return NotFound();
            }

            return lobby;
        }

        // POST: api/Lobbies/PostLobby
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostLobby")]
        public async Task<ActionResult<Lobby>> PostLobby()
        {
            Lobby lobby = new Lobby();
            lobby.Id = 0;
            lobby.IsFull = false;

            _context.Lobby.Add(lobby);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLobby", new { id = lobby.Id }, lobby);
        }

        [HttpPost("PostLobbyID")]
        public async Task<ActionResult<Lobby>> PostLobbyById([FromQuery] string roomId)
        {

            Lobby lobby = new Lobby();
            lobby.Id = 0;
            lobby.IsFull = false;

            _context.Lobby.Add(lobby);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLobby", new { id = lobby.Id }, lobby);
        }


        // DELETE: api/Lobbies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLobby(int id)
        {
            if (_context.Lobby == null)
            {
                return NotFound();
            }
            var lobby = await _context.Lobby.FindAsync(id);
            if (lobby == null)
            {
                return NotFound();
            }

            _context.Lobby.Remove(lobby);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LobbyExists(int id)
        {
            return (_context.Lobby?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        



    }

    
}
