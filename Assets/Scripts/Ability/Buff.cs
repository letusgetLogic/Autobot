using System;
using System.Collections.Generic;

public class Buff : AbilityBase
{
    private FromWho fromWho;
    private ToWho toWho;
    private int toWhoCount;
    private int health;
    private int attack;

    public Buff(UnitController controller, AbilityDuration duration, 
        FromWho fromWho, ToWho toWho, int count,
        int health, int attack) : 
        base(controller, duration)
    {
        this.fromWho = fromWho;
        this.toWho = toWho;
        this.toWhoCount = count;
        this.health = health;
        this.attack = attack;
    }

    public override void Activate(bool isBattle)
    {
        if (toWho == ToWho.RandomFriend)
        {
            var slots = Controller.Model.IsTeam1 ? 
                PhaseBattleController.Instance.Slots1 :
                PhaseBattleController.Instance.Slots2;

            List<UnitController> unitOnSlot = new List<UnitController>();

            for (int i = 0; i < slots.Length; i++)
            {
                var unit = slots[i].UnitController();
                if (unit != null && unit != Controller)
                {
                    unitOnSlot.Add(unit);
                }
            }

            for (int i = 0; i < toWhoCount && i < unitOnSlot.Count; i++)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, unitOnSlot.Count);

                var unit = unitOnSlot[index];

                unit.Buff(AbilityBase.IsPernament(Duration, isBattle), health, attack);

                unitOnSlot.Remove(unit);
            }
        }
    }
}
