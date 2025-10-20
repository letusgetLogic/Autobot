using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Mode Development")]

    [SerializeField]
    private bool isDeveloper;

    private GameData gameData { get; set; }
    private Game currentGame { get; set; }
    public Game CurrentGame => currentGame;
    private Player[] players { get; set; }


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
    /// Starts game mode singleplayer.
    /// </summary>
    public void RunModeSingle()
    {
        var playerDatas = new PlayerData[]
                {
                new PlayerData("Player 1", 6, 0),
                new PlayerData("Player 2", 6, 0)
                };
        currentGame = new Game(
                GameMode.Single,
                2,
                90,
                6,
                0,
                GameState.LoadGame,
                playerDatas[0],
                playerDatas[1]
                );
        players[0].Data = playerDatas[0];
        players[1].Data = playerDatas[1];

        if (currentGame.CurrentPlayerIndex < players.Length)
        {
            SceneManager.LoadScene("PhaseShop");
            StartCoroutine(StartTurn(players[currentGame.CurrentPlayerIndex]));
            currentGame.CurrentPlayerIndex++;
        }
        else
        {
            currentGame.CurrentPlayerIndex = 0;
            SceneManager.LoadScene("PhaseBattle");
            StartCoroutine(RunPhaseBattle());
        }
    }

    /// <summary>
    /// Waits until the <see cref="PhaseShopUnitManager.Instance"/> is initialized and then transitions the game state to the start
    /// of the turn.
    /// </summary>
    private IEnumerator StartTurn(Player player)
    {
        yield return new WaitUntil(() =>
            PhaseShopUnitManager.Instance != null &&
            PhaseShopUI.Instance != null &&
            StarterPack.Instance != null);

        currentGame.State = GameState.StartOfTurn;
        gameData = new GameData(currentGame);
        player.StartShop();
    }

    /// <summary>
    /// Sets the shop phase.
    /// </summary>
    public void SetShopPhase()
    {
        currentGame.State = GameState.ShopPhase;
    }

    /// <summary>
    /// Ends the phase of shop.
    /// </summary>
    public void EndShopPhase(UnitController[] battleUnits, UnitController[] frezzedUnits)
    {
        currentGame.State = GameState.EndOfTurn;
        RunModeSingle();
    }


    private IEnumerator RunPhaseBattle()
    {
        yield return new WaitUntil(() =>
        PhaseBattleController.Instance != null &&
        PhaseBattleView.Instance != null
        );

        currentGame.State = GameState.StartOfBattle;
        PhaseBattleController.Instance.Run(players[0], players[1]);
    }

    /// <summary>
    /// 0 = draw, 1 = right wins, -1 = left wins.
    /// </summary>
    /// <param name="outcome"></param>
    public void UpdatePlayerStats(int outcome)
    {
        players[1].Data.Lives += outcome;
        players[0].Data.Lives += -outcome;
    }

    public void EndGame()
    {
        currentGame.State = GameState.EndOfGame;

        SceneManager.LoadScene("Menu");
    }

}
