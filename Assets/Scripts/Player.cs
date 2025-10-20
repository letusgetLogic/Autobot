using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }


    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    public void StartShop()
    {
        Data.Turns++;
        Data.Coins = PhaseShopUI.Instance.StartCoins;
        PhaseShopUnitManager.Instance.Initialize(Data);
        PhaseShopUnitManager.Instance.TriggerStartOfTurn();
        StarterPack.Instance.AddUnitsByTier(Data.Turns);
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        Data.BattleUnits = new UnitController[PhaseShopUnitManager.Instance.BattleSlots.Length];
        for (int i = 0; i < Data.BattleUnits.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.BattleSlots[i].UnitController();
            Data.BattleUnits[i] = unit;
            if (unit != null)
            {
                unit.transform.SetParent(null);
                unit.transform.position = new Vector3(100, 100, 0);
                DontDestroyOnLoad(unit.gameObject);
            }
        }

        Data.FreezedUnits = new UnitController[PhaseShopUnitManager.Instance.ShopUnitSlots.Length];
        for (int i = 0; i < Data.FreezedUnits.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.ShopUnitSlots[i].UnitController();
            if (unit != null && unit.Model.ManageState == UnitState.Freezed)
            {
                Data.FreezedUnits[i] = unit;
                unit.transform.SetParent(null);
                unit.transform.position = new Vector3(100, 100, 0);
                DontDestroyOnLoad(unit.gameObject);
            }
            else
                Data.FreezedUnits[i] = null;
        }

        GameManager.Instance.EndShopPhase(Data.BattleUnits, Data.FreezedUnits);
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

