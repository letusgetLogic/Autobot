using UnityEngine;

public class HideObject : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    private void OnEnable()
    {
        if (_gameObject != null)
            _gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_gameObject != null)
            _gameObject.SetActive(true);
    }
}
