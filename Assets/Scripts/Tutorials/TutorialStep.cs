using System;
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

    public UnityAction OnLabelPopup { get; set; }
    /// <summary>
    /// Make sure, all actions of current step are animated done, to go the next step.
    /// </summary>
    public List<ScaleUpDown> ActiveActions { get; set; } = new();
    public UnityAction<ScaleUpDown> OnDone { get; set; }

    private Stack<Transform> targetParents = new();
    private Stack<int> posInParent = new();

    public List<Coroutine> Coroutines => coroutines;
    private List<Coroutine> coroutines = new();
    private float animTime;

    [SerializeField] private float maxLagTime = 3.0f;
    private float lagCount;

    private void OnDisable()
    {
        if (coroutines != null)
        {
            coroutines = null;
        }
    }

    public void OnEnter()
    {
        SetParentOfComponents(true);

        bool hasAnim = false;
        bool hasAnim1 = SetScaleUp(CoverPanels, true);
        bool hasAnim2 = SetScaleUp(CoverPanelsToDeactivate, true);
        hasAnim = hasAnim1 || hasAnim2;

        if (hasAnim)
        {
            coroutines.Add(StartCoroutine(DelayActivate(Labels, animTime, true)));
            coroutines.Add(StartCoroutine(DelayActivate(Hints, animTime, false)));
        }
        else
        {
            SetActive(Labels, true);
            SetActive(Hints, true);
        }

        SetActive(CoverPanels, true);
        SetActive(CoverPanelsToDeactivate, true);
    }

    private IEnumerator DelayActivate(GameObject[] _objects, float _delay, bool _withScalingUp)
    {
        yield return new WaitForSeconds(_delay);

        SetActive(_objects, true);

        if (_withScalingUp)
            SetScaleUp(_objects, true);
    }

    public void OnLateEnter()
    {
        SetActive(HintsAFK, true);
    }

    public void OnUpdate(float _speed)
    {
        lagCount += _speed;
    }

    public float OnExit()
    {
        SetActive(HintsAFK, false);
        SetActive(Labels, false);
        SetActive(Hints, false);
        SetParentOfComponents(false);

        lagCount = 0f;

        if (SetScaleUp(CoverPanelsToDeactivate, false))
        {
            coroutines.Add(StartCoroutine(DelayDeactivate(animTime)));
            return animTime;
        }
        else
            SetActive(CoverPanelsToDeactivate, false);

        return 0f;
    }

    /// <summary>
    /// Delays deactivation of cover panels after animation.
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator DelayDeactivate(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        yield return new WaitUntil(() =>
        {
            // If the time count was exceeded, continue.
            if (lagCount > maxLagTime)
            {
                ActiveActions.Clear();
            }

            return ActiveActions.Count == 0;
        });

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
                    if (scale != null &&
                        ((_up && obj.activeSelf == false) ||
                        (!_up && obj.activeSelf == true)))
                    {
                        obj.SetActive(true);
                        scale.ScaleUp(_up);
                        animTime = scale.AnimTime;

                        ActiveActions.Add(scale);
                        scale.OnRunningDone += () => RemoveAction(scale);
                        isRunning = true;
                    }
                }
            }
        }

        return isRunning;
    }

    private void SetParentOfComponents(bool _value)
    {
        if (TargetsToBeChild == null || TargetsToBeChild.Length == 0)
            return;

        if (_value)
        {
            Stack<Transform> stackChilds = new Stack<Transform>();

            for (int i = 0; i < TargetsToBeChild.Length; i++)
            {
                var target = TargetsToBeChild[i];
                if (target != null)
                {
                    targetParents.Push( target.parent);
                    posInParent.Push(target.GetSiblingIndex());
                    stackChilds.Push(target);
                  
                }
            }
            while (stackChilds.Count > 0)
            {
                var child = stackChilds.Pop();
                child.SetParent(transform.parent, true);
            }
        }
        else
        {
            Stack<Transform> stackChilds = new Stack<Transform>();

            for (int i = 0; i < TargetsToBeChild.Length; i++)
            {
                var target = TargetsToBeChild[i];
                if (target != null)
                {
                    stackChilds.Push(target);
                }
            }
            while (stackChilds.Count > 0)
            {
                var child = stackChilds.Pop();
                var parent = targetParents.Pop();
                var index = posInParent.Pop();
                child.SetParent(parent, true);
                child.SetSiblingIndex(index);
            }
        }
    }

    private void RemoveAction(ScaleUpDown _scale)
    {
        ActiveActions.Remove(_scale);
    }
}

