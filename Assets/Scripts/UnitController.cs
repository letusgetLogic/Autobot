using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

[System.Serializable]
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
    public bool IsFaint => BattleHealth <= 0;
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
    /// <param name="_data"></param>
    public void Initialize(SoUnit _data, int index, UnitModel _model, UnitState unitState)
    {
        Data = _data;

        if (_model == null)
            model = new UnitModel(_data, index, unitState);
        else
            model = _model;

        UpdateLevelXP(model.XP, IsPhaseShop(unitState));
        UpdateHealthAttack();

        view.SetData(Data.Sprite, Data.Name);
        view.SetData(CurrentLevel.Description);
        view.SetData(Coin(unitState));
        view.SetData(BattleHealth, BattleAttack);

        if (unitState == UnitState.Freezed)
            GetFrezzed();
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

    #region Manage Buttons

    /// <summary>
    /// Sets the manage state freezed and set the sprite active.
    /// </summary>
    public void GetFrezzed()
    {
        model.UnitState = UnitState.Freezed;
        view.IceCube.SetActive(true);
    }

    /// <summary>
    /// Sets the manage state unfreezed and set the sprite active.
    /// </summary>
    public void GetUnfrezzed()
    {
        model.UnitState = UnitState.InSlotShop;
        view.IceCube.SetActive(false);
    }

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

    #endregion


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
        UpdateHealthAttack();
        view.SetData(BattleHealth, BattleAttack);

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
                view.SetStepActive("1", false, true, false, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 2:
                view.SetStepActive("1", false, true, true, false, false, false, false, false);
                SetCurrentLevel(0);
                break;
            case 3:
                view.SetStepActive("1", false, true, true, true, false, false, false, false);
                SetCurrentLevel(0);
                StartCoroutine(DelayLevel2(isPhaseShop));
                break;
            case 4:
                view.SetStepActive("2", false, false, false, false, true, true, false, false);
                SetCurrentLevel(1);
                break;
            case 5:
                view.SetStepActive("2", false, false, false, false, true, true, true, false);
                SetCurrentLevel(1);
                break;
            case 6:
                view.SetStepActive("2", false, false, false, false, true, true, true, true);
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

        view.SetStepActive("2", false, false, false, false, true, false, false, false);
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

        view.SetStepActive("3", true, false, false, false, false, false, false, false);
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

    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }

    public int TriggerAttack()
    {
        var ability = TriggerAbility(TriggerType.BeforeAttack);
        if (ability != null)
        {
            ability.Activate();
        }

        Attack?.Invoke();

        ability = TriggerAbility(TriggerType.AfterAttack);
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
        if (triggerType == CurrentLevel.TriggerType)
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
            UpdateHealthAttack();
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
                UpdateHealthAttack();
            }
        }

        view.SetData(BattleHealth, BattleAttack);
        view.ShowBuff(addHealth, addAttack);
    }

    /// <summary>
    /// Updates battle health/attack.
    /// </summary>
    public void UpdateHealthAttack()
    {
        BattleHealth = model.BasisHealth + model.BuffHealth + model.BuffHealthTemp;
        BattleAttack = model.BasisAttack + model.BuffAttack + model.BuffAttackTemp;
    }

    public void TriggerFaint()
    {
        var ability = TriggerAbility(TriggerType.Faint);
        if (ability != null)
        {
            PhaseBattleController.Instance.UnitAbilities.Enqueue(ability);
            Faint?.Invoke();
        }

        PhaseBattleController.Instance.FaintUnits.Enqueue(gameObject);
    }

    public void MoveTo(Vector3 target)
    {
        //transform.DoMove();
    }

}
