using System;
using System.Collections;
using System.Collections.Generic;

public class Buff : AbilityBase
{
    private readonly Slot[] teamSlots;

    /// <summary>
    /// Constructor of Buff.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teamSlots"></param>
    public Buff(UnitController _controller, Level _currentLevel, Queue<UnitController> _targets)
        : base(_controller, _currentLevel, _targets)
    {
        teamSlots = _controller.TeamSlots;
    }

    protected override IEnumerator Activate()
    {
        switch (CurrentLevel.ToWho)
        {
            case ToWho.None:
                UnityEngine.Debug.LogWarning($"{Controller.name} has ToWho.None!");
                break;

            case ToWho.Self:
                BuffUnit(Controller);
                break;

            case ToWho.RandomMate:
                BuffRandomMate();
                break;

            case ToWho.TargetBot:
                BuffTargetByItem();
                break;

            case var a when a == ToWho.NearestMateAhead:
                BuffNearest(GetNearest(a, CurrentLevel.ToWhoCount));
                break;

            case var b when b == ToWho.NearestMateBehind:
                BuffNearest(GetNearest(b, CurrentLevel.ToWhoCount));
                break;

            case ToWho.AllMates:
                BuffAllMates();
                break;
        }

        if (CurrentLevel.ToWho != ToWho.None)
            EventManager.Instance.OnBuff?.Invoke();

        yield return null;

        Coroutine = null;
    }

    private void BuffRandomMate()
    {
        List<UnitController> teams = AllBotsIn(teamSlots);

        for (int i = 0; i < CurrentLevel.ToWhoCount; i++)
        {
            if (teams.Count == 0)
                return;

            var unit = teams[new Random().Next(0, teams.Count)];

            if (BuffUnit(unit))
                teams.Remove(unit);
        }
    }

    private void BuffTargetByItem()
    {
        if (Targets.Count <= 0)
            return;

        var unit = Targets.Dequeue();
        BuffUnit(unit);
    }

    private void BuffNearest(List<UnitController> _units)
    {
        if (_units.Count == 0)
            return;

        for (int i = 0; i < _units.Count; i++)
        {
            BuffUnit(_units[i]);
        }
    }

    private void BuffAllMates()
    {
        List<UnitController> teams = AllBotsIn(teamSlots);

        for (int i = 0; i < teams.Count; i++)
        {
            BuffUnit(teams[i]);
        }
    }

    private bool BuffUnit(UnitController _unit)
    {
        if (_unit == null)
            return false;

        _unit.Buff(
            IsPernament(CurrentLevel.AbilityDuration),
            CurrentLevel.Buff);

        return true;
    }
}
