using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [SerializeField]
    private int 
        maxHealth = 10,
        minHealth = 3,
        maxWins = 10,
        minWins = 3;

    public GameMode Mode { get; set; }
    public int PlayerCount { get; private set; }
    public bool WithTimer { get; private set; }
    public int Wins { get; private set; }
    public int PlayerHealth { get; private set; }

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

}

