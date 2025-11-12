using System;
using System.Collections.Generic;

public class Buff : AbilityBase
{
    private FromWho fromWho;
    private ToWho toWho;
    private int toWhoCount;
    private int health;
    private int attack;

    public Buff(UnitController controller, AbilityDuration duration, Level currentLevel) :
        base(controller, duration, currentLevel)
    {
        this.fromWho = currentLevel.FromWho;
        this.toWho = currentLevel.ToWho;
        this.toWhoCount = currentLevel.ToWhoCount;
        this.health = currentLevel.HealthBuff;
        this.attack = currentLevel.AttackBuff;
    }

    public override void Run()
    {
        if (toWho == ToWho.RandomFriend)
        {
            Slot[] slots;

            if (GameManager.Instance.IsPhaseBattle)
            {
                slots = Controller.Model.IsTeam1 ?
                    PhaseBattleController.Instance.Slots1 :
                    PhaseBattleController.Instance.Slots2;
            }
            else
            {
                slots = PhaseShopUnitManager.Instance.BattleSlots;
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

            for (int i = 0; i < toWhoCount; i++)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, unitOnSlot.Count);

                var unit = unitOnSlot[index];

                unit.Buff(
                    AbilityBase.IsPernament(Duration), 
                    health, 
                    attack);

                unitOnSlot.Remove(unit);
            }
        }
    }
}
