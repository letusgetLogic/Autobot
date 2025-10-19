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

        if (player1.Lives > 0 && player2.Lives > 0)
        {
            GameManager.Instance.RunModeSingle();
            return;
        }
        if (player1.Lives == 0)
        {
            PhaseBattleView.Instance.ShowWinner(player2.Name, true);
        }
        else if (player2.Lives == 0)
        {
            PhaseBattleView.Instance.ShowWinner(player1.Name, true);
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

