using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UnitModel
{
    private UnitView View { get; set; }
    public RepairSystem Repair { get; set; }

    public SaveUnitData Data;

    public SoUnit SoUnit { get; private set; }
    public Level CurrentLevel { get; set; }

    public SoPack Pack => PackManager.Instance.MyPack;
    public bool IsMaxed => CurrentLevel.Index + 1 == SoUnit.Levels.Length;

    public Currency Cost => Pack.CurrencyData.UnitCost;
    public Currency Sell => Pack.CurrencyData.Sell[SellIndex];
    public Currency RepairCost => Pack.CurrencyData.RepairCost[CurrentLevel.Index];

    public int SellIndex
    {
        get
        {
            var pack = PackManager.Instance.MyPack;

            return SoTradingCurrency.ConvertToIndex1D(
                Data.Cur.HP == Data.FullHP ? pack.CurrencyData.HealthPortion : Data.Durability,
                Pack.CurrencyData.LevelAmount,
                CurrentLevel.Index,
                GameManager.Instance.IsRepairSystemActive);
        }
    }

    /// <summary>
    /// Constructor of UnitModel for new unit.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_index"></param>
    /// <param name="_repair"></param>
    public UnitModel(SoUnit _soUnit, int _index, RepairSystem _repair)
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

    /// <summary>
    /// Constructor of UnitModel for loaded unit.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_data"></param>
    /// <param name="_repair"></param>
    public UnitModel(SoUnit _soUnit, SaveUnitData _data, RepairSystem _repair) 
    {
        Repair = _repair;
        SoUnit = _soUnit;
        Data = _data;
        Debug.Log(Data.ID + " loaded.");
    }

    /// <summary>
    /// Initializes the view and the repair system.
    /// </summary>
    /// <param name="_view"></param>
    public void InitView(UnitView _view)
    {
        View = _view;

        if (Repair != null)
        {
            Repair.Initialize(this, _view);
            Repair.SetDurability(true, true, 0, 0);
            Repair.SetRepairPanel();
        }
        else
        {
            View.HideFullAttributes();
        }

        View.Shadow.enabled = false;
        View.SetData(SoUnit.Sprite, SoUnit.Name, Data.ID);
        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
        Debug.Log(Data.ID + " Energy: " + Data.Cur.ENG);
        UpdateLevelXP(IsPhaseShop(Data.UnitState));
    }

    /// <summary>
    /// Is phase shop based on unit state?
    /// </summary>
    /// <param name="unitState"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Sets the unit state and updates view.
    /// </summary>
    /// <param name="_unitState"></param>
    public void SetData(UnitState _unitState)
    {
        if (GameManager.Instance.CurrentGame.State == GameState.BattlePhase)
            _unitState = UnitState.InPhaseBattle;

        switch (_unitState)
        {
            case UnitState.InSlotShop:
                View.IceCube.SetActive(false);
                View.Shadow.enabled = true;
                break;
            case UnitState.Freezed:
                View.IceCube.SetActive(true);
                View.Shadow.enabled = true;
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


        Repair?.SetDisplay(_unitState);

        Data.UnitState = _unitState;
        bool isForBuying = _unitState == UnitState.InSlotShop || _unitState == UnitState.Freezed;
        View.SetBuyOrSell(Currency(Data.UnitState), isForBuying);
    }

    /// <summary>
    /// Gets the data for showing.
    /// </summary>
    /// <param name="unitState"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Adds the given values to the unit by buff.
    /// </summary>
    /// <param name="_buff"></param>
    /// <param name="_buffTemp"></param>
    public void Add(Attribute _buff, Attribute _buffTemp)
    {
        int maxHP = PackManager.Instance.MyPack.MaxHP.Value;
        int maxATK = PackManager.Instance.MyPack.MaxATK.Value;

        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBuffHP(Data.Buff.HP + GetAddValue(ref remainFill_HP, _buff.HP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddValue(ref remainFill_ATK, _buff.ATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddValue(ref remainFill_HP, _buffTemp.HP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddValue(ref remainFill_ATK, _buffTemp.ATK));

        Data.SetHP(Data.Cur.HP + _buff.HP + _buffTemp.HP, Repair == null ? null : Repair.SetRepairPanel);
        Data.SetATK(Data.Cur.ATK + _buff.ATK + _buffTemp.ATK);

        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
    }

    /// <summary>
    /// Adds the given values to the unit by fusion.
    /// </summary>
    /// <param name="_basis"></param>
    /// <param name="_buff"></param>
    /// <param name="_buffTemp"></param>
    /// <param name="_otherCurrent"> The average of model.Data.Cur and _otherCurrent would be calculated.</param>
    public void Add(Attribute _basis, Attribute _buff, Attribute _buffTemp, Attribute _otherCurrent, bool _hasOtherFullHP)
    {
        bool hasFullHP = Data.Cur.HP == Data.FullHP;
        int maxHP = PackManager.Instance.MyPack.MaxHP.Value;
        int maxATK = PackManager.Instance.MyPack.MaxATK.Value;

        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBasisHP(Data.Basis.HP + GetAddValue(ref remainFill_HP, _basis.HP));
        Data.SetBasisATK(Data.Basis.ATK + GetAddValue(ref remainFill_ATK, _basis.ATK));

        Data.SetBuffHP(Data.Buff.HP + GetAddValue(ref remainFill_HP, _buff.HP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddValue(ref remainFill_ATK, _buff.ATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddValue(ref remainFill_HP, _buffTemp.HP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddValue(ref remainFill_ATK, _buffTemp.ATK));

        int hp = GetAverage(Data.Cur.HP, _otherCurrent.HP);
        int atk = GetAverage(Data.Cur.ATK, _otherCurrent.ATK);
        int energy = GetAverage(Data.Cur.ENG, _otherCurrent.ENG);

        int addHP = Repair != null ? 0 : _basis.HP + _buff.HP + _buffTemp.HP;
        int addATK = Repair != null ? 0 : _basis.ATK + _buff.ATK + _buffTemp.ATK;

        Data.SetHP(hp + addHP, Repair == null ? null : Repair.SetRepairPanel);
        Data.SetATK(atk + addATK);

        // Prevent reduction of Durability when both units have Full HP.
        if (hasFullHP && _hasOtherFullHP)
        {
            Data.SetHP(Data.FullHP, Repair == null ? null : Repair.SetRepairPanel);
            Data.SetATK(Data.FullATK);
        }

        Data.SetEnergy(energy);

        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
    }

    /// <summary>
    /// Gets the average of 2 values.
    /// </summary>
    /// <param name="_current"></param>
    /// <param name="_addCurrent"></param>
    /// <returns></returns>
    private int GetAverage(int _current, int _addCurrent)
    {
        if (Repair != null)
        {
            int sum = _current + _addCurrent;
            return Mathf.RoundToInt(sum * 0.5f);
        }
        return _current;
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

    /// <summary>
    /// Reduces HP at the damage and updates view.
    /// </summary>
    /// <param name="damage"></param>
    public void ReduceHp(int damage)
    {
        if (damage < 0)
            damage = 0;

        Data.SetHP(Data.Cur.HP - damage, Repair == null ? null : Repair.SetRepairPanel);
        View.ShowDamage(damage, Data.Cur.HP);
    }


}

