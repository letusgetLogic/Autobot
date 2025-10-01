using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitView view;

    private UnitModel model;

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        model = new UnitModel();
    }

    /// <summary>
    /// Initializes data.
    /// </summary>
    /// <param name="_data"></param>
    public void Initialize(SoUnit _data)
    {
        model.Data = _data;
        model.CurrentLevel = model.Data.Levels[0];
        model.BattleHealth = model.Data.Health;
        model.BattleAttack = model.Data.Attack;

        view.SetData(
            model.Data.Sprite,
            model.Data.Name,
            model.CurrentLevel.Description,
            model.CurrentLevel.Sell,
            model.Data.Health,
            model.Data.Attack
            );
       
    }
}
