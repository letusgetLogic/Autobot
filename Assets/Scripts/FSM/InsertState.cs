public class InsertState : StateBase
{
    public InsertState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        MoveCloserTogether(PhaseBattleController.Instance.Slots1);
        MoveCloserTogether(PhaseBattleController.Instance.Slots2);
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (Count < MaxCount)
        {
            Count += speed;
        }
        else
        {
            ctx.SetState(new AttackState(0));
        }
    }

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
                    movedUnit.transform.SetParent(slots[mostFrontEmpty].transform, false);
                    mostFrontEmpty++;
                    continue;
                }

                mostFrontEmpty++;
            }
        }
    }
}
