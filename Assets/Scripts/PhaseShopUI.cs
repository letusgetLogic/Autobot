using TMPro;
using UnityEngine;

public class PhaseShopUI : MonoBehaviour
{
    public static PhaseShopUI Instance { get; private set; }

    [SerializeField]
    private float durationCoinsRedDefault = 0.2f;

    [SerializeField]
    private TextMeshProUGUI
        nameText,
        coinsText,
        toolText,
        heartText,
        turnText,
        trophyText;

    [SerializeField]
    private GameObject
        trophyLabel,
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
    /// <param name="_coins"></param>
    /// <param name="_hearth"></param>
    /// <param name="_turn"></param>
    /// <param name="_trophy"></param>
    /// <param name="max_trophy"></param>
    public void UpdateUI(Player player)
    {
        Player = player;
        nameText.text = Player.Data.Name;
        coinsText.text = Player.Data.Coins.ToString();
        heartText.text = Player.Data.Lives.ToString();
        turnText.text = Player.Data.Turns.ToString();

        if (Player.Data.WinCondition <= 0)
        {
            trophyLabel.SetActive(false);
            return;
        }
        trophyText.text = Player.Data.Wins.ToString() + " / " + Player.Data.WinCondition.ToString();
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    /// <param name="amount">The current amount of coins.</param>
    public void UpdateCoin(int deviation)
    {
        Player.Data.Coins += deviation;
        coinsText.text = Player.Data.Coins.ToString();
    }

    public void HintNotEnoughCoins()
    {
        var markColorRed = GetComponent<MarkColorRed>();
        if (markColorRed == null)
            markColorRed = gameObject.AddComponent<MarkColorRed>();

        markColorRed.SetComponent(coinsText, durationCoinsRedDefault);
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
    /// Activates button, which manages units und items, or deactives with ManageButton.None.
    /// </summary>
    /// <param name="manageButton"></param>
    public void SetButtonActive(UnitState manageButton)
    {
        switch (manageButton)
        {
            case UnitState.None:
                sellButton.SetActive(false);
                freezeButton.SetActive(false);
                unfreezeButton.SetActive(false);
                break;

            case UnitState.InSlotShop:
                sellButton.SetActive(false);
                freezeButton.SetActive(true);
                unfreezeButton.SetActive(false);
                break;

            case UnitState.Freezed:
                sellButton.SetActive(false);
                freezeButton.SetActive(false);
                unfreezeButton.SetActive(true);
                break;

            case UnitState.InSlotBattle:
                sellButton.SetActive(true);
                freezeButton.SetActive(false);
                unfreezeButton.SetActive(false);
                break;
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

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            var unit = PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>();

            unit.GetSelled();
            UpdateCoin(unit.CurrentLevel.Sell);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
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
}
