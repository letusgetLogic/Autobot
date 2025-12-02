using System.Collections;
using TMPro;
using UnityEngine;

public class MarkColorRed : MonoBehaviour
{
    /// <summary>
    /// Sets the color of the text red and back to default color with a delay.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    public void SetComponent(TextMeshProUGUI target, float duration)
    {
        var defaultColor = target.color;
        target.color = Color.red;
        StartCoroutine(SetDefault(target, duration, defaultColor));
    }

    /// <summary>
    /// Delays the setting color default.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    /// <param name="defaultColor"></param>
    /// <returns></returns>
    private IEnumerator SetDefault(TextMeshProUGUI text, float duration, Color defaultColor)
    {
        yield return new WaitForSeconds(duration);

        text.color = defaultColor;
    }

    /// <summary>
    /// Sets the color of the input field red and back to default color with a delay.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    public void SetComponent(TMP_InputField target, float duration)
    {
        var defaultColor = target.textComponent.color;
        target.textComponent.color = Color.red;
        StartCoroutine(SetDefault(target, duration, defaultColor));
    }

    /// <summary>
    /// Delays the setting color default.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    /// <param name="defaultColor"></param>
    /// <returns></returns>
    private IEnumerator SetDefault(TMP_InputField target, float duration, Color defaultColor)
    {
        yield return new WaitForSeconds(duration);

        target.textComponent.color = defaultColor;
    }
}
