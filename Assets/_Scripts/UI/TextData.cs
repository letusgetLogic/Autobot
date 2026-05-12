using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditText : MonoBehaviour
{
    // Serialize to allow dragging and dropping in the Inspector
    [SerializeField] private List<TextMeshProUGUI> targetList;

    // Optional: Store the text content
    [SerializeField] private string pc;
    [SerializeField] private string mobile;
    [SerializeField] private bool isMobile;

    public List<TextMeshProUGUI> TargetText => targetList;

    private void OnEnable()
    {
        UpdateText();
    }

    [ContextMenu("Update Text")]
    public void UpdateText()
    {
        if (targetList != null)
        {
            foreach (var text in targetList)
            {
                if (text != null)
                {
                    text.text = GameManager.Instance ? (GameManager.Instance.IsMobile ? mobile : pc) :
                        (isMobile ? mobile : pc);
                }
            }
        }
    }
}