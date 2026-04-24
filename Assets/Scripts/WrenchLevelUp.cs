using UnityEngine;
using UnityEngine.Events;

public class WrenchLevelUp : MonoBehaviour
{
    [SerializeField] private LerpMovement spawnAnim;
    [SerializeField] private LerpMovement addingAnim;
    [SerializeField] private ScaleUpDown spawnScale;

    public UnityAction OnPosition;

    private Vector3 spawnEnd;
    private Vector3 addingEnd;

    public void SetPosition(Vector3 _spawnEnd, Vector3 _addingEnd)
    {
        spawnEnd = _spawnEnd;
        addingEnd = _addingEnd;
    }

    private void OnEnable()
    {
        spawnAnim.OnPosition += MoveNext;
        addingAnim.OnPosition += Deactivate;
        spawnAnim.MoveTo(spawnEnd, null);   
    }

    private void OnDisable()
    {
        spawnAnim.OnPosition -= MoveNext;
        addingAnim.OnPosition -= Deactivate;
    }

    private void MoveNext(Transform _tf)
    {
        addingAnim.MoveTo(addingEnd, null);
    }

    private void Deactivate(Transform _tf)
    {
        OnPosition?.Invoke();
        gameObject.SetActive(false);
    }
}