using UnityEngine;

public class InsertState : StateBase
{
    /// <summary>
    /// Constructor of InsertState. Moves the units to the center.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public InsertState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- InsertState");
        MoveCloserTogether(PhaseBattleController.Instance.Slots1);
        MoveCloserTogether(PhaseBattleController.Instance.Slots2);
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            _ctx.SetState(new AttackState(
                PhaseBattleController.Instance.Process.DurationAttack));
        }
    }

    /// <summary>
    /// Moves the units to the center.
    /// </summary>
    /// <param name="_slots"></param>
    private void MoveCloserTogether(Slot[] _slots)
    {
        bool[] isOccupied = new bool[_slots.Length];
        int mostFrontEmpty = 0;

        for (int i = 0; i < _slots.Length; i++)
        {
            isOccupied[i] = false;

            var movedUnit = _slots[i].Unit();

            if (movedUnit != null)
            {
                isOccupied[i] = true;

                if (i > 0) // skip the first slot
                {
                    PhaseBattleController.Instance.HideDescriptionByTransport();
                    movedUnit.transform.SetParent(_slots[mostFrontEmpty].transform, false);
                    mostFrontEmpty++;
                    continue;
                }

                mostFrontEmpty++;
            }
        }
    }
}