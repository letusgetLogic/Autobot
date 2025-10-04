using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private TextMeshProUGUI 
        name,
        description,
        cost,
        health,
        attack;

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_description"></param>
    /// <param name="_cost"></param>
    /// <param name="_health"></param>
    /// <param name="_attack"></param>
    public void SetData(Sprite _sprite, string _name, string _description, 
        int _cost, int _health, int _attack)
    {
        spriteRenderer.sprite = _sprite;
        //name.text = _name;
        //description.text = _description;
        //cost.text = _cost.ToString();
        //health.text = _health.ToString();
        //attack.text = _attack.ToString();
    }
}
