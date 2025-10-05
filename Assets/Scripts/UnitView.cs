using TMPro;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer shadowSpriteRenderer;

    [SerializeField]
    private SpriteRenderer dragSpriteRenderer;

    [SerializeField] 
    private GameObject description;

    [SerializeField]
    private TextMeshProUGUI 
        name,
        ability,
        coin,
        health,
        attack;

    private void Awake()
    {
        description.SetActive(false);
    }

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
        dragSpriteRenderer.sprite = _sprite;
        shadowSpriteRenderer.sprite = _sprite;
        name.text = _name;
        ability.text = _description;
        coin.text = _cost.ToString();
        health.text = _health.ToString();
        attack.text = _attack.ToString();
    }

    /// <summary>
    /// Sets game object description active true/false.
    /// </summary>
    /// <param name="value"></param>
    public void SetDescriptionActive(bool value)
    {
        description.SetActive(value);
    }
}
