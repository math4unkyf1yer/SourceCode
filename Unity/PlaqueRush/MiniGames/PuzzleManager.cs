using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] GameObject[] puzzlePage;
    [SerializeField] GameObject[] puzzleObject;
    [SerializeField] GameObject timerPage;
    public Transform[] puzzlePosition;
    private GameObject puzzleChosen;

    public GameObject player;
    public MapGenerator mapScript;
    private GameObject playerCamera;
    private GameObject playerCameraFpp;

    private Timer timerScript;

    //puzzle count
    private int howManyPuzzleFinish;
    public int puzzleNeededFinish;

    //make sure can only do one puzzle at a time
    public bool puzzleActive = false;

    private void Start()
    {
        timerScript = gameObject.GetComponent<Timer>();
        
    }

    public void startPuzzle()
    {
        MixedPuzzles(puzzleObject);
        //start timer
        timerScript.startTimer();
        PlayerIs(false);
        SpawnPuzzle();
    }
    public void SpawnPuzzle()
    {

        timerPage.SetActive(true);
        //Instantiate spawn the first 3 
        for (int i = 0; i < 3; i++)
        {
            GameObject spawnObject = Instantiate(puzzleObject[i]);
            spawnObject.transform.position = puzzlePosition[i].position;
            ClickableObject clickObjScript = spawnObject.GetComponent<ClickableObject>();
            clickObjScript.GetPuzzleManager(this);
        }
    }

    public void SpawnPuzzleUI(int index)
    {
        puzzleActive = true;
        //for now
        puzzlePage[index].SetActive(true);
        if (index == 4)
        {
            lightPuzzle lightScript = puzzlePage[index].GetComponent<lightPuzzle>();
            lightScript.StartPuzzle();
        }
        puzzleChosen = puzzlePage[index]; 
    }

    public void FinishActivePuzzle()
    {
        puzzleChosen.SetActive(false);
        howManyPuzzleFinish++;
        puzzleActive = false;
        if (puzzleNeededFinish == howManyPuzzleFinish)
        {
            AllPuzzleFinish();
        }
    }
     private void AllPuzzleFinish()
     {
        if (mapScript)
        {
            Debug.Log("map");
            mapScript.Delete();
        }

        timerScript.StopTimer(false);
        ClosePages();

        //Destroys objects

        //make player move
        PlayerIs(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        //make wall move
        GameObject wall = GameObject.FindWithTag("EndWALL");
        Destroy(wall);
        //reset values
        howManyPuzzleFinish = 0;
        //2 generator for different region 
     }

    public  void ClosePages()
    {
        puzzleChosen.SetActive(false);
        timerPage.SetActive(false);
    }

    void MixedPuzzles(GameObject[] array) //no need will change now 
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            //swapping
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    //Player stuff
    void PlayerIs(bool enabled)
    {
        if (enabled)
        {
            playerMovement plaScript = player.GetComponent<playerMovement>();
            plaScript.enabled = true;
            plaScript.SetMovement();
            plaScript.forwardSpeed += 3;
            playerCamera.SetActive(true);
            playerCameraFpp.SetActive(false);
        }
        else
        {
            Transform playerCameraTP = player.transform.Find("PlayerCamera");
            Transform playerCameraFP = player.transform.Find("PlayerCameraFP");
            playerCamera = playerCameraTP.gameObject;
            playerCameraFpp = playerCameraFP.gameObject;
            playerCamera.SetActive(false);
            playerCameraFpp.SetActive(true);
        }
    }

}
