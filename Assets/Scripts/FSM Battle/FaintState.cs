using UnityEngine;

public class FaintState : StateBase
{
    public FaintState(float maxTimeCount) : base(maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- FaintState");

        HideUnit();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += speed;
        }
        else
        {
            if (PhaseBattleController.Instance.UnitAbilities.Count > 0)
                ctx.SetState(new HandleAbilityState(0));
            else
                ctx.SetState(new CheckOutcomeState(
                    PhaseBattleController.Instance.Process.DurationCheckOutcome, false));
        }
    }

    private void HideUnit()
    {
        while (PhaseBattleController.Instance.FaintUnits.Count > 0)
        {
            var unit = PhaseBattleController.Instance.FaintUnits.Dequeue();
            unit.SetActive(false);
            unit.transform.parent = null;
            Debug.Log($"Fainted unit {unit.name} destroyed");
            PhaseBattleController.Instance.Player1.UpdateTeamUnitData();
            PhaseBattleController.Instance.Player2.UpdateTeamUnitData();
        }
    }
}