using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDropDiscard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DragDrop dragdrop = eventData.pointerDrag.GetComponent<DragDrop>();
        dragdrop.transform.SetParent(dragdrop.staticParent.transform);
        InventorySlot slotMoving = dragdrop.GetComponentInParent<InventorySlot>();
        slotMoving.OnRemoveButton();
    }
}
