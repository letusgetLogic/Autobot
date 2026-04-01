using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelConfirmation : MonoBehaviour
{
    private enum Type
    {
        None,
        LeftCurrency,
        ToMenu
    }

    public Button OnContinueDeclined;
    public Button OnContinueConfirmed;

    [SerializeField] private Type type;
    [SerializeField] private TextMeshProUGUI tool, nut;
    [SerializeField] private List<GameObject> leftCurrencyComponents;
    [SerializeField] private List<GameObject> toMenuComponents;

    public static readonly int MinLeftTool = 1, MinLeftNut = 5;

    private void OnEnable()
    {
        leftCurrencyComponents.ForEach(x => x.SetActive(type == Type.LeftCurrency));
        toMenuComponents.ForEach(x => x.SetActive(type == Type.ToMenu));

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

        switch (type)
        {
            case Type.LeftCurrency:
                PhaseShopUI.Instance.Player.EndShop();
                break;
            case Type.ToMenu:
                if (GameManager.Instance.CurrentGame != null)
                {
                    GameManager.Instance.CurrentGame.State = GameState.EndOfGame;
                }
                GameManager.Instance.LoadScene("Menu");
                break;
        }

    }
}
