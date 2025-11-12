using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
    {
    public static SpawnManager Instance { get; private set; }

    [SerializeField]
    private GameObject unitPrefab;

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

    public UnitController Spawn(SoUnit _soUnit, int _index, SaveUnitData _data, UnitState _unitState,
        Transform parent)
    {
        var unit = Instantiate(unitPrefab);
        unit.transform.SetParent(parent, false);

        var controller = unit.GetComponent<UnitController>();
        controller.Initialize(_soUnit, _index, _data, _unitState);

        return controller;
    }
  
}

