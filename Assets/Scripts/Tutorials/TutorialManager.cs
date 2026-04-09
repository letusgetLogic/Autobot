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



    private enum StepState
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

        FusionRobot,
        LevelUp,
        Roll,
        Rool2,
        BonusEnergy,


    }
    private StepState stepState = StepState.Idle;

    private float countTime = 0f;

    private enum RunState { None, Start, Delay, Duration, DurationHide, AFK }
    private RunState runState = RunState.None;

    private List<InputKey> currentAllowedInputs;
    public List<InputKey> CurrentAllowedInputs
    {
        get => currentAllowedInputs;
    }

    private Coroutine coroutine;

    [ContextMenu("OnReset")]
    private void Reset()
    {
        GameManager.Instance.StartTutorial();
    }

    private void Awake()
    {
        Instance = this;

        if (GameManager.Instance.TutorialCompleted)
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
    }

    private void Update()
    {
        if (settings == null || (int)stepState >= settings.Length
            || steps == null || (int)stepState >= steps.Length)
            return;

        if (countTime <= 0)
        {
            switch (runState)
            {
                case RunState.None:
                    break;

                case RunState.Start:
                    countTime = settings[(int)stepState].Delay;
                    runState = RunState.Delay;
                    break;

                case RunState.Delay:
                    Debug.Log($"{stepState}.OnEnter");
                    steps[(int)stepState].OnEnter();
                    currentAllowedInputs = settings[(int)stepState].AllowedInputs;
                    countTime = settings[(int)stepState].Duration;
                    runState = RunState.Duration;
                    break;

                case RunState.Duration:
                    if (settings[(int)stepState].AutoCompleted)
                    {
                        SetNextStep();
                        return;
                    }
                    Debug.Log($"{stepState}.OnAnimateAFK");
                    steps[(int)stepState].OnAnimateAFK();
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

        float delay = 0f;
        if (stepState >= 0)
        {
            Debug.Log($"{stepState}.OnExit");
            delay += steps[(int)stepState].OnExit();
        }
        if (delay == 0f)
        {
            stepState++;
            runState = RunState.Start;
            return;
        }

        coroutine = StartCoroutine(DelaySetNextStep(delay));
    }

    private IEnumerator DelaySetNextStep(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        stepState++;
        runState = RunState.Start;

        coroutine = null;
    }

    public void CheckInput(UnitController _unit)
    {
        if (stepState == StepState.ClickRobot && _unit && _unit.Model.IsRobotInShop())
        {
            SetNextStep();
        }
        if (stepState == StepState.ShowFactoryReseted && _unit && _unit.Model.Data.UnitType == UnitType.Item)
        {
            SetNextStep();
        }
    }

    public void CheckInput(InputKey _inputKey)
    {
        if (_inputKey == InputKey.DropSlotTeam)
        {
            if (stepState == StepState.PickRobot ||
                stepState == StepState.PickOthers && PhaseShopController.Instance.HasAnyBotInShop() == false ||
                stepState == StepState.PickBattery)
                SetNextStep();
        }
        if (_inputKey == InputKey.ClickButtonLock)
        {
            if (stepState == StepState.LockBattery)
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