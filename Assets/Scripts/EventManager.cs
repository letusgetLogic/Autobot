using UnityEngine.Events;

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


    public UnityAction OnIncreaseLives { get; set; }
    public UnityAction OnDecreaseLives { get; set; }



    public UnityAction OnInvalidInput { get; set; }
    public UnityAction OnCloseScene { get; set; }
    public UnityAction OnMoveHintClick { get; set; }
    public UnityAction OnOpenScene { get; set; }



    #region Phase Shop

    public UnityAction<UnitController> OnAttachedUnit { get; set; }
    public UnityAction OnDropUnit { get; set; }

    public UnityAction OnRoll {  get; set; }
    public UnityAction OnEndTurn {  get; set; }

    public UnityAction OnCraft {  get; set; }
    public UnityAction OnRecycle { get; set; }

    public UnityAction OnRepair {  get; set; }

    public UnityAction OnLock {  get; set; }
    public UnityAction OnUnlock {  get; set; }

    public UnityAction OnFusion { get; set; }
    public UnityAction OnLevelUp { get; set; }

    public UnityAction OnSwap { get; set; }

    public UnityAction OnNotEnoughCurrency { get; set; }

    /// <summary>
    /// bool isDestroyingUnit
    /// </summary>
    public UnityAction<AbilityBase, bool> OnTriggerAbility { get; set; }


    #endregion



    #region Unit Actions

    public UnityAction OnAbilityDone { get; set; }

    public UnityAction OnAttack { get; set; }
    public UnityAction OnHurt { get; set; }
    public UnityAction<UnitController> OnShutdown { get; set; }
    public UnityAction OnBuff { get; set; }
    public UnityAction<UnitController> OnShootOut { get; set; }


    #endregion





    public UnityAction OnMatchOver { get; set; }
    public UnityAction OnWaitingForClick { get; set; }
    public UnityAction OnGameOver { get; set; }
}
