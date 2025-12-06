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

    public override void Run()
    {
        if (CurrentLevel.ToWho == ToWho.RandomFriend)
        {
            List<UnitController> unitOnSlot = new List<UnitController>();

            for (int i = 0; i < TeamSlots.Length; i++)
            {
                var unit = TeamSlots[i].UnitController();
                if (unit != null && unit != Controller)
                {
                    unitOnSlot.Add(unit);
                }
            }

            if (unitOnSlot.Count <= 0)
                return;

            for (int i = 0; i < CurrentLevel.ToWhoCount; i++)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, unitOnSlot.Count);

                var unit = unitOnSlot[index];

                unit.Buff(
                    AbilityBase.IsPernament(CurrentLevel.AbilityDuration), 
                    CurrentLevel.HealthBuff, 
                    CurrentLevel.AttackBuff);

                unitOnSlot.Remove(unit);
            }
        }
    }
}
