using System.Collections;
using System.Collections.Generic;
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
                //if (Application.isPlaying)
                //    SceneManager.LoadScene("Menu");

                Debug.LogWarning("GameManager instance is null.");
            }
            return _Instance;
        }
    }
    private static GameManager _Instance;


    [Header("Develop Settings")]
    [SerializeField] private bool isModeDevelop;
    [SerializeField] private bool isNotSavingGame;
    public bool IsRepairSystemActive;
    public bool TestBattle;
    [SerializeField] private int defaultTutorialLives = 3;
    [SerializeField] private int devLives = 3;
    //[SerializeField] private float timer = 90.0f;

    [Header("Global Settings")]
    [SerializeField] private float clickCooldown = 0.5f;

    private float lastClickTime = 0f;
    public bool IsClickable
    {
        get
        {
            if (canRegisterClick)
            {
                // Register the click and start the cooldown.
                lastClickTime = Time.time;
                canRegisterClick = false;
                return true;
            }

            // Still in cooldown, ignore the click.
            return false;
        }
    }
    private bool canRegisterClick = true;

    public bool IsGameOverSceneRunning { get; set; }

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
    public int Timer { get; set; }
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
                Debug.LogWarning("currentGame is null");
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
                Debug.LogWarning("The list of players is null");
                return null;
            }
            return players;
        }
    }
    private List<Player> players;

    public Player CurrentPlayer { get; set; }
    //public bool IsCatalogActive { get; set; } = false;

    // Lazy Loading is initialized once to create and hold an instance.
    private InputManager input => InputManager.Instance;

    #endregion


    public bool IsMobile => isMobile;
    public void SetIsMobile(bool _value) => isMobile = _value;

    private bool isMobile = false;


    public bool IsCatalogActive { get; set; }
    public ReplayManager Replay { get; set; }

    #region Time
    public void SetTime(float _time) => Time.timeScale = _time;
    public bool IsStopped { get; set; } = false; // is for SetRunningButton
    public float BattleSpeed { get; set; } = 1f;  // 1 = running, 0 = stopped


    public TutorialManager.StepState TutorialStepState { get; set; }

    public bool IsTutorialRunning => isTutorialRunning;
    public void SetTutorialRunning(bool _value) => isTutorialRunning = _value;

    private bool isTutorialRunning = false;
    #endregion

    public bool IsMode1P =>
        currentGame.Mode == GameMode.Tutorial ||
        currentGame.Mode == GameMode.AI ||
        currentGame.Mode == GameMode.TestBattle;

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

    private float aiLogicTime = 0f;
    private float aiLogicRefresh = 1f;


    private void Awake()
    {
        Debug.Log(this.name + ".Awake()");

        if (_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _Instance = this;

        DontDestroyOnLoad(gameObject);

        if (input != null)
        {
            // this if-query is used to initialize instances once.
        };
    }

    private void Start()
    {
        // auto play in dev mode.
        if (isModeDevelop)
        {
            PlayerLives = devLives;

            if (TestBattle)
            {
                LoadGame(GameMode.TestBattle);
                return;
            }

            LoadGame(GameMode.AI);
        }
    }

    private void Update()
    {
        // Check if the cooldown has passed.
        if (Time.time - lastClickTime >= clickCooldown)
        {
            canRegisterClick = true; // Reset the clickable state after cooldown.
        }

        if (aiLogicTime > 0f)
        {
            aiLogicTime -= Time.deltaTime;
        }
        if (aiCoroutine != null && aiLogicTime <= 0f)
        {
            aiCoroutine = null;
            Switch(GameState.EndOfTurn);
        }
    }

    /// <summary>
    /// Loads game.
    /// </summary>
    public void LoadGame(GameMode _mode)
    {
        Mode = _mode;

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

        switch (Mode)
        {
            case GameMode.None:
                break;

            case GameMode.TestBattle:

                // Create a new game.
                var name1 = NameList.GetRandomName();
                var name2 = NameList.GetRandomExclusive(new[] { name1 });
                players[0].Data = new PlayerData(name1, PlayerLives, 0, true);
                players[1].Data = new PlayerData(name2, PlayerLives, 0, true);

                currentGame = new Game(Mode, 2, Timer, PlayerLives, 0, GameState.None);

                testBattleCoroutine = StartCoroutine(DelayLoadShop());
                break;

            case GameMode.Tutorial:
                PlayerLives = defaultTutorialLives;

                // Create a new game.
                players[0].Data = new PlayerData("You", PlayerLives, 0);
                players[1].Data = new PlayerData(NameList.GetRandomName(), PlayerLives, 0, true);

                currentGame = new Game(Mode, 2, Timer, PlayerLives, 0, GameState.None);

                Switch(GameState.StartScene);
                break;

            case GameMode.PvP:

                // Create a new game.
                players[0].Data = new PlayerData(Name1, PlayerLives, 0);
                players[1].Data = new PlayerData(Name2, PlayerLives, 0);

                currentGame = new Game(Mode, 2, Timer, PlayerLives, 0, GameState.None);

                Switch(GameState.StartScene);
                break;

            case GameMode.AI:

                // Create a new game.
                players[0].Data = new PlayerData(Name1, PlayerLives, 0);
                players[1].Data = new PlayerData(NameList.GetRandomExclusive(new [] {Name1}), PlayerLives, 0, true);

                currentGame = new Game(Mode, 2, Timer, PlayerLives, 0, GameState.None);

                Switch(GameState.StartScene);
                break;

            case GameMode.Friends:
                // Load Online Versus game mode
                break;
        }

        // Set default speed multiplier for phase battle
        //CurrentSpeedMultiplier = DefaultSpeedMultiplier;
    }

    private Coroutine testBattleCoroutine;
    private IEnumerator DelayLoadShop()
    {
        yield return new WaitUntil(() => PackManager.Instance.MyPack != null);
        Switch(GameState.StartScene);
        testBattleCoroutine = null;
    }

    [ContextMenu("Reload Shop")]
    public void ReloadShop()
    {
        Switch(GameState.PlayCutSceneShop);
        TutorialStepState = TutorialManager.StepState.Turn2;
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
                CutScene.Instance.CloseScene();
                break;

            case GameState.PlayCutSceneBattle:
                CutScene.Instance.SetHintClick("", true);
                CutScene.Instance.CloseScene();
                break;

            case GameState.WaitingCutScene:
                input.BlocksInput = false;
                // Waiting for player input to switch in load scene
                CurrentGame.State = prevState;
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
                aiCoroutine = null;
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
                InputManager.Instance.BlocksInput = false;
                break;

            case GameState.EndOfBattle:
                CurrentGame.CurrentPlayerIndex = 0;
                SaveSystem.SaveGame(CurrentGame);

                Switch(GameState.WaitingEndOfBattle);
                break;

            case GameState.WaitingEndOfBattle:
                input.BlocksInput = false;
                EventManager.Instance.OnBattleDelayHintClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.EndOfGame:
                SaveSystem.SaveGame(CurrentGame);
                input.BlocksInput = true;
                break;

            case GameState.WaitingEndOfGame:
                input.BlocksInput = false;
                // Waiting for player input
                break;
        }
    }

    private Coroutine aiCoroutine;

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

            Debug.Log("--------------- Phase Shop " + PhaseShopIndex + " / " + CurrentPlayer.Data.Name + " ----------------");
            PhaseShopIndex++;

            if (IsMode1P && CurrentPlayer.Data.IsAI)
            {
                if (TestBattle)
                    LoadScene("PhaseShop");

                aiLogicTime = aiLogicRefresh;
                aiCoroutine = StartCoroutine(CurrentPlayer.ExecuteByTutorialAI());
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

    public void SetTutorialStartAtSceneStart()
    {
        bool isUnlocking = false;
        if (PackManager.Instance != null && PhaseShopUI.Instance && CurrentPlayer != null)
        {
            isUnlocking =
                PackManager.Instance.IsUnlockingTier(CurrentPlayer.Data.Turn).Item1 &&
                PackManager.Instance.IsUnlockingTier(CurrentPlayer.Data.Turn).Item2 > 1;
        }

        if (IsTutorialRunning && isUnlocking == false)
        {
            switch(TutorialStepState)
            {
                case TutorialManager.StepState.Turn1:
                    TutorialManager.Instance.StartStep(TutorialManager.StepState.Welcome);
                    break;
                case TutorialManager.StepState.ShopToBattle:
                    TutorialManager.Instance.StartStep(TutorialManager.StepState.BattleIntro1);
                    break;
                case TutorialManager.StepState.Turn2:
                    TutorialManager.Instance.StartStep(TutorialManager.StepState.ClickRobotToRepair);
                    break;
            }
        }
    }

    public void SetByOpenSceneEnd()
    {
        if (currentGame.Mode != GameMode.TestBattle &&
            PackManager.Instance != null &&
            PhaseShopController.Instance &&
            PhaseShopUI.Instance &&
            CurrentPlayer != null)
        {
            PhaseShopController.Instance.SetStartTurn(StartTurnState.OpenSceneEnd);
            Switch(GameState.ShopPhase);
        }

        if (PhaseBattleView.Instance)
        {
            InputManager.Instance.BlocksInput = false;
            PhaseBattleView.Instance.OnOpenSceneEnd();
        }
    }

    public void SetActive(Component _comp, bool _active)
    {
        if (_comp != null)
            _comp.gameObject.SetActive(_active);
    }

    public void Log(string _text)
    {
        Debug.Log(_text);
    }

    public void LogWarning(string _text)
    {
        Debug.LogWarning(_text);
    }

    public void LogError(string _text)
    {
        Debug.LogError(_text);
    }
}
