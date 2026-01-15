using UnityEngine;

public class TutorialStep : MonoBehaviour
{
    public GameObject[] CoverPanels;
    public GameObject[] Labels;
    public GameObject[] Hints;
    public GameObject[] HintsAFK;
    public bool IsValidating = false;

    public void OnEnter()
    {
        SetActive(CoverPanels, true);
        SetScaleUp(CoverPanels, true);
    }

    public void OnLateEnter()
    {
        if (IsValidating)
            return;

        SetActive(Labels, true);
        SetActive(Hints, true);
    }

    public void OnExit()
    {
        SetActive(HintsAFK, false);
        SetActive(Labels, false);
        SetActive(Hints, false);

        if (SetScaleUp(CoverPanels, false) == false)
            SetActive(CoverPanels, false);
    }

    public void OnAFKAnimator()
    {
        SetActive(HintsAFK, true);
    }

    private void SetActive(GameObject[] _objectArray, bool _active)
    {
        if (_objectArray != null)
        {
            for (int i = 0; i < _objectArray.Length; i++)
            {
                var obj = _objectArray[i];
                if (obj != null && obj.activeSelf != _active)
                    obj.SetActive(_active);
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
                        isRunning = true;
                    }
                }
            }
        }

        return isRunning;
    }

}

