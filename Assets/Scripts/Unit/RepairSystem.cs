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

    #region Durability

    public void SetRepairPanel()
    {
        view.SetRepairStepActive(model.Data.FullHP >= 2, model.Data.FullHP >= 3);
    }

    public void SetDurability(bool _shouldGetDurability, int _durability)
    {
        // if durability wasn't setted, we get it from health.
        // even if it was setted and it is 1.0f, get durability shouldn't cause issue.
        if (_shouldGetDurability)
        {
            model.Data.Durability = GetDurabilityFromHealth(true);
        }

        if (!_shouldGetDurability)
        {
            model.Data.Durability = _durability;
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

    public int GetDurabilityFromHealth(bool _updateAtk)
    {
        float portion0 = portionSize / 2;
        float portionHp = model.Data.Cur.HP / model.Data.FullHP;
        model.Data.DurabilityRatio = portionHp;

        // update attack based on hp
        if (_updateAtk)
        {
            int atk = (int)(model.Data.FullATK * portionHp);
            model.Data.SetATK(atk);
        }

        if (model.Data.FullHP <= model.Pack.CurrencyData.HealthPortion)
            return model.Data.Cur.HP;

        if (model.Data.Cur.HP == model.Data.FullHP)
            return model.Pack.CurrencyData.HealthPortion;

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
        int hp = (int)(model.Data.FullHP * model.Data.DurabilityRatio);
        int atk = (int)(model.Data.FullATK * model.Data.DurabilityRatio);
        model.Data.SetHP(hp, SetRepairPanel);
        model.Data.SetATK(atk);
        float portionHp = model.Data.Cur.HP / model.Data.FullHP;
        model.Data.DurabilityRatio = portionHp;
        view.SetData(
            model.Data.FullHP, model.Data.FullATK,
            model.Data.Cur.HP, model.Data.Cur.ATK, model.Data.Cur.Energy);
    }

    public void RiseDurability()
    {
        model.Data.DurabilityRatio += portionSize;
        model.Data.Durability++;

        if (model.Data.DurabilityRatio > 1f ||
             model.Data.Durability > model.Pack.CurrencyData.HealthPortion)
        {
            model.Data.DurabilityRatio = 1f;
            model.Data.Durability = model.Pack.CurrencyData.HealthPortion;
        }

        SetStatsBasedDurability();
        ShowDurability();
        view.SetBuyOrSell(model.Sell, false);
    }

    #endregion

}
