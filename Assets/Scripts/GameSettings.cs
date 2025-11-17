using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public TMP_InputField ModeSingleHeart;

    [Header("Mode Development")]
    public bool IsModeDevelop;
    
    [SerializeField] private int lives = 6;
    [SerializeField] private float timer = 90.0f;

    private string name1 = "Player 1";
    private string name2 = "Player 2";

    [Header("References")]
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

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.Name1 = name1;
        GameManager.Instance.Name2 = name2;

        if (IsModeDevelop)
        {
            PackManager.Instance.InitPack(packs[0].SoPack);
            GameManager.Instance.Mode = GameMode.Single;
            GameManager.Instance.PlayerLives = lives;
        
            GameManager.Instance.LoadGame();
        }
    }

    public void OnModeSingle()
    {
        GameManager.Instance.Mode = GameMode.Single;
    }


    public void UnCheckAll()
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
            case GameMode.Single:

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
                if (int.TryParse(ModeSingleHeart.text, out b))
                    b = int.Parse(ModeSingleHeart.text);
                else
                {
                    hint.text = "Enter a number of lives!";
                    hint.enabled = true;
                    StartCoroutine(Hide(hint, durationHintDefault));
                    return;
                }
                if (b < minLives || b > maxLives)
                {
                    HintInvalid(ModeSingleHeart);
                    return;
                }

                GameManager.Instance.PlayerLives = b;
                GameManager.Instance.LoadGame();

                break;
        }
    }


    public void HintInvalid(TMP_InputField target)
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(target, durationColorDefault);
    }

    public IEnumerator Hide(TextMeshProUGUI target, float delay)
    {
        yield return new WaitForSeconds(delay);
        target.enabled = false;
    }


}

