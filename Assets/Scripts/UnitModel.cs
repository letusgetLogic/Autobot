[System.Serializable]
public class UnitModel
{
    public UnitView View { get; set; }
    public int Index { get; private set; }
    public int BasisHealth { get; set; }
    public int BasisAttack { get; set; }
    public int HealthState => healthState;
    private int healthState;
    public int Energy { get; set; }
    public int XP
    {
        get { return xp; }
        set
        {
            if (value > PackManager.Instance.MyPack.XpToLv3.Value)
                xp = PackManager.Instance.MyPack.XpToLv3.Value;
            else xp = value;
        }
    }
    private int xp;
    public UnitState UnitState => unitState;
    private UnitState unitState;
    public UnitModel(SoUnit _data, int index)
    {
        Index = index;
        BasisHealth = _data.Health;
        BasisAttack = _data.Attack;
        Energy = _data.Energy;
        XP = 1;

        BuffHealthTemp = 0;
        BuffAttackTemp = 0;
    }
    public int BuffHealth { get; set; }
    public int BuffAttack { get; set; }
    public int BuffHealthTemp { get; set; }
    public int BuffAttackTemp { get; set; }
    public bool IsTeam1 { get; set; } = true;

    public void SetData(UnitView _view)
    {
        View = _view;
    }

    public void SetData(UnitState _unitState)
    {
        if (View != null)
        {
            switch (_unitState)
            {
                case UnitState.InSlotShop:
                    View.IceCube.SetActive(false);
                    View.SetRepairDisplayActive(false);
                    break;
                case UnitState.Freezed:
                    View.IceCube.SetActive(true);
                    View.SetRepairDisplayActive(false);
                    break;
                case UnitState.InSlotBattle:
                    View.SetRepairDisplayActive(true);
                    break;
                case UnitState.InSlotCharge:
                    View.SetRepairDisplayActive(true);
                    break;
                case UnitState.InPhaseBattle:
                    View.SetRepairDisplayActive(false);
                    break;
            }
        }

        unitState = _unitState;
    }

    public void SetData(int _healthState)
    {
        if (View != null)
        {
            switch (_healthState)
            {
                case 0:
                    View.SetRepairStepActive(false, false, false);
                    break;
                case 1:
                    View.SetRepairStepActive(true, false, false);
                    break;
                case 2:
                    View.SetRepairStepActive(true, true, false);
                    break;
                case 3:
                    View.SetRepairStepActive(true, true, true);
                    break;
            }
        }

        if (_healthState > PackManager.Instance.MyPack.HealthPortion.Value)
            healthState = PackManager.Instance.MyPack.HealthPortion.Value;
        else healthState = _healthState;
    }

    public void UpdateHealthState()
    {

    }
}

