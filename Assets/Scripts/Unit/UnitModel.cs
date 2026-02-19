using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class UnitModel
{
    public UnitController Controller { get; set; }
    private UnitView view { get; set; }
    public RepairSystem Repair { get; set; }

    public SaveUnitData Data { get; set; } = new SaveUnitData();

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
    /// The index fro sell is calculated from 2D to 1D array.
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

    public bool IsItemDoRandomness => Data.UnitType == UnitType.Item && CurrentLevel.ToWho == ToWho.RandomMate;

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
        Data.Index = _index;
        Data.UnitType = _soUnit.UnitType;
        Data.Max.HP = Controller.Pack.MaxHP.Value;
        Data.Max.ATK = Controller.Pack.MaxATK.Value;
        Data.Max.ENG = Controller.Pack.MaxENG.Value;
        Data.MaxXP = Controller.Pack.MaxXP.Value;
        Data.SetBasisHP(_soUnit.Health);
        Data.SetBasisATK(_soUnit.Attack);

        Data.SetHP(_soUnit.Health, null);
        Data.SetATK(_soUnit.Attack);
        Data.SetEnergy(_soUnit.Energy == null ? 0 : _soUnit.Energy.Value);
        Data.SetXP(1);

        if (Application.isPlaying == false)
            return;

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
    public void InitView(UnitView _view, bool _isTeamLeft)
    {
        view = _view;

        if (_isTeamLeft)
        {
            Data.IsTeamLeft = true;
        }
        else
        {
            view.SetRightSide();
            Data.IsTeamLeft = false;
        }

        if (IsRobot())
        {
            if (Repair != null)
            {
                Repair.Initialize(this, _view);
                Repair.SetDurability(GameManager.Instance.IsReplay == false ? true : false);
                Repair.SetRepairPanel();
            }
            else
            {
                view.ShowFullAttributes(false);
            }

            view.SetData(Data.FullHP, Data.FullATK, Data.Cur.HP, Data.Cur.ATK, Data.Cur.ENG);
            view.SetTemporaryItem(Data.TempBuff.HasValue);
        }

        if (Data.UnitType == UnitType.Item)
        {
            view.HideAttributes();
        }

        view.SetData(SoUnit.Sprite, SoUnit.Name, Data.ID);

        if (Application.isPlaying == false)
            return;

        Controller.StartCoroutine(UpdateLevelXP(IsPhaseShop(Data.UnitState), false));
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
                view.SetBuyOrSell(Currency(_unitState), true, Data.UnitType);
                view.SetShopView(true, false, false);
                break;

            case UnitState.Freezed:
                view.SetBuyOrSell(Currency(_unitState), true, Data.UnitType);
                view.SetShopView(true, false, true);
                break;

            case UnitState.InSlotTeam:
                view.SetBuyOrSell(Currency(_unitState), false, Data.UnitType);
                view.SetShopView(false, true, false);
                break;

            case UnitState.InSlotCharge:
                view.SetBuyOrSell(Currency(_unitState), false, Data.UnitType);
                view.SetShopView(false, true, false);
                break;

            case UnitState.InPhaseBattle:
                view.HideObjectsDuringBattle();
                view.SetBuyOrSell(Currency(_unitState), false, Data.UnitType);
                view.SetShopView(false, false, false);
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
    public IEnumerator UpdateLevelXP(bool _isPhaseShop, bool _isMakingSound)
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
                SetCurrentLevel(1);

                yield return new WaitForSeconds(_isPhaseShop ? view.DelayUpdateLevel : 0f);
                view.SetXpStepActive("2", false, false, false, false, true, false, false, false);
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
                SetCurrentLevel(2);

                yield return new WaitForSeconds(_isPhaseShop ? view.DelayUpdateLevel : 0f);
                view.SetXpStepActive("3", true, false, false, false, false, false, false, false);
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
        view.SetAbility(CurrentLevel.Description, CurrentLevel.ConsumedEnergy != null ? CurrentLevel.ConsumedEnergy.Value : 0);
    }

    #endregion

    /// <summary>
    /// Adds the given values to the unit.
    /// </summary>
    /// <param name="_buff"></param>
    /// <param name="_buffTemp"></param>
    public void Add(Attribute _buff, Attribute _buffTemp)
    {
        int maxHP = Controller.Pack.MaxHP.Value;
        int maxATK = Controller.Pack.MaxATK.Value;

        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBuffHP(Data.Buff.HP + GetAddValue(ref remainFill_HP, _buff.HP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddValue(ref remainFill_ATK, _buff.ATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddValue(ref remainFill_HP, _buffTemp.HP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddValue(ref remainFill_ATK, _buffTemp.ATK));

        Data.SetHP(Data.Cur.HP + _buff.HP + _buffTemp.HP, Repair == null ? null : Repair.SetRepairPanel);
        Data.SetATK(Data.Cur.ATK + _buff.ATK + _buffTemp.ATK);
        Data.SetEnergy(Data.Cur.ENG + _buff.ENG);

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
    public void AddFusion(Attribute _basis, Attribute _buff, Attribute _buffTemp, Attribute _otherCurrent, bool _hasOtherFullHP)
    {
        bool hasFullHP = Data.Cur.HP == Data.FullHP;
        int maxHP = Controller.Pack.MaxHP.Value;
        int maxATK = Controller.Pack.MaxATK.Value;

        int remainFill_HP = maxHP - Data.Basis.HP - Data.Buff.HP - Data.TempBuff.HP;
        int remainFill_ATK = maxATK - Data.Basis.ATK - Data.Buff.ATK - Data.TempBuff.ATK;

        Data.SetBasisHP(Data.Basis.HP + GetAddValue(ref remainFill_HP, _basis.HP));
        Data.SetBasisATK(Data.Basis.ATK + GetAddValue(ref remainFill_ATK, _basis.ATK));

        Data.SetBuffHP(Data.Buff.HP + GetAddValue(ref remainFill_HP, _buff.HP));
        Data.SetBuffATK(Data.Buff.ATK + GetAddValue(ref remainFill_ATK, _buff.ATK));

        Data.SetTempBuffHP(Data.TempBuff.HP + GetAddValue(ref remainFill_HP, _buffTemp.HP));
        Data.SetTempBuffATK(Data.TempBuff.ATK + GetAddValue(ref remainFill_ATK, _buffTemp.ATK));

        int hp = Repair == null ? Data.Cur.HP : GetAverage(Data.Cur.HP, _otherCurrent.HP);
        int atk = Repair == null ? Data.Cur.ATK : GetAverage(Data.Cur.ATK, _otherCurrent.ATK);

        int addHP = Repair == null ? _basis.HP + _buff.HP + _buffTemp.HP : 0;
        int addATK = Repair == null ? _basis.ATK + _buff.ATK + _buffTemp.ATK : 0;

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
    public void ReduceHp(Damage _damage)
    {
        Data.SetHP(Data.Cur.HP - _damage.Value, Repair == null ? null : Repair.SetRepairPanel);
        view.ShowDamage(_damage.Value, Data.Cur.HP);
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
}

