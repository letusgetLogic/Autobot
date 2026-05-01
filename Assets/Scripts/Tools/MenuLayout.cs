using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLayout : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private List<Button> buttons;

    [ContextMenu("Switch To This Layout")]
    public void OnSwitch()
    {
        mainMenu.Layouts.ForEach(x => x.gameObject.SetActive(false));
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        buttons.ForEach(x => x.interactable = true);
    }

    public void DisableButtons()
    {
        buttons.ForEach(x => x.interactable = false);
    }
}