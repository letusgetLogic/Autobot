using UnityEngine.Events;

public class EventManager
{
    #region Instance Lazy Loading
    public static EventManager Instance
    {
        get
        {
            // Lazy loading
            if (_Instance == null)
            {
                _Instance = new EventManager();
            }

            return _Instance;
        }
    }
    private static EventManager _Instance;

    private EventManager() { }
    #endregion


    public UnityAction OnButtonSound { get; set; }

    #region Menu
    public UnityAction OnIncreaseLives { get; set; }
    public UnityAction OnDecreaseLives { get; set; }
    public UnityAction OnInvalidInput { get; set; }
    #endregion


    public UnityAction OnCloseScene { get; set; }
    public UnityAction OnMoveHintClick { get; set; }
    public UnityAction OnOpenScene { get; set; }

    

    #region Phase Shop

    public UnityAction<UnitController> OnAttachedUnit { get; set; }
    public UnityAction<UnitController> OnAttachedUnitCatalog { get; set; }
    public UnityAction OnDropUnit { get; set; }

    public UnityAction OnRoll {  get; set; }
    public UnityAction<InputKey> OnEndTurnClick {  get; set; }
    public UnityAction OnEndShop {  get; set; }

    public UnityAction<InputKey> OnCraft {  get; set; }
    public UnityAction<InputKey> OnRecycle { get; set; }

    public UnityAction<InputKey> OnRepair {  get; set; }

    public UnityAction<InputKey> OnLock {  get; set; }
    public UnityAction<InputKey> OnUnlock {  get; set; }

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


    #region Phase Battle

    public UnityAction OnInitDone { get; set; }
    public UnityAction OnBattleDone { get; set; }

    public UnityAction OnMatchOver { get; set; }
    public UnityAction OnBattleDelayHintClick { get; set; }
    public UnityAction OnGameOver { get; set; }
    #endregion
}

