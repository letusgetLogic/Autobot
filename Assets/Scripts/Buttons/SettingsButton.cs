using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsButton : MonoBehaviour
{
    public UnityAction OnChangedTimeScale { get; set; }

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject rewatchButton;
    [SerializeField] private List<GameObject> deactivateButtons;

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

        if (deactivateButtons != null && deactivateButtons.Count > 0)
        {
            foreach (GameObject button in deactivateButtons)
            {
                button.SetActive(!isSettingsOpen);
            }
        }

        Time.timeScale = isSettingsOpen ? 0 : 1;

        OnChangedTimeScale?.Invoke();
    }
}
