using System;
using System.Collections.Generic;

public class Buff : AbilityBase
{
    /// <summary>
    /// Constructor of Buff.
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="currentLevel"></param>
    public Buff(UnitController controller, Level currentLevel) : base(controller, currentLevel)
    {
    }

    public override void Run()
    {
        if (CurrentLevel.ToWho == ToWho.RandomFriend)
        {
            Slot[] slots;

            if (GameManager.Instance.IsPhaseBattle)
            {
                slots = Controller.Model.Data.IsTeamLeft ?
                    PhaseBattleController.Instance.Slots1 :
                    PhaseBattleController.Instance.Slots2;
            }
            else
            {
                slots = PhaseShopUnitManager.Instance.TeamSlots;
            }

            List<UnitController> unitOnSlot = new List<UnitController>();

            for (int i = 0; i < slots.Length; i++)
            {
                var unit = slots[i].UnitController();
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
