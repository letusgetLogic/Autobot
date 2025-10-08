using UnityEngine;

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

        view.SetData(
            model.Data.Sprite,
            model.Data.Name,
            model.CurrentLevel.Description,
            model.Data.Cost,
            model.Data.Health,
            model.Data.Attack
            );
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
}
