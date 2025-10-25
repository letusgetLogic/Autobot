public class FaintState : StateBase
{
    public FaintState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        ManageFaintUnits(
            PhaseBattleController.Instance.Slots1, true);

       ManageFaintUnits(
            PhaseBattleController.Instance.Slots2, true);

        ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, false));
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        
    }


    /// <summary>
    /// Destroy the faint unit and trigger ability
    /// </summary>
    public void ManageFaintUnits(Slot[] slots, bool isBattle)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var controller = slots[i].UnitController();
            if (controller != null && controller.IsFaint)
            {

            }
        }
    }

}