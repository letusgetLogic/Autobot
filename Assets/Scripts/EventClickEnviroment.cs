using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventClickEnviroment : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Button click calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        var game = GameManager.Instance.CurrentGame;
        if (game != null && game.State == GameState.WaitingCutScene)
        {
            GameManager.Instance.Switch(GameState.LoadScene);
            GameManager.Instance.IsBlockingInput = true;
            return;
        }


        switch (GameManager.Instance.SceneName)
        {
            case "PhaseShop":
                var tutorial = TutorialManager.Instance;
                if (tutorial != null && tutorial.TutorialCompleted == false && tutorial.ShouldClickForNextStep)
                {
                    tutorial.SetNextStep();
                }
                else
                {
                    var phaseShop = PhaseShopController.Instance;
                    if (phaseShop)
                        phaseShop.SetAttachedGameObject(null);
                }
                break;
            case "PhaseBattle":
                if (game.State == GameState.WaitingEndOfBattle)
                {
                    GameManager.Instance.IsBlockingInput = true;
                    GameManager.Instance.Switch(GameState.PlayCutScene);
                }

                if (game.State == GameState.WaitingEndOfGame)
                {
                    GameManager.Instance.LoadScene("Menu");
                }

                var replay = GameManager.Instance.Replay;
                if (replay != null && replay.State == GameState.WaitingEndOfBattle)
                {
                    GameManager.Instance.IsBlockingInput = true;
                    replay.Switch(GameState.PlayCutScene);
                }

                //PhaseBattleView.Instance.OnRunningButtonClick();
                break;

        }
    }
}
