using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropCardZone : MonoBehaviour,IDropHandler
{
    private SwipeCard cardScript;
    private Vector2 startPosition;
    public PuzzleManager puzzleManagerScript;
    public float minTimer;
    public float maxTimer;
    private float distance;

    private void Start()
    {
        startPosition = new Vector2(transform.position.x, transform.position.y);
        
    }
    public void OnDrop(PointerEventData data)
    {
        if (data.pointerDrag != null)
        {
            GameObject droppedObject = data.pointerDrag;
            //check how long it took is to fast or to slow 
            if (droppedObject != null)
            {
                cardScript = droppedObject.GetComponent<SwipeCard>();
                distance = cardScript.maxOffsets - cardScript.minOffsets;
                float swipeSpeed = distance / cardScript.timer;
                Debug.Log(swipeSpeed);
                //see how long it took to drag at the end 
                if (swipeSpeed is >= 200 and <= 1000f)
                {
                    //perfect time ---
                    if (cardScript != null)
                    {
                        cardScript.drop = true;
                        droppedObject.transform.position = startPosition;
                        //need to tell  puzzle master 
                        puzzleManagerScript.FinishActivePuzzle();
                    }
                }
                else
                {
                    Debug.Log("To slow or to fast");
                }
            }
        }
    }
}
