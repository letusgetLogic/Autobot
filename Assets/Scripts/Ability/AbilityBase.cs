using System.Collections;
using UnityEngine;

public abstract class AbilityBase
{
    protected UnitController Controller { get; private set; }
    protected Level CurrentLevel { get; private set; }
    protected Slot[] TeamSlots;

    /// <summary>
    /// Base constructor with the given parameters. 
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    public AbilityBase(UnitController _controller, Level _currentLevel, Slot[] _teamSlots)
    {
        Controller = _controller;
        CurrentLevel = _currentLevel;
        TeamSlots = _teamSlots;
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
    /// <param name="_controller"></param>
    /// <param name="_level"></param>
    /// <param name="_teamSlots"></param>
    /// <param name="_slot"></param>
    /// <returns></returns>
    public static AbilityBase GetAbility(UnitController _controller, Level _level, Slot[] _teamSlots, Slot _slot)
    {
        var type = _level.DoType;
        switch (type)
        {
            case DoType.Buff:
                if (CheckOutcomeState.IsAnyoneIn(_teamSlots, _slot) == false)
                    return null;

                return new Buff(_controller, _level, _teamSlots);

            case DoType.Summon:
                return new Summon(_controller, _controller.Model, _level, _teamSlots);
        }

        return null;
    }
    
    /// <summary>
    /// Returns the boolean, whether the ability is permanent.
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    public static bool IsPernament(AbilityDuration _duration)
    {
        if (_duration == AbilityDuration.Permanent)
            return true;

        if (_duration == AbilityDuration.Both && !GameManager.Instance.IsPhaseBattle)
            return true;

        return false;
    }

    
}
