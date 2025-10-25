public class Summon : AbilityBase
{
    private SoUnit[] summonedUnits;
    private int slotIndex;
    public Summon(UnitController controller, AbilityDuration duration, Level currentLevel, int slotIndex) :
        base(controller, duration, currentLevel)
    {
        summonedUnits = CurrentLevel.SummonUnits;
        this.slotIndex = slotIndex;
    }

    public override void Activate()
    {
        if (summonedUnits == null || summonedUnits.Length == 0)
            return;

        foreach (var unit in summonedUnits)
        {
            if (GameManager.Instance.IsPhaseBattle)
                PhaseBattleController.Instance.SummonUnits.Enqueue(this);
        }

    }

    public void SpawnSummonedUnits()
    {

        Slot[] slots;

        if (GameManager.Instance.IsPhaseBattle)
        {
            slots = Controller.Model.IsTeam1 ?
                PhaseBattleController.Instance.Slots1 :
                PhaseBattleController.Instance.Slots2;
        }
        else
        {
            slots = PhaseShopUnitManager.Instance.BattleSlots;
        }
        for (int i = 0; i < summonedUnits.Length; i++)
        {

        }
    }

}

