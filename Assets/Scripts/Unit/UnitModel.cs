using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UnitModel
{
    public UnitView View { get; set; }
    public RepairSystem Repair { get; set; }

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

    public UnitModel(SoUnit _soUnit, int _index, RepairSystem _repair) // For new unit
    {
        Repair = _repair;
        SoUnit = _soUnit;
        Data.HasReference = true;
        Data.Index = _index;
        Data.SetBasisHP(_soUnit.Health);
        Data.SetBasisATK(_soUnit.Attack);

        Data.SetHP(_soUnit.Health, null);
        Data.SetATK(_soUnit.Attack);
        Data.SetEnergy(_soUnit.Energy);
        Data.SetXP(1);

        Data.ID = PackManager.Instance.DebugID++.ToString() + "_" + SoUnit.Name;
        Debug.Log(Data.ID + " created.");
    }

    public UnitModel(SoUnit _soUnit, SaveUnitData _data, RepairSystem _repair) // For loaded unit
    {
        Repair = _repair;
        SoUnit = _soUnit;
        Data = _data;
        Debug.Log(Data.ID + " loaded.");
    }

    public void InitView(UnitView _view)
    {
        Repair?.Initialize(this, _view);

        View = _view;
        View.SetData(SoUnit.Sprite, SoUnit.Name);

        Repair?.SetDurability(true, 0);

        if (GameManager.Instance.RepairSystem == false) 
            View.HideFullAttributes();

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

        switch (_unitState)
            {
                case UnitState.InSlotShop:
                    View.IceCube.SetActive(false);
                    break;
                case UnitState.Freezed:
                    View.IceCube.SetActive(true);
                    break;
                case UnitState.InSlotTeam:
                    View.IceCube.SetActive(false);
                    break;
                case UnitState.InSlotCharge:
                    View.IceCube.SetActive(false);
                    break;
                case UnitState.InPhaseBattle:
                    View.HideObjectsDuringBattle();
                    break;
            }
        

        if (GameManager.Instance.RepairSystem)
        {
            switch (_unitState)
            {
                case UnitState.InSlotShop:
                    View.SetRepairDisplayActive(false);
                    break;
                case UnitState.Freezed:
                    View.SetRepairDisplayActive(false);
                    break;
                case UnitState.InSlotTeam:
                    View.SetRepairDisplayActive(true);
                    break;
                case UnitState.InSlotCharge:
                    View.SetRepairDisplayActive(true);
                    break;
                case UnitState.InPhaseBattle:
                    View.SetRepairDisplayActive(false);
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

        Data.SetBasisHP(Data.Basis.HP + GetAddValue(ref remainFill_HP, _basisHP));
        Data.SetBasisATK(Data.Basis.ATK + GetAddValue(ref remainFill_ATK, _basisATK));

        Data.SetBuffHP(Data.Buff.HP + GetAddValue(ref remainFill_HP, _buffHP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddValue(ref remainFill_ATK, _buffATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddValue(ref remainFill_HP, _buffTempHP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddValue(ref remainFill_ATK, _buffTempATK));

        Data.SetHP(Data.Cur.HP + _basisHP + _buffHP + _buffTempHP, Repair == null ? null : Repair.SetRepairPanel);
        Data.SetATK(Data.Cur.ATK + _basisATK + _buffATK + _buffTempATK);

        if (Repair != null) 
            Data.Durability = Repair.GetDurabilityFromHealth(false);

        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.Energy);
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

        Data.SetHP(Data.Cur.HP - damage, Repair == null ? null : Repair.SetRepairPanel);
        View.ShowDamage(damage, Data.Cur.HP);
    }


}

