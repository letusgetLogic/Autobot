using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI showLives;
    [SerializeField] private Pack[] packs;
    [SerializeField] private TMP_InputField inputName1;
    [SerializeField] private TMP_InputField inputName2;
    [SerializeField] private TextMeshProUGUI hint;
    [SerializeField] private Button startButton;

    [Header("Game Settings")]
    [SerializeField] private int minLives = 3;
    [SerializeField] private int defaultLives = 5;
    [SerializeField] private int maxLives = 10;



    [Header("Settings")]
    [SerializeField] private float durationColorDefault = 0.2f;
    [SerializeField] private float durationHintDefault = 0.5f;

    public SoPack DefaultPack => packs[0].SoPack;    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    /// <summary>
    /// Button click calls. set game mode to local 1v1.
    /// </summary>
    public void OnModeLocal1v1()
    {
        GameManager.Instance.Mode = GameMode.Local1v1;
        PackManager.Instance.InitPack(packs[0].SoPack);

        if (GameManager.Instance.PlayerLives == default)
        {
            GameManager.Instance.PlayerLives = defaultLives;
            showLives.text = defaultLives.ToString();
        }
        else
        {
            showLives.text = GameManager.Instance.PlayerLives.ToString();
        }
    }

    /// <summary>
    /// Increases the play coins.
    /// </summary>
    public void OnLivesUp()
    {
        int choice = GameManager.Instance.PlayerLives + 1;

        if (choice > maxLives)
        {
            hint.text = "Highest play energy reached!";
            hint.enabled = true;
            StartCoroutine(Hide(hint, durationHintDefault));
            EventManager.Instance.OnInvalidInput?.Invoke();
            return;
        }

        GameManager.Instance.PlayerLives = choice;
        showLives.text = choice.ToString();

        EventManager.Instance.OnIncreaseLives?.Invoke();
    }

    /// <summary>
    /// Decreases the play coins.
    /// </summary>
    public void OnLivesDown()
    {
        int choice = GameManager.Instance.PlayerLives - 1;

        if (choice < minLives)
        {
            hint.text = "Lowest play energy reached!";
            hint.enabled = true;
            StartCoroutine(Hide(hint, durationHintDefault));
            EventManager.Instance.OnInvalidInput?.Invoke();
            return;
        }

        GameManager.Instance.PlayerLives = choice;
        showLives.text = choice.ToString();

        EventManager.Instance.OnDecreaseLives?.Invoke();
    }

    ///// <summary>
    ///// Unchecks all packs.
    ///// </summary>
    //public void UnCheckAllPacks()
    //{
    //    for (int i = 0; i < packs.Length; i++)
    //    {
    //        packs[i].UnCheck();
    //    }
    //}

    /// <summary>
    /// Start game with selected settings.  
    /// </summary>
    public void StartGame()
    {
        startButton.interactable = true;

        switch (GameManager.Instance.Mode)
        {
            case GameMode.Local1v1:

                //if (PackManager.Instance.MyPack == null)
                //{
                //    hint.text = "Select a pack!";
                //    hint.enabled = true;
                //    StartCoroutine(Hide(hint, durationHintDefault));
                //    EventManager.Instance.OnInvalidInput?.Invoke();
                //    return;
                //}

                if (inputName1.text != "")
                    GameManager.Instance.Name1 = inputName1.text;

                if (inputName2.text != "")
                    GameManager.Instance.Name2 = inputName2.text;

                startButton.interactable = false;
                GameManager.Instance.LoadGame(GameMode.Local1v1);

                break;
        }
    }

    /// <summary>
    /// Hint invalid input.
    /// </summary>
    /// <param name="_target"></param>
    public void HintInvalid(TMP_InputField _target)
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(_target, durationColorDefault);
    }

    /// <summary>
    /// Hides the targeted component TextMeshProUGUI with a delay.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_delay"></param>
    /// <returns></returns>
    public IEnumerator Hide(TextMeshProUGUI _target, float _delay)
    {
        yield return new WaitForSeconds(_delay);
        _target.enabled = false;
    }


}

