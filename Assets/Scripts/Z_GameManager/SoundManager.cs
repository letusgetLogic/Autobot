
public class SoundManager
{
    #region Instance Lazy Loading
    public static SoundManager Instance
    {
        get
        {
            // Lazy loading
            if (_Instance == null)
            {
                _Instance = new SoundManager();
            }

            return _Instance;
        }
    }
    private static SoundManager _Instance;
    #endregion

    /// <summary>
    /// Constructor of SoundManager, subscribes to events.
    /// </summary>
    private SoundManager() 
    {
        EventManager.Instance.OnButtonSound += () => PlayOneShot("Button");

        EventManager.Instance.OnIncreaseLives += () => PlayOneShot("Button");
        EventManager.Instance.OnDecreaseLives += () => PlayOneShot("Button");

        EventManager.Instance.OnInvalidInput += () => PlayOneShot("Invalid");
        EventManager.Instance.OnCloseScene += () => PlayOneShot("Swap");
        EventManager.Instance.OnMoveHintClick += () => PlayOneShot("Swap");
        EventManager.Instance.OnOpenScene += () => PlayOneShot("Swap");

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

        EventManager.Instance.OnCraft += () => PlayOneShot("Buy");
        EventManager.Instance.OnRecycle += () => PlayOneShot("Sell");

        EventManager.Instance.OnRepair += () => PlayOneShot("Repair");

        EventManager.Instance.OnLock += () => PlayOneShot("Lock");
        EventManager.Instance.OnUnlock += () => PlayOneShot("Unlock");

        EventManager.Instance.OnFusion += () => PlayOneShot("Fusion");
        EventManager.Instance.OnLevelUp += () => PlayOneShot("Level_Up");

        EventManager.Instance.OnSwap += () => PlayOneShot("Swap");

        EventManager.Instance.OnNotEnoughCurrency += () => PlayOneShot("Invalid");

        EventManager.Instance.OnHurt += () => PlayOneShot("Collide");
        EventManager.Instance.OnBuff += () => PlayOneShot("Buff");
        EventManager.Instance.OnShootOut += (unit) => PlayOneShot("Summon");

        EventManager.Instance.OnGameOver += () => PlayOneShot("Game_Over");
    }

    public void PlayOneShot(string eventPath)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/" + eventPath);
    }
}
