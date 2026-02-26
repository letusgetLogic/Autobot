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
    public Buff(UnitController _controller, Level _currentLevel, int _seed)
        : base(_controller, _currentLevel, _seed)
    {
        teamSlots = _controller.TeamSlots;
    }

    protected override IEnumerator Activate()
    {
        Random rand = new Random(RandomSeed); 

        switch (CurrentLevel.ToWho)
        {
            case ToWho.None:
                UnityEngine.Debug.LogWarning($"{Controller.name} has ToWho.None!");
                break;

            case ToWho.Self:
                BuffUnit(Controller);
                break;

            case ToWho.RandomMate:
                BuffRandomMate(rand);
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

    private void BuffRandomMate(Random _rnd)
    {
        List<UnitController> teams = AllBotsIn(teamSlots);

        for (int i = 0; i < CurrentLevel.ToWhoCount; i++)
        {
            if (teams.Count == 0)
                return;

            var unit = teams[_rnd.Next(0, teams.Count)];

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

        if (Controller.Model.Data.UnitType == UnitType.Item &&
            IsPernament(CurrentLevel.AbilityDuration) == false)
            {
            _unit.View.SetTemporaryItem(true);
        }

        return true;
    }
}
