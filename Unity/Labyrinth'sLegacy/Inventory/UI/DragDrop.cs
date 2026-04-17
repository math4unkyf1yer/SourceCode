using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventorySlot inventorySlotScript;
    public GameObject movingParent;
    public Transform staticParent;
    public int startPosition;
    private Vector3 oldposition;
    private Transform oldParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        oldposition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        inventorySlotScript = GetComponentInParent<InventorySlot>();
        inventorySlotScript.dragItem = true;
        Debug.Log("OnBeginDrag");
        oldParent = this.transform.parent;
        this.transform.SetParent(movingParent.transform);
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        rectTransform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log(this.transform.parent);
        // Check if the pointer is over a valid slot
       // Optionally, handle what happens when dropped on a valid slot
        
        if(transform.parent.gameObject.name == "HoldTheOtherSlots")
        {
            transform.SetParent(oldParent);
            gameObject.transform.localPosition = oldposition;
        }
        gameObject.transform.localPosition = oldposition;
        inventorySlotScript.dragItem = false;
        canvasGroup.blocksRaycasts = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
}
