public class StartOfBattleState : StateBase
{
    public StartOfBattleState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        TriggerAbility(PhaseBattleController.Instance.Slot1);
        TriggerAbility(PhaseBattleController.Instance.Slot2);

        ctx.SetState(new InsertState(PhaseBattleController.Instance.DurationInsert));
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {

    }

    private void TriggerAbility(Slot[] slots)
    {
        foreach (var slot in slots)
        {
            var unit = slot.UnitController();
            if (unit != null)
            {
                var trigger = unit.Model.CurrentLevel.TriggerType;
                if (trigger == TriggerType.StartOfBattle)
                {

                }

            }
        }
    }
}