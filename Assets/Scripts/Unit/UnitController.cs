using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public UnitView View => view;

    public UnitModel Model => model;
    private UnitModel model;

    public AbilityBase Ability => AbilityBase.GetAbility(
        this, 
        model.CurrentLevel, 
        Targets
        );

    public Queue<UnitController> Targets
    {
        get
        {
            if (targets == null)
                targets = new Queue<UnitController>();
            return targets;
        }
    }
    private Queue<UnitController> targets;


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

    public Slot[] EnemySlots
    {
        get
        {
            if (PhaseBattleController.Instance != null)
            {
                return model.Data.IsTeamLeft ?
                    PhaseBattleController.Instance.Slots2() :
                    PhaseBattleController.Instance.Slots1();
            }
            else return null;
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
            bool isRepairActive = GameManager.Instance.IsRepairSystemActive && IsRobot(_data.UnitType);

            model = new UnitModel(this, _soUnit, _data, isRepairActive ? new RepairSystem() : null);
        }

        model.InitView(view, _isTeamLeft);
        model.SetData(_unitState);
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

    #region Triggers

    /// <summary>
    /// Triggers the ability while bot is shuting down.
    /// </summary>
    public void TriggerShutdown()
    {
        Debug.Log($"{name} shut down");

        var ability = TriggerAbility(TriggerType.Shutdown);
        if (ability != null)
        {
            EventManager.Instance.OnTriggerAbility?.Invoke(ability, true);
        }
        else
        {
            EventManager.Instance.OnShutdown?.Invoke(this);
        }
    }


    /// <summary>
    /// Triggers the ability if it is existent.
    /// </summary>
    public void TriggerCraft()
    {
        var ability = TriggerAbility(TriggerType.Craft);
        if (ability != null)
        {
            EventManager.Instance.OnTriggerAbility?.Invoke(ability, false);
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
            EventManager.Instance.OnTriggerAbility?.Invoke(ability, true);
        }
        else
        {
            GameManager.Instance.IsBlockingInput = false;
            Destroy(gameObject);
        }
    }

    public bool TriggerStartOfBattle()
    {
        var ability = TriggerAbility(TriggerType.StartOfBattle);
        if (ability != null)
        {
            EventManager.Instance.OnTriggerAbility?.Invoke(ability, false);
        }
        return ability != null;
    }

    public AbilityBase TriggerBeforeAttack(UnitController _enemy)
    {
        var ability = TriggerAbility(TriggerType.BeforeAttack);
        if (ability != null)
        {
            if (model.CurrentLevel.DoType == DoType.Steal)
            {
                if (_enemy.Model.Data.Cur.ENG <= 0)
                    return null;
            }

            if (model.CurrentLevel.FromWho == FromWho.AttackingEnemy)
                Targets.Enqueue(_enemy);
        }

        return ability;
    }

    /// <summary>
    /// Triggers the ability while attacking.
    /// </summary>
    /// <returns></returns>
    public Damage TriggerAttack()
    {
        EventManager.Instance.OnAttack?.Invoke();

        var ability = TriggerAbility(TriggerType.AfterAttack);
        if (ability != null)
            EventManager.Instance.OnTriggerAbility?.Invoke(ability, default);

        return new Damage(model.Data.Cur.ATK);
    }

    #endregion



    #region PhaseShop

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
        model.UpdateLevelXP(_isPhaseShop, true);
        model.Repair?.SetDurability(false);

        EventManager.Instance.OnFusion?.Invoke();
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

    #endregion


    #region Phase Battle

    /// <summary> 
    /// Takes damage.
    /// </summary>
    /// <param name="_damage"></param>
    public void TakeDamage(Damage _damage)
    {
        model.ReduceHp(_damage);

        EventManager.Instance.OnHurt?.Invoke();

        if (model.Data.Cur.HP <= 0)
        {
            view.SetShutdown();
            TriggerShutdown();
        }
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

    #endregion


    #region Manage Attributes

    /// <summary>
    /// Sets and updates the view of the energy.
    /// </summary>
    /// <param name="_addEnergy"></param>
    /// <param name="_onBuff"></param>
    public void SetEnergy(int _addEnergy, bool _onBuff)
    {
        int value = model.Data.Cur.ENG + _addEnergy;

        model.Data.SetEnergy(value);

        if (_addEnergy > 0)
        {
            view.ShowBuff(new Attribute(0, 0, _addEnergy));

            if (_onBuff)
                EventManager.Instance.OnBuff?.Invoke();
        }
        else
        {
            view.ShowConsume(_addEnergy);
        }

        view.SetData(
            model.Data.FullHP, model.Data.FullATK,
            model.Data.Cur.HP, model.Data.Cur.ATK, model.Data.Cur.ENG);
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
            model.Add(_attribute, new Attribute(0, 0, 0));
        else
            model.Add(new Attribute(0, 0, 0), _attribute);

        view.ShowBuff(_attribute);
        model.Repair?.SetDurability(false);
    }

    #endregion



    /// <summary>
    /// Moves the objects to the target and set it to the parent.
    /// </summary>
    /// <param name="_target"></param>
    public float MoveToParent(Vector3 _target, Transform _parent)
    {
        float animDelay = toNextSlotMover.MoveTo(_target, _parent);

        return animDelay;
    }

    /// <summary>
    /// Sets slot as parent.
    /// </summary>
    /// <param name="_parent"></param>
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
    /// Check if item can be dropped.
    /// </summary>
    /// <returns></returns>
    public bool CanItemBeDropped()
    {
        if (Slot == null)
            return false;

        if (model.SoUnit.UnitType == UnitType.Item && Slot.CompareTag("Slot Team"))
            return true;

        return false;
    }

    /// <summary>
    /// Sets parent null and game object active false.
    /// </summary>
    public IEnumerator Deactivate(float _delay)
    {
        transform.parent = null;
        view.HideVisual();

        yield return new WaitForSeconds(_delay);
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
