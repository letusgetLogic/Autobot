using System.Collections.Generic;

public class Template
{
    public string Name { get; private set; }
    public int Lives { get; set; }
    public int WinsCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public Slot[] TeamSlots { get; set; }
    public Slot[] FreezedUnitSlots { get; set; }
    public int Coins { get; set; }
    public List<UnitController> FaintUnits { get; set; }

    /// <summary>
    /// Assigns the template values.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_lives"></param>
    /// <param name="_wins"></param>
    public Template(string _name, int _lives, int _wins)
    {
        Lives = _lives;
        WinsCondition = _wins;
        Turns = 0;
        Name = _name;
    }

    /// <summary>
    /// Starts the phase shop.
    /// </summary>
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

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        TeamSlots = new Slot[PhaseShopUnitManager.Instance.BattleSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.BattleSlots.Length; i++)
        {
            TeamSlots[i] = PhaseShopUnitManager.Instance.BattleSlots[i];
        }

        FreezedUnitSlots = new Slot[PhaseShopUnitManager.Instance.ShopUnitSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.ShopUnitSlots.Length; i++)
        {
            FreezedUnitSlots[i] = PhaseShopUnitManager.Instance.ShopUnitSlots[i];
        }

        GameManager.Instance.EndShopPhase(TeamSlots, FreezedUnitSlots);
    }
}
