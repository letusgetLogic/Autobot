[System.Serializable]
public class SavedRoundData
{
    public PlayerData SavedPlayerData1 { get; set; }
    public PlayerData SavedPlayerData2 { get; set; }
    public int RandomSeed { get; set; }

    /// <summary>
    /// Constructor of SavedRoundData.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    /// <param name="_seed"></param>
    public SavedRoundData(PlayerData _player1, PlayerData _player2, int _seed)
    {
        SavedPlayerData1 = new PlayerData(_player1);
        SavedPlayerData2 = new PlayerData(_player2);
        RandomSeed = _seed;
    }
}
