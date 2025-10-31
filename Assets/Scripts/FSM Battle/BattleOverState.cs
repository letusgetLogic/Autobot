using  UnityEngine;

public class BattleOverState : StateBase
{
    public BattleOverState(float maxTimeCount) : base(maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        var player1 = PhaseBattleController.Instance.Player1;
        var player2 = PhaseBattleController.Instance.Player2;

        if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
        {
            GameManager.Instance.EndPhaseBattle();
            Debug.Log("-------------------------------");
            return;
        }
        if (player1.Data.Lives == 0)
        {
            PhaseBattleView.Instance.ShowWinner(player2.Data.Name, true);
        }
        else if (player2.Data.Lives == 0)
        {
            PhaseBattleView.Instance.ShowWinner(player1.Data.Name, true);
        }
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += speed;
        }
        else
        {
            GameManager.Instance.EndGame();
        }
    }
}

