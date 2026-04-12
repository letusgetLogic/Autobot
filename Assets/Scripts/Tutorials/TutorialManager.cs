using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private SoTutorialSettings[] settings;
    [SerializeField] private TutorialStep[] steps;
    public SoUnit[] BotsTurn1;
    public SoUnit[] ItemsTurn1;



    public enum StepState
    {
        None = -1,
        Idle = 0,
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

        BattleIdle,

        RepairRobot,
        FusionRobot,
        LevelUp,
        Roll,
        Rool2,


    }
    public StepState CurrentState { get; set; } = StepState.Idle;

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
        if (settings == null || (int)CurrentState >= settings.Length
            || steps == null || (int)CurrentState >= steps.Length)
            return;

        if (countTime <= 0)
        {
            switch (runState)
            {
                case RunState.None:
                    break;

                case RunState.Start:
                    countTime = settings[(int)CurrentState].Delay;
                    runState = RunState.Delay;
                    break;

                case RunState.Delay:
                    Debug.Log($"{CurrentState}.OnEnter");

                    steps[(int)CurrentState].OnEnter();

                    currentAllowedInputs = settings[(int)CurrentState].AllowedInputs;
                    countTime = settings[(int)CurrentState].Duration;
                    runState = RunState.Duration;
                    break;

                case RunState.Duration:
                    if (settings[(int)CurrentState].AutoCompleted)
                    {
                        SetNextStep();
                        return;
                    }
                    Debug.Log($"{CurrentState}.OnAnimateAFK");

                    steps[(int)CurrentState].OnAnimateAFK();

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

        if (CurrentState >= 0 && steps[(int)CurrentState] != null)
        {
            Debug.Log($"{CurrentState}.OnExit");
            delay += steps[(int)CurrentState].OnExit();
        }

        if (delay == 0f)
        {
            if (steps[(int)CurrentState] != null && steps[(int)CurrentState].gameObject)
                steps[(int)CurrentState].gameObject.SetActive(false);

            CurrentState++;
            runState = RunState.Start;
            return;
        }

        coroutine = StartCoroutine(DelaySetNextStep(delay));
    }

    private IEnumerator DelaySetNextStep(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        yield return new WaitUntil(() => steps[(int)CurrentState].ActiveActions.Count == 0);

        steps[(int)CurrentState].gameObject.SetActive(false);
        CurrentState++;
        runState = RunState.Start;

        coroutine = null;
    }

    public void CheckInput(UnitController _unit)
    {
        if (CurrentState == StepState.ClickRobot && _unit && _unit.Model.IsRobotInShop())
        {
            SetNextStep();
        }
        if (CurrentState == StepState.ShowFactoryReseted && _unit && _unit.Model.Data.UnitType == UnitType.Item)
        {
            SetNextStep();
        }
    }

    public void CheckInput(InputKey _inputKey)
    {
        if (_inputKey == InputKey.DropSlotTeam)
        {
            if (CurrentState == StepState.PickRobot ||
                CurrentState == StepState.PickOthers && PhaseShopController.Instance.HasAnyBotInShop() == false ||
                CurrentState == StepState.PickBattery)
                SetNextStep();
        }
        if (_inputKey == InputKey.ClickButtonLock)
        {
            if (CurrentState == StepState.LockBattery)
                SetNextStep();
        }
        if (_inputKey == InputKey.ClickButtonEndTurn)
        {
            steps[(int)StepState.EndTurn].OnExit();
            CurrentState = StepState.BattleIdle;

            if (GameManager.Instance.CurrentGame != null)
                GameManager.Instance.CurrentGame.TutorialStepState = StepState.BattleIdle;
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
