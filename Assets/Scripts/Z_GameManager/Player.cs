using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public PlayerData Data;

    public Player()
    {
        Data = new PlayerData("Player", 5, 0);
    }

    public IEnumerator ExecuteByTutorialAI()
    {
        yield return new WaitUntil(() => PhaseShopController.Instance != null &&
                                        PhaseShopController.Instance.ShopBotSlots().Length > 0);

        Data.Turn++;
        SetDefault();
        PackManager.Instance.AssignList(Data.Turn);
        BuildTeamByAI();
        GameManager.Instance.Switch(GameState.EndOfTurn);
    }

    private void BuildTeamByAI()
    {
        if (Data.TeamUnitDatas == null)
            Data.TeamUnitDatas = new SaveUnitData[PhaseShopController.Instance.TeamSlots().Length];

        List<UnitController> teamUnits = new();
        List<int> teamUnitsPos = new List<int>();
        List<int> fullDurPos = new List<int>();

        // Repair
        int repairTools = Data.Tools - PhaseShopController.Instance.ShopBotSlots().Length;

        for (int j = 0; j < Data.TeamUnitDatas.Length; j++)
        {
            var unitData = Data.TeamUnitDatas[j];
            if (unitData != null)
            {
                var unit = PhaseShopController.Instance.AddUnitController(
                    PackManager.Instance.GetSoUnit(unitData),
                    unitData.Index,
                    unitData,
                    UnitState.InSlotTeam
                    );

                teamUnits.Add(unit);
                teamUnitsPos.Add(j);

                if (unit.Model.IsFullDurability() == false)
                {
                    if (repairTools > 0)
                    {
                        unit.Model.Repair?.RiseDurability();
                        repairTools--;
                    }
                }
                else fullDurPos.Add(j);
            }
        }

        var shopBots = PhaseShopController.Instance.GetRandomShopBots();

        switch (Data.Turn)
        {
            case 1:
                for (int i = 0; i < shopBots.Length; i++)
                {
                    Data.TeamUnitDatas[i] = shopBots[i].Model.Data;
                    Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;
                }
                break;
            case int a when a >= 2:
                int index = 0;
                for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
                {
                    if (Data.TeamUnitDatas[i] == null)
                    {
                        Data.TeamUnitDatas[i] = shopBots[index].Model.Data;
                        Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;
                        fullDurPos.Add(i);
                        index++;
                    }
                }
                int leaderInTemp = Random.Range(0, teamUnits.Count);

                var leaderBase = PhaseShopController.Instance.AddUnitController(
                   teamUnits[leaderInTemp].Model.SoUnit,
                   teamUnits[leaderInTemp].Model.Data.Index,
                   teamUnits[leaderInTemp].Model.Data,
                   UnitState.InSlotTeam
                   );

                teamUnits[leaderInTemp].UpdateLevel(leaderBase.Model.Data, true);

                int leaderPosInTeam = teamUnitsPos[leaderInTemp];
                (Data.TeamUnitDatas[0], Data.TeamUnitDatas[leaderPosInTeam]) = 
                    (Data.TeamUnitDatas[leaderPosInTeam], Data.TeamUnitDatas[0]);
                break;
            case 3:
                //int index = 0;
                //for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
                //{
                //    if (Data.TeamUnitDatas[i] == null)
                //    {
                //        Data.TeamUnitDatas[i] = shopBots[index].Model.Data;
                //        Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;
                //        fullDurPos.Add(i);
                //        index++;
                //    }
                //}
                //int leaderPos = fullDurPos[Random.Range(0, fullDurPos.Count)];
                //var leader = PhaseShopController.Instance.AddUnitController(
                //    null,
                //    Data.TeamUnitDatas[leaderPos].Index,
                //    Data.TeamUnitDatas[leaderPos],
                //    UnitState.InSlotTeam
                //    );

                //SoUnit soUnit = PackManager.Instance.Bots[Data.TeamUnitDatas[leaderPos].Index];
                //var leaderBase = PhaseShopController.Instance.AddUnitController(
                //   soUnit,
                //   Data.TeamUnitDatas[leaderPos].Index,
                //   null,
                //   UnitState.InSlotTeam
                //   );

                //leader.UpdateLevel(leaderBase.Model.Data, true);
                //(Data.TeamUnitDatas[0], Data.TeamUnitDatas[leaderPos]) = (Data.TeamUnitDatas[leaderPos], Data.TeamUnitDatas[0]);
                break;
        }

        // charge at end of shop
        foreach (var unit in Data.TeamUnitDatas)
            if(unit != null)
                 unit.SetEnergy(unit.Cur.ENG + 1);
    }

    /// <summary>
    /// Starts the phase shop.
    /// </summary>
    public void StartShop()
    {
        Data.Turn++;
        SetDefault();

        var phaseShop = PhaseShopController.Instance;
        if (phaseShop == null)
        {
            Debug.LogError("PhaseShop is null");
            return;
        }

        phaseShop.Initialize(this);

    }

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        EventManager.Instance.OnEndShop?.Invoke();

        var phaseShop = PhaseShopController.Instance;
        if (phaseShop == null)
        {
            Debug.LogError("PhaseShop is null");
            return;
        }

        phaseShop.SetAttachedGameObject(null);

        float delay = 0f;

        delay = phaseShop.Process.DurationCharging + phaseShop.Process.DelayStartBattleAfterEndTurn;
        phaseShop.ChargeTeamBots();

        endShopCoroutine = phaseShop.StartCoroutine(DelayEndShop(delay));
    }
    private Coroutine endShopCoroutine;

    /// <summary>
    /// Delays ending the shop phase for charging units at turn 1.
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator DelayEndShop(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        UpdateUnitData();
        GameManager.Instance.Switch(GameState.EndOfTurn);
        endShopCoroutine = null;
    }

    /// <summary>
    /// Saves the data of units from shop phase for replaying the battle phase. This is used for replaying the battle phase after watching the replay of the battle phase.
    /// </summary>
    public void SaveDataByReplay()
    {
        var phaseShop = PhaseShopController.Instance;
        if (phaseShop == null)
        {
            Debug.LogError("PhaseShop is null");
            return;
        }

        phaseShop.SetAttachedGameObject(null);
        UpdateUnitData();
    }

    /// <summary>
    /// Loads the data of units from shop phase for replaying the battle phase. This is used for replaying the battle phase after watching the replay of the battle phase.
    /// </summary>
    public void LoadDataByReplay()
    {
        var phaseShop = PhaseShopController.Instance;
        if (phaseShop == null)
        {
            Debug.LogError("PhaseShop is null");
            return;
        }

        phaseShop.Initialize(this);
    }

    /// <summary>
    /// Start of the battle executes.
    /// </summary>
    public void StartBattle()
    {

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
    /// Creates new datas and saves the data of units from shop phase.
    /// </summary>
    public void UpdateUnitData()
    {
        var shopBotSlots = PhaseShopController.Instance.ShopBotSlots();
        Data.ShopBotDatas = new SaveUnitData[shopBotSlots.Length];
        for (int i = 0; i < Data.ShopBotDatas.Length; i++)
        {
            var unit = shopBotSlots[i].UnitController();
            if (unit == null)
                continue;

            Data.ShopBotDatas[i] = unit.Model.Data;
        }

        var shopItemSlots = PhaseShopController.Instance.ShopItemSlots();
        Data.ShopItemDatas = new SaveUnitData[shopItemSlots.Length];
        for (int i = 0; i < Data.ShopItemDatas.Length; i++)
        {
            var unit = shopItemSlots[i].UnitController();
            if (unit == null)
                continue;

            Data.ShopItemDatas[i] = unit.Model.Data;
        }

        var teamSlots = PhaseShopController.Instance.TeamSlots();
        Data.TeamUnitDatas = new SaveUnitData[teamSlots.Length];
        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
        {
            var unit = teamSlots[i].UnitController();
            if (unit == null)
                continue;

            Data.TeamUnitDatas[i] = unit.Model.Data;
        }

        Data.ChargeUnitData = default;
        var chargeUnit = PhaseShopController.Instance.ChargeSlot.UnitController();
        if (chargeUnit != null)
        {
            Data.ChargeUnitData = chargeUnit.Model.Data;
        }
        //if (GameManager.Instance.CurrentRound != null)
        //Debug.Log("currentRound.SavedPlayerData1.TeamUnitDatas[0].HP " + GameManager.Instance.CurrentRound.SavedPlayerData1.TeamUnitDatas[0].Cur.HP);
        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }

    /// <summary>
    /// Updates the data of team units for saving data from battle phase.
    /// </summary>
    public void UpdateTeamUnitData()
    {
        if (Data.TeamUnitDatas == null)
            return;

        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
        {
            if (Data.TeamUnitDatas[i] == null)
                continue;

            Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;

            // Temporary buff ends at the end of battle.

            if (GameManager.Instance.IsRepairSystemActive)
            {
                int hp = Data.TeamUnitDatas[i].Cur.HP - Data.TeamUnitDatas[i].TempBuff.HP;
                int atk = Data.TeamUnitDatas[i].Cur.ATK - Data.TeamUnitDatas[i].TempBuff.ATK;

                Data.TeamUnitDatas[i].SetHP(hp < 0 ? 0 : hp, null);
                Data.TeamUnitDatas[i].SetATK(atk < 0 ? 0 : atk);
            }
            else
            {
                int hp = Data.TeamUnitDatas[i].FullHP - Data.TeamUnitDatas[i].TempBuff.HP;
                int atk = Data.TeamUnitDatas[i].FullATK - Data.TeamUnitDatas[i].TempBuff.ATK;

                Data.TeamUnitDatas[i].SetHP(hp < 0 ? 0 : hp, null);
                Data.TeamUnitDatas[i].SetATK(atk < 0 ? 0 : atk);
            }

            Data.TeamUnitDatas[i].SetTempBuffHP(0);
            Data.TeamUnitDatas[i].SetTempBuffATK(0);
        }
        //if (GameManager.Instance.CurrentRound != null)
        //    Debug.Log("currentRound.SavedPlayerData1.TeamUnitDatas[0].HP " + GameManager.Instance.CurrentRound.SavedPlayerData1.TeamUnitDatas[0].Cur.HP);
        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }
}



