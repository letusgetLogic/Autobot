using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    [SerializeField] private SettingsButton settingsButton;

    /// <summary>
    /// Button click calls.
    /// </summary>
    public void OnButtonClick()
    {
        settingsButton.OnContinueButtonClick();
    }
}

