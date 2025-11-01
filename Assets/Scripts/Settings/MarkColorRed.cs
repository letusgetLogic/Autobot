using System.Collections;
using TMPro;
using UnityEngine;

public class MarkColorRed : MonoBehaviour
{
    public void SetComponent(TextMeshProUGUI target, float duration)
    {
        var defaultColor = target.color;
        target.color = Color.red;
        StartCoroutine(SetDefault(target, duration, defaultColor));
    }

    private IEnumerator SetDefault(TextMeshProUGUI text, float duration, Color defaultColor)
    {
        yield return new WaitForSeconds(duration);

        text.color = defaultColor;
    }

    public void SetComponent(TMP_InputField target, float duration)
    {
        var defaultColor = target.textComponent.color;
        target.textComponent.color = Color.red;
        StartCoroutine(SetDefault(target, duration, defaultColor));
    }

    private IEnumerator SetDefault(TMP_InputField target, float duration, Color defaultColor)
    {
        yield return new WaitForSeconds(duration);

        target.textComponent.color = defaultColor;
    }
}
