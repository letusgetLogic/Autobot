using UnityEngine;

public class FaintState : StateBase
{
    /// <summary>
    /// Constructor of FaintState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public FaintState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- FaintState");

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
    /// Hides the fainted units.
    /// </summary>
    private void HideUnit()
    {
        while (PhaseBattleController.Instance.FaintUnits.Count > 0)
        {
            var unit = PhaseBattleController.Instance.FaintUnits.Dequeue();
            unit.GetComponent<UnitController>().Deactivate();
            Debug.Log($"Fainted unit {unit.name} is hided");
        }
    }
}