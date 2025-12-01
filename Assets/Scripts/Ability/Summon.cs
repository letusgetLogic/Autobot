using System.Collections;
using UnityEngine;
public class Summon : AbilityBase
{
    private UnitModel model;
    private SoUnit[] summonedUnits;
    private int slotIndex;
    public Summon(UnitController controller, UnitModel model, Level currentLevel, int slotIndex) :
        base(controller, currentLevel)
    {
        this.model = model;
        summonedUnits = CurrentLevel.SummonUnits;
        this.slotIndex = slotIndex;
    }

    public override void Run()
    {
        SpawnManager.Instance.StartCoroutine(SpawnUnit());
    }
    public IEnumerator SpawnUnit()
    {
        yield return new WaitForSeconds(PhaseBattleController.Instance.Process.DurationDelaySummon);

        Controller.DeactivateInteraction();
        Controller.transform.parent = null;

        Slot[] slots;

        if (GameManager.Instance.IsPhaseBattle)
        {
            slots = model.Data.IsTeamLeft ?
                PhaseBattleController.Instance.Slots1 :
                PhaseBattleController.Instance.Slots2;
        }
        else
        {
            slots = PhaseShopUnitManager.Instance.TeamSlots;
        }
        for (int i = 0; i < summonedUnits.Length; i++)
        {
            if (slots[slotIndex].Unit() == null)
            {
                Debug.Log($"-Summon SpawnSummonedUnit at slot {slotIndex}");
                var unitController = SpawnManager.Instance.Spawn(
                    summonedUnits[i],
                    -1,
                    new(),
                    model.Data.UnitState,
                    slots[slotIndex].transform,
                    model.Data.IsTeamLeft);
            }
        }

        slotIndex = -1;
    }

}

