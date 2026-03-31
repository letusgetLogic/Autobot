using System.Collections;
using UnityEngine;

public class InsertState : StateBaseBattle
{
    /// <summary>
    /// Constructor of InsertState. Moves the units to the center.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public InsertState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(I_FSM_Battle _ctx)
    {
        Debug.Log("--- InsertState");
        PhaseBattleController.Instance.StartCoroutine(MoveCloserToCenter(PhaseBattleController.Instance.Slots1()));
        PhaseBattleController.Instance.StartCoroutine(MoveCloserToCenter(PhaseBattleController.Instance.Slots2()));
        EventManager.Instance.OnSwap?.Invoke();
    }

    public override void OnUpdate(I_FSM_Battle _ctx, float _speed)
    {
        if (IsDone)
        {
            if (GameManager.Instance.CurrentGame.State == GameState.StartOfBattle ||
                GameManager.Instance.Replay != null && GameManager.Instance.Replay.State == GameState.StartOfBattle)
            {
                _ctx.SetState(new StartOfBattleState(0));
            }
            else
            {
                _ctx.SetState(new AttackState(
                     PhaseBattleController.Instance.Process.DurationAttack));
            }
        }
    }

    /// <summary>
    /// Moves the units to the center.
    /// </summary>
    /// <param name="_slots"></param>
    private IEnumerator MoveCloserToCenter(Slot[] _slots)
    {
        int mostFrontEmpty = 0;

        for (int i = 0; i < _slots.Length; i++)
        {
            var movedUnit = _slots[i].UnitController();

            if (movedUnit != null)
            {
                if (i > 0) // skip the first slot
                {
                    PhaseBattleController.Instance.HideDescriptionByTransport();

                    var slot = _slots[mostFrontEmpty];

                    movedUnit.MoveToParent(slot.transform.position, slot.transform);

                    mostFrontEmpty++;
                    continue;
                }

                mostFrontEmpty++;
            }
        }

        yield return new WaitForSeconds(MaxTimeCount);

        IsDone = true;
    }

    /// <summary>
    /// Attempts to move the front unit in the slot array to the nearest available empty slot, if possible.
    /// </summary>
    /// <param name="_slots">An array of Slot objects to search for available space and move units.</param>
    /// <returns>A tuple containing the animation time for the move and a value indicating whether a move was possible.</returns>
    public static (float AnimTime, bool CanMove) MakeSpaceAtMostFront(Slot[] _slots)
    {
        float animTime = 0f;
        bool canMove = false;
        int searchEmpty = 1;

        while (searchEmpty > 0 && searchEmpty < _slots.Length)
        {
            Debug.Log("searchEmpty " + searchEmpty);
            if (canMove == false && _slots[searchEmpty].Unit())
            {
                searchEmpty++;
                continue;
            }

            // It has found empty slot, move the front unit to it.
            animTime = _slots[searchEmpty - 1].UnitController().MoveToParent(
                _slots[searchEmpty].transform.position, _slots[searchEmpty].transform);

            canMove = true;

            searchEmpty--;
        }
        
        return (animTime, canMove);
    }

    /// <summary>
    /// Moves the units back from center.
    /// </summary>
    /// <param name="_slots"></param>
    public static float MoveBackFromCenter(Slot[] _slots)
    {
        float countTime = 0f;

        bool[] isOccupied = new bool[_slots.Length];
        int mostBehindEmpty = _slots.Length - 1;

        for (int i = _slots.Length - 1; i == 0; i--)
        {
            isOccupied[i] = false;

            var movedUnit = _slots[i].UnitController();

            if (movedUnit != null)
            {
                isOccupied[i] = true;

                if (i < _slots.Length) // skip the last slot
                {
                    PhaseBattleController.Instance.HideDescriptionByTransport();

                    var slot = _slots[mostBehindEmpty];

                    countTime += movedUnit.MoveToParent(slot.transform.position, slot.transform);

                    mostBehindEmpty--;
                    continue;
                }

                mostBehindEmpty--;
            }
        }

        return countTime;
    }

}