using System.Collections;
using UnityEngine;

public class CustomBlinker : MonoBehaviour
{
    [Header("Target")]
    public GameObject targetObject; // The object to blink
    public Renderer targetRenderer; // Reference to the renderer

    [Header("Sequence Settings")]
    // Define your custom sequence here. 
    // 1.0 = Fully Visible, 0.0 = Fully Invisible.
    // The script will step through this array over time.
    public float[] alphaSequence = { 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f };

    [Header("Timing")]
    public float timePerStep = 0.1f; // Seconds spent on each alpha value
    public bool loop = true; // Whether to repeat the sequence indefinitely

    private int currentStep = 0;
    //private float stepTimer = 0f;

    private Coroutine coroutine;

    private void OnEnable()
    {
        if (targetObject == null) targetObject = gameObject;
        if (targetRenderer == null) targetRenderer = targetObject.GetComponent<Renderer>();

        // Start the sequence
        coroutine = StartCoroutine(BlinkSequence());
    }

    private void OnDisable()
    {
        coroutine = null;
    }

    private IEnumerator BlinkSequence()
    {
        while (true)
        {
            for (int i = 0; i < alphaSequence.Length; i++)
            {
                currentStep = i;
                // Set the renderer's alpha to the current step's value
                Color c = targetRenderer.material.color;
                c.a = alphaSequence[i];
                targetRenderer.material.color = c;

                // Wait for the specified time per step
                yield return new WaitForSeconds(timePerStep);
            }

            // If not looping, exit the coroutine
            if (!loop) break;
        }
    }
}   

