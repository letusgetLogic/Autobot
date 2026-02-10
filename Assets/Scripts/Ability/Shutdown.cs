using System.Collections;

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
    public Shutdown(UnitController _controller, Level _currentLevel, Slot[] _teamSlots, UnitController _targetedByItem) 
        : base(_controller, _currentLevel, _teamSlots, _targetedByItem)
    {
    }

    protected override IEnumerator Activate()
    {
        switch(CurrentLevel.ToWho)
        {
            case ToWho.TargetBot:
                TargetedByItem.TriggerShutdown();
                break;
        }

        yield return null;

        Coroutine = null;
    }
}
