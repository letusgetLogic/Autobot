using System.Collections;
using UnityEngine;

public abstract class AbilityBase
{
    protected UnitController Controller { get; private set; }
    protected Level CurrentLevel { get; private set; }

    /// <summary>
    /// Base constructor with the given parameters. 
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="currentLevel"></param>
    public AbilityBase(UnitController controller, Level currentLevel)
    {
        Controller = controller;
        CurrentLevel = currentLevel;
    }

    /// <summary>
    /// Shows and hides with the given delay time, and executes the ability.
    /// </summary>
    /// <param name="_delayHideDescription"></param>
    /// <returns></returns>
    public IEnumerator Handle(float _delayHideDescription)
    {
        Controller.View.SetDescriptionActive(true);
        Activate();

        yield return new WaitForSeconds(_delayHideDescription);

        if (Controller != null)
            Controller.View.SetDescriptionActive(false);
    }

    /// <summary>
    /// The energy is consumed by activating ability.
    /// </summary>
    public void Activate()
    {
        // Consume energy
        if (Controller != null)
        {
             Controller.SetEnergy(CurrentLevel.ConsumedEnergy.Value);
        }
        Run();
    }

    /// <summary>
    /// The behaviour of the ability is executed.
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// Returns the instance of an inheritanced class based on the ability type, or null, when it has no ability.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="level"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Returns the boolean, whether the ability is permanent.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static bool IsPernament(AbilityDuration duration)
    {
        if (duration == AbilityDuration.Permanent)
            return true;

        if (duration == AbilityDuration.Both && !GameManager.Instance.IsPhaseBattle)
            return true;

        return false;
    }
}
