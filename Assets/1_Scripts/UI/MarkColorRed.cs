using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarkColorRed : MonoBehaviour
{
   ///// <summary>
   // /// Sets the color of the text red and back to default color with a delay.
   // /// </summary>
   // /// <param name="_target"></param>
   // /// <param name="_duration"></param>
   // public void SetComponent(TextMeshProUGUI _target, float _duration)
   // {
   //     var defaultColor = _target.color;
   //     _target.color = Color.red;
   //     StartCoroutine(SetDefault(_target, _duration, defaultColor));
   // }

   // /// <summary>
   // /// Delays the setting color default.
   // /// </summary>
   // /// <param name="_target"></param>
   // /// <param name="_duration"></param>
   // /// <param name="_defaultColor"></param>
   // /// <returns></returns>
   // private IEnumerator SetDefault(TextMeshProUGUI _target, float _duration, Color _defaultColor)
   // {
   //     yield return new WaitForSeconds(_duration);

   //     _target.color = _defaultColor;

   // }

    ///// <summary>
    ///// Sets the color of the text red and back to default color with a delay.
    ///// </summary>
    ///// <param name="_target"></param>
    ///// <param name="_duration"></param>
    //public void SetComponent(Image _target, Color _defaultColor, float _duration, Action _action)
    //{
    //    _target.color = Color.red;
    //    StartCoroutine(SetDefault(_target, _defaultColor, _duration, _action));
    //}

    ///// <summary>
    ///// Delays the setting color default.
    ///// </summary>
    ///// <param name="_target"></param>
    ///// <param name="_duration"></param>
    ///// <param name="_defaultColor"></param>
    ///// <returns></returns>
    //private IEnumerator SetDefault(Image _target, Color _defaultColor, float _duration, Action _action)
    //{
    //    yield return new WaitForSeconds(_duration);

    //    _target.color = _defaultColor;
    //    _action?.Invoke();
    //}

    private List<Coroutine> coroutines = new();

    /// <summary>
    /// Sets the color of the text red and back to default color with a delay.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_duration"></param>
    public void SetComponent(Component _comp, Color _defaultColor, float _duration, Action _action)
    {
        _action ??= () => { };

        switch (_comp)
        {
            case TextMeshProUGUI text:
                text.color = Color.red;
                _action += () => text.color = _defaultColor;
                break;

            case TMP_InputField inputField:
                inputField.textComponent.color = Color.red;
                _action += () => inputField.textComponent.color = _defaultColor;
                break;

            case Image image:
                image.color = Color.red;
                _action += () => image.color = _defaultColor;
                break;
        }
        int index = coroutines.Count;
        coroutines.Add(StartCoroutine(SetDefault(_duration, _action, index)));
    }

    /// <summary>
    /// Delays the setting color default.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_duration"></param>
    /// <param name="_defaultColor"></param>
    /// <returns></returns>
    private IEnumerator SetDefault(float _duration, Action _action, int _index)
    {
        yield return new WaitForSeconds(_duration);
        _action?.Invoke();

        if (_index >= 0 && _index < coroutines.Count)
            coroutines[_index] = null;
        
        if (coroutines.TrueForAll(item => item == null))
            coroutines.Clear();
    }

    ///// <summary>
    ///// Sets the color of the input field red and back to default color with a delay.
    ///// </summary>
    ///// <param name="_target"></param>
    ///// <param name="_duration"></param>
    //public void SetComponent(TMP_InputField _target, float _duration)
    //{
    //    var defaultColor = _target.textComponent.color;
    //    _target.textComponent.color = Color.red;
    //    StartCoroutine(SetDefault(_target, _duration, defaultColor));
    //}

    ///// <summary>
    ///// Delays the setting color default.
    ///// </summary>
    ///// <param name="_target"></param>
    ///// <param name="_duration"></param>
    ///// <param name="_defaultColor"></param>
    ///// <returns></returns>
    //private IEnumerator SetDefault(TMP_InputField _target, float _duration, Color _defaultColor)
    //{
    //    yield return new WaitForSeconds(_duration);

    //    _target.textComponent.color = _defaultColor;
    //}
}