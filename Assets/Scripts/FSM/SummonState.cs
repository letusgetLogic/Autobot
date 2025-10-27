
using UnityEngine;

public class SummonState : StateBase
{
    private bool isDone = false;
    public SummonState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- SummonState");
        while (PhaseBattleController.Instance.SummonUnits.Count > 0)
        {
           var summon = PhaseBattleController.Instance.SummonUnits.Dequeue();
            Debug.Log($"{this.ToString()} Dequeue");
            Debug.Log($"{PhaseBattleController.Instance.SummonUnits.Count} SummonUnits");
            
            Debug.Log("-Summon SpawnSummonedUnit");
            SpawnManager.Instance.StartCoroutine(summon.SpawnUnit());
        }
        isDone = true;
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (isDone)
        {
            ctx.SetState(new HandleAbilityState(0));
        }
    }
}

