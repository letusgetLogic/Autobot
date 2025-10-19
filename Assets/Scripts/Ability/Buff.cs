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

    public override void Activate()
    {
        if (toWho == ToWho.RandomFriend)
        {
            var slots = Controller.Model.IsTeam1 ? 
                PhaseBattleController.Instance.Slots1 :
                PhaseBattleController.Instance.Slots2;

            List<int> index = new List<int>();

            for (int i = 0; i < slots.Length; i++)
            {
                var unit = slots[i].UnitController();
                if (unit != null && unit != Controller)
                {
                    index.Add(i);
                }
            }

            List<int> alreadyBuffed = new List<int>();

            for (int i = 0; i < toWhoCount && i < index.Count; i++)
            {
                Random rnd = new Random();
                int n = rnd.Next(index[0], index.Count);

                if (alreadyBuffed.Contains(n))
                {
                    i--;
                    continue;
                }

                var unit = slots[n].UnitController();
                unit.Buff(health, attack);
            }
        }
    }

}
