using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class UnitController : MonoBehaviour
{
    public UnityAction Attack { get; private set; }
    public UnityAction Faint { get; private set; }

    public UnitView View => view;
    [SerializeField]
    private UnitView view;

    public UnitModel Model => model;
    private UnitModel model;

    public AbilityBase Ability => AbilityBase.GetAbility(this, model.CurrentLevel);
    public int SlotIndex
    {
        get
        {
            var parent = transform.parent;
            if (parent != null &&
                (parent.CompareTag("Slot Battle") || parent.CompareTag("Slot Team")))
            {
                return parent.GetComponent<Slot>().Index;
            }
            return -1;
        }
    }

    private bool flipSprite = false;

    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_soUnit"></param>
    public void Initialize(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState,
        UnityAction _unityAction, bool _isTeamLeft)
    {
        // If the saved data has no reference, 
        if (_data.HasReference == false)
        {
            // ... create a new model with SO reference and the given index,

            model = new UnitModel(_soUnit, _index,
                GameManager.Instance.IsRepairSystemActive ? new RepairSystem() : null);
        }
        else // otherwise create a new model with SO reference and the saved data.
        {
            model = new UnitModel(_soUnit, _data,
                GameManager.Instance.IsRepairSystemActive ? new RepairSystem() : null);
        }

        flipSprite = !_isTeamLeft;
        _unityAction?.Invoke();
        model.InitView(view);
        model.SetData(_unitState);
    }

    /// <summary>
    /// Flips the sprite when the unit is on the right team.
    /// </summary>
    public void FlipSprite()
    {
        if (flipSprite)
        {
            view.SetRightSide();
            model.Data.IsTeamLeft = false;
        }
        else
        {
            model.Data.IsTeamLeft = true;
        }
    }


    #region PhaseShop

    /// <summary>
    /// Triggers the ability if it is existent and destroys the unit.
    /// </summary>
    public void GetSelled()
    {
        var ability = TriggerAbility(TriggerType.Sell);
        if (ability != null)
            ability.Activate();

        Destroy(gameObject);
    }

    /// <summary>
    /// Updates stats while fusioning.
    /// </summary>
    public void UpdateLevel(UnitModel _draggingModel, bool _isPhaseShop)
    {
        model.Data.SetXP(model.Data.XP + _draggingModel.Data.XP);

        model.Add(
            new Attribute(_draggingModel.Data.XP, _draggingModel.Data.XP),
            _draggingModel.Data.Buff,
            _draggingModel.Data.TempBuff,
            _draggingModel.Data.Cur,
            _draggingModel.Data.Cur.HP == _draggingModel.Data.FullHP);

        model.UpdateLevelXP(_isPhaseShop);
        model.Repair?.SetDurability(true, false, 0, 0);
    }

    #endregion


    #region Phase Battle

    /// <summary>
    /// Triggers the ability while attacking.
    /// </summary>
    /// <returns></returns>
    public int TriggerAttack()
    {
        //var ability = TriggerAbility(TriggerType.BeforeAttack);
        //if (ability != null)
        //{
        //    PhaseBattleController.Instance.StartCoroutine(
        //       ability.Handle(
        //           PhaseBattleController.Instance.Process.DurationEachAbility));
        //}

        Attack?.Invoke();

        var ability = TriggerAbility(TriggerType.AfterAttack);
        if (ability != null)
            PhaseBattleController.Instance.UnitAbilities.Enqueue(ability);

        return model.Data.Cur.ATK;
    }

    /// <summary> 
    /// Takes damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        model.ReduceHp(damage);

        if (model.Data.Cur.HP <= 0)
        {
            TriggerFaint();
            PhaseBattleController.Instance.FaintUnits.Enqueue(gameObject);
            Debug.Log($"{gameObject.name} enqueue");
            Debug.Log($"{PhaseBattleController.Instance.FaintUnits.Count} FaintUnits");
        }
    }

    #endregion

    /// <summary>
    /// Sets and updates the view of the energy.
    /// </summary>
    /// <param name="_energy"></param>
    public void SetEnergy(int _energy)
    {
        int value = model.Data.Cur.ENG + _energy;

        model.Data.SetEnergy(value);

        if (_energy > 0)
            view.ShowBuff(0, 0, _energy);
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
    /// <param name="triggerType"></param>
    /// <returns></returns>
    public AbilityBase TriggerAbility(TriggerType triggerType)
    {
        if (model.Data.Cur.ENG < model.CurrentLevel.ConsumedEnergy.Value)
            return null;

        if (triggerType == model.CurrentLevel.TriggerType)
            return Ability;

        return null;
    }

    /// <summary>
    /// Add buff to the model data and updates the view.
    /// </summary>
    /// <param name="isPernament"></param>
    /// <param name="addHealth"></param>
    /// <param name="addAttack"></param>
    public void Buff(bool isPernament, int addHealth, int addAttack)
    {
        if (isPernament)
            model.Add(new Attribute(addHealth, addAttack), new Attribute(0, 0));
        else
            model.Add(new Attribute(0, 0), new Attribute(addHealth, addAttack));

        view.ShowBuff(addHealth, addAttack, 0);
    }

    /// <summary>
    /// Triggers the ability while unit is fainting.
    /// </summary>
    public void TriggerFaint()
    {
      Debug.Log($"{name} faint");
        var ability = TriggerAbility(TriggerType.Faint);
        if (ability != null)
        {
            PhaseBattleController.Instance.UnitAbilities.Enqueue(ability);
            Faint?.Invoke();
            Debug.Log($"{ability.ToString()} enqueue");
            Debug.Log($"{PhaseBattleController.Instance.UnitAbilities.Count} UnitAbilities");
        }
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
    /// Moves the objects to the target.
    /// </summary>
    /// <param name="target"></param>
    public void MoveTo(Vector3 target)
    {
        //transform.DoMove();
    }
}
