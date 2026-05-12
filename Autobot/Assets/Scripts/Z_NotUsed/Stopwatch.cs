using TMPro;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    private float startTime;
    private bool running = false;

    private void Start()
    {
        StartTimer();   
    }

    private void Update()
    {
         timeText.text = GetElapsedTime().ToString("F3");
    }

    public void StartTimer()
    {
        startTime = Time.time;
        running = true;
    }
    public void StopTimer()
    {
        running = false;
    }
    public float GetElapsedTime()
    {
        if (running)
        {
            return Time.time - startTime;
        }
        return 0f;
    }
}
