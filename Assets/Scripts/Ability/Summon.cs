using System.Collections;
using UnityEngine;
public class Summon : AbilityBase
{
    private UnitModel model;
    private SoUnit[] summonedUnits;
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

    public override void Run()
    {
        SpawnManager.Instance.StartCoroutine(SpawnUnit());
    }

    /// <summary>
    /// Delay the instantiating of the summoned unit and sets the unit, which has triggered, invisible.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnUnit()
    {
        yield return new WaitForSeconds(PhaseBattleController.Instance.Process.DurationDelaySummon);

        Controller.Deactivate();

        for (int i = 0; i < summonedUnits.Length; i++)
        {
            if (TeamSlots[slotIndex].Unit() == null)
            {
                Debug.Log($"-Summon SpawnSummonedUnit at slot {slotIndex}");
                var unitController = SpawnManager.Instance.Spawn(
                    summonedUnits[i],
                    -1,
                    new(),
                    model.Data.UnitState,
                    TeamSlots[slotIndex].transform,
                    model.Data.IsTeamLeft);
            }
        }

        slotIndex = -1;
    }

}

