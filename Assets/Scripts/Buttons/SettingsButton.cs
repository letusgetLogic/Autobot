using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject rewatchButton;
    [SerializeField] private List<GameObject> deactivateButtons;
    [SerializeField] private Image outline;
    private Color defaultOutlineColor => new Color(0, 0, 0);
    private Color backOutlineColor => new Color(0.5215687f, 0.7215686f, 0.8039216f);

    private bool isSettingsOpen = false;


    /// <summary>
    /// Button click calls.
    /// </summary>
    public void OnButtonClick()
    {
        if (InputManager.Instance.IsBlockingInput(InputKey.AlwaysEnabled))
            return;

        isSettingsOpen = settingsPanel.activeSelf;
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        if (isSettingsOpen)
        {
            EventManager.Instance.OnSettingsButtonOpen?.Invoke();
            outline.color = backOutlineColor;
        }
        else
        {
            EventManager.Instance.OnSettingsButtonClose?.Invoke();
            outline.color = defaultOutlineColor;
        }

        if (deactivateButtons != null && deactivateButtons.Count > 0)
        {
            foreach (GameObject button in deactivateButtons)
            {
                button.SetActive(!isSettingsOpen);
            }
        }
        GameManager.Instance.SetTime(isSettingsOpen ? 0f : 1f);
    }
}
