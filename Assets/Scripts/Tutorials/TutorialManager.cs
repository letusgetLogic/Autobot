using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private List<StepRunner> test;   
    

    public bool TutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        set => PlayerPrefs.SetInt("TutorialCompleted", value ? 1 : 0);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
       
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            TutorialCompleted = false;
        }
    }

    /// <summary>
    /// Sets integer value of PlayerPrefs.
    /// </summary>
    /// <param name="KeyName"></param>
    /// <param name="Value"></param>
    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }

    /// <summary>
    /// Gets integer value of PlayerPrefs.
    /// </summary>
    /// <param name="KeyName"></param>
    /// <returns></returns>
    public int Getint(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }
}

public class StepRunner : MonoBehaviour
{
    [SerializeField] private GameObject[] Label;
    [SerializeField] private GameObject[] Hint;
    [SerializeField] private float delay = 0f;
}

