using UnityEngine;

public class InsertState : StateBase
{
    /// <summary>
    /// Constructor of InsertState. Moves the units to the center.
    /// </summary>
    /// <param name="maxTimeCount"></param>
    public InsertState(float maxTimeCount) : base(maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {Debug.Log("--- InsertState");
        MoveCloserTogether(PhaseBattleController.Instance.Slots1);
        MoveCloserTogether(PhaseBattleController.Instance.Slots2);
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += speed;
        }
        else
        {
            ctx.SetState(new AttackState(
                PhaseBattleController.Instance.Process.DurationAttack));
        }
    }

    /// <summary>
    /// Moves the units to the center.
    /// </summary>
    /// <param name="slots"></param>
    private void MoveCloserTogether(Slot[] slots)
    {
        bool[] isOccupied = new bool[slots.Length];
        int mostFrontEmpty = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            isOccupied[i] = false;

            var movedUnit = slots[i].Unit();

            if (movedUnit != null)
            {
                isOccupied[i] = true;

                if (i > 0) // skip the first slot
                {
                    PhaseBattleController.Instance.HideDescriptionByTransport();
                    movedUnit.transform.SetParent(slots[mostFrontEmpty].transform, false);
                    mostFrontEmpty++;
                    continue;
                }

                mostFrontEmpty++;
            }
        }
    }
}
