using UnityEngine.Rendering;

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
    public bool IsMaxed => CurrentLevel.Number == Data.Levels.Length;
    public bool IsFaint => BattleHealth <= 0;
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

