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
        view.Sprite = model.Data.Sprite;
        view.Name.text = model.Data.Name;
        view.Description.text = model.Data.Description;
        view.Cost.text = model.Data.Cost.ToString();
        view.Health.text = model.Data.Health.ToString();
        view.Damage.text = model.Data.Damage.ToString();
    }
}
