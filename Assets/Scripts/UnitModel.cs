using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class UnitModel
{
    public SaveUnitData Data;
    public SoUnit SoUnit { get; private set; }
    public UnitView View { get; set; }
    public Level CurrentLevel { get; set; }
    public bool IsMaxed => CurrentLevel.Number == SoUnit.Levels.Length;
    public UnitModel(SoUnit _soUnit, int _index) // For new unit
    {
        SoUnit = _soUnit;
        Data.HasReference = true;
        Data.Index = _index;
        Data.BasisHp = _soUnit.Health;
        Data.BasisAtk = _soUnit.Attack;
        Data.SetHp(_soUnit.Health, UpdateDurability);
        Data.Atk = _soUnit.Attack;
        Data.Energy = _soUnit.Energy;
        Data.XP = 1;
    }
  

    public UnitModel(SoUnit _soUnit, SaveUnitData _data) // For loaded unit
    {
        SoUnit = _soUnit;
        Data = _data;
        Data.SetHp(Data.Hp - Data.BuffTempHp, UpdateDurability);
        Data.Atk -= Data.BuffTempAtk;
        Data.BuffTempHp = 0;
        Data.BuffTempAtk = 0;
    }

    public void SetData(UnitView _view)
    {
        View = _view;
        View.SetData(SoUnit.Sprite, SoUnit.Name);
        View.SetData(CurrentLevel.Description);
        View.SetData(Data.Hp, Data.Atk, Data.Energy);
        SetDurability(GetDurability());
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
            case UnitState.InSlotBattle:
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
                case UnitState.InSlotBattle:
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
        View.SetData(Coin(Data.UnitState));
    }

    private int Coin(UnitState unitState)
    {
        switch (unitState)
        {
            case UnitState.InSlotShop:
                return SoUnit.Cost.Value;
            case UnitState.Freezed:
                return SoUnit.Cost.Value;
            case UnitState.InSlotBattle:
                return CurrentLevel.Sell;
            case UnitState.InPhaseBattle:
                return 0;
        }

        return -1;
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
        View.SetData(CurrentLevel.Description);
        View.SetData(CurrentLevel.Sell);
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

        Data.SetHp(Data.Hp + basisHp + buffHp + buffTempHp, UpdateDurability);
        Data.Atk += basisAtk + buffAtk + buffTempAtk;
        View.SetData(Data.Hp, Data.Atk, Data.Energy);
    }

    public void SubstractHp(int damage)
    {
        if (damage < 0)
            damage = 0;

        Data.SetHp(Data.Hp - damage, UpdateDurability);
        View.ShowDamage(damage, Data.Hp);
    }


    #region Durability
    private void UpdateDurability()
    {
        SetDurability(GetDurability());
    }
    public void SetDurability(int _state)
    {
        if (View != null)
        {
            switch (_state)
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

        if (_state > PackManager.Instance.MyPack.HealthPortion.Value)
            Data.Durability = PackManager.Instance.MyPack.HealthPortion.Value;
        else Data.Durability = _state;
    }

    public int GetDurability()
    {
        float portion = 1 / (float)PackManager.Instance.MyPack.HealthPortion.Value;
        float portion0 = portion / 2;

        int maxHp = Data.BasisHp + Data.BuffHp;
        float portionHp = Data.Hp / maxHp;
        
        for (int i = 0; i < PackManager.Instance.MyPack.HealthPortion.Value; i++)
        {
            float portionLimit = portion0 + (portion * i);
            if (portionHp < portionLimit)
                return i;

            if (i == PackManager.Instance.MyPack.HealthPortion.Value - 1)
            {
                return PackManager.Instance.MyPack.HealthPortion.Value;
            }
        }
        return 0;
    }

    #endregion
}

