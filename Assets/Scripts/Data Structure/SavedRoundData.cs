public partial class Game
{
    [System.Serializable]
    public class SavedRoundData
    {
        public PlayerData PlayerData1 { get; set; }
        public PlayerData PlayerData2 { get; set; }
        public int RandomSeed { get; set; }
    }
}
