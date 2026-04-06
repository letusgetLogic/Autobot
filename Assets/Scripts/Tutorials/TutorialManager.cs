using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private SoTutorialSettings[] settings;
    [SerializeField] private TutorialStep[] steps;
    

    public bool TutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        set => PlayerPrefs.SetInt("TutorialCompleted", value ? 1 : 0);
    }

    private enum StepState
    {
        None = -1,
        Welcome = 0,
        BuildTeam,
        ShowTeam,
        ShowFactory,
        ClickRobot,
        FactoryRobotSlots,
        RobotCost,
        PickRobot,
        FusionRobot,
        LevelUp,
        Roll,
        Rool2,
        Lock,
        EndTurn,
        BonusEnergy,

    }
    private StepState stepState = StepState.None;

    private float count = 0f;
    private float countAFK = 0f;

    private bool isAlreadyLateEnter = false;
    private bool isAlreadyAFK = false;

    private List<InputKey> currentAllowedInputs;
    public List<InputKey> CurrentAllowedInputs
    {
        get => currentAllowedInputs;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;


        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            TutorialCompleted = false;
        }

        SetNextStep();  
    }

    private void Update()
    {
        if (TutorialCompleted ||
            settings == null || (int)stepState >= settings.Length 
            || steps == null || (int)stepState >= steps.Length)
            return;

        if (count > 0)
        {
            count -= Time.deltaTime;
        }
        else if (isAlreadyLateEnter == false)
        {
            steps[(int)stepState].OnLateEnter();

            currentAllowedInputs = settings[(int)stepState].AllowedInputs;

            countAFK = settings[(int)stepState].DelayAFK;

            isAlreadyLateEnter = true;
        }
        else if (settings[(int)stepState].DelayAFK == 0f)
        {
            return;
        }
       
        if (countAFK > 0)
        {
            countAFK -= Time.deltaTime;
        }
        else if (isAlreadyAFK == false)
        {
            steps[(int)stepState].OnAnimateAFK();
            isAlreadyAFK = true;
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
        if (stepState >= 0)
            steps[(int)stepState].OnExit();

        isAlreadyLateEnter = false;
        isAlreadyAFK = false;
        currentAllowedInputs.Clear();

        stepState++;
        count = settings[(int)stepState].Delay;

        steps[(int)stepState].OnEnter();
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