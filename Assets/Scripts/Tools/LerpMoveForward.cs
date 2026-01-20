using System;
using UnityEngine;

public class LerpMoveForward : MonoBehaviour
{
    [SerializeField] private SoUnitMovement settings;

    public Action<Transform> OnPosition;

    private enum Direction
    {
        None,
        Forward,
        Backward
    }
    private Direction moveState = Direction.None;

    private float currentValue = 0f;
    private Vector3 defaultPosition;
    private Vector3 targetPosition;
    private Transform targetTransform;

    private void OnEnable()
    {
        SetDefault();
    }

    private void FixedUpdate()
    {
        MoveForward();
        MoveBackward();
    }

    private void SetDefault()
    {
        currentValue = 0f;
        defaultPosition = transform.position;
        targetPosition = defaultPosition + settings.DeltaPosition;
    }

    public void Move()
    {
        SetDefault();
        moveState = Direction.Forward;
    }

    public float MoveTo(Vector3 _target, Transform _parent)
    {
        SetDefault();

        targetPosition = _target;
        moveState = Direction.Forward;

        targetTransform = _parent;

        return settings.AnimTime;
    }

    /// <summary>
    /// Scales up.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void MoveForward()
    {
        if (moveState == Direction.Forward)
        {
            if (currentValue == 1f)
            {
                if (settings.RunBackward)
                {
                    moveState = Direction.Backward;
                }
                else
                {
                    Debug.Log($"{gameObject.name} transform.position {transform.position}");
                    Debug.Log($"{targetTransform.gameObject.name} transform.position {targetTransform.position}");

                    moveState = Direction.None;
                    OnPosition?.Invoke(targetTransform);
                }
                return;
            }

            Interpolate(1f);
        }
    }

    /// <summary>
    /// Scales down.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void MoveBackward()
    {
        if (moveState == Direction.Backward)
        {
            if (currentValue == 0f)
            {
                moveState = Direction.None;
                OnPosition?.Invoke(targetTransform);
                return;
            }

            Interpolate(0f);
        }
    }

    /// <summary>
    /// Interpolates the value and sets the scale.
    /// </summary>
    /// <param name="_target"></param>
    private void Interpolate(float _target)
    {
        currentValue = Mathf.MoveTowards(
            currentValue, _target, Time.fixedDeltaTime / settings.AnimTime);

        Vector3 position = Vector3.Lerp(defaultPosition, targetPosition, settings.AnimCurve.Evaluate(currentValue));
        SetPosition(position);
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    /// <param name="rValue"></param>
    private void SetPosition(Vector3 _pos)
    {
        transform.position = _pos;
        Debug.Log($"{gameObject.name} transform.position {transform.position}");
    }
}

