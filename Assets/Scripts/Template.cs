using System.Collections.Generic;
using UnityEngine;

public class Template : MonoBehaviour
{
    public string Name { get; private set; }
    public int Lives { get; set; }
    public int WinCondition { get; set; }
    public int Turns { get; set; }
    public int Wins { get; set; }
    public UnitController[] BattleUnits { get; set; }
    public UnitController[] FreezedUnits { get; set; }
    public int Coins { get; set; }

    /// <summary>
    /// Assigns the template values.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_lives"></param>
    /// <param name="_wins"></param>
    public Template(string _name, int _lives, int _wins)
    {
        Lives = _lives;
        WinCondition = _wins;
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
        BattleUnits = new UnitController[PhaseShopUnitManager.Instance.BattleSlots.Length];
        for (int i = 0; i < BattleUnits.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.BattleSlots[i].UnitController();
            BattleUnits[i] = unit;
            if (unit != null)
            {
                unit.transform.SetParent(null);
                unit.transform.position = new Vector3(100, 100, 0);
                DontDestroyOnLoad(unit.gameObject);
            }
        }

        FreezedUnits = new UnitController[PhaseShopUnitManager.Instance.ShopUnitSlots.Length];
        for (int i = 0; i < FreezedUnits.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.ShopUnitSlots[i].UnitController();
            if (unit != null && unit.Model.ManageState == UnitState.Freezed)
            {
                FreezedUnits[i] = unit;
                unit.transform.SetParent(null);
                unit.transform.position = new Vector3(100, 100, 0);
                DontDestroyOnLoad(unit.gameObject);
            }
            else
                FreezedUnits[i] = null;
        }

        GameManager.Instance.EndShopPhase(BattleUnits, FreezedUnits);
    }


    /// <summary>
    /// Move the fainted units out of scene.
    /// </summary>
    public void HideFaintUnits(Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var unit = slots[i].UnitController();
            if (unit != null && unit.Model.IsFaint)
            {
                var ability = unit.TriggerAbility(TriggerType.Faint);
                if (ability != null)
                    ability.Activate();

                unit.transform.SetParent(null);
                unit.transform.position = new Vector3(100, 100, 0);
            }
        }
    }
}
