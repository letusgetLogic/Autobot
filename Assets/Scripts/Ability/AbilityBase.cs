using System.Collections;
using UnityEngine;

public abstract class AbilityBase
{
    protected UnitController Controller { get; private set; }
    protected Level CurrentLevel { get; private set; }
    public AbilityBase(UnitController controller, Level currentLevel)
    {
        Controller = controller;
        CurrentLevel = currentLevel;
    }
    public IEnumerator Handle(float _delayHideDescription)
    {
        Controller.View.SetDescriptionActive(true);
        Activate();

        yield return new WaitForSeconds(_delayHideDescription);

        if (Controller != null)
            Controller.View.SetDescriptionActive(false);
    }

    public void Activate()
    {
        Run();
        if (Controller != null)
        {
            int value = Controller.Model.Data.Cur.Energy 
                + CurrentLevel.ConsumedEnergy.Value;
            Controller.Model.Data.SetEnergy(value);
        }
    }
    public abstract void Run();

    public static AbilityBase GetAbility(UnitController controller, Level level)
    {
        var type = level.DoType;
        switch (type)
        {
            case DoType.Buff:
                return new Buff(controller, level);
            case DoType.Summon:
                return new Summon(controller, controller.Model, level, controller.SlotIndex);
        }

        return null;
    }
    
    public static bool IsPernament(AbilityDuration duration)
    {
        if (duration == AbilityDuration.Permanent)
            return true;

        if (duration == AbilityDuration.Both && !GameManager.Instance.IsPhaseBattle)
            return true;

        return false;
    }
}
