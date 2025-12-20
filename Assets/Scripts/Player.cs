using System.Collections;
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
        Data.Turn++;
        SetDefault();
        PhaseShopUnitManager.Instance.Initialize(this);
        PackManager.Instance.AddUnitsByTier(Data.Turn);
        PhaseShopUI.Instance.UpdateUI(this);
        PhaseShopUnitManager.Instance.SpawnSavedUnits();
        PhaseShopUnitManager.Instance.SpawnShopUnits();
        UpdateUnitData();
        PhaseShopUnitManager.Instance.StartCoroutine(PhaseShopUnitManager.Instance.DelayChargeBots());
        PhaseShopUI.Instance.SetChargingEnergyAt(Data.Turn);

        GameManager.Instance.SetPhaseShop();
    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        float delay = 0f;

        if (Data.Turn == 1)
            delay = PhaseShopUnitManager.Instance.ChargeBots();

        PhaseShopUnitManager.Instance.StartCoroutine(DelayEndShop(delay));
    }

    /// <summary>
    /// Delays ending the shop phase for charging units at turn 1.
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator DelayEndShop(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        UpdateUnitData();
        GameManager.Instance.EndPhaseShop();
    }

    /// <summary>
    /// Start of the battle executes.
    /// </summary>
    public void StartBattle()
    {
        BattleUnits = new UnitController[PhaseBattleController.Instance.Slots1.Length];
    }

    /// <summary>
    /// End of the battle executes.
    /// </summary>
    public void EndBattle()
    {
        UpdateTeamUnitData();
    }

    /// <summary>
    /// Sets the default value.
    /// </summary>
    public void SetDefault()
    {
        Data.Nuts = PackManager.Instance.MyPack.CurrencyData.Capacity.Nut;
        Data.Tools = PackManager.Instance.MyPack.CurrencyData.Capacity.Tool;
    }

    /// <summary>
    /// Creates new datas and saves the data of units.
    /// </summary>
    public void UpdateUnitData()
    {
        Data.ShopUnitDatas = new SaveUnitData[PhaseShopUnitManager.Instance.ShopBotSlots.Length];
        for (int i = 0; i < PhaseShopUnitManager.Instance.ShopBotSlots.Length; i++)
        {
            var unit = PhaseShopUnitManager.Instance.ShopBotSlots[i].UnitController();
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

