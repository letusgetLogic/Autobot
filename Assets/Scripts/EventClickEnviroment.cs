using UnityEngine;
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
                PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
                break;
            case "PhaseBattle":
                //PhaseBattleView.Instance.OnRunningButtonClick();
                break;

        }
    }
}
