using UnityEngine;

public class Template
{
    public string Name { get; private set; }
    public int Health { get; set; }
    public int WinsCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public GameObject[] BattleUnits { get; set; }
    public int Coins { get; set; }

    /// <summary>
    /// Assigns the template values.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_health"></param>
    /// <param name="_wins"></param>
    public Template(string _name, int _health, int _wins)
    {
        Health = _health;
        WinsCondition = _wins;
        Turns = 0;
        Name = _name;
    }

    public void StartShop()
    {
        Turns++;
        Coins = PhaseShopUI.Instance.StartCoins;
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.TriggerStartOfTurn();
        StarterPack.Instance.AddUnitsByTier(Turns);
        PhaseShopUI.Instance.Roll();
    }
}
