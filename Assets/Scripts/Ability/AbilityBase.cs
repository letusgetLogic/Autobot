using System.Collections;
using System.Collections.Generic;
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
    public IEnumerator Handle(float _delayHideDescription, bool _isDestroying)
    {
        Controller.View.SetDescriptionActive(true);

        // Consume energy
        if (CurrentLevel.ConsumedEnergy != null)
            Controller.SetEnergy(CurrentLevel.ConsumedEnergy.Value);

        Activate();
        Debug.Log("... Wait " + _delayHideDescription);
        yield return new WaitForSeconds(_delayHideDescription);
        Debug.Log("Continue ");
        if (Controller != null)
            Controller.View.SetDescriptionActive(false);

        if (_isDestroying)
            Controller.DestroyObject();
    }

    /// <summary>
    /// The behaviour of the ability is executed.
    /// </summary>
    public abstract void Activate();

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

            case DoType.ShutDown:
                return new Shutdown(_controller, _level, _teamSlots);
        }

        return null;
    }

    /// <summary>
    /// Returns the boolean, whether the ability is permanent.
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    protected virtual bool IsPernament(AbilityDuration _duration)
    {
        if (_duration == AbilityDuration.Permanent)
            return true;

        if (_duration == AbilityDuration.Both && PhaseShopUnitManager.Instance != null)
            return true;

        return false;
    }


    protected virtual List<UnitController> GetAllMates()
    {
        List<UnitController> teamUnitControllers = new List<UnitController>();

        for (int i = 0; i < TeamSlots.Length; i++)
        {
            var teamUnitController = TeamSlots[i].UnitController();
            if (teamUnitController != null && teamUnitController != Controller)
            {
                teamUnitControllers.Add(teamUnitController);
            }
        }

        return teamUnitControllers;
    }

    protected virtual UnitController GetRandomIn(ref List<UnitController> _units)
    {
        if (_units.Count <= 0)
            return null;

        var rnd = new System.Random();
        int index = rnd.Next(0, _units.Count);

        return _units[index];
    }

    protected virtual UnitController GetOneNearest(ToWho _toWho)
    {
        int dir = _toWho switch
        {
            ToWho.NearestMateAhead => -1,
            ToWho.NearestMateBehind => 1,
            _ => 0
        };

        var index = Controller.Slot.Index;
        int target = index + dir;

        if (target < 0 && target >= TeamSlots.Length && target != index)
            return null;
        else
            return TeamSlots[target].UnitController();
    }

    protected virtual List<UnitController> GetNearest(ToWho _toWho, int _amount)
    {
        var units = new List<UnitController>();

        int dir = _toWho switch
        {
            ToWho.NearestMateAhead => -1,
            ToWho.NearestMateBehind => 1,
            _ => 0
        };

        var index = Controller.Slot.Index;
        int target = index;

        for (int i = 0; i < _amount; i++)
        {
            target += dir;

            if (target < 0 && target >= TeamSlots.Length)
                continue;
            else
            {
                if (target != index)
                {
                    units.Add(TeamSlots[target].UnitController());
                }
            }
        }

        return units;
    }


}
