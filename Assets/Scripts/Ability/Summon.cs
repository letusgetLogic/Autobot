using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
public class Summon : AbilityBase
{
    private UnitModel model;
    private SoUnit[] summonedUnits;
    private int slotIndex;
    public Summon(UnitController controller, AbilityDuration duration,
        UnitModel model, Level currentLevel, int slotIndex) :
        base(controller, duration, currentLevel)
    {
        this.model = model;
        summonedUnits = CurrentLevel.SummonUnits;
        this.slotIndex = slotIndex;
    }

    public override void Run()
    {
        Controller.transform.SetParent(null, true);
        Controller.DeactivateInteraction();
        SpawnManager.Instance.StartCoroutine(SpawnUnit());
    }
    public IEnumerator SpawnUnit()
    {
        Slot[] slots;
        var isRight = false;

        if (GameManager.Instance.IsPhaseBattle)
        {
            slots = model.IsTeam1 ?
                PhaseBattleController.Instance.Slots1 :
                PhaseBattleController.Instance.Slots2;

            isRight = !model.IsTeam1;
        }
        else
        {
            slots = PhaseShopUnitManager.Instance.BattleSlots;
        }
        for (int i = 0; i < summonedUnits.Length; i++)
        {
            if (slots[slotIndex].Unit() == null)
            {
                Debug.Log($"-Summon SpawnSummonedUnit at slot {slotIndex}");
                var unit = SpawnManager.Instance.Spawn(
                    summonedUnits[i],
                    -1,
                    null,
                    model.UnitState,
                    slots[slotIndex].transform);

                yield return new WaitUntil(() => unit != null);

                var controller = unit.GetComponent<UnitController>();
                controller.View.Shadow.enabled = false;

                if (isRight)
                {
                    controller.View.SetRightSide();
                    controller.Model.IsTeam1 = false;
                }
            }
        }

        slotIndex = -1;
    }

}

