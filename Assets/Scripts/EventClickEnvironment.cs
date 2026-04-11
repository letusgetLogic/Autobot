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
        if (game != null && game.State == GameState.WaitingCutScene)
        {
            GameManager.Instance.Switch(GameState.LoadScene);
            InputManager.Instance.BlocksInput = true;

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
                if (game.State == GameState.WaitingEndOfBattle)
                {
                    InputManager.Instance.BlocksInput = true;
                    GameManager.Instance.Switch(GameState.PlayCutScene);
                }

                if (game.State == GameState.WaitingEndOfGame)
                {
                    GameManager.Instance.LoadScene("Menu");
                }

                var replay = GameManager.Instance.Replay;
                if (replay != null && replay.State == GameState.WaitingEndOfBattle)
                {
                    InputManager.Instance.BlocksInput = true;
                    replay.Switch(GameState.PlayCutScene);
                }

                //PhaseBattleView.Instance.OnRunningButtonClick();
                break;
        }
        Debug.Log("Click " + GameManager.Instance.ClickIndex + " -> Execute");
        isChecking = false;
    }
}
