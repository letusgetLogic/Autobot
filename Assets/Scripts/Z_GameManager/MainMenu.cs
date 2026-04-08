using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }


    [Header("Replay Button")]
    [SerializeField] private GameObject replayButton;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Time.timeScale = 1.0f;

        GameManager.Instance.Replay = null;
        InputManager.Instance.BlocksInput = false;

        StartCoroutine(SetReplayButton());
    }


    /// <summary>
    /// Sets the replay button active if there is a current round in the GameManager, allowing the player to replay the last round they played. 
    /// This is done after ensuring that the GameManager instance is available, as it may not be immediately accessible when the main menu is loaded.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetReplayButton()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        replayButton.SetActive(GameManager.Instance.CurrentRound != null);
    }

    public void OnPlay()
    {
        if (!PlayerPrefs.HasKey("TutorialCompleted"))
        {
            GameManager.Instance.StartTutorial();
        }
    }
}
