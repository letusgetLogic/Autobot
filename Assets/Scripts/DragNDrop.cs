using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private Transform rootTf;

    [SerializeField]
    private float scale = 1.1f;

    private Vector3 offset;
    private Camera mainCamera;
    private Vector3 originalScale;

    private void Start()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("PointerDown");

        offset = transform.position - mainCamera.ScreenToWorldPoint(
           new Vector3(eventData.position.x, eventData.position.y, 10f));

        transform.localScale *= scale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("Drag");

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, 10f));

        rootTf.position = worldPosition + offset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("PointerUp");
       
        transform.localScale = originalScale;
    }
}