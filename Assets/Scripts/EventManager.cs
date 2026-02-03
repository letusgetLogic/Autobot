using System;

public class EventManager
{
    #region Instance Lazy Loading
    public static EventManager Instance
    {
        get
        {
            // Lazy loading
            if (instance == null)
            {
                instance = new EventManager();
            }

            return instance;
        }
    }
    private static EventManager instance;

    private EventManager() { }

    #endregion


    #region Phase Shop

    public Action<UnitController> OnSettingAttachedObject { get; set; }
    
    public Action OnTransportUnit { get; set; }
    public Action OnRoll {  get; set; }
    public Action OnRepair {  get; set; }
    public Action OnRecycle { get; set; }
    public Action OnLock {  get; set; }
    public Action OnUnlock {  get; set; }
    public Action OnUpgrade { get; set; }
    public Action OnEndTurn {  get; set; }
    public Action OnUpdateLevel { get; set; }


    public Action OnNotEnoughCurrency { get; set; }


    public Action<AbilityBase, bool> OnTriggerAbility { get; set; }


    #endregion


    public Action<UnitController> OnAttack { get; set; }
    public Action<UnitController> OnShutdown { get; set; }

}
