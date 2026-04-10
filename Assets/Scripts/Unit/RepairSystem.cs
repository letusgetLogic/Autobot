using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides functionality for managing and displaying the repair mechanic, including durability, for a unit.
/// </summary>
public class RepairSystem
{
    private UnitModel model;

    private float portionSize => 1 / (float)PortionAmount;
    public int PortionAmount
    {
        get
        {
            if (model.Data.FullHP <= model.Pack.CurrencyData.HealthPortion)
                return model.Data.FullHP;

            return model.Pack.CurrencyData.HealthPortion;
        }
    }

    /// <summary>
    /// Initializes the references.
    /// </summary>
    /// <param name="_model"></param>
    /// <param name="_view"></param>
    public void Initialize(UnitModel _model)
    {
        model = _model;
    }

    /// <summary>
    /// Gets the durability based on health.
    /// </summary>
    /// <param name="_updateAtk"></param>
    /// <returns></returns>
    public int GetDurabilityFromHealth(bool _updateAtk)
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

        for (int i = model.Pack.CurrencyData.HealthPortion; i > 0; i--)
        {
            float portionShiftingRelativeToDurability = portionSize / 2;
            float portionLowerLimit = (portionSize * i) - portionShiftingRelativeToDurability;
            if (portionHp > portionLowerLimit)
                return i;
        }

        return 0;
    }

    /// <summary>
    /// Sets and shows the current HP and ATK based on the durability.
    /// </summary>
    private void SetStatsBasedDurability()
    {
        float floatHP = model.Data.FullHP * model.Data.DurabilityRatio;
        int hp = Mathf.RoundToInt(floatHP);

        float floatATK = model.Data.FullATK * model.Data.DurabilityRatio;
        int atk = Mathf.RoundToInt(floatATK);

        int currentHp = model.Data.Cur.HP;
        int currentAtk = model.Data.Cur.ATK;
    
        model.Data.SetHP(hp < currentHp ? currentHp : hp, model.SetRepairPanel);
        model.Data.SetATK(atk < currentAtk ? currentAtk : atk);

        model.View.SetData(
            model.Data.FullHP, model.Data.FullATK,
            model.Data.Cur.HP, model.Data.Cur.ATK, model.Data.Cur.ENG);
    }

    /// <summary>
    /// Rises the durability value.
    /// </summary>
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
        model.View.ShowDurability(model.Data.Durability);

        if (Application.isPlaying == false)
            return;

        model.View.SetBuyOrSell(model.Sell, false, model.Data.UnitType);
    }
}
