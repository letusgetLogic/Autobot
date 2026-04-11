using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Represents a unit with associated data, providing functionality for
/// initialization, state management, attribute modification, and level progression.
/// </summary>
[Serializable]
public class UnitModel
{
    public RepairSystem Repair { get; set; }
    public UnitView View { get; set; }
    public SaveUnitData Data { get; set; } = new SaveUnitData();

    public SoUnit SoUnit { get; private set; }
    public Level CurrentLevel { get; set; }

    public bool IsMaxed => CurrentLevel.Index + 1 == SoUnit.Levels.Length;

    public SoPack Pack
    {
        get
        {
            if (PackManager.Instance == null)
                return definedPack;

            return PackManager.Instance.MyPack;
        }
    }
    private SoPack definedPack;

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

            return Pack.CurrencyData.UnitCost;
        }
    }

    public Currency Sell => Pack.CurrencyData.Sell[SellIndex];
    public Currency RepairCost => Pack.CurrencyData.RepairCost[CurrentLevel.Index];

    /// <summary>
    /// The index fro sell is calculated from 2D to 1D array.
    /// </summary>
    public int SellIndex
    {
        get
        {
            return SoTradingCurrency.ConvertToIndex1D(
                Data.Cur.HP == Data.FullHP ? Pack.CurrencyData.HealthPortion : Data.Durability,
                Pack.CurrencyData.LevelAmount,
                CurrentLevel.Index,
                GameManager.Instance.IsRepairSystemActive);
        }
    }

    public bool IsItemDoRandomness => Data.UnitType == UnitType.Item && CurrentLevel.ToWho == ToWho.RandomMate;

    /// <summary>
    /// Constructor of UnitModel for new unit.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_index"></param>
    /// <param name="_repair"></param>
    public UnitModel(UnitController _controller, SoUnit _soUnit, int _index, RepairSystem _repair)
    {
        definedPack = _controller.DefinedPack;
        Repair = _repair;
        SoUnit = _soUnit;
        Data.Index = _index;

        if (_soUnit)
        {
            Data.UnitType = _soUnit.UnitType;
            Data.Max.HP = Pack.MaxHP.Value;
            Data.Max.ATK = Pack.MaxATK.Value;
            Data.Max.ENG = Pack.MaxENG.Value;
            Data.MaxXP = Pack.MaxXP.Value;
            Data.SetBasisHP(_soUnit.Health);
            Data.SetBasisATK(_soUnit.Attack);

            Data.SetHP(_soUnit.Health, null);
            Data.SetATK(_soUnit.Attack);
            Data.SetEnergy(_soUnit.Energy == null ? 0 : _soUnit.Energy.Value);
            Data.SetXP(1);
        }

        string debug = "";
        if (PackManager.Instance != null)
        {
            debug = PackManager.Instance.DebugID++.ToString() + "_";
        }
        Data.ID = debug + SoUnit.Name;
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
        definedPack = _controller.DefinedPack;
        Repair = _repair;
        SoUnit = _soUnit;
        Data = _data;

        Debug.Log(Data.ID + " loaded.");
    }

    public void InitRepair()
    {
        Repair.Initialize(this);
        bool a = GameManager.Instance != null
            ? (GameManager.Instance.Replay != null ? false : true)
            : false;
        Data.Durability = Repair.GetDurabilityFromHealth(a);
    }

    /// <summary>
    /// Initializes the view and the repair system.
    /// </summary>
    /// <param name="_view"></param>
    public void InitView(UnitView _view)
    {
        View = _view;

        if (Application.isPlaying == false ||
            GameManager.Instance && GameManager.Instance.IsCatalogActive == false)
        {
            if (Data.IsTeamLeft == false)
            {
                View.SetRightSide();
            }

            if (IsRobot())
            {
                if (Repair != null)
                    View.SetRepairPanelActive(Data.FullHP >= 2, Data.FullHP >= 3);
                else
                {
                    View.ShowFullAttributes(false);
                }

                View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
                View.SetTemporaryItem(Data.TempBuff.HasValue);
            }


            if (Data.UnitType == UnitType.Item)
            {
                View.HideAttributes();
            }
        }

        if (SoUnit)
            View.SetData(SoUnit.Sprite, SoUnit.Name, SoUnit.ModelID, Data.ID);
        else
            View.SetData(null, "", "", "");
    }

    /// <summary>
    /// Is phase shop based on unit state?
    /// </summary>
    /// <param name="_unitState"></param>
    /// <returns></returns>
    public bool IsPhaseShop(UnitState _unitState)
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
    public void SetDataView(UnitState _unitState, bool _hasView)
    {
        Data.UnitState = _unitState;

        if (GameManager.Instance && GameManager.Instance.CurrentGame != null &&
            GameManager.Instance.CurrentGame.State == GameState.BattlePhase)
            _unitState = UnitState.InPhaseBattle;

        if (_hasView == false)
            return;

        switch (_unitState)
        {
            case UnitState.InSlotShop:
                View.SetBuyOrSell(Currency(_unitState), true, Data.UnitType);
                View.SetShopView(true, false, false, IsRobot() && Data.Cur.HP <= 0);
                break;

            case UnitState.Freezed:
                View.SetBuyOrSell(Currency(_unitState), true, Data.UnitType);
                View.SetShopView(true, false, true, IsRobot() && Data.Cur.HP <= 0);
                break;

            case UnitState.InSlotTeam:
                View.SetBuyOrSell(Currency(_unitState), false, Data.UnitType);
                View.SetShopView(false, true, false, IsRobot() && Data.Cur.HP <= 0);
                break;

            case UnitState.InSlotCharge:
                View.SetBuyOrSell(Currency(_unitState), false, Data.UnitType);
                View.SetShopView(false, true, false, IsRobot() && Data.Cur.HP <= 0);
                break;

            case UnitState.InPhaseBattle:
                View.HideObjectsDuringBattle();
                View.SetBuyOrSell(Currency(_unitState), false, Data.UnitType);
                View.SetShopView(false, false, false, IsRobot() && Data.Cur.HP <= 0);
                break;
        }

        if (IsRobot())
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
    public void UpdateLevelXP()
    {
        switch (Data.XP)
        { 
            case 1:
                CurrentLevel = SoUnit.Levels[0];
                 break;
            case 2:
                CurrentLevel = SoUnit.Levels[0];
                break;
            case 3:
                CurrentLevel = SoUnit.Levels[1];
                break;
            case 4:
                CurrentLevel = SoUnit.Levels[1];
                break;
            case 5:
                CurrentLevel = SoUnit.Levels[1];
                break;
            case 6:
                CurrentLevel = SoUnit.Levels[2];
                break;
        }
    }

    /// <summary>
    /// Updates the level and xp.
    /// </summary>
    /// <param name="xp"></param>
    public IEnumerator UpdateLevelXPView(bool _isPhaseShop, bool _isMakingSound)
    {
        switch (Data.XP)
        {//                        level  box1   box2  step1  step2  box3  step3  step4  step5  
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
                SetCurrentLevel(1);

                yield return new WaitForSeconds(_isPhaseShop ? View.DelayUpdateLevel : 0f);
                View.SetXpStepActive("2", false, false, false, false, true, false, false, false);
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
                SetCurrentLevel(2);

                yield return new WaitForSeconds(_isPhaseShop ? View.DelayUpdateLevel : 0f);
                View.SetXpStepActive("3", true, false, false, false, false, false, false, false);
                break;
        }
        if (IsLevelUp() && _isMakingSound)
            EventManager.Instance.OnLevelUp?.Invoke();
    }

    /// <summary>
    /// Return boolean IsLevelUp.
    /// </summary>
    /// <returns></returns>
    private bool IsLevelUp()
    {
        return Data.XP switch
        {
            3 => true,
            6 => true,
            _ => false
        };
    }

    /// <summary>
    /// Sets the current level and index for saving data.
    /// </summary>
    /// <param name="_index"></param>
    private void SetCurrentLevel(int _index)
    {
        CurrentLevel = SoUnit.Levels[_index];
        View.SetAbility(CurrentLevel.Description, CurrentLevel.ConsumedEnergy != null ? CurrentLevel.ConsumedEnergy.Value : 0);
    }

    #endregion

    /// <summary>
    /// Adds the given values to the unit.
    /// </summary>
    /// <param name="_buff"></param>
    /// <param name="_buffTemp"></param>
    public void Add(Attribute _buff, Attribute _buffTemp)
    {
        int maxHP = Pack.MaxHP.Value;
        int maxATK = Pack.MaxATK.Value;

        // Remaining values to be filled are used to prevent the sum to exceed the maximum.
        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBuffHP(Data.Buff.HP + GetAddedValue(ref remainFill_HP, _buff.HP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddedValue(ref remainFill_ATK, _buff.ATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddedValue(ref remainFill_HP, _buffTemp.HP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddedValue(ref remainFill_ATK, _buffTemp.ATK));

        Data.SetHP(Data.Cur.HP + _buff.HP + _buffTemp.HP, Repair == null
            ? null
            : new System.Action(() => View.SetRepairPanelActive(Data.FullHP >= 2, Data.FullHP >= 3)));
        Data.SetATK(Data.Cur.ATK + _buff.ATK + _buffTemp.ATK);
        Data.SetEnergy(Data.Cur.ENG + _buff.ENG);

        View.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
    }

    /// <summary>
    /// Adds the given values to the unit by fusion.
    /// </summary>
    /// <param name="_basis"></param>
    /// <param name="_buff"></param>
    /// <param name="_buffTemp"></param>
    /// <param name="_otherCurrent"></param>
    /// <param name="_hasOtherFullHP"></param>
    public void AddFusion(Attribute _basis, Attribute _buff, Attribute _buffTemp, Attribute _otherCurrent, bool _hasOtherFullHP)
    {
        bool hasFullHP = Data.Cur.HP == Data.FullHP;
        int maxHP = Pack.MaxHP.Value;
        int maxATK = Pack.MaxATK.Value;

        // Remaining values to be filled are used to prevent the sum to exceed the maximum.
        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBasisHP(Data.Basis.HP + GetAddedValue(ref remainFill_HP, _basis.HP));
        Data.SetBasisATK(Data.Basis.ATK + GetAddedValue(ref remainFill_ATK, _basis.ATK));

        Data.SetBuffHP(Data.Buff.HP + GetAddedValue(ref remainFill_HP, _buff.HP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddedValue(ref remainFill_ATK, _buff.ATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddedValue(ref remainFill_HP, _buffTemp.HP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddedValue(ref remainFill_ATK, _buffTemp.ATK));

        int hp = Repair == null ? Data.Cur.HP : GetAverage(Data.Cur.HP, _otherCurrent.HP);
        int atk = Repair == null ? Data.Cur.ATK : GetAverage(Data.Cur.ATK, _otherCurrent.ATK);

        int addHP = Repair == null ? _basis.HP + _buff.HP + _buffTemp.HP : 0;
        int addATK = Repair == null ? _basis.ATK + _buff.ATK + _buffTemp.ATK : 0;

        Data.SetHP(hp + addHP, SetRepairPanel);
        Data.SetATK(atk + addATK);

        // Prevent reduction of Durability when both units have Full HP.
        if (hasFullHP && _hasOtherFullHP)
        {
            Data.SetHP(Data.FullHP, SetRepairPanel);
            Data.SetATK(Data.FullATK);
        }

        Data.SetEnergy(Data.Cur.ENG + _otherCurrent.ENG);

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
        //int sum = _current + _addCurrent;
        //return Mathf.RoundToInt(sum * 0.5f);
        return MathCalculator.RoundAverageBasedReference(_current, _addCurrent, _current + _addCurrent);
    }

    /// <summary>
    /// The value shouldn't be lower than 0 and greater than remaining value.
    /// </summary>
    /// <param name="_remain"></param>
    /// <param name="_add"></param>
    /// <returns></returns>
    private int GetAddedValue(ref int _remain, int _add)
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
    public void ReduceHp(Damage _damage)
    {
        Data.SetHP(Data.Cur.HP - _damage.Value, SetRepairPanel);
        View.ShowDamage(_damage.Value, Data.Cur.HP);
    }


    /// <summary>
    /// Return boolean, if it is a robot.
    /// </summary>
    /// <returns></returns>
    public bool IsRobot()
    {
        if (Data.UnitType == UnitType.Robot || Data.UnitType == UnitType.SummonedRobot)
            return true;

        return false;
    }

    /// <summary>
    /// Is unit in shop?
    /// </summary>
    /// <returns></returns>
    public bool IsInShop()
    {
        if (Data.UnitState == UnitState.InSlotShop || Data.UnitState == UnitState.Freezed)
            return true;

        return false;
    }

    /// <summary>
    /// Is that a robot in shop?
    /// </summary>
    /// <returns></returns>
    public bool IsRobotInShop()
    {
        if (Data.UnitType == UnitType.Robot && IsInShop())
            return true;

        return false;
    }

    public bool IsFullDurability()
    {
        if (Repair == null)
            return false;

        return Data.Durability >= Repair.PortionAmount;
    }

    public void SetRepairPanel()
    {
        if (Repair == null)
            return;

        View.SetRepairPanelActive(Data.FullHP >= 2, Data.FullHP >= 3);
    }
}

