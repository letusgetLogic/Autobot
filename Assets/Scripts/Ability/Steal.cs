using System;
using System.Collections;
using System.Collections.Generic;
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
    public Steal(UnitController _controller, Level _currentLevel, Queue<UnitController> _targets) : 
        base(_controller, _currentLevel, _targets)
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

                    var unit = Targets.Dequeue();
                    unit.SetEnergy(CurrentLevel.Debuff.ENG, default);

                    Controller.SetEnergy(
                        Math.Abs(CurrentLevel.Debuff.ENG), true);

                    yield return new WaitForSeconds(Controller.View.Settings.DurationShowTemporaryValue);
                }
                break;
        }

        yield return null;

        Coroutine = null;
    }
}

