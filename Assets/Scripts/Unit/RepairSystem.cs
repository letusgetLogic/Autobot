using UnityEngine;

public class RepairSystem
{
    private UnitModel model;
    private UnitView view;

    private float portionSize => 1 / (float)portionAmount;
    private int portionAmount
    {
        get
        {
            if (model.Data.FullHP <= model.Pack.CurrencyData.HealthPortion)
                return model.Data.FullHP;

            return model.Pack.CurrencyData.HealthPortion;
        }
    }

    public void Initialize(UnitModel _model, UnitView _view)
    {
        model = _model;
        view = _view;
    }

    public void SetDisplay(UnitState _unitState)
    {
        switch (_unitState)
        {
            case UnitState.InSlotShop:
                view.SetRepairDisplayActive(false);
                break;
            case UnitState.Freezed:
                view.SetRepairDisplayActive(false);
                break;
            case UnitState.InSlotTeam:
                view.SetRepairDisplayActive(true);
                break;
            case UnitState.InSlotCharge:
                view.SetRepairDisplayActive(true);
                break;
            case UnitState.InPhaseBattle:
                view.SetRepairDisplayActive(false);
                break;
        }
    }

    public void SetRepairPanel()
    {
        view.SetRepairStepActive(model.Data.FullHP >= 2, model.Data.FullHP >= 3);
    }

    public void SetDurability(bool _shouldGetDurability, bool _updateATK, int _durability, float _ratio)
    {
        // if durability wasn't setted, we get it from health.
        // even if it was setted and it is 1.0f, get durability shouldn't cause issue.
        if (_shouldGetDurability)
        {
            model.Data.Durability = GetDurabilityFromHealth(_updateATK);
        }

        if (!_shouldGetDurability)
        {
            model.Data.Durability = _durability;
            model.Data.DurabilityRatio = _ratio;
            SetStatsBasedDurability();
        }

        ShowDurability();
    }

    private void ShowDurability()
    {
        switch (model.Data.Durability)
        {
            case 0:
                view.SetRepairStepFillActive(false, false, false);
                break;
            case 1:
                view.SetRepairStepFillActive(true, false, false);
                break;
            case 2:
                view.SetRepairStepFillActive(true, true, false);
                break;
            case 3:
                view.SetRepairStepFillActive(true, true, true);
                break;
        }
    }

    private int GetDurabilityFromHealth(bool _updateAtk)
    {
        float portionHp = (float)model.Data.Cur.HP / model.Data.FullHP;
        model.Data.DurabilityRatio = portionHp;

        // update attack based on hp
        if (_updateAtk)
        {
            float value = model.Data.FullATK * portionHp;
            int atk = Mathf.RoundToInt(value);
            model.Data.SetATK(atk);
        }

        if (model.Data.FullHP <= model.Pack.CurrencyData.HealthPortion)
            return model.Data.Cur.HP;

        if (model.Data.Cur.HP == model.Data.FullHP)
            return model.Pack.CurrencyData.HealthPortion;

        for (int i = PackManager.Instance.MyPack.CurrencyData.HealthPortion; i > 0; i--)
        {
            float portionShiftingRelativeToDurability = portionSize / 2;
            float portionLowerLimit = (portionSize * i) - portionShiftingRelativeToDurability;
            if (portionHp > portionLowerLimit)
                return i;
        }

        return 0;
    }

    private void SetStatsBasedDurability()
    {
        float floatHP = model.Data.FullHP * model.Data.DurabilityRatio;
        int hp = Mathf.RoundToInt(floatHP);

        float floatATK = model.Data.FullATK * model.Data.DurabilityRatio;
        int atk = Mathf.RoundToInt(floatATK);

        int currentHp = model.Data.Cur.HP;
        int currentAtk = model.Data.Cur.ATK;

        model.Data.SetHP(hp < currentHp ? currentHp : hp, SetRepairPanel);
        model.Data.SetATK(atk < currentAtk ? currentAtk : atk);

        view.SetData(
            model.Data.FullHP, model.Data.FullATK,
            model.Data.Cur.HP, model.Data.Cur.ATK, model.Data.Cur.ENG);
    }

    public void RiseDurability()
    {
        model.Data.DurabilityRatio += portionSize;
        model.Data.Durability++;

        if (model.Data.DurabilityRatio >= 1f ||
             model.Data.Durability >= model.Pack.CurrencyData.HealthPortion)
        {
            model.Data.DurabilityRatio = 1f;
            model.Data.Durability = model.Pack.CurrencyData.HealthPortion;
        }

        SetStatsBasedDurability();
        ShowDurability();
        view.SetBuyOrSell(model.Sell, false);
        PhaseShopUI.Instance.SetButtonActive(model);
    }

}
