using JetBrains.Annotations;
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

    //[Header("Battle Speed Settings")] 
    //public float DefaultSpeedMultiplier = 1f;
    //public float MaxSpeedMultiplier = 2f;
    //public float CurrentSpeedMultiplier { get; set; }
    //public bool IsDefaultMult { get; set; } = true;
    //
    // This code block or the time scaling feature is disabled, because it cause inaccuracy, because the time from start coroutine wasn't scaled too.

    public GameMode Mode { get; set; }

    public string Name1 { get; set; }
    public string Name2 { get; set; }

    public int PlayerLives { get; set; }
    public int Timer { get; set; } = 0;

    public Game CurrentGame { get; set; }
    private Player[] players { get; set; }
    public string SceneName => SceneManager.GetActiveScene().name;
    public string SceneToLoad { get; set; }

    /// <summary>
    /// To block player's input while animation is running.
    /// </summary>
    public bool IsBlockingInput { get; set; } = false;

    public int PhaseShopIndex { get; set; } = 0;


    private SoundManager soundManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        soundManager = SoundManager.Instance;
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
                Switch(GameState.PlayCutScene);
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

        //// Load saved game.
        //var savedGame = SaveSystem.LoadGame(isNotSavingGame, GameMode.Local1v1);
        // if (savedGame != null)
        // {
        //     players[0].Data = savedGame.PlayerData1;
        //     players[1].Data = savedGame.PlayerData2;
        //     CurrentGame = savedGame;
        //     return;
        // }

        // Create a new game.
        players[0].Data = new PlayerData(Name1, PlayerLives, 0);
        players[1].Data = new PlayerData(Name2, PlayerLives, 0);

        CurrentGame = new Game(
                Mode,
                2,
                Timer,
                PlayerLives,
                0,
                GameState.PlayCutScene
                );

        // Set default speed multiplier for phase battle
        //CurrentSpeedMultiplier = DefaultSpeedMultiplier;
    }

    public void Switch(GameState _state)
    {
        if (CurrentGame != null)
            CurrentGame.State = _state;
        else
        {
            Debug.LogError("GameManager doesn't contain any instance of the current game.");
            return;
        }

        switch (_state)
        {
            case GameState.None:
                break;

            case GameState.PlayCutScene:
                IsBlockingInput = true;
                RunModeSingle();
                break;

            case GameState.WaitingSwitchScene:
                // Waiting for player input
                break;

            case GameState.LoadScene:
                Debug.Log("Loading Scene: " + SceneToLoad);
                SceneManager.LoadScene(SceneToLoad);
                break;

            case GameState.StartOfTurn:
                players[CurrentGame.CurrentPlayerIndex].StartShop();
                break;

            case GameState.ShopPhase:
                IsBlockingInput = false;
                break;

            case GameState.EndOfTurn:
                CurrentGame.CurrentPlayerIndex++;
                SaveSystem.SaveGame(CurrentGame);
                Destroy(PhaseShopUI.Instance.gameObject);
                Destroy(PhaseShopController.Instance.gameObject);
                Switch(GameState.PlayCutScene);
                break;

            case GameState.StartOfBattle:
                PhaseBattleController.Instance.Run(players[0], players[1]);
                break;

            case GameState.BattlePhase:
                break;

            case GameState.EndOfBattle:
                CurrentGame.CurrentPlayerIndex = 0;
                SceneToLoad = "PhaseShop";
                SaveSystem.SaveGame(CurrentGame);
                break;

            case GameState.EndOfGame:
                SceneToLoad = "Menu";
                SaveSystem.SaveGame(CurrentGame);
                Switch(GameState.WaitingSwitchScene);
                break;
        }
    }

    /// <summary>
    /// Starts game mode singleplayer.
    /// </summary>
    private void RunModeSingle()
    {
        if (CurrentGame.CurrentPlayerIndex < players.Length)
        {
            CutScene.Instance.SwitchScene("PhaseShop");

            Debug.Log("--------------- Phase Shop " + PhaseShopIndex + "----------------");
            PhaseShopIndex++;
        }
        else
        {
            CutScene.Instance.SwitchScene("PhaseBattle");

            Debug.Log("--------------- Phase Battle ----------------");
        }
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
