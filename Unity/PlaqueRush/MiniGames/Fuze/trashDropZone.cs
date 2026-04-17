using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class trashDropZone : MonoBehaviour, IDropHandler
{
    private RectTransform rectTransform;
    private DragFuze dragFuzeScript;
    private FuzeManager fuzeManagerScript;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        fuzeManagerScript = GetComponentInParent<FuzeManager>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped something on me!" + eventData.pointerDrag);
        // Try to get the draggable object
        GameObject droppedObject = eventData.pointerDrag;
        dragFuzeScript = droppedObject.GetComponent<DragFuze>();

        if (droppedObject != null)
        {
            dragFuzeScript.dropInDropZone = true;
            // Optionally, reparent the dropped object to this drop zone
            droppedObject.transform.SetParent(transform);
            // Reset position if needed
            droppedObject.GetComponent<RectTransform>().anchoredPosition = rectTransform.anchoredPosition;

            // Play disposed sound
            fuzeManagerScript.OnFuzeDisposed();

            fuzeManagerScript.FixMove();
            gameObject.SetActive(false);
        }
    }
}