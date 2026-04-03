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
                if (Application.isPlaying)
                    SceneManager.LoadScene("Menu");

                Debug.LogWarning("GameManager instance is null.");
            }
            return _Instance;
        }
    }
    private static GameManager _Instance;

    [Header("Develop Settings")]
    [SerializeField] private bool isNotSavingGame;
    [SerializeField] public bool IsRepairSystemActive;

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
    public string Name1 { get; set; }
    public string Name2 { get; set; }
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
    /// <summary>
    /// To block player's input while animation is running.
    /// </summary>
    public bool IsBlockingInput {  get; set; }
    public bool IsCatalogActive { get; set; } = false;
    public ReplayManager Replay { get; set; }

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
        };
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
                GameState.PlayCutScene
                );

        // Set default speed multiplier for phase battle
        //CurrentSpeedMultiplier = DefaultSpeedMultiplier;
    }

    /// <summary>
    /// Switches the game state and performs actions based on the new state.
    /// </summary>
    /// <param name="_state"></param>
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
                input.BlocksInput = true;
                RunModeSingle();
                break;

            case GameState.WaitingEndOfBattle:
                input.BlocksInput = false;
                EventManager.Instance.OnWaitingForClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.WaitingEndOfGame:
                input.BlocksInput = false;
                EventManager.Instance.OnWaitingForClick?.Invoke();
                // Waiting for player input
                break;

            case GameState.WaitingCutScene:
                input.BlocksInput = false;
                // Waiting for player input
                break;

            case GameState.LoadScene:
                LoadScene(SceneToLoad);
                break;

            case GameState.StartOfTurn:
                if (Replay == null)
                    CurrentPlayer.StartShop();
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
                Switch(GameState.PlayCutScene);
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

    /// <summary>
    /// Starts game mode singleplayer.
    /// </summary>
    private void RunModeSingle()
    {
        if (CurrentGame == null)
            return;

        if (CurrentGame.CurrentPlayerIndex < players.Count)
        {
            CurrentPlayer = players[CurrentGame.CurrentPlayerIndex];
            CutScene.Instance.SetHintClick(CurrentPlayer.Data.Name, false);
            CutScene.Instance.SwitchScene("PhaseShop");

            Debug.Log("--------------- Phase Shop " + PhaseShopIndex + "----------------");
            PhaseShopIndex++;
        }
        else
        {
            CutScene.Instance.SetHintClick("", true);
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

    /// <summary>
    /// Plays the replay of the last battle. It initializes a new ReplayManager and switches to the battle scene.
    /// </summary>
    public void PlayReplay()
    {
        Replay = new ReplayManager();

        CutScene.Instance.SwitchScene("PhaseBattle");
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
}
