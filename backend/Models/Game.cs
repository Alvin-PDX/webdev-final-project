namespace Connect4_API.Models;

public class Game
{
    public long Id { get; set; } = 0;

    public bool isStartGame { get; set; } = false;

    public bool isGameOver { get; set; } = false;

    public string playerType { get; set; } = "";

    public bool isSpectatorOnly { get; set; } = false;
 
    public bool isInvalidMove { get; set; } = false;

    public bool isPlayer1Turn { get; set; } = false;

    public bool isPlayer1Winner { get; set; } = false;

    public int setPlayer1Move { get; set; } = 0;

    public bool isPlayer2Turn { get; set; } = false;

    public bool isPlayer2Winner { get; set; } = false;

    public int setPlayer2Move { get; set; } = 0;

    public string? State { get; set; } = "[[0,0,0,0,0,0,0]," +
                                          "[0,0,0,0,0,0,0]," +
                                          "[0,0,0,0,0,0,0]," +
                                          "[0,0,0,0,0,0,0]," +
                                          "[0,0,0,0,0,0,0]," +
                                          "[0,0,0,0,0,0,0]]";
    
}
