using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public class UnitController : MonoBehaviour
{
    public UnitView View => view;
    [SerializeField]
    private UnitView view;

    public UnitModel Model => model;
    private UnitModel model { get; set; }

    public SoUnit Data { get; private set; }
    public int BattleHealth { get; set; }
    public int BattleAttack { get; set; }
    public Level CurrentLevel { get; set; }
    public AbilityBase Ability => AbilityBase.GetAbility(this, CurrentLevel);
    public bool IsMaxed => CurrentLevel.Number == Data.Levels.Length;

    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_data"></param>
    public void Initialize(SoUnit _data, int index, UnitModel _model)
    {
        Data = _data;

        if (model == null)
            model = new UnitModel(_data, index);
        else
            model = _model;

        InitializeLevel();
        UpdateData(true);
    }

    /// <summary>
    /// Initializes the level number and the current level of unit.
    /// </summary>
    public void InitializeLevel()
    {
        for (int i = 0; i < Data.Levels.Length; i++)
        {
            Data.Levels[i].Number = i + 1;
        }
        CurrentLevel = Data.Levels[model.CurrentLevelIndex];
    }

    #region Mouse Event

    /// <summary>
    /// Shows the description.
    /// </summary>
    public void ShowStats()
    {
        view.SetDescriptionActive(true);
    }

    /// <summary>
    /// Hides the description.
    /// </summary>
    public void HideStats()
    {
        view.SetDescriptionActive(false);
    }

    #endregion


    #region PhaseShop

    #region Manage buttons

    /// <summary>
    /// Sets the manage state freezed and set the sprite active.
    /// </summary>
    public void GetFrezzed()
    {
        model.ManageState = UnitState.Freezed;
        view.IceCube.SetActive(true);
    }

    /// <summary>
    /// Sets the manage state unfreezed and set the sprite active.
    /// </summary>
    public void GetUnfrezzed()
    {
        model.ManageState = UnitState.InSlotShop;
        view.IceCube.SetActive(false);
    }

    /// <summary>
    /// Destroys the game object.
    /// </summary>
    public void GetSelled()
    {
        Destroy(gameObject);
    }

    #endregion


    #region Update Level

    /// <summary>
    /// Updates stats.
    /// </summary>
    public void UpdateLevel()
    {
        UpdateLevelXP(model.XP);
        UpdateData(false);
    }

    /// <summary>
    /// Updates stats while fusioning.
    /// </summary>
    public void UpdateLevel(UnitModel draggingModel)
    {
        model.XP += draggingModel.XP;
        model.BuffHealth += draggingModel.BuffHealth;
        model.BuffAttack += draggingModel.BuffAttack;
        model.BuffHealthTemp += draggingModel.BuffHealthTemp;
        model.BuffAttackTemp += draggingModel.BuffAttackTemp;
        model.BasisHealth += StarterPack.Instance.AddHealthWhileFusion + draggingModel.BuffHealth;
        model.BasisAttack += StarterPack.Instance.AddAttackWhileFusion + draggingModel.BuffAttack;
        UpdateLevelXP(model.XP);
        UpdateData(false);
    }

    /// <summary>
    /// Updates the level and xp.
    /// </summary>
    /// <param name="xp"></param>
    private void UpdateLevelXP(int xp)
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
                StartCoroutine(DelayLevel2());
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
                StartCoroutine(DelayLevel3());
                break;
        }
    }

    /// <summary>
    /// Delays level 2.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel2()
    {
        yield return new WaitForSeconds(view.DelayUpdateLevel);

        view.SetStepActive("2", false, false, false, false, true, false, false, false);
        SetCurrentLevel(1);
        UpdateData(false);
    }

    /// <summary>
    /// Delays level 3.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel3()
    {
        yield return new WaitForSeconds(view.DelayUpdateLevel);

        view.SetStepActive("3", true, false, false, false, false, false, false, false);
        SetCurrentLevel(2);
        UpdateData(false);
    }

    /// <summary>
    /// Sets the current level and index for saving data.
    /// </summary>
    /// <param name="index"></param>
    private void SetCurrentLevel(int index)
    {
        CurrentLevel = Data.Levels[index];
        Model.CurrentLevelIndex = index;
    }

    #endregion

    #endregion


    #region Phase Battle

    /// <summary>
    /// Takes damage.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (damage < 0)
            damage = 0;

        model.BasisHealth -= damage;
        view.ShowDamage(damage);
        UpdateData(false);
    }

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
        }
        else
        {
            model.BuffHealthTemp += addHealth;
            model.BuffAttackTemp += addAttack;
        }

        UpdateHealthAttack();
        view.ShowBuff(addHealth, addAttack);
        UpdateData(false);
    }

    #endregion

    /// <summary>
    /// Updates the data.
    /// </summary>
    public void UpdateData(bool isCoinCost)
    {
        view.SetData(
           Data.Sprite,
           Data.Name,
           CurrentLevel.Description,
           isCoinCost ? Data.Cost.Value : CurrentLevel.Sell,
           model.BasisHealth,
           model.BasisAttack
           );
    }

    public void UpdateHealthAttack()
    {
        model.BasisHealth += model.BuffHealth + model.BuffHealthTemp;
        model.BasisAttack += model.BuffAttack + model.BuffAttackTemp;
    }

    public void MoveTo(Vector3 target)
    {
        //transform.DoMove();
    }

}
