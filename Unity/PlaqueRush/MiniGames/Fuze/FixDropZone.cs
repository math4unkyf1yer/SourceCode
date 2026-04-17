using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixDropZone : MonoBehaviour, IDropHandler
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
        // Try to get the draggable object
        GameObject droppedObject = eventData.pointerDrag;
        dragFuzeScript = droppedObject.GetComponent<DragFuze>();

        if (droppedObject != null && dragFuzeScript.fix)
        {
            dragFuzeScript.dropInDropZone = true;
            // Reset position if needed
            droppedObject.GetComponent<RectTransform>().anchoredPosition = rectTransform.anchoredPosition;

            // Play correct placement sound
            fuzeManagerScript.OnCorrectPlacement();

            //finish the mini Game
            fuzeManagerScript.FinishMiniGame();
        }
        else if (droppedObject != null && !dragFuzeScript.fix)
        {
            // Play incorrect placement sound
            fuzeManagerScript.OnIncorrectPlacement();
        }
    }
}