using UnityEngine;

public class BattleOverState : StateBaseBattle
{
    private int outcome; // 0 = draw, 1 = right wins, -1 = left wins.

    /// <summary>
    /// Constructor of BattleOverState
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public BattleOverState(float _maxTimeCount, int _outcome) : base(_maxTimeCount)
    {
        outcome = _outcome;
    }

    public override void OnEnter(I_FSM_Battle _ctx)
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

        var data1 = GameManager.Instance.CurrentRound.SavedPlayerData1;
        var data2 = GameManager.Instance.CurrentRound.SavedPlayerData2;

        switch (outcome)
        {
            case -1: // Left Wins
                if (GameManager.Instance.Replay == null)
                {
                    GameManager.Instance.UpdatePlayerStats(-1);
                    PhaseBattleView.Instance.UpdateLives(player1.Data.Lives, player2.Data.Lives);
                }
                else
                    PhaseBattleView.Instance.UpdateLives(data1.Lives, data2.Lives - 1);

                PhaseBattleView.Instance.ShowWinner(player1.Data.Name, player2.Data.Lives == 0);
                break;

            case 0: // Draw
                if (GameManager.Instance.Replay == null)
                {
                    GameManager.Instance.UpdatePlayerStats(0);
                }
                PhaseBattleView.Instance.ShowWinner("Nobody", false);
                break;

            case 1: // Right wins
                if (GameManager.Instance.Replay == null)
                {
                    GameManager.Instance.UpdatePlayerStats(1);
                    PhaseBattleView.Instance.UpdateLives(player1.Data.Lives, player2.Data.Lives);
                }
                else
                    PhaseBattleView.Instance.UpdateLives(data1.Lives - 1, data2.Lives);

                PhaseBattleView.Instance.ShowWinner(player2.Data.Name, player1.Data.Lives == 0);
                break;
        }

        if (GameManager.Instance.Replay == null)
        {
            player1.EndBattle();
            player2.EndBattle();

            // continue the game, when both have more than 0 lives.
            if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
            {
                GameManager.Instance.Switch(GameState.EndOfBattle);
            }
            else // end the game, when one of them has 0 lives.
            {
                EventManager.Instance.OnGameOver?.Invoke();
                GameManager.Instance.Switch(GameState.EndOfGame);
            }
        }
        else // go out of the replay, waiting of input click to load the current play scene 
        {
            GameManager.Instance.Replay.Switch(GameState.EndOfBattle);
        }

        _ctx.SetState(null);

        EventManager.Instance.OnBattleDone?.Invoke();
    }

    public override void OnUpdate(I_FSM_Battle _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
    }
}

