public class Shutdown : AbilityBase
{
    /// <summary>
    /// Constructor of Buff.
    /// </summary>
    /// <param name="_controller"></param>
    /// <param name="_currentLevel"></param>
    /// <param name="_teanSlots"></param>
    public Shutdown(UnitController _controller, Level _currentLevel, Slot[] _teanSlots) : base(_controller, _currentLevel, _teanSlots)
    {
    }

    public override void Activate()
    {
        switch(CurrentLevel.ToWho)
        {
            case ToWho.None:
                return;

            case ToWho.TargetBot:
                if (PhaseShopUnitManager.Instance != null &&
                    PhaseShopUnitManager.Instance.TargetedController != null)
                {
                    PhaseShopUnitManager.Instance.TargetedController.TriggerShutdown();
                }
                break;



        }

    }
}
