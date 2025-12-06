using System.Diagnostics;

public class StartOfBattleState : StateBase
{
    /// <summary>
    /// Constructor of StartOfBattleState.
    /// </summary>
    /// <param name="_maxCount"></param>
    public StartOfBattleState(float _maxCount) : base(_maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.WriteLine("--- StartOfBattleState");
        TriggerAbility(PhaseBattleController.Instance.Slots1);
        TriggerAbility(PhaseBattleController.Instance.Slots2);

        _ctx.SetState(new InsertState(
            PhaseBattleController.Instance.Process.DurationInsert));
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {

    }

    /// <summary>
    /// Triggers the ability before attacking.
    /// </summary>
    /// <param name="_slots"></param>
    private void TriggerAbility(Slot[] _slots)
    {
        foreach (var slot in _slots)
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