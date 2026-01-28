using  UnityEngine;

public class BattleOverState : StateBase
{
    /// <summary>
    /// Constructor of BattleOverState
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public BattleOverState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        var player1 = PhaseBattleController.Instance.Player1;
        var player2 = PhaseBattleController.Instance.Player2;

        player1.EndBattle();
        player2.EndBattle();

        // continue the game, when both have more than 0 lives.
        if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
        {
            PhaseBattleController.Instance.SetDetectClickActive(true);
            GameManager.Instance.Switch(GameState.EndOfBattle);
            _ctx.SetState(null);
            return;
        }

        // end game
        if (player1.Data.Lives == 0)
        {
            PhaseBattleView.Instance.ShowWinner(player2.Data.Name, true);
        }
        else if (player2.Data.Lives == 0)
        {
            PhaseBattleView.Instance.ShowWinner(player1.Data.Name, true);
        }
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            GameManager.Instance.Switch(GameState.EndOfGame);
        }
    }
}

