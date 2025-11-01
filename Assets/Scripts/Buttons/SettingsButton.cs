using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    private bool isSettingsOpen = false;

    public void OnButtonClick()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        Time.timeScale = isSettingsOpen ? 0 : 1;
    }
}
