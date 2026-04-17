using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropWires : MonoBehaviour, IDropHandler
{
    private DragWires dragWiresScript;
    private WiresComplete wirePageScript;
    public string gameObjectName;

    private void Start()
    {
        wirePageScript = GetComponentInParent<WiresComplete>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.gameObject.name == gameObjectName)
        {
            dragWiresScript = eventData.pointerDrag.gameObject.GetComponent<DragWires>();
            dragWiresScript.dragInSlot = true;

            // Snap to this drop zone
            eventData.pointerDrag.GetComponent<RectTransform>().position = transform.position;

            // Play connect sound
            wirePageScript.OnWireConnect();
            wirePageScript.PassWire(dragWiresScript);
            wirePageScript.WireDone();
        }
        else if (eventData.pointerDrag != null)
        {
            // Wrong wire - play disconnect sound
            wirePageScript.OnWireDisconnect();
        }
    }
}