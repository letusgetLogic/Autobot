using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private SoTutorialProcess process;
    [SerializeField] private TutorialStep[] steps;
    [SerializeField] private float maxCountAFK = 3f;

    private int runIndex = 0;
    private float count = 0f;
    private float countAFK = 0f;

    private bool isAlreadyLateEnter = false;
    private bool isAlreadyAFK = false;

    private enum StepState
    {
        None,
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
    private StepState stepState;

    public bool TutorialCompleted
    {
        get => PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        set => PlayerPrefs.SetInt("TutorialCompleted", value ? 1 : 0);
    }

    public bool ShouldClickForNextStep { get; private set; } = true;

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

        stepState = StepState.Welcome;
    }

    private void Update()
    {
        if (count < process.Delays[runIndex])
        {
            count += Time.deltaTime;
        }
        else if (isAlreadyLateEnter == false)
        {
            steps[runIndex].OnLateEnter();
            isAlreadyLateEnter = true;
        }



        if (countAFK < maxCountAFK)
        {
            countAFK += Time.deltaTime;
        }
        else if (isAlreadyAFK == false)
        {
            steps[runIndex].OnAFKAnimator();
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
        steps[runIndex].OnExit();

        count = 0f;
        runIndex++;
        stepState++;

        isAlreadyLateEnter = false;
        isAlreadyAFK = false;

        if (stepState == StepState.ClickRobot)
            ShouldClickForNextStep = false;

        steps[runIndex].OnEnter();
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

