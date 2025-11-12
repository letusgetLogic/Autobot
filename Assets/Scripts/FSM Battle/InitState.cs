using UnityEngine;

public class InitState : StateBase
{
    public InitState(float maxTimeCount) : base(maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log(PhaseBattleController.Instance.Player1.Data.Turns);
        Debug.Log("--- InitState");
        Initialize();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += speed;
        }
        else
        {
            ctx.SetState(new CheckOutcomeState(
                PhaseBattleController.Instance.Process.DurationCheckOutcome, true));
        }
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    private void Initialize()
    {
        PhaseBattleView.Instance.Initialize(
            PhaseBattleController.Instance.Player1.Data,
            PhaseBattleController.Instance.Player2.Data);

        SpawnUnits(
            PhaseBattleController.Instance.Player1,
            PhaseBattleController.Instance.Slots1, false);

        SpawnUnits(
            PhaseBattleController.Instance.Player2,
            PhaseBattleController.Instance.Slots2, true);
    }

    /// <summary>
    /// Instantiates and initializes the units.
    /// </summary>
    private void SpawnUnits(Player player, Slot[] slots, bool isRight)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var unitData = player.Data.BattleUnitDatas[i];
            if (unitData.HasReference)
            {
                var controller = SpawnManager.Instance.Spawn(
                    PackManager.Instance.Units[unitData.Index],
                    unitData.Index,
                    unitData,
                    UnitState.InPhaseBattle,
                    slots[i].transform);

                controller.View.Shadow.enabled = false;

                if (isRight)
                {
                    controller.View.SetRightSide();
                    controller.Model.Data.IsTeam1 = false;
                }
            }
        }
    }
}