using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Develop Settings")]
    [SerializeField] private bool isNotSavingGame;
    [SerializeField] public bool IsRepairSystemActive;

    [Header("Battle Speed Settings")]
    public float DefaultSpeedMultiplier = 1f;
    public float MaxSpeedMultiplier = 2f;
    public float CurrentSpeedMultiplier { get; set; }
    public bool IsDefaultMult { get; set; } = true;


    public GameMode Mode { get; set; }

    public string Name1 { get; set; }
    public string Name2 { get; set; }

    public int PlayerLives { get; set; }
    public int Timer { get; set; } = 0;

    public Game CurrentGame { get; set; }
    private Player[] players { get; set; }
    public string SceneName => SceneManager.GetActiveScene().name;

    /// <summary>
    /// To block player's input while animation is running.
    /// </summary>
    public bool IsBlockingInput { get; set; } = false;
    public int PhaseShopIndex { get; set; } = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    /// <summary>
    /// Loads game.
    /// </summary>
    public void LoadGame()
    {
        switch (Mode)
        {
            case GameMode.None:
                break;

            case GameMode.Local1v1:
                InitSingle();
                Switch(GameState.LoadScene, null);
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
        //var savedGame = SaveSystem.LoadGame(isNotSavingGame, GameMode.Local1v1);
        //if (savedGame != null)
        //{
        //    players[0].Data = savedGame.PlayerData1;
        //    players[1].Data = savedGame.PlayerData2;
        //    CurrentGame = savedGame;
        //    return;
        //}

        // Create a new game.
        players[0].Data = new PlayerData(Name1, PlayerLives, 0);
        players[1].Data = new PlayerData(Name2, PlayerLives, 0);

        CurrentGame = new Game(
                Mode,
                2,
                Timer,
                PlayerLives,
                0,
                GameState.LoadScene,
                players[0].Data,
                players[1].Data
                );

        // Set default speed multiplier for phase battle
        CurrentSpeedMultiplier = DefaultSpeedMultiplier;
    }

    public void Switch(GameState _state, Player _player)
    {
        if (CurrentGame != null)
            CurrentGame.State = _state;

        switch (_state)
        {
            case GameState.None:
                break;

            case GameState.LoadScene:
                IsBlockingInput = true;
                RunModeSingle();
                break;

            case GameState.StartOfTurn:
                _player.StartShop();
                break;

            case GameState.ShopPhase:
                GameManager.Instance.IsBlockingInput = false;
                break;

            case GameState.EndOfTurn:
                SaveSystem.SaveGame(CurrentGame);
                Destroy(PhaseShopUI.Instance.gameObject);
                Destroy(PhaseShopController.Instance.gameObject);
                StartCoroutine(DelayLoadScene());
                break;

            case GameState.StartOfBattle:
                PhaseBattleController.Instance.Run(players[0], players[1]);
                break;

            case GameState.BattlePhase:
                break;

            case GameState.EndOfBattle:
                SaveSystem.SaveGame(CurrentGame);
                Switch(GameState.LoadScene, null);
                break;

            case GameState.EndOfGame:
                if (PackManager.Instance != null)
                    Destroy(PackManager.Instance.gameObject);
                if (SpawnManager.Instance != null)
                    Destroy(SpawnManager.Instance.gameObject);

                break;
        }
    }

    private IEnumerator DelayLoadScene()
    {
        yield return new WaitForEndOfFrame();

        Switch(GameState.LoadScene, null);
    }

    /// <summary>
    /// Starts game mode singleplayer.
    /// </summary>
    private void RunModeSingle()
    {
        if (CurrentGame.CurrentPlayerIndex < players.Length)
        {
            CutScene.Instance.SwitchScene("PhaseShop");
            StartCoroutine(StartTurn(players[CurrentGame.CurrentPlayerIndex]));
            CurrentGame.CurrentPlayerIndex++;
        }
        else
        {
            CurrentGame.CurrentPlayerIndex = 0;
            CutScene.Instance.SwitchScene("PhaseBattle");
            StartCoroutine(RunPhaseBattle());
        }
    }


    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    private IEnumerator StartTurn(Player _player)
    {
        yield return new WaitUntil(() =>
            PhaseShopController.Instance != null &&
            PhaseShopUI.Instance != null);

        Switch(GameState.StartOfTurn, _player);
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

        Switch(GameState.StartOfBattle, null);
    }

    /// <summary>
    /// 0 = draw, 1 = right wins, -1 = left wins.
    /// </summary>
    /// <param name="_outcome"></param>
    public void UpdatePlayerStats(int _outcome)
    {
        switch (_outcome)
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
}
