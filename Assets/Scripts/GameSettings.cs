using System.Collections;
using TMPro;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }


    [Header("Mode Development")]
    public bool IsModeDevelop;
    
    [SerializeField] private int lives = 6;
    [SerializeField] private float timer = 90.0f;

    private string name1 = "Player 1";
    private string name2 = "Player 2";

    [Header("References")]
    [SerializeField] private TMP_InputField modeLocalDuelHeart;
    [SerializeField] private Pack[] packs;
    [SerializeField] private TMP_InputField inputName1;
    [SerializeField] private TMP_InputField inputName2;
    [SerializeField] private TextMeshProUGUI hint;

    [Header("Game Settings")]
    [SerializeField] private int minLives = 3;
    [SerializeField] private int maxLives = 10;

    [Header("Settings")]
    [SerializeField] private float durationColorDefault = 0.2f;
    [SerializeField] private float durationHintDefault = 0.5f;

    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;
    }

    private void Start()
    {
        GameManager.Instance.Name1 = name1;
        GameManager.Instance.Name2 = name2;

        if (IsModeDevelop)
        {
            PackManager.Instance.InitPack(packs[0].SoPack);
            GameManager.Instance.Mode = GameMode.Local1v1;
            GameManager.Instance.PlayerLives = lives;
        
            GameManager.Instance.LoadGame();
        }
    }

    /// <summary>
    /// Button click calls. set game mode to local 1v1.
    /// </summary>
    public void OnModeLocal1v1()
    {
        GameManager.Instance.Mode = GameMode.Local1v1;
    }

    /// <summary>
    /// Unchecks all packs.
    /// </summary>
    public void UnCheckAllPacks()
    {
        for (int i = 0; i < packs.Length; i++)
        {
            packs[i].UnCheck();
        }
    }

    /// <summary>
    /// Start game with selected settings.  
    /// </summary>
    public void StartGame()
    {
        switch (GameManager.Instance.Mode)
        {
            case GameMode.Local1v1:

                if (PackManager.Instance.MyPack == null)
                {
                    hint.text = "Select a pack!";
                    hint.enabled = true;
                    StartCoroutine(Hide(hint, durationHintDefault));
                    return;
                }

                if (inputName1.text != "")
                    GameManager.Instance.Name1 = inputName1.text;

                if (inputName2.text != "")
                    GameManager.Instance.Name2 = inputName2.text;

                int b;
                if (int.TryParse(modeLocalDuelHeart.text, out b))
                    b = int.Parse(modeLocalDuelHeart.text);
                else
                {
                    hint.text = "Enter a number of lives!";
                    hint.enabled = true;
                    StartCoroutine(Hide(hint, durationHintDefault));
                    return;
                }
                if (b < minLives || b > maxLives)
                {
                    HintInvalid(modeLocalDuelHeart);
                    return;
                }

                GameManager.Instance.PlayerLives = b;
                GameManager.Instance.LoadGame();

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

