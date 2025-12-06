using TMPro;
using UnityEngine;

public class Pack : MonoBehaviour
{
    public SoPack SoPack => soPack;
    [SerializeField] private SoPack soPack;
    [SerializeField] private TextMeshProUGUI packNameLabel;

    public GameObject Check => transform.Find("V").gameObject;

    private void OnValidate()
    {
        if (soPack != null)
        {
            gameObject.name = soPack.name;

            if (packNameLabel != null)
                packNameLabel.text = soPack.name;
        }
    }

    /// <summary>
    /// Uncheck the pack.
    /// </summary>
    public void UnCheck()
    {
        Check.SetActive(false);
    }

    /// <summary>
    /// Select the pack.
    /// </summary>
    public void SelectPack()
    {
        GameSettings.Instance.UnCheckAllPacks();

        Check.SetActive(true);
        PackManager.Instance.InitPack(soPack);
    }
}
