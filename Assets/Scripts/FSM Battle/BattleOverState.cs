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
        Debug.Log("--- BattleOverState ---");

        if (GameManager.Instance.Players.Count < 2)
        {
            Debug.LogWarning("Players.Count = " + GameManager.Instance.Players.Count);
            _ctx.SetState(null);
            return;
        }

        var player1 = GameManager.Instance.Players[0];
        var player2 = GameManager.Instance.Players[1];

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

        EventManager.Instance.OnGameOver?.Invoke();
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

