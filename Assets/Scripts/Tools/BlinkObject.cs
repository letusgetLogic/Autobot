using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    public GameObject targetObject;
    public float blinkInterval = 0.5f;

    public void Play()
    {
        InvokeRepeating("ToggleBlink", 0f, blinkInterval);
    }

    void ToggleBlink()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }

    // Optional: Stop blinking, ex. for interaction.
    void StopBlinking()
    {
        CancelInvoke("ToggleBlink");
    }
}