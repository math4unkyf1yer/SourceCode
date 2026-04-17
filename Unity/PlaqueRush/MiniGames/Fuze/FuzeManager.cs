using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FuzeManager : MonoBehaviour
{
    [Header("Fuze Settings")]
    private DragFuze[] dragScripts;
    private FixDropZone[] dropZones;
    private RectTransform brokenFuzePos;
    public GameObject parentGameobject;
    [SerializeField] int ypos = 122;
    trashDropZone trashScript;
    public PuzzleManager puzzleManagerScript;

    [Header("Audio Settings")]
    [SerializeField] AudioSource fuzeDisposedSound;
    [SerializeField] AudioSource correctPlacementSound;
    [SerializeField] AudioSource incorrectPlacementSound;
    [SerializeField] AudioSource fuzePickupSound;

    // Start is called before the first frame update
    void Start()
    {
        trashScript = GetComponentInChildren<trashDropZone>();
        dragScripts = GetComponentsInChildren<DragFuze>();
      //  RectTransform rect = GameObject.Find("BrokenFuze").GetComponent<RectTransform>();
    //    brokenFuzePos = rect;
        ChooseWhichIsBroken();
    }

    //start puzzle which will be in puzzle manager
    void ChooseWhichIsBroken()
    {
        int i = Random.Range(0, dragScripts.Length - 1);
        dragScripts[i].canDrag = true;
        dragScripts[i].brokenFuzeSwap();
       // brokenFuzePos.anchoredPosition = dragScripts[i].GetRectTransform().anchoredPosition;
        //Vector2 pos = brokenFuzePos.anchoredPosition;
     //   pos.y = ypos;
      //  brokenFuzePos.anchoredPosition = pos;
    }

    public void FixMove()
    {
        int i = dragScripts.Length - 1;
        dragScripts[i].canDrag = true;
    }

    // Call this when a fuse is picked up
    public void OnFuzePickup()
    {
        if (fuzePickupSound != null)
        {
            fuzePickupSound.Play();
        }
    }

    // Call this when the broken fuse is disposed in the trash
    public void OnFuzeDisposed()
    {
        if (fuzeDisposedSound != null)
        {
            fuzeDisposedSound.Play();
        }
    }

    // Call this when a fuse is placed in the correct spot
    public void OnCorrectPlacement()
    {
        if (correctPlacementSound != null)
        {
            correctPlacementSound.Play();
        }
    }

    // Call this when a fuse is placed in the incorrect spot
    public void OnIncorrectPlacement()
    {
        if (incorrectPlacementSound != null)
        {
            incorrectPlacementSound.Play();
        }
    }

    public void FinishMiniGame()
    {
        //reset game 
        trashScript.gameObject.SetActive(true);
        //set everything to there start postion 
        foreach (DragFuze script in dragScripts)
        {
            script.ResetPosition(transform);
        }
        puzzleManagerScript.FinishActivePuzzle();
    }
}