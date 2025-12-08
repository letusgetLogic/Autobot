using System;

public class SoundManager
{
    #region Instance 
    public static SoundManager Instance
    {
        get
        {
            // Lazy loading
            if (instance == null)
                instance = new SoundManager();

            return instance;
        }
    }
    private static SoundManager instance;

    private SoundManager() { }

    #endregion

    public void PlayRollSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Roll");
    }

    public void PlayCoinSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Coin");
    }

    public void PlayButtonSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Button");
    }

    public void PlayFusionSound()
    {
        bool rnd = Convert.ToBoolean(new Random().Next(2));
        FMODUnity.RuntimeManager.PlayOneShot(
            rnd == true ? "event:/Fusion_1" : "event:/Fusion_2");
    }

    public void PlayLockSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Fusion_1");
    }

    public void PlayUnlockSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Fusion_2");
    }

    public void PlayOnDropSound()
    {
        bool rnd = Convert.ToBoolean(new Random().Next(2));
        FMODUnity.RuntimeManager.PlayOneShot(
            rnd == true ? "event:/Fusion_1" : "event:/Fusion_2");
    }



}
