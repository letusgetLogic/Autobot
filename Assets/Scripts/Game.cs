[System.Serializable]
public class Game
{
    public GameMode Mode { get; set; }
    public int PlayerAmount { get; set; }
    public float Timer { get; set; }
    public int Lives { get; set; }
    public int WinCondition { get; set; }
    public GameState State { get; set; }
    public PlayerData PlayerData1 { get; set; }
    public PlayerData PlayerData2 { get; set; }
    public int CurrentPlayerIndex { get; set; }

    /// <summary>
    /// Constructor of Game. Hold the data of game for saving and loading.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="playerAmount"></param>
    /// <param name="timer"></param>
    /// <param name="lives"></param>
    /// <param name="winCondition"></param>
    /// <param name="state"></param>
    /// <param name="playerData1"></param>
    /// <param name="playerData2"></param>
    public Game(GameMode mode, int playerAmount, float timer, int lives, int winCondition, 
        GameState state, PlayerData playerData1, PlayerData playerData2)
    {
        Mode = mode;
        PlayerAmount = playerAmount;
        Timer = timer;
        Lives = lives;
        WinCondition = winCondition;
        State = state;
        PlayerData1 = playerData1;
        PlayerData2 = playerData2;
        CurrentPlayerIndex = 0;
    }
}
