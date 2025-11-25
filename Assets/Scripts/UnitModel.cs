using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UnitModel
{
    public UnitView View { get; set; }

    public SaveUnitData Data;
    public SoUnit SoUnit { get; private set; }
    public Level CurrentLevel { get; set; }

    public SoPack Pack => PackManager.Instance.MyPack;
    public bool IsMaxed => CurrentLevel.Index + 1 == SoUnit.Levels.Length;

    public Currency Cost => Pack.CurrencyData.UnitCost;
    public Currency Sell => Pack.CurrencyData.Sell[SellIndex];
    public Currency RepairCost => Pack.CurrencyData.RepairCost[CurrentLevel.Index];

    public int SellIndex => SoTradingCurrency.ConvertToIndex1D(
       Pack.CurrencyData.HealthPortion,
       Data.Durability,
       CurrentLevel.Index);

    private float portionSize => 1 / (float)portionAmount;
    private int portionAmount
    {
        get
        {
            if (Data.FullHP <= Pack.CurrencyData.HealthPortion)
                return Data.FullHP;

            return Pack.CurrencyData.HealthPortion;
        }
    }

    public UnitModel(SoUnit _soUnit, int _index) // For new unit
    {
        SoUnit = _soUnit;
        Data.HasReference = true;
        Data.Index = _index;
        Data.SetBasisHP(_soUnit.Health);
        Data.SetBasisATK(_soUnit.Attack);

        Data.DurabilityRatio = 1.0f;
        Data.SetHP(_soUnit.Health, null);
        Data.SetATK(_soUnit.Attack);
        Data.SetEnergy(_soUnit.Energy);
        Data.SetXP(1);

        Data.ID = PackManager.Instance.DebugID++.ToString() + "_" + SoUnit.Name;
        Debug.Log(Data.ID + " created.");
    }

    public UnitModel(SoUnit _soUnit, SaveUnitData _data) // For loaded unit
    {
        SoUnit = _soUnit;
        Data = _data;
        Debug.Log(Data.ID + " loaded.");
    }

    public void InitView(UnitView _view)
    {
        View = _view;
        View.SetData(SoUnit.Sprite, SoUnit.Name);
        int hp = Data.Cur.HP - Data.TempBuff.HP;
        int atk = Data.Cur.ATK - Data.TempBuff.ATK;
        Data.SetTempBuffHP(0);
        Data.SetTempBuffATK(0);
        Data.SetHP(hp < 0 ? 0 : hp, SetRepairPanel);
        Data.SetATK(atk < 0 ? 0 : atk);
        SetDurability(true, 0);
        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.Energy);
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
        if (GameManager.Instance.CurrentGame.State == GameState.BattlePhase)
            _unitState = UnitState.InPhaseBattle;

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

    public void SetRepairPanel()
    {
        View.SetRepairStepActive(Data.FullHP >= 2, Data.FullHP >= 3);
    }

    public void SetDurability(bool _shouldGetDurability, int _durability)
    {
        // if durability wasn't setted, we get it from health.
        // even if it was setted and it is 1.0f, get durability shouldn't cause issue.
        if (_shouldGetDurability)
        {
            Data.Durability = GetDurabilityFromHealth();
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
                    View.SetRepairStepFillActive(false, false, false);
                    break;
                case 1:
                    View.SetRepairStepFillActive(true, false, false);
                    break;
                case 2:
                    View.SetRepairStepFillActive(true, true, false);
                    break;
                case 3:
                    View.SetRepairStepFillActive(true, true, true);
                    break;
            }
        }
    }

    private int GetDurabilityFromHealth()
    {
        float portion0 = portionSize / 2;
        float portionHp = Data.Cur.HP / Data.FullHP;
        Data.DurabilityRatio = portionHp;

        // update attack based on hp
        int atk = (int)(Data.FullATK * portionHp);
        Data.SetATK(atk);

        if (Data.FullHP <= Pack.CurrencyData.HealthPortion)
            return Data.Cur.HP;

        if (Data.Cur.HP == Data.FullHP)
            return Pack.CurrencyData.HealthPortion;

        for (int i = PackManager.Instance.MyPack.CurrencyData.HealthPortion; i > 0; i--)
        {
            float portionLimit = (portionSize * i) - portionSize;
            if (portionHp > portionLimit)
                return i;
        }

        return 0;
    }

    private void SetStatsBasedDurability()
    {
        int hp = (int)(Data.FullHP * Data.DurabilityRatio);
        int atk = (int)(Data.FullATK * Data.DurabilityRatio);
        Data.SetHP(hp, SetRepairPanel);
        Data.SetATK(atk);
        float portionHp = Data.Cur.HP / Data.FullHP;
        Data.DurabilityRatio = portionHp;
        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.Energy);
    }

    public void RiseDurability()
    {
        Data.DurabilityRatio += portionSize;
        Data.Durability++;

        if (Data.DurabilityRatio > 1f ||
             Data.Durability > Pack.CurrencyData.HealthPortion)
        {
            Data.DurabilityRatio = 1f;
            Data.Durability = Pack.CurrencyData.HealthPortion;
        }

        SetStatsBasedDurability();
        ShowDurability();
        View.SetBuyOrSell(Sell, false);
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
        int _basisHP, int _basisATK,
        int _buffHP, int _buffATK,
        int _buffTempHP, int _buffTempATK)
    {
        int maxHP = PackManager.Instance.MyPack.MaxHP.Value;
        int maxATK = PackManager.Instance.MyPack.MaxATK.Value;

        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBasisHP (Data.Basis.HP  + GetAddValue(ref remainFill_HP, _basisHP));
        Data.SetBasisATK(Data.Basis.ATK + GetAddValue(ref remainFill_ATK, _basisATK));

        Data.SetBuffHP (Data.Buff.HP  + GetAddValue(ref remainFill_HP, _buffHP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddValue(ref remainFill_ATK, _buffATK));

        Data.SetTempBuffHP (Data.TempBuff.HP  + GetAddValue(ref remainFill_HP, _buffTempHP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddValue(ref remainFill_ATK, _buffTempATK));

        SetStatsBasedDurability();
    }

    /// <summary>
    /// The value shouldn't be lower than 0 and greater than remaining value.
    /// </summary>
    /// <param name="_remain"></param>
    /// <param name="_add"></param>
    /// <returns></returns>
    private int GetAddValue(ref int _remain, int _add)
    {
        if (_add < 0 || _remain <= 0)
            return 0;

        if (_add > _remain)
        {
            _add = _remain;
            _remain = 0;
        }
        else
            _remain -= _add;

        return _add;
    }

    public void SubstractHp(int damage)
    {
        if (damage < 0)
            damage = 0;

        Data.SetHP(Data.Cur.HP - damage, SetRepairPanel);
        View.ShowDamage(damage, Data.Cur.HP);
    }


}

