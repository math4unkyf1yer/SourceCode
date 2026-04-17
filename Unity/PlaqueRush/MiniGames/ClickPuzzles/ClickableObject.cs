using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public int puzzleIndex;
    private PuzzleManager puzzleManagerScript;

    public void GetPuzzleManager(PuzzleManager script)
    {
        puzzleManagerScript = script;
    }
    public void OnClicked()
    {
        if(puzzleManagerScript.puzzleActive == false)
        {
            Debug.Log("puzzle start");
            //do stuff in the puzzle manager
            puzzleManagerScript.SpawnPuzzleUI(puzzleIndex);
            Destroy(gameObject);
        }
    }
}
