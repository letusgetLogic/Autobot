using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                if (Application.isPlaying)
                    SceneManager.LoadScene("Menu");

                Debug.LogWarning("GameManager instance is null.");
            }
            return _Instance;
        }
    }
    private static GameManager _Instance;


    [Header("Develop Settings")]
    public bool IsModeDevelop;
    [SerializeField] private bool shouldPlayTutorial;
    [SerializeField] private bool isNotSavingGame;
    [SerializeField] public bool IsRepairSystemActive;
    [SerializeField] public bool TestBattle;
    [SerializeField] private int defaultTutorialLives = 3;
    [SerializeField] private int devLives = 3;
    [SerializeField] private float timer = 90.0f;

    // This code block or the time scaling feature is disabled,
    // because it cause inaccuracy, when the time from start coroutine wasn't also scaled.
    //
    //[Header("Battle Speed Settings")] 
    //public float DefaultSpeedMultiplier = 1f;
    //public float MaxSpeedMultiplier = 2f;
    //public float CurrentSpeedMultiplier { get; set; }
    //public bool IsDefaultMult { get; set; } = true;
    //---------------------------------------------------

    // GameSettings set those variables, to initialize in the next scene.
    public GameMode Mode { get; set; }
    public string Name1 { get; set; } = "Player 1";
    public string Name2 { get; set; } = "Player 2";
    public int PlayerLives { get; set; }
    public int Timer { get; set; } = 0;
    //

    #region Reference Datas
    /// <summary>
    /// Contains the game datas.
    /// </summary>
    public Game CurrentGame
    {
        get
        {
            if (currentGame == null)
            {
                Debug.LogWarning("currentGame is " + currentGame);
                return null;
            }
            return currentGame;
        }
    }
    private Game currentGame;

    /// <summary>
    /// References the current round.
    /// </summary>
    public SavedRoundData CurrentRound
    {
        get
        {
            if (currentRound == null)
            {
                Debug.LogWarning("currentRound is null");
                return null;
            }
            return currentRound;
        }
    }
    private SavedRoundData currentRound;

    /// <summary>
    /// References the current players.
    /// </summary>
    public List<Player> Players
    {
        get
        {
            if (players == null)
            {
                Debug.LogWarning("The list of players is " + players);
                return null;
            }
            return players;
        }
    }
    private List<Player> players;

    public Player CurrentPlayer { get; set; }
    //public bool IsCatalogActive { get; set; } = false;

    // Lazy Loading is initialized once to create and hold an instance.
    private SoundManager sound => SoundManager.Instance;
    private InputManager input => InputManager.Instance;

    #endregion


    public string SceneName => SceneManager.GetActiveScene().name;
    public string SceneToLoad { get; set; }

    public bool IsCatalogActive { get; set; } = false;
    public ReplayManager Replay { get; set; }

    private bool isTutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        set => PlayerPrefs.SetInt("TutorialCompleted", value ? 1 : 0);
    }
    public bool IsTutorialRunning => !isTutorialCompleted;

    public int RandomSeed
    {
        get
        {
            if (PhaseBattleController.Instance != null)
                return randomSeed;

            return new System.Random().Next(0, 100);
        }
    }
    private int randomSeed = 0;


    #region Debug Variables
    public int PhaseShopIndex { get; set; } = 0;
    public int ClickIndex { get; set; } = 0;

    #endregion


    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _Instance = this;

        DontDestroyOnLoad(gameObject);

        if (sound != null && input != null)
        {
            // this if-query is used to initialize instances once.
            // for example, button's sound wouldn't be triggered,
            // because until then no one has accessed/initalized SoundManager's instence.
        }
        ;
    }

    private void Start()
    {
        PackManager.Instance.InitPack(GameSettings.Instance.DefaultPack);

        if (!PlayerPrefs.HasKey("TutorialCompleted"))
            isTutorialCompleted = false;

        // auto play in dev mode.
        if (IsModeDevelop)
        {
            PlayerLives = devLives;

            if (shouldPlayTutorial == false)
            {
                isTutorialCompleted = true;

                if (TestBattle)
                {
                    LoadGame(GameMode.TestBattle);
                    return;
                }

                LoadGame(GameMode.Local1v1);
            }
            else
            {
                isTutorialCompleted = false;
            }
        }
        isTutorialCompleted = false;
    }

    /// <summary>
    /// Loads game.
    /// </summary>
    public void LoadGame(GameMode _mode)
    {
        Mode = _mode;

        switch (Mode)
        {
            case GameMode.None:
                break;

            case GameMode.TestBattle:
                InitTest();
                Switch(GameState.StartScene);
                break;

            case GameMode.Tutorial:
                isTutorialCompleted = false;
                PlayerLives = defaultTutorialLives;
                InitTutorial();
                Switch(GameState.StartScene);
                break;

            case GameMode.Local1v1:
                InitPvP();
                Switch(GameState.StartScene);
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
    /// Switches the game state and performs actions based on the new state.
    /// </summary>
    /// <param name="_state"></param>
    public void Switch(GameState _state)
    {
        if (CurrentGame == null)
        {
            Debug.LogError("GameManager doesn't contain any instance of the current game.");
            return;
        }

        var prevState = CurrentGame.State;
        CurrentGame.State = _state;
        Debug.Log(_state.ToString());
        switch (_state)
        {
            case GameState.None:
                break;

            case GameState.StartScene:
                input.BlocksInput = true;
                StartScene();
                break;

            case GameState.PlayCutSceneShop:
                CutScene.Instance.SetHintClick(CurrentPlayer.Data.Name, false);
                CutScene.Instance.SwitchScene();
                break;

            case GameState.PlayCutSceneBattle:
                CutScene.Instance.SetHintClick("", true);
                CutScene.Instance.SwitchScene();
                break;

            case GameState.WaitingCutScene:
                input.BlocksInput = false;
                // Waiting for player input
                CurrentGame.State = prevState;
                break;

            case GameState.WaitingEndOfBattle:
                input.BlocksInput = false;
                EventManager.Instance.OnBattleDelayHintClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.WaitingEndOfGame:
                input.BlocksInput = false;
                EventManager.Instance.OnBattleDelayHintClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.LoadScene:
                switch (prevState)
                {
                    case GameState.PlayCutSceneShop:
                        LoadScene("PhaseShop");
                        break;
                    case GameState.PlayCutSceneBattle:
                        LoadScene("PhaseBattle");
                        break;
                    case GameState.WaitingEndOfBattle:
                        Switch(GameState.StartScene);
                        break;
                    case GameState.WaitingEndOfGame:
                        LoadScene("Menu");
                        break;
                }
                break;

            case GameState.StartOfTurn:
                if (Replay == null)
                {
                    if (CurrentPlayer == null)
                        CurrentPlayer = new Player();

                    if (CurrentPlayer.Data.IsAI == false)
                        CurrentPlayer.StartShop();
                }
                else
                    CurrentPlayer.LoadDataByReplay();
                break;

            case GameState.ShopPhase:
                Replay = null;
                if (PhaseShopController.Instance.IsTurnAI() == false)
                    input.BlocksInput = false;
                break;

            case GameState.EndOfTurn:
                CurrentGame.CurrentPlayerIndex++;
                SaveSystem.SaveGame(CurrentGame);
                Switch(GameState.StartScene);
                break;

            case GameState.StartOfBattle:
                randomSeed++;
                currentRound = SaveSystem.SaveRoundData(CurrentGame, players[0].Data, players[1].Data, randomSeed);

                PhaseBattleController.Instance.Run(players[0], players[1]);
                break;

            case GameState.BattlePhase:
                break;

            case GameState.EndOfBattle:
                CurrentGame.CurrentPlayerIndex = 0;
                SaveSystem.SaveGame(CurrentGame);

                Switch(GameState.WaitingEndOfBattle);
                break;

            case GameState.EndOfGame:
                SaveSystem.SaveGame(CurrentGame);

                Switch(GameState.WaitingEndOfGame);
                break;
        }
    }

    private void InitTutorial()
    {
        players = new List<Player>();

        // Initialize player instances.
        players.Add(new Player());
        players.Add(new Player());

        // Create a new game.
        players[0].Data = new PlayerData("Tutorial Mode", PlayerLives, 0);
        players[1].Data = new PlayerData(AI.Name, PlayerLives, 0, true);

        currentGame = new Game(
               Mode,
               2,
               Timer,
               PlayerLives,
               0,
               GameState.None
               );
    }

    private void InitTest()
    {
        players = new List<Player>();

        // Initialize player instances.
        players.Add(new Player());
        players.Add(new Player());

        // Create a new game.
        players[0].Data = new PlayerData(AI.Name + " 1", PlayerLives, 0, true);
        players[1].Data = new PlayerData(AI.Name + " 2", PlayerLives, 0, true);

        currentGame = new Game(
               Mode,
               2,
               Timer,
               PlayerLives,
               0,
               GameState.None
               );
    }

    /// <summary>
    /// Initialize game with mode PvP.
    /// </summary>
    private void InitPvP()
    {
        players = new List<Player>();

        // Initialize player instances.
        players.Add(new Player());
        players.Add(new Player());

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

        currentGame = new Game(
                Mode,
                2,
                Timer,
                PlayerLives,
                0,
                GameState.None
                );

        // Set default speed multiplier for phase battle
        //CurrentSpeedMultiplier = DefaultSpeedMultiplier;
    }

    /// <summary>
    /// Starts scene.
    /// </summary>
    private void StartScene()
    {
        if (CurrentGame == null)
            return;

        if (CurrentGame.CurrentPlayerIndex < players.Count)
        {
            CurrentPlayer = players[CurrentGame.CurrentPlayerIndex];

            Debug.Log("--------------- Phase Shop " + PhaseShopIndex + "----------------");
            PhaseShopIndex++;

            if ((IsTutorialRunning || TestBattle) && CurrentPlayer.Data.IsAI)
            {
                if (TestBattle && PhaseShopIndex == 1)
                    LoadScene("PhaseShop");
                StartCoroutine(CurrentPlayer.ExecuteByTutorialAI());
                return;
            }

            Switch(GameState.PlayCutSceneShop);
        }
        else
        {
            CurrentPlayer = null;
            Debug.Log("--------------- Phase Battle ----------------");
            Switch(GameState.PlayCutSceneBattle);
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

    /// <summary>
    /// Plays the replay of the last battle. It initializes a new ReplayManager and switches to the battle scene.
    /// </summary>
    public void PlayReplay()
    {
        Replay = new ReplayManager();
        Replay.Switch(GameState.PlayCutSceneBattle);
    }

    /// <summary>
    /// Loads the scene with the given name. It is called by the CutScene component after the close scene animation is finished.
    /// </summary>
    /// <param name="_scene"></param>
    public void LoadScene(string _scene)
    {
        Debug.Log("Loading Scene: " + _scene);
        SceneManager.LoadScene(_scene);
    }

    public void SetTutorialStart()
    {
        if (IsTutorialRunning)
        {
            if (TutorialManager.Instance.CurrentState == TutorialManager.StepState.BattleIdle)
                TutorialManager.Instance.CurrentState = TutorialManager.StepState.BattleIdle;

            TutorialManager.Instance.SetNextStep();
        }

    }

}
