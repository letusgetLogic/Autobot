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
        var game = GameManager.Instance.CurrentGame;
        if (game != null && game.State == GameState.WaitingSwitchScene)
        {
            GameManager.Instance.Switch(GameState.LoadScene);
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
                //PhaseBattleView.Instance.OnRunningButtonClick();
                break;

        }
    }
}
