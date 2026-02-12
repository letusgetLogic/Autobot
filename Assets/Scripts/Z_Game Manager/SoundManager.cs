using UnityEngine;

public class SoundManager
{
    #region Instance Lazy Loading
    public static SoundManager Instance
    {
        get
        {
            // Lazy loading
            if (instance == null)
            {
                instance = new SoundManager();
            }

            return instance;
        }
    }
    private static SoundManager instance;

    private SoundManager() 
    {
        EventManager.Instance.OnInvalidInput += () => PlayOneShot("Invalid");
        EventManager.Instance.OnCloseScene += () => PlayOneShot("Swap");
        EventManager.Instance.OnMoveHintClick += () => PlayOneShot("Swap");
        EventManager.Instance.OnOpenScene += () => PlayOneShot("Swap");

        EventManager.Instance.OnAttachedUnit += unit => 
        { 
            if (unit) 
                PlayOneShot("Attach_Unit"); 
        };
        EventManager.Instance.OnDropUnit += () => PlayOneShot("Drop_Unit"); 

        EventManager.Instance.OnRoll += () => PlayOneShot("Roll");
        EventManager.Instance.OnEndTurn += () => PlayOneShot("Button");

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
        EventManager.Instance.OnShootOut += () => PlayOneShot("Summon");
    }

    #endregion

    private void PlayOneShot(string eventPath)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/" + eventPath);
    }


    //public static SoundManager Instance { get; private set; }


    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;
    //    DontDestroyOnLoad(gameObject);

    //    //FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("event:/");
    //    //bus.setVolume(1f); 
    //}

    //private void OnEnable()
    //{
       
    //}

    //private void OnDisable()
    //{
    //    EventManager.Instance.OnCloseScene -= PlaySwapSound;
    //    EventManager.Instance.OnMoveHintClick -= PlaySwapSound;
    //    EventManager.Instance.OnOpenScene -= PlaySwapSound;

    //    EventManager.Instance.OnAttachedUnit -= PlayAttachUnit;
    //    EventManager.Instance.OnDropUnit -= PlayDropSound;

    //    EventManager.Instance.OnRoll -= PlayRollSound;
    //    EventManager.Instance.OnEndTurn -= PlayEndTurn;

    //    EventManager.Instance.OnCraft -= PlayBuySound;
    //    EventManager.Instance.OnRecycle -= PlaySellSound;

    //    EventManager.Instance.OnRepair -= PlayRepairSound;

    //    EventManager.Instance.OnLock -= PlayLockSound;
    //    EventManager.Instance.OnUnlock -= PlayUnlockSound;

    //    EventManager.Instance.OnFusion -= PlayFusionSound;
    //    EventManager.Instance.OnLevelUp -= PlayLevelUp;

    //    EventManager.Instance.OnSwap -= PlaySwapSound;

    //    EventManager.Instance.OnNotEnoughCurrency -= PlayInvalidInput;

    //    EventManager.Instance.OnAttack -= PlayCollideSound;
    //    EventManager.Instance.OnBuff -= PlayBuffSound;
    //    EventManager.Instance.OnSummon -= PlaySummonSound;
    //}

    //public void PlayAttachUnit(UnitController _unit)
    //{
    //    if (_unit != null)
    //    {
    //        FMODUnity.RuntimeManager.PlayOneShot("event:/Attach_Unit");
    //    }
    //}

    //public void PlayDropSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Drop_Unit");
    //}

    ////public void PlayCoinSound()
    ////{
    ////    FMODUnity.RuntimeManager.PlayOneShot("event:/Coin");
    ////}


    //public void PlayRollSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Roll");
    //}

    //public void PlayEndTurn()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Button");
    //}

    //public void PlayBuySound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Buy");
    //}

    //public void PlaySellSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Sell");
    //}

    //public void PlayRepairSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Repair");
    //}

    //public void PlayLockSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Lock");
    //}

    //public void PlayUnlockSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Unlock");
    //}

    //public void PlayFusionSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Fusion");
    //}

    //public void PlayLevelUp()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Level_Up");
    //}

    //public void PlaySwapSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Swap");
    //}

    //public void PlayInvalidInput()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Invalid");
    //}

    //public void PlayCollideSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Collide");
    //}

    //public void PlayBuffSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Buff");
    //}

    //public void PlaySummonSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Summon");
    //}

}
