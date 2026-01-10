using UnityEngine;

public class ScaleUpDown : MonoBehaviour
{
    [SerializeField] private bool isAutomatic = true;
    [SerializeField] private float animSpeedAct = 1f;
    [SerializeField] private Vector3 scaleMax = new(1f, 1f);
    [SerializeField] private Vector3 scaleMin = new(0.8f, 0.8f);
    [SerializeField] private AnimationCurve animCurve;

    private enum Scale
    {
        None,
        Up,
        Down
    }
    private Scale scaleState = Scale.None;

    private float currentValue = 0f;

    private void OnEnable()
    {
        currentValue = 0f;
        scaleState = isAutomatic ? Scale.Up : Scale.None;
    }

    private void FixedUpdate()
    {
        ScaleUp();
        ScaleDown();
    }

    private void OnDisable()
    {
        scaleState = Scale.None;
        SetScale(scaleMin);
    }

    /// <summary>
    /// Switches the lighten on or off.
    /// </summary>
    /// <param name="_isOn"></param>
    public void SwitchOn(bool _isOn)
    {
        scaleState = _isOn ? Scale.Up : Scale.Down;
    }

    /// <summary>
    /// Scale up.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ScaleUp()
    {
        if (scaleState == Scale.Up)
        {
            if (currentValue == 1)
            {
                scaleState = isAutomatic ? Scale.Down : Scale.None;
                return;
            }

            Interpolate(1f);
        }
    }

    /// <summary>
    /// Lightens the border down.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ScaleDown()
    {
        if (scaleState == Scale.Down)
        {
            if (currentValue == 0)
            {
                scaleState = isAutomatic ? Scale.Up : Scale.None;
                return;
            }

            Interpolate(0f);
        }
    }

    /// <summary>
    /// Interolates the value and sets the scale.
    /// </summary>
    /// <param name="target"></param>
    private void Interpolate(float target)
    {
        currentValue = Mathf.MoveTowards(
            currentValue, target, animSpeedAct * 0.0001f / Time.fixedDeltaTime);

        Vector3 scaleValue = Vector3.Lerp(scaleMin, scaleMax, animCurve.Evaluate(currentValue));
        SetScale(scaleValue);
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    /// <param name="rValue"></param>
    private void SetScale(Vector3 _scaleValue)
    {
        gameObject.transform.localScale = _scaleValue;
    }
}

