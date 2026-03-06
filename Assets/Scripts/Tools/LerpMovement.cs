using System;
using UnityEngine;

[ExecuteAlways]
public class LerpMovement : MonoBehaviour
{
    [Header("External Settings")]
    [SerializeField] private SoLerpMovementSettings soSettings;
    public SoLerpMovementSettings SoSettings => soSettings;
    
    [Header("Editor Settings")]
    [SerializeField] private Transform definedTargetTransform;

    public Action<Transform> OnPosition {  get; set; }

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

    private void Update()
    {
        MoveForward();
        MoveBackward();
    }

    /// <summary>
    /// Sets the default values for the movement.
    /// </summary>
    private void SetDefault()
    {
        currentValue = 0f;
        defaultPosition = transform.position;

        if (definedTargetTransform)
            targetPosition = definedTargetTransform.position;
        else
        {
            if (soSettings)
                targetPosition = defaultPosition + soSettings.DeltaPosition;
        }
    }

    /// <summary>
    /// OnMove is called from the context menu to start the movement.
    /// </summary>
    [ContextMenu("Move")]
    public void OnMove()
    {
        SetDefault();
        moveState = Direction.Forward;
    }

    /// <summary>
    /// Triggers the movement and returns the animation time.
    /// </summary>
    /// <returns></returns>
    public float Trigger()
    {
        SetDefault();
        moveState = Direction.Forward;

        if (soSettings)
            return soSettings.AnimTime;

        return default;
    }

    /// <summary>
    /// Moves to the target position and returns the animation time.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_targetTf"></param>
    /// <returns></returns>
    public float MoveTo(Vector3 _target, Transform _targetTf)
    {
        SetDefault();

        targetPosition = _target;
        targetTransform = _targetTf;

        moveState = Direction.Forward;

        if (soSettings)
            return soSettings.AnimTime;

        return default;
    }

    /// <summary>
    /// Moves with the delta position and returns the animation time.
    /// </summary>
    /// <param name="_deltaPosition"></param>
    /// <returns></returns>
    public float MoveWithDelta(Vector3 _deltaPosition)
    {
        SetDefault();

        targetPosition = defaultPosition + _deltaPosition;

        moveState = Direction.Forward;

        if (soSettings)
            return soSettings.AnimTime;

        return default;
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
                if (soSettings.RunBackward)
                {
                    moveState = Direction.Backward;
                }
                else
                {
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
            currentValue, _target, Time.deltaTime / soSettings.AnimTime);

        Vector3 position = Vector3.Lerp(defaultPosition, targetPosition, soSettings.AnimCurve.Evaluate(currentValue));
        SetPosition(position);
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    /// <param name="rValue"></param>
    private void SetPosition(Vector3 _pos)
    {
        transform.position = _pos;
        //Debug.Log($"{gameObject.name} transform.position {transform.position}");
    }
}

