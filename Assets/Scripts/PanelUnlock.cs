using UnityEngine;
using UnityEngine.EventSystems;

public class PanelUnlock : MonoBehaviour, IPointerClickHandler
{
    bool wasClicked = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (wasClicked) 
            return;

        PhaseShopUI.Instance.SetUnlockedTier(false, 0);
        if (GameManager.Instance.IsTutorialRunning)
        {
            TutorialManager.Instance.SetNextStep();
        }
        wasClicked = true;
    }
}
