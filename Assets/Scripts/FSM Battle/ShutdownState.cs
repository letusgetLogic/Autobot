using UnityEngine;

public class ShutdownState : StateBase
{
    /// <summary>
    /// Constructor of ShutdownState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public ShutdownState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- ShutdownState");

        HideUnit();
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            if (PhaseBattleController.Instance.UnitAbilities.Count > 0)
                _ctx.SetState(new HandleAbilityState(0));
            else
                _ctx.SetState(new CheckOutcomeState(
                    PhaseBattleController.Instance.Process.DurationCheckOutcome, false));
        }
    }

    /// <summary>
    /// Hides the shutdowned units.
    /// </summary>
    private void HideUnit()
    {
        while (PhaseBattleController.Instance.ShutdownUnits.Count > 0)
        {
            var unit = PhaseBattleController.Instance.ShutdownUnits.Dequeue();
            if (unit.gameObject.activeSelf) 
                unit.StartCoroutine(unit.Deactivate(0f));
            Debug.Log($"Shutdowned unit {unit.name} is hided");
        }
    }
}