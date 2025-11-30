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

    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_soUnit"></param>
    public void Initialize(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState)
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

        model.InitView(view);
        model.SetData(_unitState);
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
            _draggingModel.Data.XP, _draggingModel.Data.XP,
            _draggingModel.Data.Buff.HP, _draggingModel.Data.Buff.ATK,
            _draggingModel.Data.TempBuff.HP, _draggingModel.Data.TempBuff.ATK);

        model.UpdateLevelXP(_isPhaseShop);

        if (GameManager.Instance.RepairSystem)
        {
            bool isDurabilityGreater = model.Data.Durability > _draggingModel.Data.Durability;
            int durability = isDurabilityGreater ? _draggingModel.Data.Durability : model.Data.Durability;
            model.Repair?.SetDurability(false, durability);
        }
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
            model.Add(0, 0, addHealth, addAttack, 0, 0);
        else
            model.Add(0, 0, 0, 0, addHealth, addAttack);

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
