using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

[DisallowMultipleComponent]
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private SoTutorialSettings[] settings;
    [SerializeField] private TutorialStep[] steps;
    public SoUnit[] BotsTurn1;
    public SoUnit[] ItemsTurn1;
    [SerializeField] private float maxLagTime = 3.0f;

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

        BattleIntro1,
        BattleIntro2,
        BattleIntro3,
        WaitingForAbility,
        RobotHasAbility,
        RobotEnergyConsumption,
        RobotUseAbility,
        BattleIdle,
        WaitingEndBattle,

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
    private float lagCount = 0f;

    private enum RunState { None, Start, Delay, Duration, DurationHide, AFK }
    private RunState runState = RunState.None;

    public List<InputKey> CurrentAllowedInputs => currentAllowedInputs;
    private List<InputKey> currentAllowedInputs;

    private Coroutine coroutine;

    [HideInInspector] public SlotTutorial AbilitySlot;


    [ContextMenu("OnReset")]
    private void Reset()
    {
        GameManager.Instance.LoadGame(GameMode.Tutorial);
    }
#if UNITY_EDITOR
    [ContextMenu("OnRename")]
    private void OnRename()
    {
        if (steps != null)
        {
            for (int i = 0; i < steps.Length; i++)
            {
                var step = steps[i];
                if (step == null)
                    continue;
                step.gameObject.name = $"{i}_{(StepState)i}";
            }
        }
        if (settings != null)
        {
            for (int i = 0; i < settings.Length; i++)
            {
                var setting = settings[i];
                if (setting == null)
                    continue;

                RenameScriptableObject.RenameAsset(setting, $"{i}_{(StepState)i}");
                //setting.name = $"{i}_{(StepState)i}";
            }
        }
    }
#endif
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
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
        EventManager.Instance.OnInitDone += () => Check();
        EventManager.Instance.OnBattleDone += () => Check();
    }

    private void Update()
    {
        if (settings == null || steps == null)
        {
            return;
        }

        if ((int)currentState >= settings.Length)
        {
            Debug.LogWarning("currentState " + currentState + " out of settings.length!");
            return;
        }
        if ((int)currentState >= steps.Length)
        {
            Debug.LogWarning("currentState " + currentState + " out of steps.length!");
            return;
        }

        if (lagCount < maxLagTime)
            lagCount += Time.deltaTime;

        if (steps[(int)currentState] &&
            steps[(int)currentState].gameObject && steps[(int)currentState].gameObject.activeSelf)
            steps[(int)currentState].OnUpdate(Time.deltaTime);

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

                    if (steps[(int)currentState] && steps[(int)currentState].gameObject)
                        steps[(int)currentState].OnEnter();

                    OnValidatedEnter();

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
                    Debug.Log($"{currentState}.OnLateEnter");

                    if (steps[(int)currentState] && steps[(int)currentState].gameObject)
                        steps[(int)currentState].OnLateEnter();

                    OnValidatedLateEnter();

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

    public void SetNextStep()
    {
        runState = RunState.None;
        currentAllowedInputs = new();
        countTime = 0f;
        float delay = 0f;

        if (currentState >= 0)
        {
            Debug.Log($"{currentState}.OnExit");

            if (steps[(int)currentState] != null && steps[(int)currentState].gameObject)
                delay += steps[(int)currentState].OnExit();

            OnValidatedExit();
        }

        if (delay == 0f)
        {
            if (steps[(int)currentState] != null &&
                steps[(int)currentState].gameObject && steps[(int)currentState].gameObject.activeSelf)
            {
                steps[(int)currentState].gameObject.SetActive(false);
            }

            currentState++;
            runState = RunState.Start;
            return;
        }

        coroutine = StartCoroutine(DelaySetNextStep(delay));
    }

    private IEnumerator DelaySetNextStep(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        lagCount = 0f;
        yield return new WaitUntil(() =>
        {
            if (lagCount >= maxLagTime)
            {
                steps[(int)currentState].ActiveActions.Clear();
            }
            return steps[(int)currentState] == null || steps[(int)currentState].ActiveActions.Count == 0;
        });

        if (steps[(int)currentState] != null &&
            steps[(int)currentState].gameObject && steps[(int)currentState].gameObject.activeSelf)
        {
            steps[(int)currentState].gameObject.SetActive(false);
        }

        currentState++;
        runState = RunState.Start;

        coroutine = null;
    }

    private void OnValidatedEnter()
    {
        if (currentState == StepState.RobotEnergyConsumption)
        {
            if (AbilitySlot)
                AbilitySlot.EnergyConsumptionIndicator.SetActive(true);
        }
        if (currentState == StepState.RobotUseAbility)
        {
            if (AbilitySlot)
            {
                AbilitySlot.AbilityIndicator.SetActive(true);
                AbilitySlot.EnergyIndicator.SetActive(true);
            }
        }
        if (currentState == StepState.WaitingForAbility || currentState == StepState.BattleIdle)
        {
            PhaseBattleController.Instance.SetRunning(true);
        }
    }

    private void OnValidatedLateEnter()
    {

    }

    private void OnValidatedExit()
    {
        if (currentState == StepState.RobotEnergyConsumption)
        {
            if (AbilitySlot)
                AbilitySlot.EnergyConsumptionIndicator.SetActive(false);
        }
        if (currentState == StepState.RobotUseAbility)
        {
            if (AbilitySlot)
                AbilitySlot.AbilityIndicator.SetActive(false);
        }
        if (currentState == StepState.RobotUseAbility)
        {
            if (AbilitySlot)
                AbilitySlot.EnergyIndicator.SetActive(false);
        }
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
            if (steps[(int)StepState.EndTurn] != null && steps[(int)StepState.EndTurn].gameObject)
                steps[(int)StepState.EndTurn].OnExit();

            currentState = StepState.ShopToBattle;
        }
    }

    public void Check()
    {
        if (currentState == StepState.ShopToBattle)
        {
            PhaseBattleController.Instance.SetRunning(false);
        }
        if (currentState == StepState.WaitingForAbility)
        {
            SetNextStep();
            PhaseBattleController.Instance.SetRunning(false);
        }
        if (currentState == StepState.BattleIdle)
        {
            SetNextStep();
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
