using NUnit.Framework.Interfaces;
using UnityEngine;

public class ScaleUpDown : MonoBehaviour
{
    private enum RunState
    {
        None,
        Automatic,
        Up,
        Down,
        Validate,
        ValidateFromScaleMin
    }
    [SerializeField] private RunState runState = RunState.None;

    [SerializeField] private float animTime = 1f;
    [SerializeField] private Vector3 scaleMax = new(1f, 1f);
    [SerializeField] private Vector3 scaleMin = new(0.8f, 0.8f);
    [SerializeField] private AnimationCurve animCurve;

    public float AnimTime { get => animTime; set => animTime = value; }

    private enum Scale
    {
        None,
        Up,
        Down
    }
    private Scale scaleState = Scale.None;

    private float currentValue = 0f;
    private Vector3 defaultValue;

    private void OnEnable()
    {
        GetDefault();
        currentValue = 0f;
        switch(runState)
        {
            case RunState.None:
                scaleState = Scale.None;
                break;

            case RunState.Automatic:
                scaleState = Scale.Up;
                break;

            case RunState.Up:
                scaleState = Scale.Up;
                break;

            case RunState.Down:
                currentValue = 1f;
                scaleState = Scale.Down;
                break;

            case RunState.Validate:
                scaleState = Scale.None;
                break;

            case RunState.ValidateFromScaleMin:
                scaleState = Scale.None;
                break;
        }
    }

    private void FixedUpdate()
    {
        ScaleUp();
        ScaleDown();
    }

    private void OnDisable()
    {
        scaleState = Scale.None;
        SetDefault();
    }

    /// <summary>
    /// Scales up (true) or down (false).
    /// </summary>
    /// <param name="_isUp"></param>
    public void ScaleUp(bool _isUp)
    {
        currentValue = _isUp ? 0f : 1f;
        scaleState = _isUp ? Scale.Up : Scale.Down;
    }

    /// <summary>
    /// Scales up.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ScaleUp()
    {
        if (scaleState == Scale.Up)
        {
            if (currentValue == 1)
            {
                scaleState = runState == RunState.Automatic ? Scale.Down : Scale.None;
                return;
            }

            Interpolate(1f);
        }
    }

    /// <summary>
    /// Scales down.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ScaleDown()
    {
        if (scaleState == Scale.Down)
        {
            if (currentValue == 0)
            {
                scaleState = runState == RunState.Automatic ? Scale.Up : Scale.None;
                return;
            }

            Interpolate(0f);
        }
    }

    /// <summary>
    /// Interpolates the value and sets the scale.
    /// </summary>
    /// <param name="target"></param>
    private void Interpolate(float target)
    {
        currentValue = Mathf.MoveTowards(
            currentValue, target, Time.fixedDeltaTime / animTime);

        //currentValue = Mathf.MoveTowards(
        //    currentValue, target, animSpeedAct * 0.0001f / Time.fixedDeltaTime);

        Vector3 scaleValue = Vector3.Lerp(scaleMin, scaleMax, animCurve.Evaluate(currentValue));
        SetScale(scaleValue);
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    /// <param name="rValue"></param>
    private void SetScale(Vector3 _scaleValue)
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = _scaleValue;
            return;
        }
        transform.localScale = _scaleValue;
    }

    private void GetDefault()
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            defaultValue = rect.localScale;
            if (runState == RunState.ValidateFromScaleMin)
            {
                defaultValue = scaleMin;
                rect.localScale = defaultValue;
            }
            return;
        }

        defaultValue = transform.localScale;
        if (runState == RunState.ValidateFromScaleMin)
        {
            defaultValue = scaleMin;
            transform.localScale = defaultValue;
        }
    }

    private void SetDefault()
    {
        var rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.localScale = defaultValue;
            return;
        }
        transform.localScale = defaultValue;
    }
}

