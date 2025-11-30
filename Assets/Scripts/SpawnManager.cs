using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
    {
    public static SpawnManager Instance { get; private set; }

    [SerializeField]  private GameObject unitPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Spawns a unit with given parameters in the shop slots.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_index"></param>
    /// <param name="_data"></param>
    /// <param name="_unitState"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public UnitController Spawn(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState,
        Transform parent)
    {
        var unit = Instantiate(unitPrefab, parent, false);

        var controller = unit.GetComponent<UnitController>();
        controller.Initialize(_soUnit, _index, _data, _unitState);

        return controller;
    }

    /// <summary>
    /// Spawns a unit with given parameters.
    /// </summary>
    /// <param name="_soUnit"></param>
    /// <param name="_index"></param>
    /// <param name="_data"></param>
    /// <param name="_unitState"></param>
    /// <param name="_parent"></param>
    /// <param name="_flipSprite"></param>
    /// <returns></returns>
    public UnitController Spawn(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState,
        Transform _parent, bool _flipSprite)
    {
        var unit = Instantiate(unitPrefab, _parent, false);

        var unitController = unit.GetComponent<UnitController>();
        unitController.Initialize(_soUnit, _index, _data, _unitState);
        Debug.Log("Hallooooooooooooooooooooooooo");
        unitController.View.Shadow.enabled = false;

        if (_flipSprite)
        {
            unitController.View.SetRightSide();
            unitController.Model.Data.IsTeam1 = false;
        }
        else
        {
            unitController.Model.Data.IsTeam1 = true;
        }

        return unitController;
    }
  
}

