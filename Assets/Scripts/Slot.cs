using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer border;
    public SpriteRenderer Border
    {
        get { return border; }
        set { border = value; }
    }

    [SerializeField]
    private EventHover eventHover;
    public EventHover EventHover => eventHover;

    //public GameObject GameObjectIsOnMe { get; set; }
    public int Index { get; set; }  


    private void Start()
    {
        border.enabled = false;
    }

    private void OnEnable()
    {
        eventHover.OnMouseOverEvent += ShowStats;
        eventHover.OnMouseExitEvent += HideStats;

    }

    private void OnDisable()
    {
        eventHover.OnMouseOverEvent -= ShowStats;
        eventHover.OnMouseExitEvent -= HideStats;
    }

    /// <summary>
    /// Returns the component UnitView.
    /// </summary>
    /// <returns></returns>
    public UnitView UnitView()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Unit"))
                return child.GetComponent<UnitView>();
        }
        return null;
    }

    /// <summary>
    /// Returns the component UnitController.
    /// </summary>
    /// <returns></returns>
    public UnitController UnitVController()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Unit"))
                return child.GetComponent<UnitController>();
        }
        return null;
    }

    #region Hover Event - Show description

    /// <summary>
    /// Shows the description.
    /// </summary>
    private void ShowStats()
    {
        //if (GameObjectIsOnMe == null || !GameObjectIsOnMe.CompareTag("Unit"))
        //    return;

        //if (eventHover.Data != null && eventHover.Data.pointerDrag != null) 
        //    return;

        //border.enabled = true;

        //var view = GameObjectIsOnMe.GetComponent<UnitView>();
        //view.SetDescriptionActive(true);
    }

    /// <summary>
    /// Hides the description.
    /// </summary>
    private void HideStats()
    {
        //if (border.enabled)
        //    border.enabled = false;

        //if (GameObjectIsOnMe == null || !GameObjectIsOnMe.CompareTag("Unit"))
        //    return;

        //var view = GameObjectIsOnMe.GetComponent<UnitView>();
        //view.SetDescriptionActive(false);
    }

    #endregion

}
