using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseShopUI : MonoBehaviour
{
    public static PhaseShopUI Instance { get; private set; }

    [SerializeField]
    private float durationCoinsRedDefault = 0.2f;

    [Header("Label Components")]
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField]
    private TextMeshProUGUI
        turnLabel,
        heartLabel,
        nutLabel,
        toolLabel;
    [SerializeField] private GameObject energyBonusLabel;

    [Header("Roll Button")]
    [SerializeField] private TextMeshProUGUI rollCostNut;
    [SerializeField] private TextMeshProUGUI rollCostTool;

    [Header("Manage Buttons")]
    [SerializeField] private GameObject[] manageButtons;
    [SerializeField]
    private GameObject
        repairButton,
        recycleButton,
        lockButton,
        unlockButton;
    [SerializeField]
    private TextMeshProUGUI
        repairTool,
        repairNut,
        recycleTool,
        recycleNut;

    [Header("Replay Button")]
    [SerializeField] private GameObject replayButton;

    [Header("Charging Station")]
    [SerializeField] private GameObject chargingStation;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private int turnToEnable = 1;


    public Player Player { get; private set; }

    private Currency rollCost => PackManager.Instance.MyPack.CurrencyData.RollCost;

    private ColorBlock defaultColorBlock;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Settings();
    }

    /// <summary>
    /// Setup the UI screen.
    /// </summary>
    private void Settings()
    {
        if (GameManager.Instance.IsRepairSystemActive == false)
        {
            toolLabel.transform.parent.parent.gameObject.SetActive(false);
        }

        replayButton.SetActive(GameManager.Instance.CurrentRound != null);

        defaultColorBlock = new ColorBlock();
        SetButtonActive(null);
    }

    /// <summary>
    /// Updates template.
    /// </summary>
    public void UpdateUI(Player _player)
    {
        Player = _player;
        nameLabel.text = Player.Data.Name;
        nutLabel.text = Player.Data.Nuts.ToString();
        toolLabel.text = Player.Data.Tools.ToString();
        heartLabel.text = Player.Data.Lives.ToString();
        turnLabel.text = Player.Data.Turn.ToString();

        SetButtonData(rollCostTool, rollCostNut, rollCost);
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    public void UpdateCurrency(int _addNuts, int _addTools)
    {
        Player.Data.Nuts += _addNuts;
        Player.Data.Tools += _addTools;
        nutLabel.text = Player.Data.Nuts.ToString();
        toolLabel.text = Player.Data.Tools.ToString();
    }

    /// <summary>
    /// Sets the charging station.
    /// </summary>
    /// <param name="_turn"></param>
    public void SetChargingStationAt(int _turn)
    {
        if (_turn >= turnToEnable)
        {
            chargingStation.SetActive(true);
        }
        else
        {
            chargingStation.SetActive(false);
        }

            switch (_turn)
            {
                case 0:
                    break;
                case 1:
                    energyBonusLabel.SetActive(true);
                    break;
                case >= 2:
                    energyBonusLabel.SetActive(false);
                    break;
            }
        energyText.text = "+" + PackManager.Instance.MyPack.ChargingEnergy.Value.ToString();
    }

    #region Buttons

    /// <summary>
    /// Roll the shop for new units and items.
    /// </summary>
    public void OnRoll()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (!HasEnoughCurrency(rollCost.Nut, rollCost.Tool, true))
            return;

        UpdateCurrency(rollCost.Nut, rollCost.Tool);
        PhaseShopController.Instance.SpawnShopUnits();

        EventManager.Instance.OnRoll?.Invoke();
    }

    /// <summary>
    /// Ends turn.
    /// </summary>
    public void OnEndTurn()
    {
        Debug.Log("End Turn Button Clicked");
        if (GameManager.Instance.IsBlockingInput)
            return;

        GameManager.Instance.IsBlockingInput = true;
        Player.EndShop();

        EventManager.Instance.OnEndTurn?.Invoke();
    }

    #region Manage Buttons

    /// <summary>
    /// Destroys the attached object and adds sell coins. 
    /// </summary>
    public void OnRepair()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (PhaseShopController.Instance.AttachedController.CompareTag("Unit"))
        {
            var unit = PhaseShopController.Instance.AttachedController;

            if (!HasEnoughCurrency( unit.Model.RepairCost.Nut, unit.Model.RepairCost.Tool, true))
                return;

            UpdateCurrency(unit.Model.RepairCost.Nut, unit.Model.RepairCost.Tool);

            unit.Model.Repair?.RiseDurability();

            SetButtonActive(unit.Model);

            EventManager.Instance.OnRepair?.Invoke();
        }
    }

    /// <summary>
    /// Lock the attached object in the shop. It can't be reseted in next shop.
    /// </summary>
    public void OnLock()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (PhaseShopController.Instance.AttachedController.CompareTag("Unit"))
        {
            var unit = PhaseShopController.Instance.AttachedController; 

            unit.Model.SetData(UnitState.Freezed);

            SetButtonActive(unit.Model);

            EventManager.Instance.OnLock?.Invoke();
        }
    }

    /// <summary>
    /// Unlock the attached object in the shop. It can be replaced in next shop.
    /// </summary>
    public void OnUnlock()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (PhaseShopController.Instance.AttachedController.CompareTag("Unit"))
        {
            var unit = PhaseShopController.Instance.AttachedController;

            unit.Model.SetData(UnitState.InSlotShop);

            SetButtonActive(unit.Model);

            EventManager.Instance.OnUnlock?.Invoke();
        }
    }

    /// <summary>
    /// Destroys the attached object and manages recycle cost + price. 
    /// </summary>
    public void OnRecycle()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        GameManager.Instance.IsBlockingInput = true;

        SetButtonActive(null);
        DeactivateManageButtons();

        if (PhaseShopController.Instance.AttachedController.CompareTag("Unit"))
        {
            var unit = PhaseShopController.Instance.AttachedController;

            if (!HasEnoughCurrency(unit.Model.Sell.Nut, unit.Model.Sell.Tool, true))
                return;

            unit.GetSelled();
            UpdateCurrency(unit.Model.Sell.Nut, unit.Model.Sell.Tool);

            PhaseShopController.Instance.SetAttachedGameObject(null);

            EventManager.Instance.OnRecycle?.Invoke();
        }
        else GameManager.Instance.IsBlockingInput = false;
    }

    /// <summary>
    /// Deactivates all buttons or/and activates button, which manages units und items.
    /// </summary>
    /// <param name="_manage"></param>
    public void SetButtonActive(UnitModel _unitModel)
    {
        DeactivateManageButtons();

        if (_unitModel == null)
            return;

        switch (_unitModel.Data.UnitState)
        {
            case UnitState.InSlotShop:
                lockButton.SetActive(true);
                break;

            case UnitState.Freezed:
                unlockButton.SetActive(true);
                break;

            case UnitState.InSlotTeam:
                recycleButton.SetActive(true);
                SetButtonData(
                    recycleTool.GetComponentInChildren<TextMeshProUGUI>(),
                    recycleNut.GetComponentInChildren<TextMeshProUGUI>(),
                    _unitModel.Sell);
                break;

            case UnitState.InSlotCharge:
                recycleButton.SetActive(true);
                SetButtonData(
                    recycleTool.GetComponentInChildren<TextMeshProUGUI>(),
                    recycleNut.GetComponentInChildren<TextMeshProUGUI>(),
                    _unitModel.Sell);
                break;
        }

        if (GameManager.Instance.IsRepairSystemActive && _unitModel.IsRobot())
        {
            float durability = _unitModel.Data.DurabilityRatio;
            if (durability < 1f)
            {
                repairButton.SetActive(true);
                SetButtonData(
                    repairTool.GetComponentInChildren<TextMeshProUGUI>(),
                    repairNut.GetComponentInChildren<TextMeshProUGUI>(),
                    _unitModel.RepairCost);
            }
        }
    }

    /// <summary>
    /// Sets the currency for the referenced text components if the button.
    /// </summary>
    /// <param name="_tool"></param>
    /// <param name="_nut"></param>
    /// <param name="_currency"></param>
    public void SetButtonData(TextMeshProUGUI _tool, TextMeshProUGUI _nut, Currency _currency)
    {
        if (_currency.Tool == 0)
        {
            _tool.text = "";
            _tool.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _tool.transform.parent.gameObject.SetActive(true);
            _tool.text = (_currency.Tool > 0 ? "+" : "") + _currency.Tool.ToString();
        }

        if (_currency.Nut == 0)
        {
            _nut.text = "";
            _nut.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _nut.transform.parent.gameObject.SetActive(true);
            _nut.text = (_currency.Nut > 0 ? "+" : "") + _currency.Nut.ToString();
        }
    }

    /// <summary>
    /// Deactivates all manage buttons.
    /// </summary>
    private void DeactivateManageButtons()
    {
        foreach (var button in manageButtons)
            button.SetActive(false);
    }

    #endregion


    #endregion

    /// <summary>
    /// Compares the currencies. 
    /// </summary>
    /// <param name="_nuts"></param>
    /// <param name="_tools"></param>
    /// <param name="_activeHint"> It can</param>
    /// <returns></returns>
    public bool HasEnoughCurrency(int _nuts, int _tools, bool _activeHint)
    {
        bool value = true;

       if (Player.Data.Nuts + _nuts < 0)
        {
            if (_activeHint)
                HintNotEnoughCoins();
            value = false;
        }

        if (Player.Data.Tools + _tools < 0)
        {
            if (_activeHint)
                HintNotEnoughTools();
            value = false;
        }

        if (_activeHint && value == false)
        {
            EventManager.Instance.OnNotEnoughCurrency?.Invoke();
        }

        return value;
    }

    /// <summary>
    /// Hints not enough coins.
    /// </summary>
    public void HintNotEnoughCoins()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(nutLabel, durationCoinsRedDefault);
    }

    /// <summary>
    /// Hints not enough tools.
    /// </summary>
    private void HintNotEnoughTools()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(toolLabel, durationCoinsRedDefault);
    }

}
