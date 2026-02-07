using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnAttachedUnit += PlayAttachUnit;
        EventManager.Instance.OnDropUnit += PlayDropSound;

        EventManager.Instance.OnRoll += PlayRollSound;
        EventManager.Instance.OnEndTurn += PlayEndTurn;

        EventManager.Instance.OnCraft += PlayBuySound;
        EventManager.Instance.OnRecycle += PlaySellSound;

        EventManager.Instance.OnRepair += PlayRepairSound;

        EventManager.Instance.OnLock += PlayLockSound;
        EventManager.Instance.OnUnlock += PlayUnlockSound;

        EventManager.Instance.OnFusion += PlayFusionSound;
        EventManager.Instance.OnLevelUp += PlayLevelUp;

        EventManager.Instance.OnSwap += PlaySwapSound;

        EventManager.Instance.OnAttack += PlayCollideSound;
        EventManager.Instance.OnBuff += PlayBuffSound;
        EventManager.Instance.OnSummon += PlaySummonSound;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnAttachedUnit -= PlayAttachUnit;
        EventManager.Instance.OnDropUnit -= PlayDropSound;

        EventManager.Instance.OnRoll -= PlayRollSound;
        EventManager.Instance.OnEndTurn -= PlayEndTurn;

        EventManager.Instance.OnCraft -= PlayBuySound;
        EventManager.Instance.OnRecycle -= PlaySellSound;

        EventManager.Instance.OnRepair -= PlayRepairSound;

        EventManager.Instance.OnLock -= PlayLockSound;
        EventManager.Instance.OnUnlock -= PlayUnlockSound;

        EventManager.Instance.OnFusion -= PlayFusionSound;
        EventManager.Instance.OnLevelUp -= PlayLevelUp;

        EventManager.Instance.OnSwap -= PlaySwapSound;

        EventManager.Instance.OnAttack -= PlayCollideSound;
        EventManager.Instance.OnBuff -= PlayBuffSound;
        EventManager.Instance.OnSummon -= PlaySummonSound;
    }

    public void PlayAttachUnit(UnitController _unit)
    {
        if (_unit != null)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Attach_Unit");
        }
    }

    public void PlayDropSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Drop_Unit");
    }

    //public void PlayCoinSound()
    //{
    //    FMODUnity.RuntimeManager.PlayOneShot("event:/Coin");
    //}


    public void PlayRollSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Roll");
    }

    public void PlayEndTurn()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Button");
    }

    public void PlayBuySound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Buy");
    }

    public void PlaySellSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sell");
    }

    public void PlayRepairSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Repair");
    }

    public void PlayLockSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Lock");
    }

    public void PlayUnlockSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Unlock");
    }

    public void PlayFusionSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Fusion");
    }

    public void PlayLevelUp()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Level_Up");
    }

    public void PlaySwapSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Swap");
    }

    public void PlayCollideSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Collide");
    }

    public void PlayBuffSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Buff");
    }

    public void PlaySummonSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Summon");
    }
}
