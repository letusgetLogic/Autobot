using UnityEngine;

public class Catalog : MonoBehaviour
{
    public static Catalog Instance { get; private set; }

    public SoUnit[] Tier1 = null;
    public SoUnit[] Tier2 = null;
    public SoUnit[] Tier3 = null;
    public SoUnit[] Tier4 = null;
    public SoUnit[] Tier5 = null;
    public SoUnit[] Tier6 = null;

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}

