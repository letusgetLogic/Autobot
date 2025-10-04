using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitView view;
    [SerializeField]
    private UnitMouseEvent mouseEvent;

    private UnitModel model;

    private void Awake()
    {
        model = new UnitModel();
    }

    private void OnEnable()
    {
        mouseEvent.OnMouseOverEvent += ShowStats;
    }

    private void OnDisable()
    {
        mouseEvent.OnMouseOverEvent -= ShowStats;
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

    private void ShowStats()
    {

    }

    private void HideStats()
    {

    }

    private void BeginDrag()
        {

    }

    private void Drag()
    {
        
    }

    private void EndDrag()
    {

    }
}
