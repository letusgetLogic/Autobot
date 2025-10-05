public class UnitModel
{
    public SoUnit Data { get; set; }

    public Level CurrentLevel { get; set; }
    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }
    public int XP { get; set; }

    /// <summary>
    /// Initializes the level number.
    /// </summary>
    public void InitializeLevel()
    {
        for (int i = 0; i < Data.Levels.Length; i++)
        {
            Data.Levels[i].Number = i + 1;
        }
    }
}

