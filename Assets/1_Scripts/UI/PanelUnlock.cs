using UnityEngine;
using UnityEngine.EventSystems;

public class PanelUnlock : MonoBehaviour, IPointerClickHandler
{
    bool wasClicked = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (wasClicked) 
            return;

        PhaseShopController.Instance.SetStartTurn(StartTurnState.ClickPanelUnlock);
        //if (GameManager.Instance.IsTutorialRunning)
        //{
        //    TutorialManager.Instance.StartStep(GameManager.Instance.TutorialStepState + 1);
        //}
        wasClicked = true;
    }
}
