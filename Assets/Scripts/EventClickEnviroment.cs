using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventClickEnviroment : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Button click calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
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
                    var phaseShop = PhaseShopUnitManager.Instance;
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
