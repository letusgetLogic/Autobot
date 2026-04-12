using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickEnvironment : MonoBehaviour, IPointerClickHandler
{
    private readonly InputKey inputKey = InputKey.ClickEnvironment;
    private bool isChecking = false;

    /// <summary>
    /// Button click calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        int a = GameManager.Instance.ClickIndex++;
        Debug.Log("Click " + a);
        if (isChecking)
            return;

      
        isChecking = true;

        if (InputManager.Instance.IsBlockingInput(inputKey))
        {
            isChecking = false;
            return;
        }

        var game = GameManager.Instance.CurrentGame;
        if (game != null && 
            game.State == GameState.PlayCutSceneShop || game.State == GameState.PlayCutSceneBattle)
        {
            InputManager.Instance.BlocksInput = true;
            GameManager.Instance.Switch(GameState.LoadScene);

            isChecking = false;
            return;
        }


        switch (GameManager.Instance.SceneName)
        {
            case "PhaseShop":
                var tutorial = TutorialManager.Instance;
                if (tutorial != null && GameManager.Instance.IsTutorialRunning)
                {
                    Debug.Log("SetNextStep " + GameManager.Instance.ClickIndex);
                    tutorial.SetNextStep();
                }

                var phaseShop = PhaseShopController.Instance;
                if (phaseShop)
                    phaseShop.SetAttachedGameObject(null);

                break;

            case "PhaseBattle":
                var replay = GameManager.Instance.Replay;
                if (replay != null && 
                    replay.State == GameState.WaitingEndOfBattle || game.State == GameState.WaitingEndOfGame)
                {
                    InputManager.Instance.BlocksInput = true;
                    replay.Switch(GameState.LoadScene);
                    return;
                }

                if (game.State == GameState.WaitingEndOfBattle || game.State == GameState.WaitingEndOfGame)
                    GameManager.Instance.Switch(GameState.LoadScene);

                //PhaseBattleView.Instance.OnRunningButtonClick();
                break;
        }
        Debug.Log("Click " + GameManager.Instance.ClickIndex + " -> Execute");
        isChecking = false;
    }
}
