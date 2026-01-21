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

    public override void Activate()
    {
        switch(CurrentLevel.ToWho)
        {
            case ToWho.None:
                return;

            case ToWho.TargetBot:
                TargetedByItem.TriggerShutdown();
                break;



        }

    }
}
