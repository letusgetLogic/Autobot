using System.Collections;
using UnityEngine;
public class ShootOut : AbilityBase
{
    private readonly UnitModel model;
    private readonly SoUnit[] craftedUnits;
    private int slotIndex;
    private float duration
    {
        get
        {
            if (PhaseShopController.Instance != null)
                return PhaseShopController.Instance.Process.DurationShootOut;
            if (PhaseBattleController.Instance != null)
                return PhaseBattleController.Instance.Process.DurationShootOut;

            return 0f;
        }
    }

    /// <summary>
    ///  Constructor of Craft.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_model"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teamSlots"></param>
    public ShootOut(UnitController _controller, UnitModel _model, Level _currentLevel, Slot[] _teamSlots, UnitController _targetedByItem)
        : base(_controller, _currentLevel, _teamSlots, _targetedByItem)
    {
        this.model = _model;
        craftedUnits = CurrentLevel.SummonUnits;
        this.slotIndex = _controller.Slot.Index;
    }

    protected override IEnumerator Activate()
    {
        if (CurrentLevel.TriggerType == TriggerType.Shutdown)
        {
            Controller.StartCoroutine(Controller.Deactivate(DurationDescription));

            yield return new WaitUntil(() => TeamSlots[slotIndex].Unit() == null);

            for (int i = 0; i < CurrentLevel.SummonUnits.Length; i++)
            {
                if (TeamSlots[0].Unit() != null)
                {
                    var makeSpace = InsertState.MakeSpaceAtMostFront(TeamSlots);
                    if (makeSpace.CanMove == false)
                        break;

                    yield return new WaitForSeconds(makeSpace.AnimTime);
                }
                
                SpawnManager.Instance.Spawn(
                     craftedUnits[i],
                     craftedUnits[i].ID,
                     new(),
                     model.Data.UnitState,
                     TeamSlots[0].transform,
                     PhaseShopController.Instance != null ? true : model.Data.IsTeamLeft);
            }
        }

        yield return null;

        Coroutine = null;








        //EventManager.Instance.OnShootOut?.Invoke();

        //int teamlength = TeamSlots.Length;
        //int leftCrafted = craftedUnits.Length;
        //Stack<Slot> occupiedSlotsAhead = new Stack<Slot>();
        //Stack<Slot> occupiedSlotsBehind = new Stack<Slot>();

        //// count occupied ahead slot index
        //for (int i = slotIndex - 1; i >= 0; i--)
        //{
        //    if (i < 0)
        //        break;
        //    if (TeamSlots[i] != null)
        //    {
        //        occupiedSlotsAhead.Push(TeamSlots[i]);
        //    }
        //}

        //// count occupied behind slot index
        //for (int i = slotIndex + 1; i < teamlength; i++)
        //{
        //    if (i >= teamlength)
        //        break;
        //    if (TeamSlots[i] != null)
        //    {
        //        occupiedSlotsBehind.Push(TeamSlots[i]);
        //    }
        //}

        //for (int i = 0; i < TeamSlots.Length; i++)
        //{
        //    if (occupiedSlotsAhead.Count > 0 && occupiedSlotsAhead.Peek().Index > 0)
        //    {
        //        var slot = occupiedSlotsAhead.Pop();
        //        slot.transform.SetParent(TeamSlots[i].transform, false);
        //    }
        //}
    }

    //private Slot EmptySlot()
    //{
    //    for (int i = 0; )
    //}
}


