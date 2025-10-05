using Unity.VisualScripting;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitView view;

    [SerializeField]
    private HoverEvent hoverEvent;

    [SerializeField]
    private DragNDrop dragEvent;

    private UnitModel model;

    private void Awake()
    {
        model = new UnitModel();
    }

    private void OnEnable()
    {
        hoverEvent.OnMouseOverEvent += ShowStats;
        hoverEvent.OnMouseExitEvent += HideStats;
        dragEvent.OnMouseUp += MoveBackToParent;

    }

    private void OnDisable()
    {
        hoverEvent.OnMouseOverEvent -= ShowStats;
        hoverEvent.OnMouseExitEvent -= HideStats;
        dragEvent.OnMouseDown -= MoveBackToParent;
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

    #region Mouse Event

    /// <summary>
    /// Shows the description.
    /// </summary>
    private void ShowStats()
    {
        view.SetDescriptionActive(true);
    }

    /// <summary>
    /// Hides the description.
    /// </summary>
    private void HideStats()
    {
        view.SetDescriptionActive(false);
    }

    /// <summary>
    /// Moves sprite back to his parent.
    /// </summary>
    private void MoveBackToParent()
    {
        dragEvent.transform.localPosition = Vector3.zero;
    }

    #endregion
}
