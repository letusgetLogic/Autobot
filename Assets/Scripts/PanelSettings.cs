using UnityEngine;
using UnityEngine.UI;

public class PanelSettings : MonoBehaviour
{
    [Header("Mode Mobile")]
    [SerializeField] private Toggle modeMobile;

    [Header("Audio Settings")]
    [SerializeField] private Slider sfxSlider;


    private void OnEnable()
    {
        if (modeMobile != null)
        {
            modeMobile.isOn = GameManager.Instance.IsMobile;
        }
        sfxSlider.value = SoundManager.Instance.GetSliderValue();
    }

    public void OnModeMobile()
    {
        GameManager.Instance.SetIsMobile(modeMobile.isOn);
    }

    public void OnSFXSliderValueChanged(float _value)
    {
        SoundManager.Instance.SetMasterVolume(_value);
    }
}