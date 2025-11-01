using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public TMP_InputField ModeSingleHeart;

    [Header("Mode Development")]
    public bool IsModeDevelop;
    
    [SerializeField] private int lives = 6;
    [SerializeField] private float timer = 90.0f;

    private string name1 = "Player 1";
    private string name2 = "Player 2";

    [SerializeField] private int startCoins = 10;
    [SerializeField] private int rollCost = 1;

    [SerializeField] private Pack[] packs;
    [SerializeField] private int minLives = 3;
    [SerializeField] private int maxLives = 10;

    [Header("Settings")]
    [SerializeField] private float durationColorDefault = 0.2f;

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

        GameManager.Instance.StartCoins = startCoins;
        GameManager.Instance.RollCost = rollCost;
    }

    private void Start()
    {
        if (IsModeDevelop)
        {
            PackManager.Instance.InitPack(packs[0].SoPack);
            GameManager.Instance.Mode = GameMode.Single;
            GameManager.Instance.Name1 = name1;
            GameManager.Instance.Name2 = name2;
            GameManager.Instance.PlayerLives = lives;
        
            GameManager.Instance.LoadGame();
        }
    }

    public void OnModeSingle()
    {
        GameManager.Instance.Mode = GameMode.Single;
    }


    public void UnCheckAll()
    {
        for (int i = 0; i < packs.Length; i++)
        {
            packs[i].UnCheck();
        }
    }



    /// <summary>
    /// Start game with selected settings.  
    /// </summary>
    public void StartGame()
    {
        switch (GameManager.Instance.Mode)
        {
            case GameMode.Single:

                if (PackManager.Instance.MyPack == null)
                {
                    // Hint to select a pack.
                    return;
                }

                int b = int.Parse(ModeSingleHeart.text);
                if (b < minLives || b > maxLives)
                {
                    HintInvalid(ModeSingleHeart);
                    return;
                }

                GameManager.Instance.PlayerLives = b;
                GameManager.Instance.LoadGame();

                break;
        }
    }


    public void HintInvalid(TMP_InputField target)
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(target, durationColorDefault);
    }


}

