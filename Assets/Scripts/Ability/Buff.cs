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

            case ToWho.RandomTeammate:
                BuffRandom();
                break;

            case ToWho.TargetBot:
                BuffTarget();
                break;

        }

    }

    private void BuffRandom()
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

        if (teamUnitControllers.Count <= 0)
            return;

        for (int i = 0; i < CurrentLevel.ToWhoCount; i++)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, teamUnitControllers.Count);

            var unit = teamUnitControllers[index];

            unit.Buff(
                AbilityBase.IsPernament(CurrentLevel.AbilityDuration),
                CurrentLevel.Buff);

            teamUnitControllers.Remove(unit);
        }
    }

    private void BuffTarget()
    {
        if (PhaseShopUnitManager.Instance != null &&
                   PhaseShopUnitManager.Instance.TargetedController != null)
        {
            PhaseShopUnitManager.Instance.TargetedController.Buff(true, CurrentLevel.Buff);
        }
       
    }
}
