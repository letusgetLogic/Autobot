using TMPro;
using UnityEngine;

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

    private Template player { get; set; }

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
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
}
