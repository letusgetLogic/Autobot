using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }

    [Header("Replay Button")]
    [SerializeField] private GameObject replayButton;
    [SerializeField] private GameObject tutorialButton;

    private bool wasTutorialButtonSetted = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Time.timeScale = 1.0f;
       
        InputManager.Instance.BlocksInput = false;

        StartCoroutine(Settings());
    }


    /// <summary>
    /// This is done after ensuring that the GameManager instance is available, 
    /// as it may not be immediately accessible when the main menu is loaded.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Settings()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        GameManager.Instance.Replay = null;
        replayButton.SetActive(GameManager.Instance.CurrentRound != null);

        if (wasTutorialButtonSetted == false)
            tutorialButton.SetActive(GameManager.Instance.IsTutorialCompleted == false);
    }

    public void SetTutorialButton()
    {
        tutorialButton.SetActive(false);
        wasTutorialButtonSetted = true;
    }

    public void OnPlay()
    {
        Debug.Log(PlayerPrefs.GetInt("TutorialCompleted", 0));
        if (GameManager.Instance.IsTutorialCompleted == false)
        {
            GameManager.Instance.LoadGame(GameMode.Tutorial);
        }
    }

    public void OnPlayTutorial()
    {
        GameManager.Instance.SetTutorialCompleted(false);
        GameManager.Instance.LoadGame(GameMode.Tutorial);
    }
}
