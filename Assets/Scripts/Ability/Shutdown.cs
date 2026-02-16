using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Only items trigger Shutdown.
/// </summary>
public class Shutdown : AbilityBase
{
    /// <summary>
    /// Constructor of Buff.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teanSlots"></param>
    public Shutdown(UnitController _controller, Level _currentLevel, Queue<UnitController> _targets, int _seed) 
        : base(_controller, _currentLevel, _targets, _seed)
    {
    }

    protected override IEnumerator Activate()
    {
        switch(CurrentLevel.ToWho)
        {
            case ToWho.TargetBot:
                if (Targets.Count > 0)
                {
                    var unit = Targets.Dequeue();
                    unit.TriggerShutdown();
                }
                break;
        }

        yield return null;

        Coroutine = null;
    }
}
