using UnityEngine;
public class HandleAbilityState : StateBase
{
    private bool isDone = false;
    private float hideTimeCountdowm = 0f;

    /// <summary>
    /// Consturctor of HandleAbilityState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public HandleAbilityState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- HandleAbilityState");
        TimeCount = 0f;
        HandleAbility();
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < 0f)
        {
            TimeCount += _speed;
        }
        else
        {
            HandleAbility();
        }

        if (isDone)
        {
            if (PhaseBattleController.Instance.ShutdownUnits.Count > 0)
                _ctx.SetState(new ShutdownState(
                     PhaseBattleController.Instance.Process.DurationShutdown));
            else
                _ctx.SetState(new CheckOutcomeState(
                    PhaseBattleController.Instance.Process.DurationCheckOutcome, false));
        }
    }

    /// <summary>
    /// Handles all registered abilities.
    /// </summary>
    private void HandleAbility()
    {
        if (PhaseBattleController.Instance.UnitAbilities.Count > 0)
        {
            TimeCount -= PhaseBattleController.Instance.Process.DurationHandleEachAbility;

            var ability = PhaseBattleController.Instance.UnitAbilities.Dequeue();

            Debug.Log($"{ability.ToString()} dequeue/activate");
            Debug.Log($"{PhaseBattleController.Instance.UnitAbilities.Count} UnitAbilities left");

            PhaseBattleController.Instance.StartCoroutine(ability.Handle(
                PhaseBattleController.Instance.Process.DelayHideAbilityDescription, false));
        }
        else
            isDone = true;
    }
}
