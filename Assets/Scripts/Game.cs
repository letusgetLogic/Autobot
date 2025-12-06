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
    /// <param name="_mode"></param>
    /// <param name="_playerAmount"></param>
    /// <param name="_timer"></param>
    /// <param name="_lives"></param>
    /// <param name="_winCondition"></param>
    /// <param name="_state"></param>
    /// <param name="_playerData1"></param>
    /// <param name="_playerData2"></param>
    public Game(GameMode _mode, int _playerAmount, float _timer, int _lives, int _winCondition, 
        GameState _state, PlayerData _playerData1, PlayerData _playerData2)
    {
        Mode = _mode;
        PlayerAmount = _playerAmount;
        Timer = _timer;
        Lives = _lives;
        WinCondition = _winCondition;
        State = _state;
        PlayerData1 = _playerData1;
        PlayerData2 = _playerData2;
        CurrentPlayerIndex = 0;
    }
}
