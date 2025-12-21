using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
public class Summon : AbilityBase
{
    private readonly UnitModel model;
    private readonly SoUnit[] summonedUnits;
    private int slotIndex;

    /// <summary>
    ///  Constructor of Summon.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_model"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teamSlots"></param>
    public Summon(UnitController _controller, UnitModel _model, Level _currentLevel, Slot[] _teamSlots) :
        base(_controller, _currentLevel, _teamSlots)
    {
        this.model = _model;
        summonedUnits = CurrentLevel.SummonUnits;
        this.slotIndex = _controller.Slot.Index;
    }

    public override void Activate()
    {
        SpawnManager.Instance.StartCoroutine(SpawnUnit());
    }

    /// <summary>
    /// Delay the instantiating of the summoned unit and sets the unit, which has triggered, invisible.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnUnit()
    {
        float duration = 0f;

        if (PhaseShopUnitManager.Instance != null)
            duration = PhaseShopUnitManager.Instance.Process.DurationDelaySummon;
        if (PhaseBattleController.Instance != null)
            duration = PhaseBattleController.Instance.Process.DurationDelaySummon;

        yield return new WaitForSeconds(duration);

        Controller.Deactivate();

        for (int i = 0; i < summonedUnits.Length; i++)
        {
            if (TeamSlots[slotIndex].Unit() == null)
            {
                Debug.Log($"-Summon SpawnSummonedUnit at slot {slotIndex}");
                SpawnManager.Instance.Spawn(
                    summonedUnits[i],
                    summonedUnits[i].ID,
                    new(),
                    model.Data.UnitState,
                    TeamSlots[slotIndex].transform,
                    PhaseShopUnitManager.Instance != null ? true : model.Data.IsTeamLeft);
            }
        }

        slotIndex = -1;
    }

}

