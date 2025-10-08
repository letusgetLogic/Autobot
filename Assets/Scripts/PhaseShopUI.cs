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
        heartText.text = player.Health.ToString();
        turnText.text = player.Turns.ToString();
        trophyText.text = player.Wins.ToString() + " / " + player.WinsCondition.ToString();
    }

    /// <summary>
    /// Updates the coins display.
    /// </summary>
    /// <param name="amount">The current amount of coins.</param>
    public void UpdateUI(int _coins)
    {
        coinsText.text = _coins.ToString();
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

        player.Coins -= rollCost;
        UpdateUI(player.Coins);
        PhaseShopUnitManager.Instance.SpawnUnits();
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

    /// <summary>
    /// Freeze the attached object.
    /// </summary>
    public void Freeze()
    {
        SetButtonActive(UnitState.None);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
            PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>().GetFrezzed();

    }

    /// <summary>
    /// Unfreeze the attached object.
    /// </summary>
    public void Unfreeze()
    {
        SetButtonActive(UnitState.None);

        if (PhaseShopUnitManager.Instance.AttachedGameObject.CompareTag("Unit"))
            PhaseShopUnitManager.Instance.AttachedGameObject.
                GetComponent<UnitController>().GetUnfrezzed();

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
            player.Coins += unit.Model.CurrentLevel.Sell;
            UpdateUI(player.Coins);
        }
    }

    #endregion
}
