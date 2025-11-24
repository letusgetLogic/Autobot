using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }

    public UnitController[] BattleUnits;

    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    public void StartShop()
    {
        SetDefault();
        PhaseShopUnitManager.Instance.Initialize(this);
        PackManager.Instance.AddUnitsByTier(Data.Turns);
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.SpawnSavedUnits();
        PhaseShopUnitManager.Instance.SpawnShopUnits();
        UpdateUnitData();
        PhaseShopUnitManager.Instance.ChargeUnit();

        GameManager.Instance.SetPhaseShop();
    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        UpdateUnitData();
        GameManager.Instance.EndPhaseShop();
    }

    public void StartBattle()
    {
        BattleUnits = new UnitController[PhaseBattleController.Instance.Slots1.Length];
    }

    public void EndBattle()
    {
        UpdateTeamUnitData();
    }

    /// <summary>
    /// Sets the default value.
    /// </summary>
    public void SetDefault()
    {
        Data.Turns++;
        Data.Coins = PackManager.Instance.MyPack.CurrencyData.Capacity.Coin;
        Data.Tools = PackManager.Instance.MyPack.CurrencyData.Capacity.Tool;
    }

    /// <summary>
    /// Creates new datas and saves the data of units.
    /// </summary>
    public void UpdateUnitData()
    {
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

        Data.TeamUnitDatas = new SaveUnitData[PhaseShopUnitManager.Instance.TeamSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.TeamSlots.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.TeamSlots[i].UnitController();
            if (unit == null)
            {
                continue;
            }

            Data.TeamUnitDatas[i] = unit.Model.Data;
        }

        Data.ChargeUnitData = default;
        var chargeUnit = PhaseShopUnitManager.Instance.ChargeSlot.UnitController();
        if (chargeUnit != null)
        {
            Data.ChargeUnitData = chargeUnit.Model.Data;
        }

        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }

    /// <summary>
    /// Updates the data of team units for saving data.
    /// </summary>
    public void UpdateTeamUnitData()
    {
        for (int i = 0; i < PhaseShopUnitManager.Instance.TeamSlots.Length; i++)
        {
            if (BattleUnits[i] == null)
            {
                continue;
            }

            Data.TeamUnitDatas[i] = BattleUnits[i].Model.Data;
        }

        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }
}

