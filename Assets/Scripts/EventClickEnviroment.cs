using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventClickEnviroment : MonoBehaviour, IPointerClickHandler
{
    private bool paused = false;
    private float currentSpeed;
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(GameManager.Instance.SceneName)
        {
            case "PhaseShop":
                PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
                break;
            case "PhaseBattle":
                if (paused == false)
                    currentSpeed = GameManager.Instance.CurrentSpeedMultiplier;
                
                paused = !paused;

                GameManager.Instance.CurrentSpeedMultiplier = 
                    paused ? 0f : currentSpeed;
                break;

        }

       
    }
}
