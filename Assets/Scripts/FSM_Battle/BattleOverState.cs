using UnityEngine;
using UnityEngine.Events;

public class BattleOverState : StateBaseBattle
{
    /// <summary>
    /// -1 = team 1 wins / 0 = draw / 1 = team 2 wins.
    /// </summary>
    private int outcome;

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

        

        bool isReplay = GameManager.Instance.Replay != null;

        // First the lives were calculated and showed.

        if (isReplay)
        {
            int lives1 = outcome switch
            {
                1 => data1.Lives - 1,
                _ => data1.Lives
            };
            int lives2 = outcome switch
            {
                -1 => data2.Lives - 1,
                _ => data2.Lives
            };
            PhaseBattleView.Instance.UpdateLives(lives1, lives2);
        }
        else // is not replay
        {
            GameManager.Instance.UpdatePlayerStats(outcome);
            PhaseBattleView.Instance.UpdateLives(player1.Data.Lives, player2.Data.Lives);
        }

        // Then define the winner.

        string winner = outcome switch
        {
            1 => player2.Data.Name,
            -1 => player1.Data.Name,
            _ => ""
        };
        PlayerData winnerData = outcome switch
        {
            1 => data2,
            -1 => data1,
            _ => null
        };

        // Then check end battle or end game.
        // If end game, it shows the animation and then set state to end game.

        if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
        {
            PhaseBattleController.Instance.StartCoroutine(
               PhaseBattleView.Instance.ShowWinnerAtEndOfBattle(outcome == 0, winner));

            if (isReplay)
                GameManager.Instance.Replay.Switch(GameState.EndOfBattle);
            else
            {
                player1.EndBattle();
                player2.EndBattle();
                GameManager.Instance.Switch(GameState.EndOfBattle);
            }
            EventManager.Instance.OnBattleDone?.Invoke();
        }
        else // one of them has 0 lives, game over
        {
            if (isReplay)
                GameManager.Instance.Replay.Switch(GameState.EndOfGame);
            else
                GameManager.Instance.Switch(GameState.EndOfGame);

            UnityAction action = () =>
            {
                EventManager.Instance.OnGameOverSound?.Invoke();
                EventManager.Instance.OnBattleDone?.Invoke();

                if (isReplay)
                    GameManager.Instance.Replay.Switch(GameState.WaitingEndOfGame);
                else
                    GameManager.Instance.Switch(GameState.WaitingEndOfGame);
            };

            PhaseBattleController.Instance.StartCoroutine(
                PhaseBattleView.Instance.ShowWinnerAtEndOfGame(winner, winnerData, action));
        }
    }

    public override void OnUpdate(I_FSM_Battle _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
    }
}

