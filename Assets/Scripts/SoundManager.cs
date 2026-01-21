using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {  get; private set; }


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
        EventManager.Instance.OnTransportUnit += PlayFusionSound;
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
        int rnd = Random.Range(0, 2);
        FMODUnity.RuntimeManager.PlayOneShot(
            rnd == 0 ? "event:/Fusion_1" : "event:/Fusion_2");
    }

    public void PlayLockSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Lock");
    }

    public void PlayUnlockSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Unlock");
    }

    public void PlayOnDropSound()
    {
        int rnd = Random.Range(0, 2);
        FMODUnity.RuntimeManager.PlayOneShot(
            rnd == 0 ? "event:/Fusion_1" : "event:/Fusion_2");
    }



}
