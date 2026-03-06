using System;
using System.Collections;
using UnityEngine;

public class Steal : AbilityBase
{
    /// <summary>
    /// Constructor of Steal.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teamSlots"></param>
    /// <param name="_targets"></param>
    public Steal(UnitController _controller, Level _currentLevel, int _seed) : 
        base(_controller, _currentLevel, _seed)
    {
    }

    protected override IEnumerator Activate()
    {
        switch(CurrentLevel.FromWho)
        {
            case FromWho.AttackingEnemy:
                if (Targets.Count > 0)
                {
                    yield return new WaitForSeconds(Controller.View.Settings.DurationShowTemporaryValue);

                    var target = Targets.Dequeue();

                    int stolen = StolenEnergy(target, Controller);

                    target.SetEnergy(stolen, default);
                    Controller.SetEnergy(Math.Abs(stolen), true);

                    yield return new WaitForSeconds(Controller.View.Settings.DurationShowTemporaryValue);
                }
                break;
        }

        yield return null;

        Coroutine = null;
    }

    /// <summary>
    /// Calculates the amount of energy stolen from a target unit based on the causer's debuff value.
    /// </summary>
    /// <param name="_target">The unit from which energy is being stolen.</param>
    /// <param name="_causer">The unit causing the energy to be stolen.</param>
    /// <returns>The amount of energy stolen, as a negative value up to the target's current energy.</returns>
    public static int StolenEnergy(UnitController _target, UnitController _causer)
    {
        int targetENG = _target.Model.Data.Cur.ENG;
        int debuffENG = _causer.Model.CurrentLevel.Debuff.ENG;

        return targetENG < Math.Abs(debuffENG) ? -targetENG : debuffENG;
    }
}

