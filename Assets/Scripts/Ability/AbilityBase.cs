public abstract class AbilityBase
{
    protected UnitController Controller { get; private set; }
    protected AbilityDuration Duration { get; private set; }
    public AbilityBase(UnitController controller, AbilityDuration duration)
    {
        Controller = controller;
        Duration = duration;
    }

    public abstract void Activate();

    public static AbilityBase GetAbility(UnitController controller, Level level)
    {
        var type = level.DoType;
        switch (type)
        {
            case DoType.Buff:
                return new Buff(controller, level.AbilityDuration, 
                    level.FromWho, level.ToWho, level.ToWhoCount,
                    level.HealthBuff, level.AttackBuff);
        }

        return null;
    }
}
