using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract partial class AbilityBase
{
    public UnitController Controller { get; private set; }
    protected Level CurrentLevel { get; private set; }
    protected Queue<UnitController> Targets { get; private set; }

    protected Coroutine Coroutine { get; set; }
    public bool IsDone { get; private set; } = false;
    protected float DurationDescription { get; private set; }
    protected int RandomSeed { get; private set; }

    /// <summary>
    /// Base constructor with the given parameters. 
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    public AbilityBase(
        UnitController _controller, Level _currentLevel, int _seed)
    {
        Controller = _controller;
        CurrentLevel = _currentLevel;
        Targets = _controller.Targets;
        RandomSeed = _seed;
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
        Controller.View.ShowAbility(true);

        // Feature Tutorial
        yield return new WaitUntil(() => TutorialManager.Instance);
        if (GameManager.Instance.TutorialStepState == TutorialManager.StepState.WaitingForAbility)
        {
            TutorialManager.Instance.SetNextStep();
        }
        yield return new WaitUntil(() =>
        GameManager.Instance.TutorialStepState < TutorialManager.StepState.WaitingForAbility ||
        GameManager.Instance.TutorialStepState > TutorialManager.StepState.RobotUseAbility
        );
        //

        if (CurrentLevel.ConsumedEnergy != null)
            Controller.AddEnergy(CurrentLevel.ConsumedEnergy.Value, true, true);

        Coroutine = GameManager.Instance.StartCoroutine(Activate());

        yield return new WaitForSeconds(_delayHideDescription);

        if (Controller != null)
        {
            Controller.View.SetDescriptionActive(false);
            Controller.View.ShowAbility(false);
        }

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
        Queue<UnitController> _targets, int _seed)
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

                return new Buff(_controller, _level, _seed);

            case DoType.ShootOut:
                return new ShootOut(_controller, _level, _seed);

            case DoType.ShutDown:
                return new Shutdown(_controller, _level, _seed);

            case DoType.Steal:
                return new Steal(_controller, _level, _seed);

            case DoType.Debuff:
                return new Debuff(_controller, _level, _seed);

            case DoType.ConvertEnergy:
                if (_controller.Model.Data.Cur.ENG > 0)
                    return new ConvertEnergy(_controller, _level, _seed);
                return null;
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

    /// <summary>
    /// Retrieves all unit controllers in the specified slots except the current controller.
    /// </summary>
    /// <param name="_slots">An array of slots to search for unit controllers.</param>
    /// <returns>A list of unit controllers found in the provided slots, excluding the current controller.</returns>
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

    /// <summary>
    /// Finds and returns the nearest unit controller in the specified direction relative to the current unit.
    /// </summary>
    /// <param name="_toWho">Specifies the direction in which to search for the nearest unit.</param>
    /// <returns>The nearest UnitController in the specified direction, or null if none is found.</returns>
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

    /// <summary>
    /// Finds an amount of the nearest unit controllers based on the specified direction.
    /// </summary>
    /// <param name="_toWho">Specifies the direction to search for nearby units.</param>
    /// <param name="_amount">The number of nearest units to retrieve.</param>
    /// <returns>A list of the nearest unit controllers in the specified direction.</returns>
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
