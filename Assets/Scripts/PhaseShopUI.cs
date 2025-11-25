using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PhaseShopUI : MonoBehaviour
{
    public static PhaseShopUI Instance { get; private set; }

    [SerializeField]
    private float durationCoinsRedDefault = 0.2f;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI
        coinText,
        toolText,
        heartText,
        turnText;

    [Header("Manage Buttons")]
    [SerializeField] private GameObject[] manageButtons;
    [SerializeField]
    private GameObject
        repairButton,
        sellButton,
        freezeButton,
        unfreezeButton;

    public Player Player { get; private set; }

    private Currency RollCost => PackManager.Instance.MyPack.CurrencyData.RollCost;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        SetButtonActive(default);
    }

    /// <summary>
    /// Updates template.
    /// </summary>
    public void UpdateUI(Player player)
    {
        Player = player;
        nameText.text = Player.Data.Name;
        coinText.text = Player.Data.Coins.ToString();
        toolText.text = Player.Data.Tools.ToString();
        heartText.text = Player.Data.Lives.ToString();
        turnText.text = Player.Data.Turns.ToString();
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    public void UpdateCurrency(int addCoins, int addTools)
    {
        Player.Data.Coins += addCoins;
        Player.Data.Tools += addTools;
        coinText.text = Player.Data.Coins.ToString();
        toolText.text = Player.Data.Tools.ToString();
    }

    #region Button

    /// <summary>
    /// Roll the shop for new units and items.
    /// </summary>
    public void Roll()
    {
        if (!HasEnoughCurrency(RollCost.Coin, RollCost.Tool))
            return;

        UpdateCurrency(RollCost.Coin, RollCost.Tool);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
    }

    #region Manage Button

    /// <summary>
    /// Destroys the attached object and adds sell coins. 
    /// </summary>
    public void Repair()
    {
        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            if (!HasEnoughCurrency(unit.Model.RepairCost.Coin, unit.Model.RepairCost.Tool))
                return;

            UpdateCurrency(unit.Model.RepairCost.Coin, unit.Model.RepairCost.Tool);

            unit.Repair();
            if (unit.Model.Data.Durability >= PackManager.Instance.MyPack.
                CurrencyData.HealthPortion)
            {
                repairButton.SetActive(false);
            }

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Freeze the attached object.
    /// </summary>
    public void Freeze()
    {
        SetButtonActive(default);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            PhaseShopUnitManager.Instance.AttachedGameObject.
               GetComponent<UnitController>().Model.SetData(UnitState.Freezed);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Unfreeze the attached object.
    /// </summary>
    public void Unfreeze()
    {
        SetButtonActive(default);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>().Model.SetData(UnitState.InSlotShop);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Destroys the attached object and adds sell coins. 
    /// </summary>
    public void Sell()
    {
        SetButtonActive(default);
        DeactivateManageButtons();

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            if (!HasEnoughCurrency(unit.Model.Sell.Coin, unit.Model.Sell.Tool))
                return;

            unit.GetSelled();
            UpdateCurrency(unit.Model.Sell.Coin, unit.Model.Sell.Tool);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Deactivates all buttons and activates button, which manages units und items.
    /// </summary>
    /// <param name="_manage"></param>
    public void SetButtonActive(SaveUnitData _unitData)
    {
        switch (_unitData.UnitState)
        {
            default:
                DeactivateManageButtons();
                return;

            case UnitState.InSlotShop:
                DeactivateManageButtons();
                freezeButton.SetActive(true);
                break;

            case UnitState.Freezed:
                DeactivateManageButtons();
                unfreezeButton.SetActive(true);
                break;

            case UnitState.InSlotTeam:
                DeactivateManageButtons();
                sellButton.SetActive(true);
                break;
        }

        float durability = _unitData.DurabilityRatio;
        if (durability < 1f)
        {
            repairButton.SetActive(true);
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
    public void EndTurn()
    {
        Player.EndShop();
    }

    #endregion

    public bool HasEnoughCurrency(int _coin, int _tool)
    {
        bool value = true;
        if (Player.Data.Coins + _coin < 0)
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

        markColorRed.SetComponent(coinText, durationCoinsRedDefault);
    }

    private void HintNotEnoughTools()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(toolText, durationCoinsRedDefault);
    }

}
