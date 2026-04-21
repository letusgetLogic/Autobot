using UnityEditor;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    public GameObject targetObject;
    public float blinkInterval = 0.5f;

    public void OnEnable()
    {
        InvokeRepeating(nameof(ToggleBlink), 0f, blinkInterval);
    }

    private void OnDisable()
    {
        targetObject.SetActive(false);
        StopBlinking();
    }

    void ToggleBlink()
    {
        if (gameObject.activeSelf == false)
            return;

        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }

    // Optional: Stop blinking, ex. for interaction.
    void StopBlinking()
    {
        CancelInvoke(nameof(ToggleBlink));
    }
}
