using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelLeftCurrency : MonoBehaviour
{
    public Button OnContinueDeclined;
    public Button OnContinueConfirmed;

    [SerializeField] private TextMeshProUGUI tool, nut;

    public static readonly int MinLeftTool = 1, MinLeftNut = 5;

    private void OnEnable()
    {
        OnContinueDeclined.interactable = true;
        OnContinueDeclined.onClick.AddListener(Decline);
        OnContinueConfirmed.interactable = true;
        OnContinueConfirmed.onClick.AddListener(Confirm);
    }


    public bool IsEnough(int _tool, int _nut)
    {
        tool.text = _tool.ToString();
        nut.text = _nut.ToString();

        return _tool >= MinLeftTool || _nut >= MinLeftNut;
    }

    private void Decline()
    {
        OnContinueDeclined.interactable = false;
        gameObject.SetActive(false);
        GameManager.Instance.IsBlockingInput = false;
    }

    private void Confirm()
    {
        OnContinueConfirmed.interactable = false;
        gameObject.SetActive(false);
        PhaseShopUI.Instance.Player.EndShop();
    }
}
