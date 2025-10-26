using NUnit.Framework;

public abstract class AbilityBase
{
    protected UnitController Controller { get; private set; }
    protected AbilityDuration Duration { get; private set; }
    protected Level CurrentLevel { get; private set; }
    public AbilityBase(UnitController controller, AbilityDuration duration, Level currentLevel)
    {
        Controller = controller;
        Duration = duration;
        CurrentLevel = currentLevel;
    }

    public abstract void Activate();

    public static AbilityBase GetAbility(UnitController controller, Level level)
    {
        var type = level.DoType;
        switch (type)
        {
            case DoType.Buff:
                return new Buff(controller, level.AbilityDuration, level);
            case DoType.Summon:
                return new Summon(controller, level.AbilityDuration, controller.Model, level, controller.SlotIndex);
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
