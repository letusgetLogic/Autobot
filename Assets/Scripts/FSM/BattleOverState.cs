public class BattleOverState : StateBase
{
    private bool isGameOver = false;

    public BattleOverState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        var player1 = PhaseBattleController.Instance.Player1;
        var player2 = PhaseBattleController.Instance.Player2;

        if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
        {
            player1.SetDefault();
            player2.SetDefault();
            GameManager.Instance.RunModeSingle();
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
        isGameOver = true;
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (Count < 1f)
        {
            Count += speed;
        }
        else
        {
            GameManager.Instance.EndGame();
        }
    }
}

