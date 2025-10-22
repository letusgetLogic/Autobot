using System.Collections.Generic;

[System.Serializable]
public class Game
{
    public GameMode Mode { get; set; }
    public int PlayerAmount { get; set; }
    public float Timer { get; set; }
    public int Lives { get; set; }
    public int WinCondition { get; set; }
    public GameState State { get; set; }
    public PlayerData Player1 { get; set; }
    public PlayerData Player2 { get; set; }
    public int CurrentPlayerIndex { get; set; }

    public Game(GameMode mode, int playerAmount, float timer, int lives, int winCondition, 
        GameState state, PlayerData player1, PlayerData player2)
    {
        Mode = mode;
        PlayerAmount = playerAmount;
        Timer = timer;
        Lives = lives;
        WinCondition = winCondition;
        State = state;
        Player1 = player1;
        Player2 = player2;
        CurrentPlayerIndex = 0;
    }
}
