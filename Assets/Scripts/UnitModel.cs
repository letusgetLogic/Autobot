using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UnitModel
{
    public SaveUnitData Data;
    public SoUnit SoUnit { get; private set; }
    public UnitView View { get; set; }
    public Level CurrentLevel { get; set; }
    public SoPack Pack => PackManager.Instance.MyPack;
    public bool IsMaxed => CurrentLevel.Index + 1 == SoUnit.Levels.Length;
    private float portionSize => 1 / (float)Pack.CurrencyData.HealthPortion;
    public int FullHp => Data.BasisHp + Data.BuffHp + Data.BuffTempHp;
    public int FullAtk => Data.BasisAtk + Data.BuffAtk + Data.BuffTempAtk;
    public Currency Cost => Pack.CurrencyData.UnitCost;
    public int SellIndex => SoTradingCurrency.ConvertToIndex1D(
        Pack.CurrencyData.HealthPortion,
        Data.Durability, 
        CurrentLevel.Index);
    public Currency Sell => Pack.CurrencyData.Sell[SellIndex];
    public Currency RepairCost => Pack.CurrencyData.RepairCost[CurrentLevel.Index];
    public int BattleID {  get; set; }

    public UnitModel(SoUnit _soUnit, int _index) // For new unit
    {
        SoUnit = _soUnit;
        Data.HasReference = true;
        Data.Index = _index;
        Data.BasisHp = _soUnit.Health;
        Data.BasisAtk = _soUnit.Attack;

        Data.DurabilityRatio = 1.0f;
        Data.SetHp(_soUnit.Health);
        Data.SetAtk(_soUnit.Attack);
        Data.Energy = _soUnit.Energy;
        Data.XP = 1;
    }

    public UnitModel(SoUnit _soUnit, SaveUnitData _data) // For loaded unit
    {
        SoUnit = _soUnit;
        Data = _data;
        int hp = Data.Hp - Data.BuffTempHp;
        int atk = Data.Atk - Data.BuffTempAtk;
        Data.SetHp(hp < 0 ? 0 : hp);
        Data.SetAtk(atk < 0 ? 0 : atk);
        Data.BuffTempHp = 0;
        Data.BuffTempAtk = 0;
    }

    public void InitView(UnitView _view)
    {
        View = _view;
        View.SetData(SoUnit.Sprite, SoUnit.Name);
        SetDurability(true, 0);
        View.SetData(FullHp, FullAtk, Data.Hp, Data.Atk, Data.Energy);
        UpdateLevelXP(IsPhaseShop(Data.UnitState));
    }

    private bool IsPhaseShop(UnitState unitState)
    {
        switch (unitState)
        {
            case UnitState.InSlotShop:
                return true;
            case UnitState.Freezed:
                return true;
            case UnitState.InSlotTeam:
                return true;
            case UnitState.InPhaseBattle:
                return false;
        }

        return default;
    }

    public void SetData(UnitState _unitState)
    {
        if (View != null)
        {
            switch (_unitState)
            {
                case UnitState.InSlotShop:
                    View.IceCube.SetActive(false);
                    View.SetRepairDisplayActive(false);
                    break;
                case UnitState.Freezed:
                    View.IceCube.SetActive(true);
                    View.SetRepairDisplayActive(false);
                    break;
                case UnitState.InSlotTeam:
                    View.IceCube.SetActive(false);
                    View.SetRepairDisplayActive(true);
                    break;
                case UnitState.InSlotCharge:
                    View.IceCube.SetActive(false);
                    View.SetRepairDisplayActive(true);
                    break;
                case UnitState.InPhaseBattle:
                    View.SetRepairDisplayActive(false);
                    View.HideDescriptionStats();
                    break;
            }
        }

        Data.UnitState = _unitState;
        bool isForBuying = _unitState == UnitState.InSlotShop;
        View.SetBuyOrSell(Currency(Data.UnitState), isForBuying);
    }

    private Currency Currency(UnitState unitState)
    {
        switch (unitState)
        {
            case UnitState.InSlotShop:
                return Cost;
            case UnitState.Freezed:
                return Cost;
            case UnitState.InSlotTeam:
                return Sell;
            case UnitState.InPhaseBattle:
                return default;
        }

        return default;
    }


    #region Durability

    public void SetDurability(bool _shouldGetDurability, int _durability)
    {
        // if durability wasn't setted, we get it from health.
        // even if it was setted and it is 1.0f, get durability shouldn't cause issue.
        if (_shouldGetDurability && Data.DurabilityRatio == 1.0f) 
        {
            GetDurabilityFromHealth();
        }
        
        if (!_shouldGetDurability)
        {
            Data.Durability = _durability;
            SetStatsBasedDurability();
        }
        
        ShowDurability();
       
    }

    private void ShowDurability()
    {
        if (View != null)
        {
            switch (Data.Durability)
            {
                case 0:
                    View.SetRepairStepActive(false, false, false);
                    break;
                case 1:
                    View.SetRepairStepActive(true, false, false);
                    break;
                case 2:
                    View.SetRepairStepActive(true, true, false);
                    break;
                case 3:
                    View.SetRepairStepActive(true, true, true);
                    break;
            }
        }
    }

    private void GetDurabilityFromHealth()
    {
        float portion0 = portionSize / 2;
        float portionHp = Data.Hp / FullHp;
        Data.DurabilityRatio = portionHp;

        // update attack based on hp
        int atk = (int)(FullAtk * portionHp);
        Data.SetAtk(atk);

        for (int i = 0; i < PackManager.Instance.MyPack.CurrencyData.HealthPortion; i++)
        {
            float portionLimit = portion0 + (portionSize * i);
            if (portionHp < portionLimit)
            {
                Data.Durability = i;
                return;
            }

            if (i == Pack.CurrencyData.HealthPortion - 1)
            {
                Data.Durability = Pack.CurrencyData.HealthPortion;
            }
        }
    }

    public void RiseDurability()
    {
        Data.Durability++;

        if (Data.Durability > Pack.CurrencyData.HealthPortion)
            Data.Durability = Pack.CurrencyData.HealthPortion;

        Data.DurabilityRatio += portionSize;

        if (Data.DurabilityRatio > 1 ||
             Data.Durability == Pack.CurrencyData.HealthPortion)
        {
            Data.DurabilityRatio = 1;
        }

        SetStatsBasedDurability();
        ShowDurability();
        View.SetBuyOrSell(Sell, false);
    }

    private void SetStatsBasedDurability()
    {
        int hp = (int)(FullHp * Data.DurabilityRatio);
        int atk = (int)(FullAtk * Data.DurabilityRatio);
        Data.SetHp(hp);
        Data.SetAtk(atk);

        View.SetData(FullHp, FullAtk, Data.Hp, Data.Atk, Data.Energy);
    }

    #endregion


    #region Update Level

    /// <summary>
    /// Updates the level and xp.
    /// </summary>
    /// <param name="xp"></param>
    public void UpdateLevelXP(bool isPhaseShop)
    {
        switch (Data.XP)
        {//                       level  box1   box2  step1  step2  box3  step3  step4  step5  
            case 1:
                View.SetXpStepActive("1", false, true, false, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 2:
                View.SetXpStepActive("1", false, true, true, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 3:
                View.SetXpStepActive("1", false, true, true, true, false, false, false, false);
                SetCurrentLevel(0);
                View.StartCoroutine(DelayLevel2(isPhaseShop));
                break;
            case 4:
                View.SetXpStepActive("2", false, false, false, false, true, true, false, false);
                SetCurrentLevel(1);
                break;
            case 5:
                View.SetXpStepActive("2", false, false, false, false, true, true, true, false);
                SetCurrentLevel(1);
                break;
            case 6:
                View.SetXpStepActive("2", false, false, false, false, true, true, true, true);
                SetCurrentLevel(1);
                View.StartCoroutine(DelayLevel3(isPhaseShop));
                break;
        }
    }

    /// <summary>
    /// Delays level 2.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel2(bool isPhaseShop)
    {
        yield return new WaitForSeconds(isPhaseShop ?
            View.DelayUpdateLevel :
            0f);

        View.SetXpStepActive("2", false, false, false, false, true, false, false, false);
        SetCurrentLevel(1);
    }

    /// <summary>
    /// Delays level 3.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel3(bool isPhaseShop)
    {
        yield return new WaitForSeconds(isPhaseShop ?
            View.DelayUpdateLevel :
            0f);

        View.SetXpStepActive("3", true, false, false, false, false, false, false, false);
        SetCurrentLevel(2);
    }

    /// <summary>
    /// Sets the current level and index for saving data.
    /// </summary>
    /// <param name="index"></param>
    private void SetCurrentLevel(int index)
    {
        CurrentLevel = SoUnit.Levels[index];
        View.SetAbility(CurrentLevel.Description, CurrentLevel.ConsumedEnergy.Value);
        View.SetBuyOrSell(Sell, false);
    }

    #endregion

    public void Add(
        int basisHp, int basisAtk,
        int buffHp, int buffAtk,
        int buffTempHp, int buffTempAtk)
    {
        Data.BasisHp += basisHp;
        Data.BasisAtk += basisAtk;
        Data.BuffHp += buffHp;
        Data.BuffAtk += buffAtk;
        Data.BuffTempHp += buffTempHp;
        Data.BuffTempAtk += buffTempAtk;

        Data.SetHp(Data.Hp + basisHp + buffHp + buffTempHp);
        Data.SetAtk(Data.Atk + basisAtk + buffAtk + buffTempAtk);
        SetStatsBasedDurability();
    }
    public void SubstractHp(int damage)
    {
        if (damage < 0)
            damage = 0;

        Data.SetHp(Data.Hp - damage);
        View.ShowDamage(damage, Data.Hp);
    }


}

