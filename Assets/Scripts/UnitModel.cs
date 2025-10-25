using System;

[System.Serializable]
public class UnitModel
{
    public int Index { get; private set; }
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
    public UnitState UnitState { get; set; }
    public UnitModel(SoUnit _data, int index, UnitState unitState)
    {
        Index = index;
        UnitState = unitState;
        BasisHealth = _data.Health;
        BasisAttack = _data.Attack;
        XP = 1;
        UnitState = unitState;

        BuffHealthTemp = 0;
        BuffAttackTemp = 0;
    }
    public int BuffHealth { get; set; }
    public int BuffAttack { get; set; }
    public int BuffHealthTemp { get; set; }
    public int BuffAttackTemp { get; set; }
    public bool IsTeam1 { get; set;} = true;

}

