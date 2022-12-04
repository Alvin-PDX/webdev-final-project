using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Connect4_API.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Reflection.Metadata;

namespace Connect4_API.Controllers
{
    [Route("api/Game")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly PlayerContext _playerContext;

        public GamesController(GameContext context, PlayerContext playerContext)
        {
            _context = context;
            _playerContext = playerContext;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGame()
        {
          if (_context.Game == null)
          {
              return NotFound();
          }
            return await _context.Game.ToListAsync();
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(long id)
        {
          if (_context.Game == null)
          {
              return NotFound();
          }
            var game = await _context.Game.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return game;
        }

        // PUT: api/Play/Red/Column/{columnId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Play/Red/Column/{columnId}")]
        public async Task<IActionResult> PutColumnRed(long id, int columnId)
        {

            // Get Current game            
            var dbEntry = await _context.Game.FindAsync(id);

            if(dbEntry == null)
            {
                return NotFound();
            }        

            if(dbEntry.Id != id)
            {
                return NotFound();
            }
            
            if(columnId <= 0 && columnId > 7)
            {
                return Problem("Error. Illegal move by red.");
            }

            if(dbEntry.isGameOver == true)
            {
                return Problem("Error. Game is over. Please create a new round.");
            }

            // CHECK IF RED'S TURN
            if(dbEntry.isPlayer1Turn == true)
            {
                
                // CHECK IF VALID MOVE
                var state = convertStringTo2DArray(dbEntry.State);

                if(isMoveLegal(state, columnId))
                { 
                    // UPDATE PLAYER 1 PROPERTIES
                    dbEntry.isPlayer1Turn = false;
                    dbEntry.setPlayer1Move = columnId;

                    // UPDATE STATE
                    var tempState = UpdateGameState(state, columnId, 1);
                    string newState = convert2DArrayToString(tempState);  
                    dbEntry.State = newState;

                    // CHECK IF WINNER
                    if(isWinner(tempState, 1))
                    {
                        dbEntry.isPlayer1Winner = true;
                        dbEntry.isGameOver = true;
                    }
                    else
                    {
                        // GIVE TURN TO BLACK
                        dbEntry.isPlayer2Turn = true; 
                    }
                    
                }
            }

            _context.Entry(dbEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetGame), new { id = dbEntry.Id }, dbEntry);
        }

        // PUT: api/Play/Black/Column/{columnId}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Play/Black/Column/{columnId}")]
        public async Task<IActionResult> PutColumnBlack(long id, int columnId)
        {

            // Get Current game            
            var dbEntry = await _context.Game.FindAsync(id);

            if(dbEntry == null)
            {
                return NotFound();
            }        

            if(dbEntry.Id != id)
            {
                return NotFound();
            }
            
            if(columnId <= 0 && columnId > 7)
            {
                return Problem("Error. Illegal move by black.");
            }

            if(dbEntry.isGameOver == true)
            {
                return Problem("Error. Game is over. Please create a new round.");
            }

            // Check if black's turn
            if(dbEntry.isPlayer2Turn == true)
            {
                
                // Check if valid move
                var state = convertStringTo2DArray(dbEntry.State);

                if(isMoveLegal(state, columnId))
                { 
                    // UPDATE PLAYER 2 PROPERTIES
                    dbEntry.isPlayer2Turn = false;
                    dbEntry.setPlayer2Move = columnId;

                    // UPDATE STATE
                    var tempState = UpdateGameState(state, columnId, 2);
                    string newState = convert2DArrayToString(tempState);
                    dbEntry.State = newState;

                    // CHECK IF WINNER
                    if(isWinner(tempState, 2))
                    {
                        dbEntry.isPlayer2Winner = true;
                        dbEntry.isGameOver = true;
                    }
                    else
                    {
                        // GIVE TURN TO BLACK
                        dbEntry.isPlayer1Turn = true; 
                    }

                }
            }

            _context.Entry(dbEntry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetGame), new { id = dbEntry.Id }, dbEntry);
        }


        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutGame(long id, Game game)
        {
            if (id != game.Id)
            {
                return BadRequest();
            }

            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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


        // PUT: api/Games/Play/ResetGame/{id}
        [HttpPut("Play/ResetGame/{id}")]
        public async Task<ActionResult<Game>> PutResetGame(long id)
        {
            if (_context.Game == null)
            {
                return Problem("Entity set 'GameContext.Game'  is null.");
            }

            long redId = 1;
            var player1 = await _playerContext.Player.FindAsync(redId);

            long blackId = 2;
            var player2 = await _playerContext.Player.FindAsync(blackId);

            if(player1 == null || player2 == null)
            {
                return Problem("Error. Unable to reset game. Not a full lobby.");
            }

            Game game = new Game();
            game.Id = id;
            game.isStartGame = true;
            game.isPlayer1Turn = true;
            game.isSpectatorOnly= true;

            _context.Entry(game).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
        }


        // POST: api/Games/Play/StartGame
        [HttpPost("Play/StartGame")]
        public async Task<ActionResult<Game>> PostGame()
        {
          if (_context.Game == null)
          {
              return Problem("Entity set 'GameContext.Game'  is null.");
          }

            // DEBUG
            long redId = 1;
            var player1 = await _playerContext.Player.FindAsync(redId);

            long blackId = 2;
            var player2 = await _playerContext.Player.FindAsync(blackId);

            // Check if player 1 is in the room. if not, then create player 1
            if(player1 == null)
            {
                var redPlayer = new Player();
                redPlayer.Id = redId;
                redPlayer.PlayerColor = "red";

                _playerContext.Player.Add(redPlayer);
                await _playerContext.SaveChangesAsync();

                Game game = new Game();

                long roomId = 1;
                game.Id = roomId;
                game.isPlayer1Turn = true;
                // Switch the isStartGame to true if player 1 and 2 are in the lobby
                if(player1 != null && player2 != null)
                {
                    game.isStartGame = true;     
                }

                convertStringTo2DArray(game.State);
                
                _context.Game.Add(game);
                await _context.SaveChangesAsync();

                game.playerType = "red";
                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);

            } 

            // Check if player 1 is in the room. if yes, then create player 2
            else if(player2 == null)
            {
                var blackPlayer = new Player();
                blackPlayer.Id = blackId;
                blackPlayer.PlayerColor = "black";

                _playerContext.Player.Add(blackPlayer);
                await _playerContext.SaveChangesAsync();

                long roomId = 1; 
                var game = await _context.Game.FindAsync(roomId);
                game.isStartGame = true;     
                game.isSpectatorOnly = true;
                await _context.SaveChangesAsync();
                game.playerType = "black";

                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
            }

            // Create isSpectator
            else if(player1 != null && player2 != null)
            {
                long id = 0;
                var spectatorPlayer = new Player();
                spectatorPlayer.Id = id;

                spectatorPlayer.PlayerColor = "spectator";
                _playerContext.Player.Add(spectatorPlayer);
                await _playerContext.SaveChangesAsync();

                long roomId = 1; 
                var game = await _context.Game.FindAsync(roomId);

                game.isSpectatorOnly = true;
                await _context.SaveChangesAsync();

                game.playerType = "spectator";

                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
            }

            long Id = 1;
            var room = await _context.Game.FindAsync(Id);

            if(room == null)
            {
                return NotFound();
            }

            return room;
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(Game game)
        {
          if (_context.Game == null)
          {
              return Problem("Entity set 'GameContext.Game'  is null.");
          }
            _context.Game.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGame", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(long id)
        {
            if (_context.Game == null || _playerContext == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }


            foreach(var player in _playerContext.Player)
            {
                _playerContext.Player.Remove(player);
            }

            await _playerContext.SaveChangesAsync();

            _context.Game.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GameExists(long id)
        {
            return (_context.Game?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private int[][] convertStringTo2DArray(string state)
        {
            var tempArray = state.Split(new string[] { "[[", "],[", "]]" }, StringSplitOptions.RemoveEmptyEntries);

            var resultArray = tempArray.Select(x => x.Split(',').ToArray()).ToArray();

            int[][] result = new int[6][];

            for(int i = 0; i < 6; ++i)
            {
                result[i] = new int[7];
                for(int j = 0; j < 7; ++j)
                {
                    result[i][j] = Int32.Parse(resultArray[i][j]);
                    //Console.Write(result[i][j] + "\t");
                }
                //Console.WriteLine();
            }

            return result;
        }
          
        private string convert2DArrayToString(int[][] state)
        {
            string result = "";
            result += "[";
            for(int i = 0; i < 6; ++i)
            {

                result += "[";
                for(int j = 0; j < 7; ++j)
                {
                    result += state[i][j].ToString();

                    if(j != 6)
                    {
                        result += ",";
                    }
                }
                //Console.WriteLine();
                
                if(i == 5)
                {
                    result += "]";
                }
                else
                {
                    result += "],";
                }
            }
            result += "]";

            Console.WriteLine(result);
            return result; 
        }

        private bool isMoveLegal(int[][] state, int move)
        {
            // Check if placed outside of columns range  
            if(move < 0 || move > 7)
            {
                return false;
            }

            // Check if placed in full column
            if (state[0][move] != 0)
            {
               return false;
            }

           return true;
        }
        

        private int[][] UpdateGameState(int[][] state, int move, int player)
        {
            for(int row = 5; 0 < row; --row)
            {
                if (state[row][move] == 0) 
                { 
                    state[row][move] = player;
                    break;
                }
            }

            // DEBUG
            for(int i = 0; i < 6; ++i)
            {
                for(int j = 0; j < 7; ++j)
                {
                    Console.Write(state[i][j] + "\t");
                }
                Console.WriteLine();
            }


            return state;
        }

        private bool isWinner(int[][] state, int player)
        {
            int count = 0;

            // CHECK HORIZONTAL
            for(int row = 0; row < 6; ++row)
            {
                count = 0;
                for(int col = 0; col < 7; ++col)
                {
                    if (state[row][col] == player)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    if(count >= 4)
                    {
                        return true;
                    }
                }
            }

            // CHECK VERTICAL
            for(int col = 0; col < 7; ++col)
            {
                count = 0;
                for(int row = 0; row < 6; ++row)
                {
                    if (state[row][col] == player)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }

                    if(count >= 4) { return true; }
                }
            }

            count = 0;
            // Top Left -> Bottom Right Diagonals START [5,0]
            for(int row = 5, col = 0; row >= 0 && col < 7; --row, ++col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else{
                    count = 0;
                }
            }
    
            count = 0;
            // Top Left -> Bottom Right Diagonals START [4,0]
            for(int row = 4, col = 0; row >= 0 && col < 7; --row, ++col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else {
                    count = 0;
                }
            }

            count = 0;
            // Top Left -> Bottom Right Diagonals START [3,0]
            for(int row = 3, col = 0; row >= 0 && col < 7; --row, ++col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else
                {
                    count = 0;
                }
            }

            count = 0;
            // Top Left -> Bottom Right Diagonals START [5,1]
            for(int row = 5, col = 1; row >= 0 && col < 7; --row, ++col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else
                {
                    count = 0;
                }
            }

            count = 0;
            // Top Left -> Bottom Right Diagonals START [5,2]
            for(int row = 5, col = 2; row >= 0 && col < 7; --row, ++col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else
                {
                    count = 0;
                }
            }

            count = 0;
            // Top Left -> Bottom Right Diagonals START [5,3]
            for(int row = 5, col = 3; row >= 0 && col < 7; --row, ++col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else {
                    count = 0;
                }
            }

            count = 0;
            // Top Right -> Bottom Left Diagonals START [5,3]
            for(int row = 5, col = 3; row >= 0 && col >= 0; --row, --col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else {
                    count = 0;
                }
            }

            count = 0;
            // Top Right -> Bottom Left Diagonals START [5,4]
            for(int row = 5, col = 4; row >= 0 && col >= 0; --row, --col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else {
                    count = 0;
                }
            }

            count = 0;
            // Top Right -> Bottom Left Diagonals START [5,5]
            for(int row = 5, col = 5; row >= 0 && col >= 0; --row, --col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else {
                    count = 0;
                }
            }

            count = 0;
            // Top Right -> Bottom Left Diagonals START [5,6]
            for(int row = 5, col = 6; row >= 0 && col >= 0; --row, --col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else {
                    count = 0;
                }
            }

            count = 0;
            // Top Right -> Bottom Left Diagonals START [4,6]
            for(int row = 4, col = 6; row >= 0 && col >= 0; --row, --col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else
                {
                    count = 0;
                }
            }

            count = 0;
            // Top Right -> Bottom Left Diagonals START [3,6]
            for(int row = 3, col = 6; row >= 0 && col >= 0; --row, --col)
            {
                if (state[row][col] == player)
                {
                    count++;
                    if(count >= 4)
                    {
                        return true;
                    }
                } 
                else
                {
                    count = 0;
                }
            }

            return false;
        }
    }
}
