using UnityEngine;

public class LightenUpDown : MonoBehaviour
{
    [SerializeField] private bool isAutomatic = true;
    [SerializeField] private float animSpeedAct = 1f;
    [SerializeField] private float colorMax = 1f;
    [SerializeField] private float colorMin = 0.1f;
    [SerializeField] private AnimationCurve animCurve;

    private enum Lighten
    {
        None,
        Up,
        Down
    }
    private Lighten lightenState = Lighten.None;

    private float currentValue = 0f;

    private void OnEnable()
    {
        currentValue = 0f;
        lightenState = isAutomatic ? Lighten.Up : Lighten.Down;
    }

    private void FixedUpdate()
    {
        LightenUp();
        LightenDown();
    }

    private void OnDisable()
    {
        lightenState = Lighten.None;
        SetAlpha(colorMin);
    }

    /// <summary>
    /// Switches the lighten on or off.
    /// </summary>
    /// <param name="_isOn"></param>
    public void SwitchOn(bool _isOn)
    {
        lightenState = _isOn ? Lighten.Up : Lighten.Down;
    }

    /// <summary>
    /// Lightens up.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void LightenUp()
    {
        if (lightenState == Lighten.Up)
        {
            if (currentValue == 1)
            {
                lightenState = isAutomatic ? Lighten.Down : Lighten.None;
                return;
            }

            Interpolate(1f);
        }
    }

    /// <summary>
    /// Lightens down.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void LightenDown()
    {
        if (lightenState == Lighten.Down)
        {
            if (currentValue == 0)
            {
                lightenState = isAutomatic ? Lighten.Up : Lighten.None;
                return;
            }

            Interpolate(0f);
        }
    }

    /// <summary>
    /// Interolates the value and sets the alpha value.
    /// </summary>
    /// <param name="target"></param>
    private void Interpolate(float target)
    {
        currentValue = Mathf.MoveTowards(
            currentValue, target, animSpeedAct * 0.0001f / Time.deltaTime);

        float dimValue =
            Mathf.Lerp(colorMin, colorMax, animCurve.Evaluate(currentValue));
        SetAlpha(dimValue);
    }

    /// <summary>
    /// Sets the alpha with the dim value.
    /// </summary>
    /// <param name="_dimValue"></param>
    private void SetAlpha(float _dimValue)
    {
        var renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, _dimValue);
    }
}

