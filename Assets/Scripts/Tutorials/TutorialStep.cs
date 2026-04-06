using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialStep : MonoBehaviour
{
    public GameObject[] CoverPanels;
    public GameObject[] CoverPanelsToDeactivate;
    public GameObject[] Labels;
    public Transform[] TargetsToBeChild;
    public GameObject[] Hints;
    public GameObject[] HintsAFK;

    public UnityAction OnLabelPopup;

    private Transform[] targetParents;
    private List<Coroutine> coroutines = new();
    private float animTime;

    private void OnDisable()
    {
        if (coroutines != null)
        {
            coroutines = null;
        }
    }

    public void OnEnter()
    {
        SetActive(CoverPanels, true);
        SetActive(CoverPanelsToDeactivate, true);
        SetParent(true);

        bool hasAnim = false;
        hasAnim = SetScaleUp(CoverPanels, true);
        hasAnim = SetScaleUp(CoverPanelsToDeactivate, true);

        if (hasAnim)
        {
            coroutines.Add(StartCoroutine(DelayActivate(Labels, animTime)));
            coroutines.Add(StartCoroutine(DelayActivate(Hints, animTime)));
        }
        else
        {
            SetActive(Labels, true);
            SetActive(Hints, true);
        }
    }

    private IEnumerator DelayActivate(GameObject[] _objects, float _delay)
    {
        yield return new WaitForSeconds(_delay);

        SetActive(_objects, true);
    }

    public void OnAnimateAFK()
    {
        SetActive(HintsAFK, true);
    }

    public void OnExit()
    {
        SetActive(HintsAFK, false);
        SetActive(Labels, false);
        SetActive(Hints, false);
        SetScaleUp(CoverPanels, false);
        SetParent(false);

        if (SetScaleUp(CoverPanelsToDeactivate, false))
            coroutines.Add(StartCoroutine(DelayDeactivate(animTime)));
        else
            SetActive(CoverPanelsToDeactivate, false);
    }

    private IEnumerator DelayDeactivate(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        SetActive(CoverPanelsToDeactivate, false);
    }

    private void SetActive(GameObject[] _objectArray, bool _active)
    {
        if (_objectArray != null)
        {
            for (int i = 0; i < _objectArray.Length; i++)
            {
                var obj = _objectArray[i];
                if (obj != null && obj.activeSelf != _active)
                {
                    obj.SetActive(_active);
                    if (_active && _objectArray == Labels)
                    {
                        OnLabelPopup?.Invoke();
                    }
                }
            }
        }
    }

    private bool SetScaleUp(GameObject[] _objectArray, bool _up)
    {
        bool isRunning = false;

        if (_objectArray != null)
        {
            for (int i = 0; i < _objectArray.Length; i++)
            {
                var obj = _objectArray[i];
                if (obj != null)
                {
                    var scale = obj.GetComponent<ScaleUpDown>();
                    if (scale != null)
                    {
                        scale.ScaleUp(_up);
                        animTime = scale.AnimTime;
                        isRunning = true;
                    }
                }
            }
        }

        return isRunning;
    }

    private void SetParent(bool _value)
    {
        if (TargetsToBeChild == null || TargetsToBeChild.Length == 0)
            return;

        if (_value)
        {
            targetParents = new Transform[TargetsToBeChild.Length];
            for (int i = 0; i < TargetsToBeChild.Length; i++)
            {
                var target = TargetsToBeChild[i];
                if (target != null)
                {
                    targetParents[i] = target.parent;
                    target.SetParent(transform.parent, true);
                }
            }
        }
        else
        {
            for (int i = 0; i < TargetsToBeChild.Length; i++)
            {
                var target = TargetsToBeChild[i];
                if (target != null && targetParents != null && targetParents.Length > i)
                {
                    target.SetParent(targetParents[i], true);
                }
            }

            targetParents = null;
        }
    }
}

