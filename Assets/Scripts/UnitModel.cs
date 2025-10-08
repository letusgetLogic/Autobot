public class UnitModel
{
    public UnitModel(SoUnit _data)
    {
        Data = _data;
        CurrentLevel = Data.Levels[0];
        BattleHealth = Data.Health;
        BattleAttack = Data.Attack;
        XP = 1;
        ManageState = UnitState.InSlotShop;
    }

    public SoUnit Data { get; set; }

    public Level CurrentLevel { get; set; }
    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }
    public int XP { get; set; }
    public UnitState ManageState { get; set; }

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

