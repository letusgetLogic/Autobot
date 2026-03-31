using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private GameObject unitPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Spawns a unit with given parameters in the shop phase.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_index"></param>
    /// <param name="_data"></param>
    /// <param name="_unitState"></param>
    /// <param name="_parent"></param>
    /// <returns></returns>
    public UnitController Spawn(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState,
        Transform _parent)
    {
        var unit = Instantiate(unitPrefab, _parent, false);

        var controller = unit.GetComponent<UnitController>();
        controller.Initialize(_soUnit, _index, _data, _unitState, true);

        return controller;
    }

    /// <summary>
    /// Spawns a unit with given parameters in battle phase.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_index"></param>
    /// <param name="_data"></param>
    /// <param name="_unitState"></param>
    /// <param name="_parent"></param>
    /// <param name="_flipSprite"></param>
    /// <returns></returns>
    public UnitController Spawn(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState,
        Transform _parent, bool _isTeam1)
    {
        var unit = Instantiate(unitPrefab, _parent, false);

        var unitController = unit.GetComponent<UnitController>();
        unitController.Initialize(_soUnit, _index, _data, _unitState, _isTeam1);

        return unitController;
    }


}

