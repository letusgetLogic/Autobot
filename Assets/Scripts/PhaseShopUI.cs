using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseShopUI : MonoBehaviour
{
    public static PhaseShopUI Instance { get; private set; }

    [SerializeField]
    private int
       startCoins = 11,
       rollCost = 1;

    public int StartCoins => startCoins;

    [SerializeField]
    private TextMeshProUGUI
        nameText,
        coinsText,
        heartText,
        turnText,
        trophyText;

    [SerializeField]
    private GameObject
        sellButton,
        freezeButton,
        unfreezeButton;

    private Template player { get; set; }

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
    public void UpdateUI(Template _template)
    {
        player = _template;
        nameText.text = player.Name;
        coinsText.text = player.Coins.ToString();
        heartText.text = player.Lives.ToString();
        turnText.text = player.Turns.ToString();
        trophyText.text = player.Wins.ToString() + " / " + player.WinsCondition.ToString();
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    /// <param name="amount">The current amount of coins.</param>
    public void UpdateCoin(int deviation)
    {
        player.Coins += deviation;
        coinsText.text = player.Coins.ToString();
    }

    #region Button

    /// <summary>
    /// Roll the shop for new units and items.
    /// </summary>
    public void Roll()
    {
        if (player.Coins < rollCost)
        {
            // hint no enough coins
            return;
        }

        UpdateCoin(-rollCost);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
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

    #region Manage Button

    /// <summary>
    /// Freeze the attached object.
    /// </summary>
    public void Freeze()
    {
        SetButtonActive(UnitState.None);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
        {
            PhaseShopUnitManager.Instance.AttachedGameObject.
               GetComponent<UnitController>().GetFrezzed();

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
                GetComponent<UnitController>().GetUnfrezzed();

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
            UpdateCoin(unit.Model.CurrentLevel.Sell);

            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        }
    }

    #endregion

    /// <summary>
    /// Ends turn.
    /// </summary>
    public void EndTurn()
    {
        player.EndShop();
    }

    #endregion
}
