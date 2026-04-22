
public class ReplayManager
{
    public GameState State => state;
    private GameState state;

    private InputManager input => InputManager.Instance;

    public ReplayManager()
    {
        if (PhaseShopController.Instance != null)
        {
            var player = PhaseShopController.Instance.Player;

            player.SaveDataByReplay();
        }
    }

    /// <summary>
    /// Switch the game state and execute the corresponding actions for each state.
    /// </summary>
    /// <param name="_state"></param>
    public void Switch(GameState _state)
    {
        var prevState = state;
            state = _state;

        switch (_state)
        {
            case GameState.None:
                break;

            case GameState.PlayCutSceneBattle:
                CutScene.Instance.SwitchScene();
                break;

            case GameState.PlayCutSceneShop:
                CutScene.Instance.SwitchScene();
                break;

            case GameState.LoadScene:
                switch (prevState)
                {
                    case GameState.PlayCutSceneBattle:
                        GameManager.Instance.LoadScene("PhaseBattle");
                        break;
                    case GameState.PlayCutSceneShop:
                        GameManager.Instance.LoadScene("PhaseShop");
                        break;
                    case GameState.WaitingEndOfBattle:
                        Switch(GameState.PlayCutSceneShop);
                        break;
                    case GameState.WaitingEndOfGame:
                        GameManager.Instance.LoadScene("Menu");
                        break;
                }
                break;

            case GameState.WaitingEndOfBattle:
                input.BlocksInput = false;
                EventManager.Instance.OnBattleDelayHintClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.WaitingEndOfGame:
                input.BlocksInput = false;
                EventManager.Instance.OnBattleDelayHintClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.StartOfBattle:
                PhaseBattleController.Instance.Run(
                    GameManager.Instance.Players[0],
                    GameManager.Instance.Players[1]);
                break;

            case GameState.BattlePhase:
                input.BlocksInput = false;
                break;

            case GameState.EndOfBattle:
                Switch(GameState.WaitingEndOfBattle);
                break;

            case GameState.EndOfGame:
                Switch(GameState.WaitingEndOfGame);
                break;
        }

        
    }


}
