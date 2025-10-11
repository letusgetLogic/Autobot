using UnityEngine;

public class Template
{
    public string Name { get; private set; }
    public int Health { get; set; }
    public int WinsCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public Slot[] BattleSlots { get; set; }
    public Slot[] FreezedUnitSlots { get; set; }
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
        PhaseShopUnitManager.Instance.Initialize(this);
        PhaseShopUnitManager.Instance.TriggerStartOfTurn();
        StarterPack.Instance.AddUnitsByTier(Turns);
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
    }

    public void EndShop()
    {
        BattleSlots = new Slot[PhaseShopUnitManager.Instance.BattleSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.BattleSlots.Length; i++)
        {
            BattleSlots[i] = PhaseShopUnitManager.Instance.BattleSlots[i];
        }

        FreezedUnitSlots = new Slot[PhaseShopUnitManager.Instance.ShopUnitSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.ShopUnitSlots.Length; i++)
        {
            FreezedUnitSlots[i] = PhaseShopUnitManager.Instance.ShopUnitSlots[i];
        }

        GameManager.Instance.EndShopPhase();
    }
}
