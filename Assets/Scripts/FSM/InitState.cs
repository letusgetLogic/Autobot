using UnityEngine;

public class InitState : StateBase
{
    public InitState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Initialize();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (Count < MaxCount)
        {
            Count += speed;
        }
        else
        {
            ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, true));
        }
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    private void Initialize()
    {
        PhaseBattleView.Instance.Initialize(
            PhaseBattleController.Instance.Player1,
            PhaseBattleController.Instance.Player2);

        SetUnitsToPosition(
            PhaseBattleController.Instance.Player1,
            PhaseBattleController.Instance.Slots1, false);

        SetUnitsToPosition(
            PhaseBattleController.Instance.Player2,
            PhaseBattleController.Instance.Slots2, true);
    }

    /// <summary>
    /// Instantiates the units.
    /// </summary>
    private void SetUnitsToPosition(Template player, Slot[] slots, bool isRight)
    {
        for (int i = 0; i < player.BattleUnits.Length; i++)
        {
            var unit = player.BattleUnits[i];
            if (unit != null)
            {
                unit.transform.SetParent(slots[i].transform, false);
                unit.transform.localPosition = Vector3.zero;

                if (isRight)
                {
                    unit.View.SetRightSide();
                    unit.Model.IsTeam1 = false;
                }
            }
        }
    }
}