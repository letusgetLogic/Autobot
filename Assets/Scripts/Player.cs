using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }

    private UnitController[] battleUnits;

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

    public void StartBattle()
    {
        battleUnits = new UnitController[PhaseBattleController.Instance.Slots1.Length];
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

        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }

    /// <summary>
    /// Updates the data of team units for saving data.
    /// </summary>
    public void UpdateTeamUnitData()
    {
        for (int i = 0; i < PhaseShopUnitManager.Instance.TeamSlots.Length; i++)
        {
            if (battleUnits[i] == null)
            {
                continue;
            }

            Data.TeamUnitDatas[i] = battleUnits[i].Model.Data;
        }

        SaveSystem.SaveGame(GameManager.Instance.CurrentGame);
    }

    public void SaveReference(UnitController[] _battleUnits)
    {
        battleUnits = new UnitController[PhaseBattleController.Instance.Slots1.Length];
        for (int i = 0; i < _battleUnits.Length;i++)
        {
            battleUnits[i] = _battleUnits[i];
        }
    }
}

