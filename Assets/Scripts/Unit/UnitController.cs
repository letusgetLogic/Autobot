using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

public class UnitController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitView view;
    [SerializeField] private LerpMovement toNextSlotMover;
    [SerializeField] private LerpMovement attackMover;

    [Header("Editor Mode")]
    [SerializeField] private SoUnit editorSoUnit;
    [SerializeField] private bool editorRename = false;
    [SerializeField] private bool editorIsOnRightSide = false;
    [SerializeField] private UnitState editorUnitState;
    [SerializeField] private bool editorIsRepairOnValidateActive = true;
    [SerializeField] private SoPack editorDefinedPack;
    public SoPack DefinedPack => editorDefinedPack;

    public event Action<Slot> OnAttack;
    public event Action<Slot> OnShutdown;

    public UnitView View => view;

    public UnitModel Model => model;
    private UnitModel model;

    public AbilityBase Ability => AbilityBase.GetAbility(this, model.CurrentLevel, TeamSlots, Slot, TargetedByItem);
    public UnitController TargetedByItem { get; set; } = null;

    public SoPack Pack
    {
        get
        {
            if (Application.isPlaying == false)
                return DefinedPack;

            return PackManager.Instance.MyPack;
        }
    }

    public Slot[] TeamSlots
    {
        get
        {
            if (PhaseBattleController.Instance != null)
            {
                return model.Data.IsTeamLeft ?
                    PhaseBattleController.Instance.Slots1() :
                    PhaseBattleController.Instance.Slots2();
            }
            else return PhaseShopController.Instance.TeamSlots();
        }
    }
    public Slot Slot
    {
        get
        {
            var parent = transform.parent;
            if (parent != null)
            {
                return parent.GetComponent<Slot>();
            }
            return null;
        }
    }


    private void OnValidate()
    {
        if (editorSoUnit != null)
        {
            if (editorRename)
                gameObject.name = editorSoUnit.Name;

             Initialize(editorSoUnit, 0, default, editorUnitState, !editorIsOnRightSide);
        }
    }

    private void OnEnable()
    {
        toNextSlotMover.OnPosition += SetParent;
    }

    private void OnDisable()
    {
        toNextSlotMover.OnPosition -= SetParent;
    }

    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_soUnit"></param>
    public void Initialize(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState, bool _isTeamLeft)
    {
        // If the saved data has no reference, 
        if (_data.HasReference == false)
        {
            // ... create a new model with SO reference and the given index,

            bool isRepairActive = GameManager.Instance == null
                ? editorIsRepairOnValidateActive
                : GameManager.Instance.IsRepairSystemActive && IsRobot(_soUnit.UnitType);

            model = new UnitModel(this, _soUnit, _index, isRepairActive ? new RepairSystem() : null);
        }
        else // otherwise create a new model with SO reference and the saved data.
        {
            model = new UnitModel(this, _soUnit, _data,
                GameManager.Instance.IsRepairSystemActive && IsRobot(_data.UnitType)
                ? new RepairSystem() : null);
        }

        SetSprite(_isTeamLeft);
        model.InitView(view);
        model.SetData(_unitState);
    }

    /// <summary>
    /// Flips the sprite when the unit is on the right team.
    /// </summary>
    public void SetSprite(bool _isTeamLeft)
    {
        if (_isTeamLeft)
        {
            model.Data.IsTeamLeft = true;
        }
        else
        {
            view.SetRightSide();
            model.Data.IsTeamLeft = false;
        }
    }


    #region PhaseShop

    /// <summary>
    /// Triggers the ability if it is existent and destroys the unit.
    /// </summary>
    public void TriggerCraft()
    {
        var ability = TriggerAbility(TriggerType.Craft);
        if (ability != null)
        {
            PhaseShopController.Instance.StartCoroutine(
                PhaseShopController.Instance.HandleAbility(ability, false));
        }
    }

    /// <summary>
    /// Triggers the ability if it is existent and destroys the unit.
    /// </summary>
    public void GetSelled()
    {
        var ability = TriggerAbility(TriggerType.Recycle);
        if (ability != null)
        {
            PhaseShopController.Instance.StartCoroutine(
                PhaseShopController.Instance.HandleAbility(ability, true));
        }
        else
        {
            GameManager.Instance.IsBlockingInput = false;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates stats while fusioning.
    /// </summary>
    public void UpdateLevel(UnitModel _draggingModel, bool _isPhaseShop)
    {
        model.AddFusion(
            new Attribute(_draggingModel.Data.XP, _draggingModel.Data.XP),
            _draggingModel.Data.Buff,
            _draggingModel.Data.TempBuff,
            _draggingModel.Data.Cur,
            _draggingModel.Data.Cur.HP == _draggingModel.Data.FullHP);

        model.Data.SetXP(model.Data.XP + _draggingModel.Data.XP);
        model.UpdateLevelXP(_isPhaseShop);
        model.Repair?.SetDurability(false);

        SoundManager.Instance.PlayFusionSound();
    }

    #endregion


    #region Phase Battle

    /// <summary>
    /// Triggers the ability while attacking.
    /// </summary>
    /// <returns></returns>
    public uint TriggerAttack()
    {
        //var ability = TriggerAbility(TriggerType.BeforeAttack);
        //if (ability != null)
        //{
        //    PhaseBattleController.Instance.StartCoroutine(
        //       ability.Handle(
        //           PhaseBattleController.Instance.Process.DurationEachAbility));
        //}

        OnAttack?.Invoke(Slot);

        var ability = TriggerAbility(TriggerType.AfterAttack);
        if (ability != null)
            PhaseBattleController.Instance.UnitAbilities.Enqueue(ability);

        return (uint)model.Data.Cur.ATK;
    }

    /// <summary> 
    /// Takes damage.
    /// </summary>
    /// <param name="_damage"></param>
    public void TakeDamage(uint _damage)
    {
        model.ReduceHp(_damage);

        if (model.Data.Cur.HP <= 0)
        {
            TriggerShutdown();
            PhaseBattleController.Instance.ShutdownUnits.Enqueue(gameObject);

            Debug.Log($"{gameObject.name} enqueue");
            Debug.Log($"{PhaseBattleController.Instance.ShutdownUnits.Count} FaintUnits");
        }
    }

    #endregion

    /// <summary>
    /// Triggers the ability while bot is shuting down.
    /// </summary>
    public void TriggerShutdown()
    {
        Debug.Log($"{name} shut down");

        var ability = TriggerAbility(TriggerType.Shutdown);
        if (ability != null)
        {
            if (PhaseBattleController.Instance != null)
            {
                PhaseBattleController.Instance.UnitAbilities.Enqueue(ability);
                OnShutdown?.Invoke(Slot);

                Debug.Log($"{ability.ToString()} enqueue");
                Debug.Log($"{PhaseBattleController.Instance.UnitAbilities.Count} UnitAbilities");
            }
            if (PhaseShopController.Instance != null)
            {
                PhaseShopController.Instance.StartCoroutine(ability.Handle(
                    PhaseShopController.Instance.Process.DelayHideDescription,
                    true));
            }
        }
        else
        {
            if (PhaseShopController.Instance != null)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Sets and updates the view of the energy.
    /// </summary>
    /// <param name="_energy"></param>
    public void SetEnergy(int _energy)
    {
        int value = model.Data.Cur.ENG + _energy;

        model.Data.SetEnergy(value);

        if (_energy > 0)
            view.ShowBuff(new Attribute(0, 0, _energy));
        else
        {
            view.ShowConsume(_energy);
        }

        view.SetData(
            model.Data.FullHP, model.Data.FullATK,
            model.Data.Cur.HP, model.Data.Cur.ATK, model.Data.Cur.ENG);
    }

    /// <summary>
    /// Returns the ability if the energy isn't smaller as the value of energy to consume.
    /// </summary>
    /// <param name="_triggerType"></param>
    /// <returns></returns>
    public AbilityBase TriggerAbility(TriggerType _triggerType)
    {
        int consumedEnergy = 0;
        if (model.CurrentLevel.ConsumedEnergy != null)
        {
            consumedEnergy = Mathf.Abs(model.CurrentLevel.ConsumedEnergy.Value);
        }

        if (model.Data.Cur.ENG < consumedEnergy)
            return null;

        if (_triggerType == model.CurrentLevel.TriggerType)
            return Ability;

        return null;
    }

    /// <summary>
    /// Add buff to the model data and updates the view.
    /// </summary>
    /// <param name="_isPernament"></param>
    /// <param name="_addHealth"></param>
    /// <param name="_addAttack"></param>
    public void Buff(bool _isPernament, Attribute _attribute)
    {
        if (_isPernament)
            model.AddBuff(_attribute, new Attribute(0, 0, 0));
        else
            model.AddBuff(new Attribute(0, 0, 0), _attribute);

        view.ShowBuff(_attribute);
        model.Repair?.SetDurability(false);
    }

    /// <summary>
    /// Begins swap.
    /// </summary>
    public void BeginSwap()
    {
        transform.SetParent(null, true);

        // Position the root game object to the sprite.
        var spritePos = view.DragSpritePosition;
        view.SetLocalPositionDefault();
        transform.position = spritePos;

        view.SetSpriteOverOther();
    }

    /// <summary>
    /// Moves the objects to the target.
    /// </summary>
    /// <param name="_target"></param>
    public void MoveTo(Vector3 _target)
    {
        toNextSlotMover.MoveTo(_target, null);
        //transform.DoMove();
    }

    /// <summary>
    /// Moves the objects to the target.
    /// </summary>
    /// <param name="_target"></param>
    public float MoveWhileAttacking()
    {
        float direction = model.Data.IsTeamLeft ? 1 : -1;

        Vector3 deltaPosition = new Vector3(
            attackMover.SoSettings.DeltaPosition.x * direction,
            attackMover.SoSettings.DeltaPosition.y,
            attackMover.SoSettings.DeltaPosition.z);

        float animDelay = attackMover.MoveWithDelta(deltaPosition);

        return animDelay;
    }

    /// <summary>
    /// Moves the objects to the target and set it to the parent.
    /// </summary>
    /// <param name="_target"></param>
    public float MoveToParent(Vector3 _target, Transform _parent)
    {
        float animDelay = toNextSlotMover.MoveTo(_target, _parent);

        return animDelay;
    }

    private void SetParent(Transform _parent)
    {
        if (_parent != null)
            transform.SetParent(_parent, true);
    }


    /// <summary>
    /// Return boolean, if it is a robot.
    /// </summary>
    /// <returns></returns>
    public bool IsRobot(UnitType _unitType)
    {
        if (_unitType == UnitType.Robot || _unitType == UnitType.SummonedRobot)
            return true;

        return false;
    }

    /// <summary>
    /// Sets parent null and game object active false.
    /// </summary>
    public void Deactivate()
    {
        transform.parent = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Detroys game object.
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
