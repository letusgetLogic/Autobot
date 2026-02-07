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

    public Action<UnitController> OnAttachedUnit { get; set; }
    public Action OnDropUnit { get; set; }

    public Action OnRoll {  get; set; }
    public Action OnEndTurn {  get; set; }

    public Action OnCraft {  get; set; }
    public Action OnRecycle { get; set; }

    public Action OnRepair {  get; set; }

    public Action OnLock {  get; set; }
    public Action OnUnlock {  get; set; }

    public Action OnFusion { get; set; }
    public Action OnLevelUp { get; set; }

    public Action OnSwap { get; set; }

    public Action OnNotEnoughCurrency { get; set; }


    public Action<AbilityBase, bool> OnTriggerAbility { get; set; }


    #endregion



    #region Unit Actions

    public Action OnAttack { get; set; }
    public Action<UnitController> OnShutdown { get; set; }
    public Action OnBuff { get; set; }
    public Action OnSummon { get; set; }


    #endregion



    public Action OnMatchOver { get; set; }
    public Action OnGameOver { get; set; }
}
