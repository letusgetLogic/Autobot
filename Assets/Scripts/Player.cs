using Unity.Collections;
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
        PhaseShopUnitManager.Instance.StartCoroutine(PhaseShopUnitManager.Instance.ChargeUnit());

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
        Data.Nuts = PackManager.Instance.MyPack.CurrencyData.Capacity.Nut;
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
            Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;

            // Temporary buff ends at the end of battle.

            int hp = Data.TeamUnitDatas[i].Cur.HP - Data.TeamUnitDatas[i].TempBuff.HP;
            int atk = Data.TeamUnitDatas[i].Cur.ATK - Data.TeamUnitDatas[i].TempBuff.ATK;

            Data.TeamUnitDatas[i].SetTempBuffHP(0);
            Data.TeamUnitDatas[i].SetTempBuffATK(0);

            if (GameManager.Instance.IsRepairSystemActive)
            {
                Data.TeamUnitDatas[i].SetHP(hp < 0 ? 0 : hp, null);
                Data.TeamUnitDatas[i].SetATK(atk < 0 ? 0 : atk);
            }
            else
            {
                Data.TeamUnitDatas[i].SetHP(Data.TeamUnitDatas[i].FullHP, null);
                Data.TeamUnitDatas[i].SetATK(Data.TeamUnitDatas[i].FullATK);
            }
        }

        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }
}

