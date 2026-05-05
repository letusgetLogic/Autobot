using System.Collections;
using UnityEngine;

/// <summary>
/// Only items trigger Shutdown.
/// </summary>
public class Shutdown : AbilityBase
{
    /// <summary>
    /// Constructor of Shutdown.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teanSlots"></param>
    public Shutdown(UnitController _controller, Level _currentLevel, int _seed) 
        : base(_controller, _currentLevel, _seed)
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
                    unit.View.SetShutdown();

                    yield return new WaitForSeconds(0.5f);
                    unit.TriggerShutdown();
                }
                break;
        }

        yield return null;

        Coroutine = null;
    }
}
