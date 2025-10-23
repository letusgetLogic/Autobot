using System;

[System.Serializable]
public class UnitModel
{
    public int Index { get; private set; }
    public int CurrentLevelIndex { get; set; }  
    public int BasisHealth { get; set; }
    public int BasisAttack { get; set; }
    private int xp;
    public int XP 
    { 
        get { return xp; }
        set
        {
            if (value > StarterPack.Instance.XpToLv3)
                xp = StarterPack.Instance.XpToLv3;
            else xp = value;
        }
    }
    public UnitState ManageState { get; set; }
    public UnitModel(SoUnit _data, int index)
    {
        Index = index;
        CurrentLevelIndex = 0;
        BasisHealth = _data.Health;
        BasisAttack = _data.Attack;
        XP = 1;
        ManageState = UnitState.InSlotShop;
    }
    public int BuffHealth { get; set; }
    public int BuffAttack { get; set; }
    public int BuffHealthTemp { get; set; }
    public int BuffAttackTemp { get; set; }
    public bool IsFaint => BasisHealth <= 0;
    public bool IsTeam1 { get; set;} = true;

}

