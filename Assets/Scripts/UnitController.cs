using System;
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
        if (_data.HasReference == false)
            model = new UnitModel(_soUnit, _index);
        else
            model = new UnitModel(_soUnit, _data);

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
        model.Data.XP += _draggingModel.Data.XP;
        
        model.Add(
            _draggingModel.Data.XP, _draggingModel.Data.XP,
            _draggingModel.Data.BuffHp, _draggingModel.Data.BuffAtk,
            _draggingModel.Data.BuffTempHp, _draggingModel.Data.BuffTempAtk);

        model.UpdateLevelXP(_isPhaseShop);

        bool isDurabilityGreater = model.Data.Durability > _draggingModel.Data.Durability;
        int durability = isDurabilityGreater ? _draggingModel.Data.Durability : model.Data.Durability;
        model.SetDurability(false, durability);
    }
    public void Repair()
    {
        model.RiseDurability();
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

        return model.Data.Atk;
    }

    /// <summary>
    /// Takes damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        model.SubstractHp(damage);

        if (model.Data.Hp <= 0)
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
        if (model.Data.Energy <= 0)
            return null;

        if (triggerType == model.CurrentLevel.TriggerType && model.Data.Energy > 0)
            return Ability;

        return null;
    }

    public void Buff(bool isPernament, int addHealth, int addAttack)
    {
        if (isPernament)
            model.Add(0, 0, addHealth, addAttack, 0, 0);
        else
            model.Add(0, 0, 0, 0, addHealth, addAttack);

        View.SetData(Model.FullHp, Model.FullAtk, Model.Data.Hp, Model.Data.Atk, Model.Data.Energy);
        view.ShowBuff(addHealth, addAttack);
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
