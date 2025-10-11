using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitView view;

    private UnitModel model;
    public UnitModel Model => model;

    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_data"></param>
    public void Initialize(SoUnit _data)
    {
        model = new UnitModel(_data);

        model.InitializeLevel();

        UpdateData(true);
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

    /// <summary>
    /// Makes a fusion and give this additional xp.
    /// </summary>
    /// <param name="additionalXP"></param>
    public void DoFusion(int additionalXP)
    {
        model.XP += additionalXP;

        if (model.XP > StarterPack.Instance.XpToLv3)
            model.XP = StarterPack.Instance.XpToLv3;
    }

    /// <summary>
    /// Checks, if the level is maxed.
    /// </summary>
    /// <returns></returns>
    public bool IsMaxed()
    {
        if (model.CurrentLevel.Number == model.Data.Levels.Length) 
            return true;    

        return false;
    }

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

    /// <summary>
    /// Updates the level and xp.
    /// </summary>
    public void UpdateLevel()
    {
        model.XP++;
        model.BattleHealth += StarterPack.Instance.AddHealth;
        model.BattleAttack += StarterPack.Instance.AddAttack;
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
            case 0:
                view.SetStepActive("1", false, true, false, false, false, false, false, false);
                Model.CurrentLevel = Model.Data.Levels[0];
                break;
            case 1:
                view.SetStepActive("1", false, true, true, false, false, false, false, false);
                Model.CurrentLevel = Model.Data.Levels[0];
                break;
            case 2:
                view.SetStepActive("1", false, true, true, true, false, false, false, false);
                Model.CurrentLevel = Model.Data.Levels[0];
                StartCoroutine(DelayLevel2());
                break;
            case 3:
                view.SetStepActive("2", false, false, false, false, true, true, false, false);
                Model.CurrentLevel = Model.Data.Levels[1];
                break;
            case 4:
                view.SetStepActive("2", false, false, false, false, true, true, true, false);
                Model.CurrentLevel = Model.Data.Levels[1];
                break;
            case 5:
                view.SetStepActive("2", false, false, false, false, true, true, true, true);
                Model.CurrentLevel = Model.Data.Levels[1];
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
        Model.CurrentLevel = Model.Data.Levels[1];
    }

    /// <summary>
    /// Delays level 3.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayLevel3()
    {
        yield return new WaitForSeconds(view.DelayUpdateLevel);

        view.SetStepActive("3", true, false, false, false, false, false, false, false);
        Model.CurrentLevel = Model.Data.Levels[2];
    }

    /// <summary>
    /// Updates the data.
    /// </summary>
    public void UpdateData(bool isCoinCost)
    {
        view.SetData(
           model.Data.Sprite,
           model.Data.Name,
           model.CurrentLevel.Description,
           isCoinCost ? model.Data.Cost : model.CurrentLevel.Sell,
           model.BattleHealth,
           model.BattleAttack
           );
    }
}
