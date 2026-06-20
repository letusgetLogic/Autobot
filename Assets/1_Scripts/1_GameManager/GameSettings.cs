using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI showLivesAI;
    [SerializeField] private TextMeshProUGUI showLivesPvP;
    [SerializeField] private Pack[] packs;
    [SerializeField] private TMP_InputField inputName1;
    [SerializeField] private TMP_InputField inputPvpName1;
    [SerializeField] private TMP_InputField inputPvpName2;
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
    /// Button click calls. set game mode to AI.
    /// </summary>
    public void OnModeAI()
    {
        GameManager.Instance.Mode = GameMode.AI;
        PackManager.Instance.InitPack(packs[0].SoPack);

        if (GameManager.Instance.PlayerLives == default)
        {
            GameManager.Instance.PlayerLives = defaultLives;
            showLivesAI.text = defaultLives.ToString();
        }
        else
        {
            showLivesAI.text = GameManager.Instance.PlayerLives.ToString();
        }
    }

    /// <summary>
    /// Button click calls. set game mode to local 1v1.
    /// </summary>
    public void OnModeLocal1v1()
    {
        GameManager.Instance.Mode = GameMode.PvP;
        PackManager.Instance.InitPack(packs[0].SoPack);

        if (GameManager.Instance.PlayerLives == default)
        {
            GameManager.Instance.PlayerLives = defaultLives;
            showLivesPvP.text = defaultLives.ToString();
        }
        else
        {
            showLivesPvP.text = GameManager.Instance.PlayerLives.ToString();
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
        showLivesAI.text = choice.ToString();
        showLivesPvP.text = choice.ToString();

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
        showLivesAI.text = choice.ToString();
        showLivesPvP.text = choice.ToString();

        EventManager.Instance.OnDecreaseLives?.Invoke();
    }

    public void OnRandomName1()
    {
        inputName1.text = NameList.GetRandomExclusive(new[] { inputName1.text });
    }

    public void OnRandomPvpName1()
    {
        inputPvpName1.text = NameList.GetRandomExclusive(new[] { inputPvpName1.text, inputPvpName2.text } );
    }

    public void OnRandomPvpName2()
    {
        inputPvpName2.text = NameList.GetRandomExclusive(new[] { inputPvpName1.text, inputPvpName2.text } );
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
            case GameMode.AI:
                
                if (inputName1.text != "")
                    GameManager.Instance.Name1 = inputName1.text;
                else
                    GameManager.Instance.Name1 = "Player 1";

                startButton.interactable = false;
                GameManager.Instance.LoadGame(GameMode.AI);
                break;

            case GameMode.PvP:

                //if (PackManager.Instance.MyPack == null)
                //{
                //    hint.text = "Select a pack!";
                //    hint.enabled = true;
                //    StartCoroutine(Hide(hint, durationHintDefault));
                //    EventManager.Instance.OnInvalidInput?.Invoke();
                //    return;
                //}

                if (inputPvpName1.text != "")
                    GameManager.Instance.Name1 = inputPvpName1.text;
                else
                    GameManager.Instance.Name1 = "Player 1";

                if (inputPvpName2.text != "")
                    GameManager.Instance.Name2 = inputPvpName2.text;
                else
                    GameManager.Instance.Name2 = "Player 2";

                startButton.interactable = false;
                GameManager.Instance.LoadGame(GameMode.PvP);
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

        markColorRed.SetComponent(_target, _target.textComponent.color, durationColorDefault, null);
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
