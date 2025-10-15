using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Mode Development")]

    [SerializeField]
    private int setHealth = 3;
    [SerializeField]
    private int setWins = 5;
    [SerializeField]
    private bool isDeveloper;

    private GameState state;
    private Template[] templates;

    [HideInInspector]
    public List<SoUnit> AvaiableUnits = new();
    [HideInInspector]
    public List<SoItem> AvaiableItems = new();

    public GameMode Mode { get; set; }
    public int PlayerAmount { get; private set; }
    public int PlayerCount { get; private set; }
    public bool WithTimer { get; private set; }
    public int WinsCondition { get; set; }
    public int PlayerHealth { get; set; }

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this; 
            DontDestroyOnLoad(this);
        }
        if (isDeveloper)
        {
            LoadGame();
        }
    }

    /// <summary>
    /// Loads game.
    /// </summary>
    public void LoadGame()
    {
        state = GameState.LoadGame;

        switch (Mode)
        {
            case GameMode.None:
                Mode = GameMode.Single;
                PlayerHealth = setHealth;
                WinsCondition = setWins;
                LoadGame();
                break;

            case GameMode.Single:
                PlayerCount = 0;
                PlayerAmount = 2;
                templates = new[]
                    {
                    new Template("Player 1", PlayerHealth, WinsCondition),
                    new Template("Player 2", PlayerHealth, WinsCondition)
                    };
                RunSingle();
                break;

            case GameMode.AI:
                // Load AI game mode
                break;
            case GameMode.Friends:
                // Load Versus game mode
                break;
        }
    }

    /// <summary>
    /// Starts game mode single.
    /// </summary>
    private void RunSingle()
    {
        state = GameState.StartOfTurn;
        PlayerCount++;

        if (PlayerCount <= templates.Length)
        {
            SceneManager.LoadScene("PhaseShop");
            StartCoroutine(StartTurn(templates[PlayerCount - 1]));
        }
        else
        {
            SceneManager.LoadScene("PhaseBattle");
            StartCoroutine(RunPhaseBattle());
        }
    }

    /// <summary>
    /// Waits until the <see cref="PhaseShopUnitManager.Instance"/> is initialized and then transitions the game state to the start
    /// of the turn.
    /// </summary>
    private IEnumerator StartTurn(Template _template)
    {
        yield return new WaitUntil(() =>
            PhaseShopUnitManager.Instance != null &&
            PhaseShopUI.Instance != null &&
            StarterPack.Instance != null);
       
        state = GameState.StartOfTurn;
        _template.StartShop();
    }

    /// <summary>
    /// Sets the shop phase.
    /// </summary>
    public void SetShopPhase()
    {
        state = GameState.ShopPhase;
    }

    /// <summary>
    /// Ends the phase of shop.
    /// </summary>
    public void EndShopPhase(Slot[] battleSlots, Slot[] shopUnitSlots)
    {
        foreach (Slot slot in battleSlots)
        {
            slot.transform.position = new Vector3(1000, 1000);
            slot.transform.SetParent(transform);
        }
        foreach (Slot slot in shopUnitSlots)
        {
            slot.transform.position = new Vector3(1000, 1000);
            slot.transform.SetParent(transform);
        }
        state = GameState.EndOfTurn;
        RunSingle();
    }

    private IEnumerator RunPhaseBattle()
    {
        yield return new WaitUntil(() => PhaseBattle.Instance != null);

        state = GameState.StartOfBattle;
        PhaseBattle.Instance.Initialize(templates[0], templates[1]);
    }

    public void UpdatePlayerStats(int outcome)
    {

    }

    public void EndGame()
    {
        state = GameState.EndOfGame;
        AvaiableUnits.Clear();
        AvaiableItems.Clear();
    }

}
