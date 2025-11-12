using System.Collections;
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
    private UnitModel model { get; set; }

    public SoUnit Data { get; private set; }
    public Level CurrentLevel { get; set; }
    public AbilityBase Ability => AbilityBase.GetAbility(this, CurrentLevel);
    public bool IsMaxed => CurrentLevel.Number == Data.Levels.Length;
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
    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }
    
    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_data"></param>
    public void Initialize(SoUnit _data, int _index, UnitModel _model, UnitState _unitState)
    {
        Data = _data;

        if (_model == null)
            model = new UnitModel(_data, _index);
        else
            model = _model;

        model.SetData(view);
        model.SetData(PackManager.Instance.MyPack.HealthPortion.Value);
        model.SetData(_unitState);

        UpdateLevelXP(model.XP, IsPhaseShop(model.UnitState));
        UpdateStats();

        view.SetData(Data.Sprite, Data.Name);
        view.SetData(CurrentLevel.Description);
        view.SetData(Coin(model.UnitState));
        view.SetData(BattleHealth, BattleAttack, model.Energy);
    }

    private int Coin(UnitState unitState)
    {
        switch (unitState)
        {
            case UnitState.InSlotShop:
                return Data.Cost.Value;
            case UnitState.Freezed:
                return Data.Cost.Value;
            case UnitState.InSlotBattle:
                return CurrentLevel.Sell;
            case UnitState.InPhaseBattle:
                return 0;
        }

        return -1;
    }

    private bool IsPhaseShop(UnitState unitState)
    {
        switch (unitState)
        {
            case UnitState.InSlotShop:
                return true;
            case UnitState.Freezed:
                return true;
            case UnitState.InSlotBattle:
                return true;
            case UnitState.InPhaseBattle:
                return false;
        }

        return default;
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

    #region Update Level

    /// <summary>
    /// Updates stats while fusioning.
    /// </summary>
    public void UpdateLevel(UnitModel draggingModel, bool isPhaseShop)
    {
        model.XP += draggingModel.XP;
        model.BuffHealth += draggingModel.BuffHealth;
        model.BuffAttack += draggingModel.BuffAttack;
        model.BuffHealthTemp += draggingModel.BuffHealthTemp;
        model.BuffAttackTemp += draggingModel.BuffAttackTemp;
        model.BasisHealth += draggingModel.XP;
        model.BasisAttack += draggingModel.XP;
        UpdateLevelXP(model.XP, isPhaseShop);
        UpdateStats();
        view.SetData(BattleHealth, BattleAttack, model.Energy);

    }

    /// <summary>
    /// Updates the level and xp.
    /// </summary>
    /// <param name="xp"></param>
    private void UpdateLevelXP(int xp, bool isPhaseShop)
    {
        switch (xp)
        {//                       level  box1   box2  step1  step2  box3  step3  step4  step5  
            case 1:
                view.SetXpStepActive("1", false, true, false, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 2:
                view.SetXpStepActive("1", false, true, true, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 3:
                view.SetXpStepActive("1", false, true, true, true, false, false, false, false);
                SetCurrentLevel(0);
                StartCoroutine(DelayLevel2(isPhaseShop));
                break;
            case 4:
                view.SetXpStepActive("2", false, false, false, false, true, true, false, false);
                SetCurrentLevel(1);
                break;
            case 5:
                view.SetXpStepActive("2", false, false, false, false, true, true, true, false);
                SetCurrentLevel(1);
                break;
            case 6:
                view.SetXpStepActive("2", false, false, false, false, true, true, true, true);
                SetCurrentLevel(1);
                StartCoroutine(DelayLevel3(isPhaseShop));
                break;
        }
    }

    /// <summary>
    /// Delays level 2.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel2(bool isPhaseShop)
    {
        yield return new WaitForSeconds(isPhaseShop ?
            view.DelayUpdateLevel :
            0f);

        view.SetXpStepActive("2", false, false, false, false, true, false, false, false);
        SetCurrentLevel(1);
    }

    /// <summary>
    /// Delays level 3.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel3(bool isPhaseShop)
    {
        yield return new WaitForSeconds(isPhaseShop ?
            view.DelayUpdateLevel :
            0f);

        view.SetXpStepActive("3", true, false, false, false, false, false, false, false);
        SetCurrentLevel(2);
    }

    /// <summary>
    /// Sets the current level and index for saving data.
    /// </summary>
    /// <param name="index"></param>
    private void SetCurrentLevel(int index)
    {
        CurrentLevel = Data.Levels[index];
        view.SetData(CurrentLevel.Description);
        view.SetData(CurrentLevel.Sell);
    }

    #endregion

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
        {
            PhaseBattleController.Instance.UnitAbilities.Enqueue(ability);
        }

        return BattleAttack;
    }

    /// <summary>
    /// Takes damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (damage < 0)
            damage = 0;

        BattleHealth -= damage;
        view.ShowDamage(damage, BattleHealth);

        if (BattleHealth <= 0)
            TriggerFaint();
    }


    #endregion

    public AbilityBase TriggerAbility(TriggerType triggerType)
    {
        if (triggerType == CurrentLevel.TriggerType && Model.Energy > 0)
        {
            return Ability;
        }
        return null;
    }

    public void Buff(bool isPernament, int addHealth, int addAttack)
    {
        if (isPernament)
        {
            model.BuffHealth += addHealth;
            model.BuffAttack += addAttack;
            UpdateStats();
        }
        else
        {
            if (GameManager.Instance.IsPhaseBattle)
            {
                BattleHealth += addHealth;
                BattleAttack += addAttack;
            }
            else
            {
                model.BuffHealthTemp += addHealth;
                model.BuffAttackTemp += addAttack;
                UpdateStats();
            }
        }

        view.SetData(BattleHealth, BattleAttack, model.Energy);
        view.ShowBuff(addHealth, addAttack);
    }

    /// <summary>
    /// Updates stats.
    /// </summary>
    public void UpdateStats()
    {
        BattleHealth = model.BasisHealth + model.BuffHealth + model.BuffHealthTemp;
        BattleAttack = model.BasisAttack + model.BuffAttack + model.BuffAttackTemp;
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

        PhaseBattleController.Instance.FaintUnits.Enqueue(gameObject);
        Debug.Log($"{gameObject.name} enqueue");
        Debug.Log($"{PhaseBattleController.Instance.FaintUnits.Count} FaintUnits");
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
