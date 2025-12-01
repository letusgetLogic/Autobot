using UnityEngine;
using UnityEngine.Events;

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
                GameManager.Instance.RepairSystem ? new RepairSystem() : null);
        }
        else // otherwise create a new model with SO reference and the saved data.
        {
            model = new UnitModel(_soUnit, _data,
                GameManager.Instance.RepairSystem ? new RepairSystem() : null);
        }
        flipSprite = !_isTeamLeft;
        _unityAction?.Invoke();
        model.InitView(view);
        model.SetData(_unitState);
    }


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
    /// Destroys the game object.
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
            _draggingModel.Data.Cur);

        model.UpdateLevelXP(_isPhaseShop);
        model.Repair?.SetDurability(true, false, 0, 0);
    }

    public void AddEnergy(int _addEnergy)
    {
        var data = model.Data;
        int value = data.Cur.Energy + _addEnergy;
        data.SetEnergy(value);

        view.ShowBuff(0, 0 , _addEnergy);
        view.SetData(data.FullHP, data.FullATK, data.Cur.HP, data.Cur.ATK, data.Cur.Energy);
    }

    #endregion


    #region Phase Battle

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
        model.SubstractHp(damage);

        if (model.Data.Cur.HP <= 0)
        {
            TriggerFaint();
            PhaseBattleController.Instance.FaintUnits.Enqueue(gameObject);
            Debug.Log($"{gameObject.name} enqueue");
            Debug.Log($"{PhaseBattleController.Instance.FaintUnits.Count} FaintUnits");
        }
    }

    #endregion

    public AbilityBase TriggerAbility(TriggerType triggerType)
    {
        if (model.Data.Cur.Energy <= 0)
            return null;

        if (triggerType == model.CurrentLevel.TriggerType)
            return Ability;

        return null;
    }

    public void Buff(bool isPernament, int addHealth, int addAttack)
    {
        if (isPernament)
            model.Add(new Attribute(addHealth, addAttack), new Attribute(0, 0));
        else
            model.Add(new Attribute(0, 0), new Attribute(addHealth, addAttack));

        view.ShowBuff(addHealth, addAttack, 0);
    }


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

    public void DeactivateInteraction()
    {
        View.HideVisuals();
        View.enabled = false;
        this.enabled = false;
    }

    public void MoveTo(Vector3 target)
    {
        //transform.DoMove();
    }


}
