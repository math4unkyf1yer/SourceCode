using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragFuze : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Vector2 startTransform;
    public Image Imagesprite;
    public Sprite brokenImage;
    public Canvas canvas;
    public bool canDrag;
    public bool fix;
    public bool dropInDropZone;
    private CanvasGroup canvasGp;
    private FuzeManager fuzeManagerScript;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGp = GetComponent<CanvasGroup>();
        startTransform = rectTransform.anchoredPosition;
        fuzeManagerScript = GetComponentInParent<FuzeManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canDrag)
        {
            // Play pickup sound
            if (fuzeManagerScript != null)
            {
                fuzeManagerScript.OnFuzePickup();
            }
        }

        // Optional: Bring to front or highlight
        canvasGp.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null && canDrag)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGp.blocksRaycasts = true;
        if (!dropInDropZone)
        {
            // Optional: Snap to grid or finalize position
            rectTransform.anchoredPosition = startTransform;
        }
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void ResetPosition(Transform parent)
    {
        transform.SetParent(parent);
        rectTransform.anchoredPosition = startTransform;
        canDrag = true;
    }

    public void brokenFuzeSwap()
    {
        Imagesprite.sprite = brokenImage;
    }
}