using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UnitModel
{
    public UnitController Controller { get; set; }
    private UnitView view { get; set; }
    public RepairSystem Repair { get; set; }

    public SaveUnitData Data;

    public SoUnit SoUnit { get; private set; }
    public Level CurrentLevel { get; set; }

    public bool IsMaxed => CurrentLevel.Index + 1 == SoUnit.Levels.Length;

    public Currency Cost
    {
        get
        {
            if (SoUnit.HasUniqueCost)
            {
                Currency cost = new Currency(
                    SoUnit.UniqueCostNuts == null ? 0 : SoUnit.UniqueCostNuts.Value,
                    SoUnit.UniqueCostTools == null ? 0 : SoUnit.UniqueCostTools.Value);
                return cost;
            }

            return Controller.Pack.CurrencyData.UnitCost;
        }
    }

    public Currency Sell => Controller.Pack.CurrencyData.Sell[SellIndex];
    public Currency RepairCost => Controller.Pack.CurrencyData.RepairCost[CurrentLevel.Index];

    /// <summary>
    /// The index is calculated from 2D to 1D array.
    /// </summary>
    public int SellIndex
    {
        get
        {
            return SoTradingCurrency.ConvertToIndex1D(
                Data.Cur.HP == Data.FullHP ? Controller.Pack.CurrencyData.HealthPortion : Data.Durability,
                Controller.Pack.CurrencyData.LevelAmount,
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
    public UnitModel(UnitController _controller, SoUnit _soUnit, int _index, RepairSystem _repair)
    {
        Controller = _controller;
        Repair = _repair;
        SoUnit = _soUnit;
        Data.HasReference = true;
        Data.Index = _index;
        Data.UnitType = _soUnit.UnitType;
        Data.Pack = Controller.Pack;
        Data.SetBasisHP(_soUnit.Health);
        Data.SetBasisATK(_soUnit.Attack);

        Data.SetHP(_soUnit.Health, null);
        Data.SetATK(_soUnit.Attack);
        Data.SetEnergy(_soUnit.Energy == null ? 0 : _soUnit.Energy.Value);
        Data.SetXP(1);

#if UNITY_EDITOR
        return;
#endif
        Data.ID = PackManager.Instance.DebugID++.ToString() + "_" + SoUnit.Name;
        Debug.Log(Data.ID + " new created.");
    }

    /// <summary>
    /// Constructor of UnitModel for loaded unit.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_data"></param>
    /// <param name="_repair"></param>
    public UnitModel(UnitController _controller, SoUnit _soUnit, SaveUnitData _data, RepairSystem _repair)
    {
        Controller = _controller;
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
        view = _view;

        if (Data.IsRobot())
        {
            if (Repair != null)
            {
                Repair.Initialize(this, _view);
                Repair.SetDurability(true);
                Repair.SetRepairPanel();
            }
            else
            {
                view.ShowFullAttributes(false);
            }

            view.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
        }

        if (Data.UnitType == UnitType.Item)
        {
            view.HideAttributes();
        }

        view.Shadow.enabled = false;
        view.SetData(SoUnit.Sprite, SoUnit.Name, Data.ID);

#if UNITY_EDITOR
        return;
#endif
        UpdateLevelXP(IsPhaseShop(Data.UnitState));
    }

    /// <summary>
    /// Is phase shop based on unit state?
    /// </summary>
    /// <param name="_unitState"></param>
    /// <returns></returns>
    private bool IsPhaseShop(UnitState _unitState)
    {
        switch (_unitState)
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
        if (GameManager.Instance && GameManager.Instance.CurrentGame.State == GameState.BattlePhase)
            _unitState = UnitState.InPhaseBattle;

        switch (_unitState)
        {
            case UnitState.InSlotShop:
                view.IceCube.SetActive(false);
                view.Shadow.enabled = true;
                view.SetBuyOrSell(Currency(_unitState), true);
                view.SetShopView(true, false);
                break;

            case UnitState.Freezed:
                view.IceCube.SetActive(true);
                view.Shadow.enabled = true;
                view.SetBuyOrSell(Currency(_unitState), true);
                view.SetShopView(true, false);
                break;

            case UnitState.InSlotTeam:
                view.IceCube.SetActive(false);
                view.SetBuyOrSell(Currency(_unitState), false);
                view.SetShopView(false, true);
                break;

            case UnitState.InSlotCharge:
                view.IceCube.SetActive(false);
                view.SetBuyOrSell(Currency(_unitState), false);
                view.SetShopView(false, true);
                break;

            case UnitState.InPhaseBattle:
                view.HideObjectsDuringBattle();
                view.SetBuyOrSell(Currency(_unitState), false);
                view.SetShopView(false, false);
                break;
        }

        Repair?.SetDisplay(_unitState);

        Data.UnitState = _unitState;
    }

    /// <summary>
    /// Gets the data for showing.
    /// </summary>
    /// <param name="_unitState"></param>
    /// <returns></returns>
    private Currency Currency(UnitState _unitState)
    {
        switch (_unitState)
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
    public void UpdateLevelXP(bool _isPhaseShop)
    {
        switch (Data.XP)
        {//                        level  box1   box2  step1  step2  box3  step3  step4  step5  
            case 1:
                view.SetXpStepActive("1", false, true, false, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 2:
                view.SetXpStepActive("1", false, true, true, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 3:
                view.SetXpStepActive("1", false, true, true, true, false, false, false, false);
                SetCurrentLevel(0);
                view.StartCoroutine(DelayLevel2(_isPhaseShop));
                break;
            case 4:
                view.SetXpStepActive("2", false, false, false, false, true, true, false, false);
                SetCurrentLevel(1);
                break;
            case 5:
                view.SetXpStepActive("2", false, false, false, false, true, true, true, false);
                SetCurrentLevel(1);
                break;
            case 6:
                view.SetXpStepActive("2", false, false, false, false, true, true, true, true);
                SetCurrentLevel(1);
                view.StartCoroutine(DelayLevel3(_isPhaseShop));
                break;
        }
    }

    /// <summary>
    /// Delays level 2.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel2(bool _isPhaseShop)
    {
        yield return new WaitForSeconds(_isPhaseShop ?
            view.DelayUpdateLevel :
            0f);
        //                  level  box1   box2  step1  step2  box3  step3  step4  step5  
        view.SetXpStepActive("2", false, false, false, false, true, false, false, false);
        SetCurrentLevel(1);
    }

    /// <summary>
    /// Delays level 3.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel3(bool _isPhaseShop)
    {
        yield return new WaitForSeconds(_isPhaseShop ?
            view.DelayUpdateLevel :
            0f);
        //                 level  box1   box2  step1  step2  box3  step3  step4  step5  
        view.SetXpStepActive("3", true, false, false, false, false, false, false, false);
        SetCurrentLevel(2);
    }

    /// <summary>
    /// Sets the current level and index for saving data.
    /// </summary>
    /// <param name="_index"></param>
    private void SetCurrentLevel(int _index)
    {
        CurrentLevel = SoUnit.Levels[_index];
        view.SetAbility(CurrentLevel.Description, CurrentLevel.ConsumedEnergy != null ? CurrentLevel.ConsumedEnergy.Value : 0);
        view.SetBuyOrSell(Sell, false);
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

        view.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
    }

    /// <summary>
    /// Adds the given values to the unit by fusion.
    /// </summary>
    /// <param name="_basis"></param>
    /// <param name="_buff"></param>
    /// <param name="_buffTemp"></param>
    /// <param name="_otherCurrent"></param>
    /// <param name="_hasOtherFullHP"></param>
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

        Data.SetEnergy(Data.Cur.ENG + _otherCurrent.ENG);

        view.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
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
    /// <param name="_damage"></param>
    public void ReduceHp(uint _damage)
    {
        Data.SetHP(Data.Cur.HP - (int)_damage, Repair == null ? null : Repair.SetRepairPanel);
        view.ShowDamage((int)_damage, Data.Cur.HP);
    }


}

