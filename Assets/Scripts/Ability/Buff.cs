using System;
using System.Collections.Generic;

public class Buff : AbilityBase
{
    /// <summary>
    /// Constructor of Buff.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teanSlots"></param>
    public Buff(UnitController _controller, Level _currentLevel, Slot[] _teanSlots) : base(_controller, _currentLevel, _teanSlots)
    {
    }

    public override void Activate()
    {
        switch(CurrentLevel.ToWho)
        {
            case ToWho.None:
                return;

            case ToWho.RandomMate:
                BuffRandom();
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

            case ToWho.AllFriends:
                BuffAll();
                break;
        }
    }

    private void BuffAll()
    {
        List<UnitController> teamUnitControllers = GetAllMates();

        for (int i = 0; i < teamUnitControllers.Count; i++)
        {
            BuffUnit(teamUnitControllers[i]);
        }
    }

    private void BuffRandom()
    {
        List<UnitController> teamUnitControllers = GetAllMates();

        for (int i = 0; i < CurrentLevel.ToWhoCount; i++)
        {
            var unit = GetRandomIn(ref teamUnitControllers);
            BuffUnit(unit);

            teamUnitControllers.Remove(unit);
        }
    }

    private void BuffTargetByItem()
    {
        if (PhaseShopUnitManager.Instance != null &&
                   PhaseShopUnitManager.Instance.TargetedController != null)
        {
            PhaseShopUnitManager.Instance.TargetedController.Buff(true, CurrentLevel.Buff);
        }
       
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

    private void BuffUnit(UnitController _unit)
    {
        if (_unit == null)
            return;

        _unit.Buff(
            IsPernament(CurrentLevel.AbilityDuration),
            CurrentLevel.Buff);
    }
}
