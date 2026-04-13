using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private SoTutorialSettings[] settings;
    [SerializeField] private TutorialStep[] steps;
    public SoUnit[] BotsTurn1;
    public SoUnit[] ItemsTurn1;

    public UnityAction OnEnter { get; private set; }
    public UnityAction OnAnimateAFK { get; private set; }
    public UnityAction OnExit { get; private set; }

    public enum StepState
    {
        None = -1,
        Idle = 0,

        // --- Turn 1 ---
        Welcome = 1,
        BuildTeam,
        ShowTeam,
        ShowFactory,
        ShowCurrency,
        ClickRobot,
        PickRobot,
        PickOthers,
        PickBattery,
        ShowFactoryReseted,
        LockBattery,
        EndTurn,

        ShopToBattle,

        BattleIdle,
        RobotHasAbility,
        RobotEnergyConsumption,
        RobotUseAbility,

        BattleToShop,

        // --- Turn 2 ---
        ClickRobotToRepair,
        RepairRobot,
        ClickRobotToSell,
        SellRobot,
        ClickRobotToFusion,
        PickToFusion,
        ShowRoll,
        
        ShopIdle,
        ShopToBattle2,
        BattleIdle2,
        BattleToShop2,

        // --- Turn 3 ---
        UnlockTier,
        RepairToLevelUp,
        ClickRobotToLevelUp,
        PickToLevelUp,
        LevelUpEffect,
        ShowChargingStation,

        Reserve1,
        Reserve2,
        Reserve3,
        Reserve4,
        Reserve5,
        
        Done,
    }
    private StepState currentState
    {
        get => GameManager.Instance.TutorialStepState;
        set
        {
            GameManager.Instance.TutorialStepState = value;
        }
    }

    private float countTime = 0f;

    private enum RunState { None, Start, Delay, Duration, DurationHide, AFK }
    private RunState runState = RunState.None;

    public List<InputKey> CurrentAllowedInputs => currentAllowedInputs;
    private List<InputKey> currentAllowedInputs;

    private Coroutine coroutine;

    [ContextMenu("OnReset")]
    private void Reset()
    {
        GameManager.Instance.LoadGame(GameMode.Tutorial); 
    }

    private void Awake()
    {
        Instance = this;

        if (GameManager.Instance.IsTutorialRunning == false)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void OnEnable()
    {
        foreach (var step in steps)
        {
            step.OnLabelPopup += () => SoundManager.Instance.PlayOneShot("Drop_Unit");
        }

        EventManager.Instance.OnAttachedUnit += CheckInput;
        EventManager.Instance.OnCraft += (unit) => CheckInput(InputKey.DropSlotTeam);
        EventManager.Instance.OnLock += () => CheckInput(InputKey.ClickButtonLock);
        EventManager.Instance.OnEndTurnClick += () => currentAllowedInputs = new(); 
        EventManager.Instance.OnEndShop += () => CheckInput(InputKey.ClickButtonEndTurn);
    }

    private void Update()
    {
        if (settings == null || (int)currentState >= settings.Length
            || steps == null || (int)currentState >= steps.Length)
            return;

        if (countTime <= 0)
        {
            switch (runState)
            {
                case RunState.None:
                    break;

                case RunState.Start:
                    countTime = settings[(int)currentState].Delay;
                    runState = RunState.Delay;
                    break;

                case RunState.Delay:
                    Debug.Log($"{currentState}.OnEnter");

                    steps[(int)currentState].OnEnter();

                    currentAllowedInputs = settings[(int)currentState].AllowedInputs;
                    countTime = settings[(int)currentState].Duration;
                    runState = RunState.Duration;
                    break;

                case RunState.Duration:
                    if (settings[(int)currentState].AutoCompleted)
                    {
                        SetNextStep();
                        return;
                    }
                    Debug.Log($"{currentState}.OnAnimateAFK");

                    steps[(int)currentState].OnAnimateAFK();

                    runState = RunState.AFK;
                    break;

                case RunState.AFK:
                    break;
            }
        }

        if (countTime > 0)
        {
            countTime -= Time.deltaTime;
        }
    }

    private void OnValidate()
    {
        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
        {
            var step = steps[i];
            if (step == null)
                continue;
            step.gameObject.name = $"{i}_{(StepState)i}";
        }
    }

    public void SetNextStep()
    {
        runState = RunState.None;
        currentAllowedInputs = new();
        countTime = 0f;
        float delay = 0f;

        if (currentState >= 0 && steps[(int)currentState] != null)
        {
            Debug.Log($"{currentState}.OnExit");
            delay += steps[(int)currentState].OnExit();
        }

        if (delay == 0f)
        {
            if (steps[(int)currentState] != null && steps[(int)currentState].gameObject)
                steps[(int)currentState].gameObject.SetActive(false);

            currentState++;
            runState = RunState.Start;
            return;
        }

        coroutine = StartCoroutine(DelaySetNextStep(delay));
    }

    private IEnumerator DelaySetNextStep(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        yield return new WaitUntil(() => steps[(int)currentState].ActiveActions.Count == 0);

        steps[(int)currentState].gameObject.SetActive(false);
        currentState++;
        runState = RunState.Start;

        coroutine = null;
    }

    public void CheckInput(UnitController _unit)
    {
        if (currentState == StepState.ClickRobot && _unit && _unit.Model.IsRobotInShop())
        {
            SetNextStep();
        }
        if (currentState == StepState.ShowFactoryReseted && _unit && _unit.Model.Data.UnitType == UnitType.Item)
        {
            SetNextStep();
        }
    }

    public void CheckInput(InputKey _inputKey)
    {
        if (_inputKey == InputKey.DropSlotTeam)
        {
            if (currentState == StepState.PickRobot ||
                currentState == StepState.PickOthers && PhaseShopController.Instance.HasAnyBotInShop() == false ||
                currentState == StepState.PickBattery)
                SetNextStep();
        }
        if (_inputKey == InputKey.ClickButtonLock)
        {
            if (currentState == StepState.LockBattery)
                SetNextStep();
        }
        if (_inputKey == InputKey.ClickButtonEndTurn)
        {
            steps[(int)StepState.EndTurn].OnExit();
            currentState = StepState.BattleIdle;

            GameManager.Instance.TutorialStepState = StepState.ShopToBattle;
        }
    }

    /// <summary>
    /// Sets integer value of PlayerPrefs.
    /// </summary>
    /// <param name="KeyName"></param>
    /// <param name="Value"></param>
    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }

    /// <summary>
    /// Gets integer value of PlayerPrefs.
    /// </summary>
    /// <param name="KeyName"></param>
    /// <returns></returns>
    public int Getint(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }
}
