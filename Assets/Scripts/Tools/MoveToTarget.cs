using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private AnimationCurve _curve;

    private bool _isMoving = false;
    private bool _isMovingToTarget = false;

    private Vector2 _originPosition;
    private Vector2 _targetPosition;

    private float _currentValue;

    /// <summary>
    /// Start method.
    /// </summary>
    private void Start()
    {
        _originPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    /// <summary>
    /// External button triggers.
    /// </summary>
    public void Trigger()
    {
        _currentValue = 0;
        _isMovingToTarget = !_isMovingToTarget;

        if (_isMovingToTarget)
            _targetPosition = _target.GetComponent<RectTransform>().anchoredPosition;
        else
            _targetPosition = _originPosition;

        _isMoving = true;
    }

    /// <summary>
    /// Update method.
    /// </summary>
    private void Update()
    {
        if (_isMoving)
        {
            MoveTo(_targetPosition);
        }
    }

    /// <summary>
    /// Move to target.
    /// </summary>
    /// <param name="targetPosition"></param>
    private void MoveTo(Vector2 targetPosition)
    {
        var currentPos = GetComponent<RectTransform>().anchoredPosition;
        var distance = currentPos - targetPosition;

        _currentValue = Mathf.MoveTowards(
            _currentValue, 1, _speed * 0.0001f / Time.deltaTime);

        float value = _curve.Evaluate(_currentValue);

        GetComponent<RectTransform>().anchoredPosition =
            Vector2.Lerp(currentPos, targetPosition, value);

        if (value >= 1)
        {
            _isMoving = false;
        }
    }
}

