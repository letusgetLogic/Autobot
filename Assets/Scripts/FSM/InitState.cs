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

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        if (Count < MaxCount)
        {
            Count += Time.deltaTime;
        }
        else
        {
            ctx.SetState(new CheckOutcomeState(0.5f, true));
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
        for (int i = 0; i < player.TeamSlots.Length; i++)
        {
            var unit = player.TeamSlots[i].Unit();
            if (unit != null)
            {
                var unitOnScene = PhaseBattleController.Instance.Spawn(unit);
                unitOnScene.GetComponent<UnitController>().SetModel(unit.GetComponent<UnitController>().Model);
                unitOnScene.transform.SetParent(slots[i].transform, false);

                if (isRight)
                {
                    unitOnScene.GetComponent<UnitView>().SetRightSide();
                    unitOnScene.GetComponent<UnitController>().Model.IsTeam1 = false;
                }
            }
        }
    }
}