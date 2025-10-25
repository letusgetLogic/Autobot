using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Mode Development")]

    [SerializeField] private bool isDeveloper;
    [SerializeField] private bool deleteSaveGame;

    private string name1 = "Player 1";
    private string name2 = "Player 2";

    [SerializeField] private int lives = 6;
    [SerializeField] private float timer = 90.0f;
    public int StartCoins => startCoins;
    [SerializeField] private int startCoins = 10;
    public int RollCost => rollCost;
    [SerializeField] private int rollCost = 1;

    public Game CurrentGame { get; set; }
    private Player[] players { get; set; }
    public string SceneName => SceneManager.GetActiveScene().name;
    public bool IsPhaseBattle
    {
        get
        {
            if (SceneName == "PhaseBattle")
                return true;

            return false;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        if (isDeveloper)
        {
            LoadGame(GameMode.Single);
        }
    }

    /// <summary>
    /// Loads game.
    /// </summary>
    public void LoadGame(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.None:
                break;

            case GameMode.Single:
                InitSingle();
                RunModeSingle();
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
    /// Initialize game with mode Singleplayer.
    /// </summary>
    private void InitSingle()
    {
        // Initialize player monobehaviours.
        players = new Player[2];
        players[0] = gameObject.AddComponent<Player>();
        players[1] = gameObject.AddComponent<Player>();

        // Load saved game.
        var savedGame = SaveSystem.LoadGame(deleteSaveGame, GameMode.Single);
        if (savedGame != null)
        {
            players[0].Data = savedGame.PlayerData1;
            players[1].Data = savedGame.PlayerData2;
            CurrentGame = savedGame;
            return;
        }

        // Create a new game.
        players[0].Data = new PlayerData(name1, lives, 0);
        players[1].Data = new PlayerData(name2, lives, 0);

        CurrentGame = new Game(
                GameMode.Single,
                2,
                timer,
                lives,
                0,
                GameState.LoadGame,
                players[0].Data,
                players[1].Data
                );

    }

    /// <summary>
    /// Starts game mode singleplayer.
    /// </summary>
    public void RunModeSingle()
    {
        if (CurrentGame.CurrentPlayerIndex < players.Length)
        {
            SceneManager.LoadScene("PhaseShop");
            StartCoroutine(StartTurn(players[CurrentGame.CurrentPlayerIndex]));
            CurrentGame.CurrentPlayerIndex++;
        }
        else
        {
            CurrentGame.CurrentPlayerIndex = 0;
            SceneManager.LoadScene("PhaseBattle");
            StartCoroutine(RunPhaseBattle());
        }
    }


    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    private IEnumerator StartTurn(Player player)
    {
        yield return new WaitUntil(() =>
            PhaseShopUnitManager.Instance != null &&
            PhaseShopUI.Instance != null);

        CurrentGame.State = GameState.StartOfTurn;

        player.StartShop();
    }

    /// <summary>
    /// Sets the phase of shop.
    /// </summary>
    public void SetPhaseShop()
    {
        CurrentGame.State = GameState.ShopPhase;
    }

    /// <summary>
    /// Ends the phase of shop.
    /// </summary>
    public void EndPhaseShop()
    {
        CurrentGame.State = GameState.EndOfTurn;
        SaveSystem.SaveGame(CurrentGame);
        RunModeSingle();
    }

    /// <summary>
    /// Runs the phase battle.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunPhaseBattle()
    {
        yield return new WaitUntil(() =>
        PhaseBattleController.Instance != null &&
        PhaseBattleView.Instance != null
        );

        CurrentGame.State = GameState.StartOfBattle;
        PhaseBattleController.Instance.Run(players[0], players[1]);
    }

    public void EndPhaseBattle()
    {
        CurrentGame.State = GameState.EndOfBattle;
        SaveSystem.SaveGame(CurrentGame);
        RunModeSingle();
    }

    /// <summary>
    /// 0 = draw, 1 = right wins, -1 = left wins.
    /// </summary>
    /// <param name="outcome"></param>
    public void UpdatePlayerStats(int outcome)
    {
        switch (outcome)
        {
            case 0:
                break;
            case -1:
                players[1].Data.Lives--;
                break;
            case 1:
                players[0].Data.Lives--;
                break;
        }
    }

    /// <summary>
    /// Ends game.
    /// </summary>
    public void EndGame()
    {
        CurrentGame.State = GameState.EndOfGame;

        SceneManager.LoadScene("Menu");
        LoadGame(GameMode.Single);
    }

}
