using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase
{
    public UnitController Controller { get; private set; }
    protected Level CurrentLevel { get; private set; }
    protected Queue<UnitController> Targets { get; private set; }

    protected Coroutine Coroutine { get; set; }
    public bool IsDone { get; private set; } = false;
    protected float DurationDescription { get; private set; }

    /// <summary>
    /// Base constructor with the given parameters. 
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    public AbilityBase(UnitController _controller, Level _currentLevel, Queue<UnitController> _targets)
    {
        Controller = _controller;
        CurrentLevel = _currentLevel;
        Targets = _targets;
    }

    /// <summary>
    /// Shows and hides with the given delay time, and executes the ability.
    /// </summary>
    /// <param name="_delayHideDescription"></param>
    /// <param name="_isDestroying"></param>
    /// <returns></returns>
    public IEnumerator Handle(float _delayHideDescription, bool _isDestroying)
    {
        DurationDescription = _delayHideDescription;
        Controller.View.SetDescriptionActive(true);

        if (CurrentLevel.ConsumedEnergy != null)
            Controller.SetEnergy(CurrentLevel.ConsumedEnergy.Value, true);

        Coroutine = GameManager.Instance.StartCoroutine(Activate());

        yield return new WaitForSeconds(_delayHideDescription);

        if (Controller != null)
            Controller.View.SetDescriptionActive(false);

        if (_isDestroying)
            EventManager.Instance.OnShutdown?.Invoke(Controller);


        yield return new WaitUntil(() => Coroutine == null);

        IsDone = true;
    }

    /// <summary>
    /// The behaviour of the ability is executed.
    /// </summary>
    protected abstract IEnumerator Activate();

    /// <summary>
    /// Returns the instance of an inheritanced class based on the ability type, or null, when it has no ability.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_level"></param>
    /// <param name="_teamSlots"></param>
    /// <param name="_slot"></param>
    /// <returns></returns>
    public static AbilityBase GetAbility(
        UnitController _controller,
        Level _level,
        Queue<UnitController> _targets)
    {
        switch (_level.DoType)
        {
            case DoType.Buff:
                if ((_level.ToWho == ToWho.NearestMateAhead ||
                    _level.ToWho == ToWho.RandomMate ||
                    _level.ToWho == ToWho.NearestMateBehind ||
                    _level.ToWho == ToWho.AllMates)
                    && CheckOutcomeState.IsAnyoneIn(_controller.TeamSlots, _controller.Slot) == 0)
                    return null;

                return new Buff(_controller, _level, _targets);

            case DoType.ShootOut:
                return new ShootOut(_controller, _level, _targets);

            case DoType.ShutDown:
                return new Shutdown(_controller, _level, _targets);

            case DoType.Steal:
                return new Steal(_controller, _level, _targets);

            case DoType.Debuff:
                return new Debuff(_controller, _level, _targets);
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

        if (_duration == AbilityDuration.Both)
            if (PhaseShopController.Instance != null)
                return true;
            else
                return false;

        if (_duration == AbilityDuration.UntilNextTurn)
            return false;

        return false;
    }


    protected virtual List<UnitController> AllBotsIn(Slot[] _slots)
    {
        List<UnitController> teamUnitControllers = new List<UnitController>();

        for (int i = 0; i < _slots.Length; i++)
        {
            var teamUnitController = _slots[i].UnitController();
            if (teamUnitController != null && teamUnitController != Controller)
            {
                teamUnitControllers.Add(teamUnitController);
            }
        }

        return teamUnitControllers;
    }

    protected virtual UnitController GetRandomIn(List<UnitController> _units)
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

        if (target < 0 && target >= Controller.TeamSlots.Length && target != index)
            return null;
        else
            return Controller.TeamSlots[target].UnitController();
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

            if (target < 0 && target >= Controller.TeamSlots.Length)
                continue;
            else
            {
                if (target != index)
                {
                    units.Add(Controller.TeamSlots[target].UnitController());
                }
            }
        }

        return units;
    }


}
