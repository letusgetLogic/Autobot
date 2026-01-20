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
        MoveCloserToCenter(PhaseBattleController.Instance.Slots1());
        MoveCloserToCenter(PhaseBattleController.Instance.Slots2());
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
    private void MoveCloserToCenter(Slot[] _slots)
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

                    var controller = movedUnit.GetComponent<UnitController>();
                    var slot = _slots[mostFrontEmpty];

                    controller.MoveToParent(slot.transform.position, slot.transform);

                    mostFrontEmpty++;
                    continue;
                }

                mostFrontEmpty++;
            }
        }
    }

    /// <summary>
    /// Moves the units back from center.
    /// </summary>
    /// <param name="_slots"></param>
    public static void MoveBackFromCenter(Slot[] _slots)
    {
        bool[] isOccupied = new bool[_slots.Length];
        int mostBehindEmpty = _slots.Length - 1;

        for (int i = _slots.Length - 1; i == 0; i--)
        {
            isOccupied[i] = false;

            var movedUnit = _slots[i].Unit();

            if (movedUnit != null)
            {
                isOccupied[i] = true;

                if (i < _slots.Length) // skip the last slot
                {
                    PhaseBattleController.Instance.HideDescriptionByTransport();
                    movedUnit.transform.SetParent(_slots[mostBehindEmpty].transform, false);
                    mostBehindEmpty--;
                    continue;
                }

                mostBehindEmpty--;
            }
        }
    }
}