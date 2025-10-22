using System;
using UnityEngine;
using static PlayerData;

public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }

    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    public void StartShop()
    {
        PhaseShopUnitManager.Instance.Initialize(this);
        PhaseShopUnitManager.Instance.TriggerStartOfTurn();
        StarterPack.Instance.AddUnitsByTier(Data.Turns);
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
        UpdateUnitID();
    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        
        UpdateUnitID();
        GameManager.Instance.EndPhaseShop();
    }

    /// <summary>
    /// Sets the default value.
    /// </summary>
    public void SetDefault()
    {
        Data.Turns++;
        Data.Coins = GameManager.Instance.StartCoins;
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

    /// <summary>
    /// Updates the ID of units for saving data.
    /// </summary>
    public void UpdateUnitID()
    {
        Data.BattleUnitDatas = new UnitData[PhaseShopUnitManager.Instance.BattleSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.BattleSlots.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.BattleSlots[i].UnitController();
            if (unit == null)
            {
                Data.BattleUnitDatas[i].Index = -1;
                continue;
            }

            Data.BattleUnitDatas[i].Index = unit.Model.Index;
            Data.BattleUnitDatas[i].XP = unit.Model.XP;
            Data.BattleUnitDatas[i].BattleHealth = unit.Model.BattleHealth;
            Data.BattleUnitDatas[i].BattleAttack = unit.Model.BattleAttack;
            Data.BattleUnitDatas[i].ManageState = unit.Model.ManageState;
        }

        Data.ShopUnitDatas = new UnitData[PhaseShopUnitManager.Instance.ShopUnitSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.ShopUnitSlots.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.ShopUnitSlots[i].UnitController();
            if (unit == null)
            {
                Data.ShopUnitDatas[i].Index = -1;
                continue;
            }

            Data.ShopUnitDatas[i].Index = unit.Model.Index;
            Data.ShopUnitDatas[i].XP = unit.Model.XP;
            Data.ShopUnitDatas[i].BattleHealth = unit.Model.BattleHealth;
            Data.ShopUnitDatas[i].BattleAttack = unit.Model.BattleAttack;
            Data.ShopUnitDatas[i].ManageState = unit.Model.ManageState;
        }
    }
}

