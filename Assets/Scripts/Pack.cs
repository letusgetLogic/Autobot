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

    public void UnCheck()
    {
        Check.SetActive(false);
    }

    public void SelectPack()
    {
        GameSettings.Instance.UnCheckAll();

        Check.SetActive(true);
        PackManager.Instance.InitPack(soPack);
    }
}
