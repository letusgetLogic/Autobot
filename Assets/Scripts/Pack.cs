using TMPro;
using UnityEngine;

public class Pack : MonoBehaviour
{
    public SoPack SoPack => soPack;
    [SerializeField] private SoPack soPack;
    [SerializeField] private TextMeshProUGUI packNameLabel;

    //public GameObject Check => transform.Find("V").gameObject;

    private void OnValidate()
    {
        if (soPack != null)
        {
            gameObject.name = soPack.name;

            if (packNameLabel != null)
                packNameLabel.text = soPack.name;
        }
    }

    ///// <summary>
    ///// Initialize Check.
    ///// </summary>
    //public void InitCheck()
    //{
    //    bool isChecked = false;

    //    if (Check)
    //    {
    //        isChecked = Check.activeSelf;
    //        Check.SetActive(isChecked);
    //    }
    //}

    ///// <summary>
    ///// Uncheck the pack.
    ///// </summary>
    //public void UnCheck()
    //{
    //    if (Check) 
    //        Check.SetActive(false);
    //}

    ///// <summary>
    ///// Select the pack.
    ///// </summary>
    //public void SelectPack()
    //{
    //    //GameSettings.Instance.UnCheckAllPacks();

    //    //if (Check)
    //    //    Check.SetActive(true);
    //    PackManager.Instance.InitPack(soPack);
    //}
}
