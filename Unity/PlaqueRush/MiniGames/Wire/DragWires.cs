using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWires : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;
    [SerializeField] private Canvas canvas;
    public bool dragInSlot; // change if drop in right slot 
    private bool isDragging = false;
    private WiresComplete wirePageScript;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        wirePageScript = GetComponentInParent<WiresComplete>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.position;
        startParent = transform.parent;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        isDragging = true;

        // Play pickup sound
        if (wirePageScript != null)
        {
            wirePageScript.OnWirePickup();
        }
    }

    //Drag with the mouse 
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragInSlot)
        {
            ReturnBack(false);
        }
        isDragging = false;
    }
    public void ReturnBack(bool theEnd)
    {
        if (theEnd)
        {
            dragInSlot = false;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.position = startPosition;
        transform.SetParent(startParent);
    }
}