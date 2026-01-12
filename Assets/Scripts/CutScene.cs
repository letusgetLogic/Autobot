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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;

        if (coverPanelOpen != null && coverPanelOpen.gameObject.activeSelf)
            StartCoroutine(OpenScene());
    }

    private IEnumerator OpenScene()
    {
        yield return new WaitForSeconds(delayOpen);

        var openScene = coverPanelOpen.GetComponent<ScaleUpDown>();
        openScene.ScaleUp(false);
    }

    public void SwitchScene(string _scene)
    {
        StartCoroutine(LoadScene(_scene));
    }

    private IEnumerator LoadScene(string _scene)
    {
        yield return new WaitForSeconds(delayClose);

        var closeScene = coverPanelClose.GetComponent<ScaleUpDown>();
        closeScene.ScaleUp(true);

        yield return new WaitForSeconds(closeScene.AnimTime);
        Debug.Log("Loading Scene: " + _scene);
        SceneManager.LoadScene(_scene);
    }
}
