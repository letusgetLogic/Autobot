using TMPro;
using UnityEngine;

public class PhaseShopUI : MonoBehaviour
{
    public static PhaseShopUI Instance { get; private set; }

    [SerializeField]
    private float durationCoinsRedDefault = 0.2f;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI
        coinsText,
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

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        SetButtonActive(UnitState.None);
    }

    /// <summary>
    /// Updates template.
    /// </summary>
    public void UpdateUI(Player player)
    {
        Player = player;
        nameText.text = Player.Data.Name;
        coinsText.text = Player.Data.Coins.ToString();
        toolText.text = Player.Data.Tools.ToString();
        heartText.text = Player.Data.Lives.ToString();
        turnText.text = Player.Data.Turns.ToString();
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    public void UpdateCoin(int deviation)
    {
        Player.Data.Coins += deviation;
        coinsText.text = Player.Data.Coins.ToString();
    }

    /// <summary>
    /// Updates the tool display.
    /// </summary>
    public void UpdateTool(int deviation)
    {
        Player.Data.Tools += deviation;
        toolText.text = Player.Data.Tools.ToString();
    }

    #region Button

    /// <summary>
    /// Roll the shop for new units and items.
    /// </summary>
    public void Roll()
    {
        if (Player.Data.Coins < GameManager.Instance.RollCost)
        {
            HintNotEnoughCoins();
            return;
        }

        UpdateCoin(-GameManager.Instance.RollCost);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
    }

    #region Manage Button

    /// <summary>
    /// Destroys the attached object and adds sell coins. 
    /// </summary>
    public void Repair()
    {
        if (Player.Data.Tools <= 0)
        {
            HintNotEnoughTools();
            return;
        }

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            UpdateTool(-1);

            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            unit.Repair();
            if (unit.Model.Data.Durability >= PackManager.Instance.MyPack.
                HealthPortion.Value)
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
        SetButtonActive(UnitState.None);

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
        SetButtonActive(UnitState.None);

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
        SetButtonActive(UnitState.None);
        repairButton.SetActive(false);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            unit.GetSelled();
            UpdateCoin(unit.Model.CurrentLevel.Sell);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    /// <summary>
    /// Activates button, which manages units und items, or deactives with ManageButton.None.
    /// </summary>
    /// <param name="manageButton"></param>
    public void SetButtonActive(UnitState manageButton)
    {
        switch (manageButton)
        {
            case UnitState.None:
                DeactivateManageButtons();
                unfreezeButton.SetActive(false);
                break;

            case UnitState.InSlotShop:
                DeactivateManageButtons();
                freezeButton.SetActive(true);
                break;

            case UnitState.Freezed:
                DeactivateManageButtons();
                unfreezeButton.SetActive(true);
                break;

            case UnitState.InSlotBattle:
                DeactivateManageButtons();
                sellButton.SetActive(true);
                break;
        }
    }

    private void DeactivateManageButtons()
    {
        foreach (var button in manageButtons)
            button.SetActive(false);
    }

    public void SetRepairButtonActiv()
    {
        repairButton.SetActive(true);
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


    public void HintNotEnoughCoins()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(coinsText, durationCoinsRedDefault);
    }

    private void HintNotEnoughTools()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(toolText, durationCoinsRedDefault);
    }

}
