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
        GameManager.Instance.IsBlockingInput = false;

        StartCoroutine(SetReplayButton());
    }


    private IEnumerator SetReplayButton()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        replayButton.SetActive(GameManager.Instance.CurrentRound != null);
    }
}
