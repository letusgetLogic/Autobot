using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        // --- Turn 1 ---

        Turn1 = 0,
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
        ShowTeamOrder,
        ShowFightOrder,
        ShowSwap,
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

        // --- Turn 2 ---

        Turn2,
        ClickRobotToRepair,
        RepairRobot,
        RepairCompliment,
        ClickRobotToRecycle,
        RecycleRobot,
        RecycleCompliment,
        ShowFusion,
        ShowChargingStation,
        TryOut,

        //ShopIdle,
        //ShopToBattle2,
        //BattleIdle2,
        //WaitingEndBattle2,

        //// --- Turn 3 ---

        //Turn3,
        //UnlockTier,
        //RepairToLevelUp,
        //ClickRobotToLevelUp,
        //PickToLevelUp,
        //LevelUpEffect,

        //Reserve1,
        //Reserve2,
        //Reserve3,
        //Reserve4,
        //Reserve5,

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

    private TutorialStep currentStep => steps[(int)currentState];

    public List<InputKey> CurrentAllowedInputs => currentAllowedInputs;
    private List<InputKey> currentAllowedInputs;

    private Coroutine coroutineNextStep;
    private Coroutine coroutineDeactivateArrow;

    public SlotTutorial AbilitySlot { get; set; }

    private List<Slot> activeHints { get; set; } = new();


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
            }
        }
    }
    [ContextMenu("OnGetName")]
    private void GetName()
    {
        foreach (var item in GetAssetNamesInFolder("Scriptable Objects/Tutorial Settings/"))
        {
            Debug.Log(item);
        }
    }
    public static List<string> GetAssetNamesInFolder(string folderPath, string typeFilter = "")
    {
        // Ensure path starts with Assets/
        if (!folderPath.StartsWith("Assets/"))
            folderPath = "Assets/" + folderPath;

        // Define the search filter (e.g., "t:ScriptableObject" or empty for all)
        string filter = string.IsNullOrEmpty(typeFilter) ? "" : typeFilter;

        // Find all assets in the specified folder
        string[] guids = AssetDatabase.FindAssets(filter, new[] { folderPath });

        List<string> assetNames = new List<string>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // Extract the filename without extension
            string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            assetNames.Add(name);
        }

        return assetNames;
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
            step.OnLabelPopup += () => EventManager.Instance.OnPopUpSound?.Invoke();
        }

        EventManager.Instance.OnAttachedUnit += CheckClick;
        EventManager.Instance.OnCraft += CheckInput;
        EventManager.Instance.OnLock += CheckInput;
        EventManager.Instance.OnEndTurnClick += CheckInput;
        EventManager.Instance.OnEndShop += Check;
        EventManager.Instance.OnInitDone += Check;
        EventManager.Instance.OnBattleDone += Check;
        EventManager.Instance.OnRepair += CheckInput;
        EventManager.Instance.OnRecycle += CheckInput;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnAttachedUnit -= CheckClick;
        EventManager.Instance.OnCraft -= CheckInput;
        EventManager.Instance.OnLock -= CheckInput;
        EventManager.Instance.OnEndTurnClick -= CheckInput;
        EventManager.Instance.OnEndShop -= Check;
        EventManager.Instance.OnInitDone -= Check;
        EventManager.Instance.OnBattleDone -= Check;
        EventManager.Instance.OnRepair -= CheckInput;
        EventManager.Instance.OnRecycle -= CheckInput;

        Instance = null;

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

        if (currentStep &&
            currentStep.gameObject && currentStep.gameObject.activeSelf)
            currentStep.OnUpdate(Time.deltaTime);

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

                    if (currentStep && currentStep.gameObject)
                        currentStep.OnEnter();

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

                    if (currentStep && currentStep.gameObject)
                        currentStep.OnLateEnter();

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

            if (currentStep != null && currentStep.gameObject)
                delay += steps[(int)currentState].OnExit();

            OnValidatedExit();
        }

        if (delay == 0f)
        {
            if (currentStep != null &&
                currentStep.gameObject && currentStep.gameObject.activeSelf)
            {
                currentStep.gameObject.SetActive(false);
            }

            currentState++;
            runState = RunState.Start;
            countTime = 0f;
            return;
        }

        coroutineNextStep = StartCoroutine(DelaySetNextStep(delay));
    }

    private IEnumerator DelaySetNextStep(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        lagCount = 0f;
        yield return new WaitUntil(() =>
        {
            if (lagCount >= maxLagTime)
            {
                currentStep.ActiveActions.Clear();
            }
            return currentStep == null || currentStep.ActiveActions.Count == 0;
        });

        if (currentStep != null &&
            currentStep.gameObject && currentStep.gameObject.activeSelf)
        {
            currentStep.gameObject.SetActive(false);
        }

        currentState++;
        runState = RunState.Start;
        countTime = 0f;

        coroutineNextStep = null;
    }

    private void OnValidatedEnter()
    {
        if (currentState == StepState.RobotEnergyConsumption)
        {
            if (AbilitySlot)
                AbilitySlot.EnergyConsumptionIndicator.SetActive(true);
        }
        else if (currentState == StepState.RobotUseAbility)
        {
            if (AbilitySlot)
            {
                AbilitySlot.AbilityIndicator.SetActive(true);
                AbilitySlot.EnergyIndicator.SetActive(true);
            }
        }
        else if (currentState == StepState.WaitingForAbility || currentState == StepState.BattleIdle)
        {
            PhaseBattleController.Instance.SetRunning(true);
        }
        else if (currentState == StepState.RepairRobot)
        {
            if (PhaseShopController.Instance && PhaseShopController.Instance.TeamSlots().Length > 0)
            {
                foreach (var slot in PhaseShopController.Instance.TeamSlots())
                {
                    var unit = slot.UnitController();
                    if (unit != null && unit.Model.IsFullDurability() == false)
                    {
                        if (slot.Tutorial)
                        {
                            slot.Tutorial.HintArrow.SetActive(true);
                            activeHints.Add(slot);
                        }
                    }
                }
            }
        }
        else if (currentState == StepState.Done)
        {
            GameManager.Instance.SetTutorialRunning(false);
            gameObject.SetActive(false);
        }
    }

    private void OnValidatedLateEnter()
    {
        if (currentState == StepState.ShowFactoryReseted)
        {
            foreach (var slot in PhaseShopController.Instance.ShopItemSlots())
            {
                if (slot.Tutorial &&
                    slot.UnitController() && slot.UnitController().Model.Data.UnitType == UnitType.Item)
                {
                    slot.Tutorial.HintArrow.SetActive(true);
                    activeHints.Add(slot);
                }
            }
        }
        else if (currentState == StepState.ClickRobotToRepair)
        {
            if (PhaseShopController.Instance && PhaseShopController.Instance.TeamSlots().Length > 0)
            {
                foreach (var slot in PhaseShopController.Instance.TeamSlots())
                {
                    var unit = slot.UnitController();
                    if (unit != null && unit.Model.IsFullDurability() == false)
                    {
                        if (slot.Tutorial)
                        {
                            slot.Tutorial.HintArrow.SetActive(true);
                        }
                    }
                }
            }
        }
        else if (currentState == StepState.ClickRobotToRecycle)
        {
            if (PhaseShopController.Instance && PhaseShopController.Instance.TeamSlots().Length > 0)
            {
                foreach (var slot in PhaseShopController.Instance.TeamSlots())
                {
                    var unit = slot.UnitController();
                    if (slot.Tutorial &&
                        unit && unit.Model.Data.UnitState == UnitState.InSlotTeam
                        && (unit.Model.SoUnit.Name == "Gold Eye" || unit.Model.SoUnit.ModelID == "RC-BF-2R"))
                    {
                        slot.Tutorial.HintArrow.SetActive(true);
                        activeHints.Add(slot);
                    }
                }
            }
        }
    }

    private void OnValidatedExit()
    {
        if (currentState == StepState.ShowFactoryReseted)
        {
            activeHints.ForEach(x => x.Tutorial.HintArrow.SetActive(false));
        }
        else if (currentState == StepState.RobotEnergyConsumption)
        {
            if (AbilitySlot)
                AbilitySlot.EnergyConsumptionIndicator.SetActive(false);
        }
        else if (currentState == StepState.RobotUseAbility)
        {
            if (AbilitySlot)
            {
                AbilitySlot.AbilityIndicator.SetActive(false);
                AbilitySlot.EnergyIndicator.SetActive(false);
            }
        }
        activeHints.Clear();
    }

    public void CheckClick(UnitController _unit)
    {
        if (currentState == StepState.ClickRobot && _unit && _unit.Model.IsRobotInShop() ||
            currentState == StepState.ShowFactoryReseted && _unit && _unit.Model.Data.UnitType == UnitType.Item ||
            currentState == StepState.ClickRobotToRepair && _unit && _unit.Model.Data.UnitState == UnitState.InSlotTeam)
        {
            // whenever get into next step, should hide the description of units
            if (PhaseShopController.Instance)
                PhaseShopController.Instance.HideDescriptionOfUnits();

            SetNextStep();
        }
        else if (currentState == StepState.ClickRobotToRecycle && _unit && _unit.Model.Data.UnitState == UnitState.InSlotTeam
                && (_unit.Model.SoUnit.Name == "Gold Eye" || _unit.Model.SoUnit.ModelID == "RC-BF-2R"))
        {
            SetNextStep();
        }
    }

    public void CheckInput(InputKey _inputKey)
    {
        switch (_inputKey)
        {
            case InputKey.DropSlotTeam:
                if (currentState == StepState.PickRobot ||
               currentState == StepState.PickOthers && PhaseShopController.Instance.HasAnyBotInShop() == false ||
               currentState == StepState.PickBattery)
                    SetNextStep();
                break;

            case InputKey.ClickButtonLock:
                if (currentState == StepState.LockBattery)
                    SetNextStep();
                break;

            case InputKey.ClickButtonEndTurn:
                currentAllowedInputs = new();
                break;

            case InputKey.ClickButtonRepair:
                if (currentState == StepState.RepairRobot)
                {
                    foreach (var slot in activeHints)
                    {
                        var unit = slot.UnitController();
                        if (unit != null && unit.Model.IsFullDurability())
                        {
                            slot.Tutorial.HintArrow.SetActive(false);
                        }
                    }

                    if (PhaseShopController.Instance && PhaseShopController.Instance.HasAllFullRobots())
                        SetNextStep();
                }
                break;

            case InputKey.ClickButtonRecycle:
                if (currentState == StepState.RecycleRobot)
                {
                    activeHints.ForEach(x => x.Tutorial.HintArrow.SetActive(false));
                    SetNextStep();
                }
                break;

        }
    }

    public void Check()
    {
        switch (currentState)
        {
            case StepState.EndTurn:
                if (currentStep != null && currentStep.gameObject)
                    currentStep.OnExit();

                currentState = StepState.ShopToBattle;
                break;
            case StepState.ShopToBattle:
                PhaseBattleController.Instance.SetRunning(false);
                break;
            case StepState.WaitingForAbility:
                SetNextStep();
                PhaseBattleController.Instance.SetRunning(false);
                break;
            case StepState.BattleIdle:
                SetNextStep();
                break;
        }
    }

    public bool IsPreventingDrop()
    {
        if (currentState == StepState.RepairRobot)
        {
            return true;
        }

        return false;
    }

    private IEnumerator DeactivateHintArrowSlot(bool _case)
    {
        yield return new WaitUntil(() => _case);
        activeHints.ForEach(slot => slot.Tutorial.HintArrow.SetActive(false));
        coroutineDeactivateArrow = null;
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

    public bool IsAbledToSetNextStep()
    {
        foreach (var allowedInput in currentAllowedInputs)
        {
            if (allowedInput == InputKey.All)
            {
                return false;
            }
        }

        return true;
    }
}
