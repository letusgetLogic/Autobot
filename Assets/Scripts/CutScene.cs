using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    public static CutScene Instance { get; private set; }

    [SerializeField] private RectTransform coverPanelOpen;
    [SerializeField] private RectTransform coverPanelClose;
    [SerializeField] private float delayOpen = 1f;
    [SerializeField] private float delayClose = 1f;

    public ScaleUpDown OpenPanel => coverPanelOpen.GetComponent<ScaleUpDown>();
    public ScaleUpDown ClosePanel => coverPanelClose.GetComponent<ScaleUpDown>();
    public float DelayOpen => delayOpen;
    public float DelayClose => delayClose;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;

        if (coverPanelOpen != null)
        {
            coverPanelOpen.gameObject.SetActive(true);
            StartCoroutine(OpenScene());
        }

        if (coverPanelClose != null)
        {
            coverPanelClose.gameObject.SetActive(true);
        }
    }

    private IEnumerator OpenScene()
    {
        yield return new WaitForSeconds(delayOpen);

        OpenPanel.ScaleUp(false);
    }

    public void SwitchScene(string _scene)
    {
        StartCoroutine(LoadScene(_scene));
    }

    private IEnumerator LoadScene(string _scene)
    {
        yield return new WaitForSeconds(delayClose);

        ClosePanel.ScaleUp(true);

        yield return new WaitForSeconds(ClosePanel.AnimTime);
        Debug.Log("Loading Scene: " + _scene);
        SceneManager.LoadScene(_scene);
    }
}
