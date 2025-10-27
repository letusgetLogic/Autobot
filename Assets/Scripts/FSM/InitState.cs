using UnityEngine;

public class InitState : StateBase
{
    public InitState(float maxCount) : base(maxCount)
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
            var unitModel = player.Data.BattleUnitModels[i];
            if (unitModel != null)
            {
                var controller = SpawnManager.Instance.Spawn(
                    StarterPack.Instance.Units[unitModel.Index],
                    unitModel.Index,
                    unitModel,
                    UnitState.InPhaseBattle,
                    slots[i].transform);

                controller.View.Shadow.enabled = false;

                if (isRight)
                {
                    controller.View.SetRightSide();
                    controller.Model.IsTeam1 = false;
                }
            }
        }
    }
}