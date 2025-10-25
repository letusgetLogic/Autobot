using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventClickEnviroment : MonoBehaviour, IPointerClickHandler
{
    private bool paused = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(GameManager.Instance.SceneName)
        {
            case "PhaseShop":
                PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
                break;
            case "PhaseBattle":
                paused = !paused;
                int value = paused ? 0 : 1;
                Time.timeScale = value;
                break;

        }

       
    }
}
