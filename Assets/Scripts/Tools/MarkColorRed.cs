using System.Collections;
using TMPro;
using UnityEngine;

public class MarkColorRed : MonoBehaviour
{
    /// <summary>
    /// Sets the color of the text red and back to default color with a delay.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_duration"></param>
    public void SetComponent(TextMeshProUGUI _target, float _duration)
    {
        var defaultColor = _target.color;
        _target.color = Color.red;
        StartCoroutine(SetDefault(_target, _duration, defaultColor));
    }

    /// <summary>
    /// Delays the setting color default.
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_duration"></param>
    /// <param name="_defaultColor"></param>
    /// <returns></returns>
    private IEnumerator SetDefault(TextMeshProUGUI _text, float _duration, Color _defaultColor)
    {
        yield return new WaitForSeconds(_duration);

        _text.color = _defaultColor;
    }

    /// <summary>
    /// Sets the color of the input field red and back to default color with a delay.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_duration"></param>
    public void SetComponent(TMP_InputField _target, float _duration)
    {
        var defaultColor = _target.textComponent.color;
        _target.textComponent.color = Color.red;
        StartCoroutine(SetDefault(_target, _duration, defaultColor));
    }

    /// <summary>
    /// Delays the setting color default.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_duration"></param>
    /// <param name="_defaultColor"></param>
    /// <returns></returns>
    private IEnumerator SetDefault(TMP_InputField _target, float _duration, Color _defaultColor)
    {
        yield return new WaitForSeconds(_duration);

        _target.textComponent.color = _defaultColor;
    }
}
