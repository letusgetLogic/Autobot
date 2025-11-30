using UnityEngine;
using UnityEngine.Events;

public class SettingsButton : MonoBehaviour
{
    public UnityAction OnChangedTimeScale { get; set; }

    [SerializeField] private GameObject settingsPanel;

    private bool isSettingsOpen = false;

    public void OnButtonClick()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        Time.timeScale = isSettingsOpen ? 0 : 1;

        OnChangedTimeScale?.Invoke();
    }
}
