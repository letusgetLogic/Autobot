using TMPro;
using UnityEngine;

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

    public Player Player { get; private set; }

    private Currency RollCost => PackManager.Instance.MyPack.CurrencyData.RollCost;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        if (GameManager.Instance.RepairSystem == false)
        {
            toolLabel.transform.parent.parent.gameObject.SetActive(false);
        }

        SetButtonActive(null);
    }

    /// <summary>
    /// Updates template.
    /// </summary>
    public void UpdateUI(Player player)
    {
        Player = player;
        nameLabel.text = Player.Data.Name;
        nutLabel.text = Player.Data.Nuts.ToString();
        toolLabel.text = Player.Data.Tools.ToString();
        heartLabel.text = Player.Data.Lives.ToString();
        turnLabel.text = Player.Data.Turns.ToString();
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    public void UpdateCurrency(int addCoins, int addTools)
    {
        Player.Data.Nuts += addCoins;
        Player.Data.Tools += addTools;
        nutLabel.text = Player.Data.Nuts.ToString();
        toolLabel.text = Player.Data.Tools.ToString();
    }

    #region Button

    /// <summary>
    /// Roll the shop for new units and items.
    /// </summary>
    public void OnRoll()
    {
        if (!HasEnoughCurrency(RollCost.Nut, RollCost.Tool))
            return;

        UpdateCurrency(RollCost.Nut, RollCost.Tool);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
    }

    #region Manage Button

    /// <summary>
    /// Destroys the attached object and adds sell coins. 
    /// </summary>
    public void OnRepair()
    {
        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            if (!HasEnoughCurrency(unit.Model.RepairCost.Nut, unit.Model.RepairCost.Tool))
                return;

            UpdateCurrency(unit.Model.RepairCost.Nut, unit.Model.RepairCost.Tool);

            unit.Model.Repair?.RiseDurability();

            if (unit.Model.Data.Durability >= PackManager.Instance.MyPack.
                CurrencyData.HealthPortion)
            {
                repairButton.SetActive(false);
            }

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Lock the attached object in the shop. It can't be reseted in next shop.
    /// </summary>
    public void OnLock()
    {
        SetButtonActive(null);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            PhaseShopUnitManager.Instance.AttachedGameObject.
               GetComponent<UnitController>().Model.SetData(UnitState.Freezed);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Unlock the attached object in the shop. It can be replaced in next shop.
    /// </summary>
    public void OnUnlock()
    {
        SetButtonActive(null);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>().Model.SetData(UnitState.InSlotShop);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Destroys the attached object and manages recycle cost + price. 
    /// </summary>
    public void OnRecycle()
    {
        SetButtonActive(null);
        DeactivateManageButtons();

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            if (!HasEnoughCurrency(unit.Model.Sell.Nut, unit.Model.Sell.Tool))
                return;

            unit.GetSelled();
            UpdateCurrency(unit.Model.Sell.Nut, unit.Model.Sell.Tool);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Deactivates all buttons or/and activates button, which manages units und items.
    /// </summary>
    /// <param name="_manage"></param>
    public void SetButtonActive(UnitModel _unitModel)
    {
        if (_unitModel == null)
        {
            DeactivateManageButtons();
            return;
        }

        switch (_unitModel.Data.UnitState)
        {
            case UnitState.InSlotShop:
                DeactivateManageButtons();
                lockButton.SetActive(true);
                break;

            case UnitState.Freezed:
                DeactivateManageButtons();
                unlockButton.SetActive(true);
                break;

            case UnitState.InSlotTeam:
                DeactivateManageButtons();
                recycleButton.SetActive(true);
                SetButtonData(
                    recycleTool.GetComponentInChildren<TextMeshProUGUI>(),
                    recycleNut.GetComponentInChildren<TextMeshProUGUI>(),
                    _unitModel.Sell);
                break;
        }

        if (GameManager.Instance.RepairSystem)
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

    private void DeactivateManageButtons()
    {
        foreach (var button in manageButtons)
            button.SetActive(false);
    }

    #endregion


    /// <summary>
    /// Ends turn.
    /// </summary>
    public void OnEndTurn()
    {
        Player.EndShop();
    }

    #endregion

    public bool HasEnoughCurrency(int _coin, int _tool)
    {
        bool value = true;
        if (Player.Data.Nuts + _coin < 0)
        {
            HintNotEnoughCoins();
            value = false;
        }

        if (Player.Data.Tools + _tool < 0)
        {
            HintNotEnoughTools();
            value = false;
        }
        return value;
    }
    private void HintNotEnoughCoins()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(nutLabel, durationCoinsRedDefault);
    }

    private void HintNotEnoughTools()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(toolLabel, durationCoinsRedDefault);
    }

}
