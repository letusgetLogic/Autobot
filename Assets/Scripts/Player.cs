using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }

    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    public void StartShop()
    {
        SetDefault();
        PhaseShopUnitManager.Instance.Initialize(this);
        PackManager.Instance.AddUnitsByTier(Data.Turns);
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.SpawnShopUnits();
        UpdateUnitData();
    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        UpdateUnitData();
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
    /// Updates the data of units for saving data.
    /// </summary>
    public void UpdateUnitData()
    {
        Data.BattleUnitDatas = new SaveUnitData[PhaseShopUnitManager.Instance.BattleSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.BattleSlots.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.BattleSlots[i].UnitController();
            if (unit == null)
            {
                continue;
            }

            Data.BattleUnitDatas[i] = unit.Model.Data;
        }

        Data.ShopUnitDatas = new SaveUnitData[PhaseShopUnitManager.Instance.ShopUnitSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.ShopUnitSlots.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.ShopUnitSlots[i].UnitController();
            if (unit == null)
            {
                continue;
            }

            Data.ShopUnitDatas[i] = unit.Model.Data;
        }

        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }
}

