public class UnitModel
{
    public UnitController Controller { get; private set; }
    public SoUnit Data { get; private set; }
    public Level CurrentLevel { get; set; }
    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }
    public int XP 
    { 
        get;
        private set;
    }
    public void SetXP(int value) 
    {
        if (value > StarterPack.Instance.XpToLv3)
            XP = StarterPack.Instance.XpToLv3;
        else XP = value;
    }
    public UnitState ManageState { get; set; }
    public UnitModel(UnitController controller, SoUnit _data)
    {
        Controller = controller;
        Data = _data;
        CurrentLevel = Data.Levels[0];
        BattleHealth = Data.Health;
        BattleAttack = Data.Attack;
        XP = 1;
        ManageState = UnitState.InSlotShop;
    }
    public AbilityBase Ability => AbilityBase.GetAbility(Controller, CurrentLevel);
    public bool IsMaxed => CurrentLevel.Number == Data.Levels.Length;
    public bool IsFaint => BattleHealth <= 0;
    public bool IsTeam1 { get; set;} = true;

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

