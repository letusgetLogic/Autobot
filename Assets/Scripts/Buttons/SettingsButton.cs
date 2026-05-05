using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject rewatchButton;
    [SerializeField] private List<GameObject> deactivateButtons;
    [SerializeField] private Image outline;
    private Color defaultOutlineColor => new Color(0, 0, 0);
    private Color backOutlineColor => new Color(0.9568627f, 0.4862745f, 0.4823529f);

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
            outline.color = backOutlineColor;
        else
            outline.color = defaultOutlineColor;

        if (deactivateButtons != null && deactivateButtons.Count > 0)
        {
            foreach (GameObject button in deactivateButtons)
            {
                button.SetActive(!isSettingsOpen);
            }
        }

        Time.timeScale = isSettingsOpen ? 0f : (PhaseBattleController.Instance.IsStopped ? 0f : 1f);
    }
}
