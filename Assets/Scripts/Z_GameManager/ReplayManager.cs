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
        state = _state;

        switch (_state)
        {
            case GameState.None:
                break;

            case GameState.LoadScene:
                GameManager.Instance.LoadScene(GameManager.Instance.SceneToLoad);
                break;

            case GameState.PlayCutScene:
                CutScene.Instance.SwitchScene(GameManager.Instance.SceneToLoad);
                break;

            case GameState.WaitingEndOfBattle:
                input.BlocksInput = false;
                EventManager.Instance.OnWaitingForClick?.Invoke();
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
                if (GameManager.Instance.CurrentGame.State == GameState.EndOfGame)
                {
                    GameManager.Instance.SceneToLoad = "Menu";
                }
                else GameManager.Instance.SceneToLoad = "PhaseShop";

                Switch(GameState.WaitingEndOfBattle);
                break;
        }
    }


}
