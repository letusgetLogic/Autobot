using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Searcher;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        GameManager.Instance.Log("ExecuteByTutorialAI");
        yield return new WaitUntil(() => PhaseShopController.Instance != null);

        Data.Turn++;
        SetDefault();
        PackManager.Instance.AssignList(Data.Turn);
        BuildTeamByAI();

        GameManager.Instance.Switch(GameState.EndOfTurn);
    }
    private void DebugTeamUnit(string _context)
    {
        GameManager.Instance.Log("___" + _context + "___");
        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
        {
            GameManager.Instance.Log("TeamUnit: " + i + " - " +
                (Data.TeamUnitDatas[i] == null ? "null" : (""
                + Data.TeamUnitDatas[i].ID + " | "
                + Data.TeamUnitDatas[i].XP + " /6" + " | "
                + Data.TeamUnitDatas[i].Durability + " /3" + " | "
                + Data.TeamUnitDatas[i].Cur.ATK + "/" + Data.TeamUnitDatas[i].Cur.HP + "/" + Data.TeamUnitDatas[i].Cur.ENG + " | "
                )
                )
                );
        }
    }
    private void DebugUnit(string _context, int i, SaveUnitData _data)
    {
        GameManager.Instance.Log(_context + i + " - "
                + _data.ID + " | "
                + _data.XP + " /6" + " | "
                + _data.Durability + " /3" + " | "
                + _data.Cur.ATK + "/" + _data.Cur.HP + "/" + _data.Cur.ENG + " | "
                );
    }

    private void BuildTeamByAI()
    {
        List<UnitController> teamUnits = new();
        int repairTools = Data.Tools - PhaseShopController.Instance.ShopBotSlots.Count;

        if (Data.TeamUnitDatas == null)
        {
            Data.TeamUnitDatas = new SaveUnitData[PhaseShopController.Instance.TeamSlots.Count];
            DebugTeamUnit("BuildTeamByAI");
        }
        else
        {
            DebugTeamUnit("BuildTeamByAI");
            GameManager.Instance.Log("PackManager.Instance.Bots " + (PackManager.Instance.Bots == null ? "null" : "Count" + PackManager.Instance.Bots.Count));
            // add controller
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
                }
            }
        }

        var shopBots = PhaseShopController.Instance.GetRandomShopBots();

        switch (Data.Turn)
        {
            case 1:
                for (int i = 0; i < shopBots.Count; i++)
                {
                    Data.TeamUnitDatas[i] = shopBots[i].Model.Data;
                    Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;
                }
                break;
            case 2:
                // shuffle / set priority based on position
                teamUnits.Shuffle();
                Data.TeamUnitDatas = new SaveUnitData[PhaseShopController.Instance.TeamSlots.Count];
                for (int i = 0; i < teamUnits.Count; i++)
                    Data.TeamUnitDatas[i] = teamUnits[i].Model.Data;

                DebugTeamUnit("BuildTeamByAI - Shuffle");

                // Repair
                for (int i = 0; i < PackManager.Instance.MyPack.CurrencyData.HealthPortion; i++)
                {
                    for (int j = 0; j < teamUnits.Count && repairTools > 0; j++)
                    {
                        if (teamUnits[j].Model.Data.Durability == i)
                        {
                            DebugUnit("repair: ", j, teamUnits[j].Model.Data);
                            repairTools -= teamUnits[j].Model.Repair.RiseDurability();
                            DebugUnit("repair: ", j, teamUnits[j].Model.Data);
                        }
                    }
                }
                DebugTeamUnit("BuildTeamByAI - Repair");

                // fill from shop
                for (int i = 0; i < Data.TeamUnitDatas.Length && shopBots.Count > 0; i++)
                {
                    if (Data.TeamUnitDatas[i] == null)
                    {
                        Data.TeamUnitDatas[i] = shopBots[shopBots.Count - 1].Model.Data;
                        Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;
                        shopBots.RemoveAt(shopBots.Count - 1);
                    }
                }
                var leader = teamUnits[0];
                var soUnit = PackManager.Instance.GetSoUnit(leader.Model.Data);
                SaveUnitData leaderBase = null;
                try
                {
                    leaderBase = new SaveUnitData(
                            1,
                         new Attribute(soUnit.Health, soUnit.Attack, soUnit.Energy.Value),
                         new Attribute(soUnit.Health, soUnit.Attack, soUnit.Energy.Value),
                         new Attribute(),
                         new Attribute()
                );

                }
                catch (System.Exception ex)
                {
                    GameManager.Instance.LogError(ex.Message);
                }
                if (leaderBase != null)
                    teamUnits[0].UpdateLevel(leaderBase, false);
                break;

            case int turn when turn >= 3:
                List<UnitController> recycleBots = new();
                // search & fusion same models in team and in shop
                for (int i = 0; i < teamUnits.Count - 1; i++)
                {
                    var target = teamUnits[i];
                    if (target == null) continue;

                    string name = target.Model.SoUnit.Name;
                    var sameInShop = shopBots.Where(z => z.Model.SoUnit.Name == name).ToList();
                    int hasSame = sameInShop.Count;

                    for (int j = i + 1; j < teamUnits.Count; j++) // in team
                    {
                        var search = teamUnits[j];
                        if (search == null) continue;
                        if (search.Model.SoUnit.Name == name)
                        {
                            bool levelUp = false;
                            int needRepair = search.Model.Repair.RepairAmount + target.Model.Repair.RepairAmount;
                            if (needRepair > 0)
                            {
                                if (repairTools >= needRepair) // repair both
                                {
                                    DebugUnit("repair: ", i, target.Model.Data);
                                    while (target.Model.Repair.RepairAmount > 0) 
                                    { repairTools -= target.Model.Repair.RiseDurability(); }
                                    DebugUnit("repair: ", i, target.Model.Data);
                                    DebugUnit("repair: ", j, search.Model.Data);
                                    while (search.Model.Repair.RepairAmount > 0) 
                                    { repairTools -= search.Model.Repair.RiseDurability(); }
                                    DebugUnit("repair: ", j, search.Model.Data);
                                }
                                else // fusion target with shop if possible
                                {
                                    if (repairTools >= target.Model.Repair.RepairAmount && sameInShop.Count > 0) // repair target
                                    {
                                        while (target.Model.Repair.RepairAmount == 0) 
                                        { repairTools -= target.Model.Repair.RiseDurability(); }
                                        DebugUnit("target: ", i, target.Model.Data);
                                        for (int k = 0; k < sameInShop.Count && shopBots.Count > EmptySlots(); k++)
                                        {
                                            DebugUnit("fusion with shop: ", k, sameInShop[k].Model.Data);
                                            levelUp = target.UpdateLevel(sameInShop[k].Model.Data, false);
                                            repairTools += levelUp ? 1 : 0;
                                            shopBots.RemoveAll(z => z == sameInShop[k]);
                                            DebugUnit("target: ", i, target.Model.Data);
                                        }
                                    }
                                    continue;
                                }
                            }
                            // fusion same in team
                            DebugUnit("target: ", i, target.Model.Data);
                            DebugUnit("fusion with: ", j, search.Model.Data);
                            levelUp = target.UpdateLevel(search.Model.Data, false);
                            repairTools += levelUp ? 1 : 0;
                            DebugUnit("target: ", i, target.Model.Data);
                            // set references to null
                            SetNullInTeam(search.Model.Data);
                            teamUnits.ForEach(x => { if (x == search) x = null; });
                            hasSame++;
                        }
                    }
                    if (hasSame <= 0)
                        recycleBots.Add(target);
                }
                DebugTeamUnit("BuildTeamByAI - Fusion same in team");

                // recycle as long as empty slots is smaller than shop slots
                for (int i = 0; i < recycleBots.Count && EmptySlots() <= shopBots.Count; i++)
                {
                    if (recycleBots[i].Model.Data.XP < 3) // planned until turn 7
                    {
                        SetNullInTeam(recycleBots[i].Model.Data);
                        teamUnits.ForEach(x => { if (x == recycleBots[i]) x = null; });
                    }
                }
                DebugTeamUnit("BuildTeamByAI - Recycle");

                // remove null in team unit
                teamUnits.RemoveAll(y => y == null);

                // repair remain
                for (int j = 0; j < teamUnits.Count && repairTools > 0; j++)
                {
                    while (teamUnits[j].Model.Repair.RepairAmount > 0 && repairTools > 0)
                    {
                        DebugUnit("repair: ", j, teamUnits[j].Model.Data);
                        repairTools -= teamUnits[j].Model.Repair.RiseDurability();
                        DebugUnit("repair: ", j, teamUnits[j].Model.Data);
                    }
                }
                DebugTeamUnit("BuildTeamByAI - Repair");

                // fill empty slot with shop bot
                for (int i = 0; i < Data.TeamUnitDatas.Length && shopBots.Count > 0; i++)
                {
                    if (Data.TeamUnitDatas[i] == null)
                    {
                        Data.TeamUnitDatas[i] = shopBots[shopBots.Count - 1].Model.Data;
                        Data.TeamUnitDatas[i].UnitState = UnitState.InSlotTeam;
                        shopBots.RemoveAt(shopBots.Count - 1);
                    }
                }
                DebugTeamUnit("BuildTeamByAI - Fill from shop");

                break;
        }

        // charge at end of shop
        foreach (var unit in Data.TeamUnitDatas)
            if (unit != null)
                unit.SetEnergy(unit.Cur.ENG + 1);

        DebugTeamUnit("BuildTeamByAI - Charge");
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
        var shopBotSlots = PhaseShopController.Instance.ShopBotSlots;
        Data.ShopBotDatas = shopBotSlots.Select(x =>
        {
            var unit = x.UnitController();
            return unit == null ? null : unit.Model.Data;
        }).ToArray();
       
        var shopItemSlots = PhaseShopController.Instance.ShopItemSlots;
        Data.ShopItemDatas = shopItemSlots.Select(x =>
        {
            var item = x.UnitController();
            return item == null ? null : item.Model.Data;
        }).ToArray();

        var teamSlots = PhaseShopController.Instance.TeamSlots;
        Data.TeamUnitDatas = teamSlots.Select(x =>
        {
            var unit = x.UnitController();
                        return unit == null ? null : unit.Model.Data;
        }).ToArray();

        var chargeUnit = PhaseShopController.Instance.ChargeSlot.UnitController();
        Data.ChargeUnitData = chargeUnit == null ? null : chargeUnit.Model.Data;

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

    private int EmptySlots()
    {
        if (Data.TeamUnitDatas == null)
            return PhaseShopController.Instance.TeamSlots.Count;

        int count = 0;
        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
        {
            count += Data.TeamUnitDatas[i] == null ? 1 : 0;
        }
        return count;
    }

    private void SetNullInTeam(SaveUnitData _unit)
    {
        for (int i = 0; i < Data.TeamUnitDatas.Length; i++)
        {
            if (Data.TeamUnitDatas[i] == _unit)
            {
                Data.TeamUnitDatas[i] = null;
                return;
            }
        }
    }
}

