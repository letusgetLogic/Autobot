
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                Debug.LogWarning("SoundManager instance is null.");
            }
            return _Instance;
        }
    }
    private static SoundManager _Instance;


    [SerializeField] 
    [Range(0f, 1f)] private float defaultVolumeValue = 0.5f;

    private bool setDefaultOnce;
    private Bus masterBus;

    private void Awake()
    {
        Debug.Log(this.name + ".Awake()");

        if (_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        // Retrieve the master bus using the standard path
        masterBus = RuntimeManager.GetBus("bus:/");
        SetMasterVolume(defaultVolumeValue);

        EventManager.Instance.OnButtonSound += () => PlayOneShot("Button");
        EventManager.Instance.OnPopUpSound += () => PlayOneShot("Drop_Unit");

        EventManager.Instance.OnIncreaseLives += () => PlayOneShot("Button");
        EventManager.Instance.OnDecreaseLives += () => PlayOneShot("Button");

        EventManager.Instance.OnInvalidInput += () => PlayOneShot("Invalid");
        EventManager.Instance.OnCloseSceneSound += () => PlayOneShot("Swap");
        EventManager.Instance.OnMoveHintClickSound += () => PlayOneShot("Swap");
        EventManager.Instance.OnOpenSceneSound += () => PlayOneShot("Swap");

        EventManager.Instance.OnAttachedUnit += unit => 
        { 
            if (unit) 
                PlayOneShot("Attach_Unit"); 
        };

        EventManager.Instance.OnAttachedUnitCatalog += unit => 
        { 
            if (unit) 
                PlayOneShot("Attach_Unit"); 
        };
        EventManager.Instance.OnDropUnit += () => PlayOneShot("Drop_Unit"); 

        EventManager.Instance.OnRoll += () => PlayOneShot("Roll");

        EventManager.Instance.OnCraft += inputKey => PlayOneShot("Buy");
        EventManager.Instance.OnRecycleSound += inputKey => PlayOneShot("Sell");

        EventManager.Instance.OnRepair += inputKey => PlayOneShot("Repair");

        EventManager.Instance.OnLock += inputKey => PlayOneShot("Lock");
        EventManager.Instance.OnUnlock += inputKey => PlayOneShot("Unlock");

        EventManager.Instance.OnFusion += () => PlayOneShot("Fusion");
        EventManager.Instance.OnLevelUpSound += () => PlayOneShot("Level_Up");

        EventManager.Instance.OnSwap += () => PlayOneShot("Swap");

        EventManager.Instance.OnNotEnoughCurrency += () => PlayOneShot("Invalid");

        EventManager.Instance.OnHurt += () => PlayOneShot("Collide");
        EventManager.Instance.OnBuff += () => PlayOneShot("Buff");
        EventManager.Instance.OnShootOut += (unit) => PlayOneShot("Summon");

        EventManager.Instance.OnBattleOverSound += () => PlayOneShot("Game_Over");
        EventManager.Instance.OnGameOverSound += () => PlayOneShot("Game_Over");
    }

    public void PlayOneShot(string _eventPath)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/" + _eventPath);
    }


    // Call this method from a UI slider
    public void SetMasterVolume(float _linearVolume)
    {
        // Ensure the value is clamped between 0 and 1
        _linearVolume = Mathf.Clamp01(_linearVolume);
        masterBus.setVolume(_linearVolume);
    }

    public float GetSliderValue()
    {
        if (setDefaultOnce == false)
        {
            setDefaultOnce = true;
            return defaultVolumeValue;
        }

        // Retrieve the current volume
        float currentVolume = 0f;
        masterBus.getVolume(out currentVolume);
        return currentVolume;
    }
}
