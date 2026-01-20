using System.Collections;
using System.Diagnostics;
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

        var phaseShop = PhaseShopUnitManager.Instance;
        if (phaseShop == null)
            return;

        phaseShop.Initialize(this);

    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        var phaseShop = PhaseShopUnitManager.Instance;
        if (phaseShop == null)
            return;

        float delay = 0f;

        //// from turn 2 charge energy at start of shop.
        //if (Data.Turn == 1)
        //{
        //    delay = phaseShop.Process.DurationCharging + phaseShop.Process.DelayStartBattleAfterEndTurn;
        //    phaseShop.ChargeTeamBots();
        //}
        //else
        //{
        //    delay = phaseShop.Process.DelayStartBattleAfterEndTurn;
        //}

        delay = phaseShop.Process.DurationCharging + phaseShop.Process.DelayStartBattleAfterEndTurn;
        phaseShop.ChargeTeamBots();

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
        GameManager.Instance.Switch(GameState.EndOfTurn, null);
    }

    /// <summary>
    /// Start of the battle executes.
    /// </summary>
    public void StartBattle()
    {
        BattleUnits = new UnitController[PhaseBattleController.Instance.Slots1().Length];
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
        var shopBotSlots = PhaseShopUnitManager.Instance.ShopBotSlots();
        Data.ShopBotDatas = new SaveUnitData[shopBotSlots.Length];
        for (int i = 0; i < Data.ShopBotDatas.Length; i++)
        {
            var unit = shopBotSlots[i].UnitController();
            if (unit == null)
            {
                continue;
            }

            Data.ShopBotDatas[i] = unit.Model.Data;
        }

        var shopItemSlots = PhaseShopUnitManager.Instance.ShopItemSlots();
        Data.ShopItemDatas = new SaveUnitData[shopItemSlots.Length];
        for (int i = 0; i < Data.ShopItemDatas.Length; i++)
        {
            var unit = shopItemSlots[i].UnitController();
            if (unit == null)
            {
                continue;
            }

            Data.ShopItemDatas[i] = unit.Model.Data;
        }

        var teamSlots = PhaseShopUnitManager.Instance.TeamSlots();
        Data.TeamUnitDatas = new SaveUnitData[teamSlots.Length];
        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
        {
            var unit = teamSlots[i].UnitController();
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
        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
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



