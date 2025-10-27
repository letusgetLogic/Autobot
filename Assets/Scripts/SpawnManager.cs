using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
    {
    public static SpawnManager Instance { get; private set; }

    [SerializeField]
    private GameObject unitPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public UnitController Spawn(SoUnit _data, int _index, UnitModel _model, UnitState _unitState,
        Transform parent)
    {
        var unit = Instantiate(unitPrefab);
        unit.transform.SetParent(parent, false);

        var controller = unit.GetComponent<UnitController>();
        controller.Initialize(_data, _index, _model, _unitState);

        return controller;
    }
  
}

