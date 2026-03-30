using System.Collections;
using TMPro;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    public static CutScene Instance { get; private set; }

    [SerializeField] private RectTransform coverPanelOpen;
    [SerializeField] private RectTransform coverPanelClose;
    [SerializeField] private float delayOpen = 1f;
    [SerializeField] private float delayClose = 1f;
    [SerializeField] private LerpMovement hintClickClose;
    [SerializeField] private LerpMovement hintClick;
    [SerializeField] private TextMeshProUGUI hintClickCloseText;
    [SerializeField] private TextMeshProUGUI hintClickText;

    public ScaleUpDown OpenPanel => coverPanelOpen.GetComponent<ScaleUpDown>();
    public ScaleUpDown ClosePanel => coverPanelClose.GetComponent<ScaleUpDown>();
    public float DelayOpen => delayOpen;
    public float DelayClose => delayClose;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning(this.name + ".Awake: GameManager instance not found.");
            return;
        }

        if (coverPanelOpen != null)
        {
            if (hintClickClose && GameManager.Instance.Replay == null)
            {
                hintClickClose.gameObject.SetActive(true);
                hintClickClose.Trigger();
                EventManager.Instance.OnMoveHintClick?.Invoke();
            }

            coverPanelOpen.gameObject.SetActive(true);
            StartCoroutine(OpenScene(GameManager.Instance.Replay != null ? 0f : delayOpen));
        }

        if (coverPanelClose != null)
        {
            coverPanelClose.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Runs the animation of disappearing hint click and then runs the open scene animation.
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenScene(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        OpenPanel.ScaleUp(false);
        EventManager.Instance.OnOpenScene?.Invoke();

        //if (GameManager.Instance.SceneName == "PhaseShop")
        //    OpenPanel.OnRunningDone += () => GameManager.Instance.Replay = null;
    }

    /// <summary>
    /// Runs the close scene animation for a time then load the new scene.
    /// </summary>
    /// <param name="_scene"></param>
    public void SwitchScene(string _scene)
    {
        if (GameManager.Instance.Replay == null)
            StartCoroutine(LoadScene(_scene));
        else
            StartCoroutine(LoadSceneByReplay(_scene));
    }

    /// <summary>
    /// Loads the new scene by loading the scene directly after the close scene animation.
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    private IEnumerator LoadScene(string _scene)
    {
        yield return new WaitForSeconds(delayClose);

        GameManager.Instance.SceneToLoad = _scene;
        ClosePanel.ScaleUp(true);

        EventManager.Instance.OnCloseScene?.Invoke();

        yield return new WaitForSeconds(ClosePanel.AnimTime);

        //SceneManager.LoadScene(GameManager.Instance.SceneToLoad);

        GameManager.Instance.Switch(GameState.WaitingCutScene);

        if (hintClick != null)
        {
            hintClick.Trigger();
            EventManager.Instance.OnMoveHintClick?.Invoke();
        }
    }

    /// <summary>
    /// Loads the new scene by replaying the cutscene instead of loading the scene directly.
    /// </summary>
    /// <param name="_scene"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneByReplay(string _scene)
    {
        yield return new WaitForSeconds(delayClose);

        GameManager.Instance.SceneToLoad = _scene;
        ClosePanel.ScaleUp(true);

        EventManager.Instance.OnCloseScene?.Invoke();

        yield return new WaitForSeconds(ClosePanel.AnimTime);

        if (GameManager.Instance.Replay == null)
            GameManager.Instance.Replay = new ReplayManager();

        GameManager.Instance.Replay.Switch(GameState.LoadScene);
    }

    /// <summary>
    /// Sets the name of current player.
    /// </summary>
    /// <param name="_lookAwayPlayer"></param>
    /// <param name="_playerIsTurn"></param>
    public void SetHintClickClose(string _playerIsTurn, bool _shouldBothWatch)
    {
        if (hintClickCloseText)
        {
            if (_shouldBothWatch)
            {
                hintClickCloseText.text = "Both players should watch. Click to continue!";
                return;
            }
            hintClickCloseText.text = $"{_playerIsTurn} should click to continue and" +
                $"\r\nthe other player should look away!";
        }
    }

    /// <summary>
    /// Sets the name of current player.
    /// </summary>
    /// <param name="_lookAwayPlayer"></param>
    /// <param name="_playerIsTurn"></param>
    public void SetHintClick(string _playerIsTurn, bool _shouldBothWatch)
    {
        if (hintClickText)
        {
            if (_shouldBothWatch)
            {
                hintClickText.text = "Both players should watch. Click to continue!";
                return;
            }
            hintClickText.text = $"{_playerIsTurn} should click to continue and" +
              $"\r\nthe other player should look away!";
        }
    }
}
