using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Craft : AbilityBase
{
    private readonly UnitModel model;
    private readonly SoUnit[] craftedUnits;
    private int slotIndex;

    /// <summary>
    ///  Constructor of Craft.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_model"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teamSlots"></param>
    public Craft(UnitController _controller, UnitModel _model, Level _currentLevel, Slot[] _teamSlots, UnitController _targetedByItem) 
        : base(_controller, _currentLevel, _teamSlots, _targetedByItem)
    {
        this.model = _model;
        craftedUnits = CurrentLevel.SummonUnits;
        this.slotIndex = _controller.Slot.Index;
    }

    public override void Activate()
    {
        SpawnManager.Instance.StartCoroutine(SpawnUnit());
    }

    /// <summary>
    /// Delay the instantiating of the crafted unit and sets the unit, which has triggered, invisible.
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnUnit()
    {
        float duration = 0f;

        if (PhaseShopController.Instance != null)
            duration = PhaseShopController.Instance.Process.DurationDelayCraft;
        if (PhaseBattleController.Instance != null)
            duration = PhaseBattleController.Instance.Process.DelayCraft;

        yield return new WaitForSeconds(duration);

        Controller.Deactivate();

        SpawnManager.Instance.Spawn(
                   craftedUnits[0],
                   craftedUnits[0].ID,
                   new(),
                   model.Data.UnitState,
                   TeamSlots[slotIndex].transform,
                   PhaseShopController.Instance != null ? true : model.Data.IsTeamLeft);


        //int teamlength = TeamSlots.Length;
        //int leftCrafted = craftedUnits.Length;
        //Stack<Slot> occupiedSlotsAhead = new Stack<Slot>();
        //Stack<Slot> occupiedSlotsBehind = new Stack<Slot>();

        //// count occupied ahead slot index
        //for (int i = slotIndex -1; i >= 0; i--)
        //{
        //    if (i < 0)
        //        break;
        //    if (TeamSlots[i] != null)
        //    {
        //        occupiedSlotsAhead.Push(TeamSlots[i]);
        //    }
        //}

        //// count occupied behind slot index
        //for (int i = slotIndex +1; i < teamlength; i++)
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

        slotIndex = -1;
    }

}
    

