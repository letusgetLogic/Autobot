using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelConfirmation : MonoBehaviour
{
    private enum Type
    {
        None,
        LeftCurrency,
        ToMenu
    }
    public enum Result
    {
        None,
        Running,
        Declined,
        Confirmed
    }
    public Result MyResult { get; private set; } = Result.None;

    public Button OnContinueDeclined;
    public Button OnContinueConfirmed;
    public UnityEvent OnCancelConfirmed;

    // you can use type to define each panel display and outcome handling,
    // if type is default (None), the panel can return the event or/and result. 
    [SerializeField] private Type type;
    [SerializeField] private TextMeshProUGUI tool, nut;
    [SerializeField] private List<GameObject> leftCurrencyComponents;
    [SerializeField] private List<GameObject> defaultComponents;

    public UnityAction ActionOnDeclined { get; set; }
    public UnityAction ActionOnConfirmed { get; set; }

    private void Start()
    {
        OnContinueDeclined.onClick.AddListener(Decline);
        OnContinueConfirmed.onClick.AddListener(Confirm);
    }

    private void OnEnable()
    {
        InputManager.Instance.BlocksInput = true;
        MyResult = Result.Running;

        leftCurrencyComponents.ForEach(x => x.SetActive(type == Type.LeftCurrency));
        defaultComponents.ForEach(x => x.SetActive(type == Type.ToMenu || type == Type.None));

        OnContinueDeclined.interactable = true;
        OnContinueConfirmed.interactable = true;
    }

    private void OnDisable()
    {
        InputManager.Instance.BlocksInput = false;
        MyResult = Result.None;
    }

    public void SetData(int _tool, int _nut)
    {
        tool.text = _tool.ToString();
        nut.text = _nut.ToString();
    }

    private void Decline()
    {
        OnContinueDeclined.interactable = false;

        MyResult = Result.Declined;
        OnCancelConfirmed?.Invoke();
        ActionOnDeclined?.Invoke();
        ActionOnDeclined = null;

        gameObject.SetActive(false);
        InputManager.Instance.BlocksInput = false;
    }

    private void Confirm()
    {
        OnContinueConfirmed.interactable = false;
        ActionOnConfirmed?.Invoke();
        MyResult = Result.Confirmed;

        gameObject.SetActive(false);

        switch (type)
        {
            case Type.LeftCurrency:
                //PhaseShopController.Instance.EndShop();
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
