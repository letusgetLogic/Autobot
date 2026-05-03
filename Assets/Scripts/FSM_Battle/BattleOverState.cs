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

        PlayerData winnerData = null;

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

                PhaseBattleController.Instance.StartCoroutine(
                    PhaseBattleView.Instance.ShowWinner(false, player1.Data.Name, player2.Data.Lives == 0));

                winnerData = data1;
                break;

            case 0: // Draw
                if (GameManager.Instance.Replay == null)
                {
                    GameManager.Instance.UpdatePlayerStats(0);
                }
                PhaseBattleController.Instance.StartCoroutine(
                    PhaseBattleView.Instance.ShowWinner(true, "Nobody", false));
                break;

            case 1: // Right wins
                if (GameManager.Instance.Replay == null)
                {
                    GameManager.Instance.UpdatePlayerStats(1);
                    PhaseBattleView.Instance.UpdateLives(player1.Data.Lives, player2.Data.Lives);
                }
                else
                    PhaseBattleView.Instance.UpdateLives(data1.Lives - 1, data2.Lives);

                PhaseBattleController.Instance.StartCoroutine(
                    PhaseBattleView.Instance.ShowWinner(false, player2.Data.Name, player1.Data.Lives == 0));

                winnerData = data2;
                break;
        }

        if (GameManager.Instance.Replay == null)
        {
            // continue the game, when both have more than 0 lives.
            if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
            {
                player1.EndBattle();
                player2.EndBattle();

                GameManager.Instance.Switch(GameState.EndOfBattle);
            }
            else // end the game, when one of them has 0 lives.
            {
                EventManager.Instance.OnGameOver?.Invoke();
                GameManager.Instance.Switch(GameState.EndOfGame);
                ShowWinnerTeam(winnerData);
            }
        }
        else // go out of the replay, waiting of input click to load the current play scene 
        {
            if (player1.Data.Lives > 0 && player2.Data.Lives > 0)
            {
                GameManager.Instance.Replay.Switch(GameState.EndOfBattle);
            }
            else // end the game, when one of them has 0 lives.
            {
                GameManager.Instance.Replay.Switch(GameState.EndOfGame);
                ShowWinnerTeam(winnerData);
            }
        }

        EventManager.Instance.OnBattleDone?.Invoke();
    }

    public override void OnUpdate(I_FSM_Battle _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
    }

    private void ShowWinnerTeam(PlayerData _data)
    {
        var init = new InitializeState(0f);
        var team = init.SpawnUnitsByData(_data, PhaseBattleController.Instance.Slots1.ToArray(), true);

        PhaseBattleController.Instance.ShowWinnerTeam(team);
    }
}

