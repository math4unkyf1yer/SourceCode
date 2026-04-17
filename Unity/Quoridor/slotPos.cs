using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slotPos : MonoBehaviour
{
    public Node currentNode;
    private pawnCoordinates pawnScript;
    public Image slotImage;
    public Sprite randomItem;
    
    public void PlaceNode(Node nodePlace)
    {
        currentNode = nodePlace;
        GameObject playerPawn = GameObject.FindGameObjectWithTag("Player");
        pawnScript = playerPawn.GetComponent<pawnCoordinates>();
    }

    public void Click()
    {
        pawnScript.movePawn(currentNode.gridPos,this.transform,currentNode);
       
    }

    //add item on the slot which just the UI for it
    public void AddItemToSlot()
    {
        //add the UI for item
        slotImage.sprite = randomItem;
        Color c = slotImage.color;
        c.a = 1;
        slotImage.color = c;
    }
    public void RemoveItemToSlot()
    {
        Debug.Log("remove item");
        //remove the UI for it 
        Color c = slotImage.color;
        c.a = 0;
        slotImage.color = c;
        slotImage.sprite = null;
    }

}
