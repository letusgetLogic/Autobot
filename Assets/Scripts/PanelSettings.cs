using UnityEngine;
using UnityEngine.UI;

public class PanelSettings : MonoBehaviour
{
    [SerializeField] private Toggle modeMobile;

    private void OnEnable()
    {
        if (modeMobile != null)
        {
            modeMobile.isOn = GameManager.Instance.IsMobile;
        }
    }

    public void OnModeMobile()
    {
        GameManager.Instance.SetIsMobile(modeMobile.isOn);
    }
}