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

    private float cooldown = 0f;
    private bool isEnabled = true;
    private bool isSetted = false;

    /// <summary>
    /// Button click calls.
    /// </summary>
    public void OnButtonClick()
    {
        if (isEnabled)
        {
            isEnabled = false;
            cooldown = 0.3f;
            isSetted = false;
        }
        else
            return;

        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        if (GameManager.Instance.CurrentRound != null)
        {
            rewatchButton.SetActive(!isSettingsOpen);
        }

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

    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        }
        else if (isSetted == false)
        {
            isEnabled = true;
            isSetted = true;
        }
    }
}
