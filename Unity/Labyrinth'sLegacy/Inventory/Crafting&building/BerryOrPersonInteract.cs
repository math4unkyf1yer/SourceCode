using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BerryOrPersonInteract : MonoBehaviour
{
    public float radius = 3f;
    Transform player;
    public Transform interactionTransform;

    public GameObject berryHolder;
    public GameObject dropObject;
    public int amountSpawn = 3;
    public bool berry;
    //for Workbench
    public bool workBench;
    private Canvas playerUI;
    public GameObject WorkBenchPage;
    public GameObject inventoryPage;
    public GameObject HoldTheSlotPage;
    public GameObject holderPage;
    private Inventory inventory;
    private Tutorial tutoScript;
    private PlayerMovement playerMovementScript;
    private ThirdPersonCam thirdPersonScript;
    private Text interactText;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        tutoScript = player.GetComponent<Tutorial>();
        inventory = Inventory.Intance;
        playerMovementScript = PlayerMovement.instance;
        thirdPersonScript = ThirdPersonCam.Intance;
    }
    private void Update()
    {
        float distance = Vector3.Distance(player.position, interactionTransform.position);
        if (distance < radius)
        {
            interactText = GameObject.Find("TextInteract").GetComponent<Text>();
            if(berry == true)
            {
                if (berryHolder.activeSelf == true)
                {
                    interactText.text = "To interact Pressed (E)".ToString();
                }
            }

            if (workBench)
            {
                if (WorkBenchPage == null || WorkBenchPage.activeSelf == false)
                {
                    interactText.text = "To interact Pressed (E)".ToString();
                }
                else
                {
                    interactText.text = "".ToString();
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(tutoScript.currentStep == 8)
                {
                    tutoScript.currentStep++;
                }
                InteractObject();///not pick up object 
            }
        }
        else
        {
            if(interactText != null)
            {
                interactText.text = "".ToString();
                interactText = null;
            }
        }
        CloseWorkBench();
    }

    void CloseWorkBench()
    {
        if (workBench && WorkBenchPage != null)
        {
            if (WorkBenchPage.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                playerMovementScript.TurnMovementOff = false;
                thirdPersonScript.StartCameraMovement();
                inventory.ClosePage();
                inventory.inventoryOpen = false;
                holderPage.SetActive(true);
                WorkBenchPage.SetActive(false);

                // Lock the cursor back when closing workbench
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    void OpenWorkBench()
    {
        playerMovementScript.TurnMovementOff = true;
        thirdPersonScript.StopCameraMovement();
        playerUI = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (berry == false)
        {
            GameObject playerUic = playerUI.transform.Find("Playerui").gameObject;
            WorkBenchPage = playerUic.transform.Find("WorkbenchPage").gameObject;
            inventoryPage = playerUic.transform.Find("Inventory").gameObject;
            GameObject panelPage = inventoryPage.transform.Find("Panel").gameObject;
            foreach (Transform child in panelPage.transform)
            {
                child.gameObject.SetActive(true);
                if (child.name == "HoldTheOtherSlots")
                    HoldTheSlotPage = child.gameObject;
            }
            holderPage = HoldTheSlotPage.transform.Find("Holder").gameObject;
        }
        WorkBenchPage.SetActive(true);
        holderPage.SetActive(false);
        //mouse can move
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //set inventory to be true
        inventory.inventoryOpen = true;
    }
    public void CloseText()
    {
        interactText.text = "".ToString();
    }
    public virtual void InteractObject()
    {
        if (berry == true)
        {
            if (berryHolder.activeSelf == true)
            {
                interactText.text = "".ToString();
                berryHolder.gameObject.SetActive(false);
                SpawnObject();
            }
        }
        else if (workBench == true)
        {
            OpenWorkBench();
        }
    }
    private void SpawnObject()
    {
        for (int i = 0; i <= amountSpawn; i++)
        {
            int zPos = Random.Range(1, 3);
            int xPos = Random.Range(1, 3);
            GameObject cloneDrop = Instantiate(dropObject);
            cloneDrop.transform.position = new Vector3(gameObject.transform.position.x + xPos, gameObject.transform.position.y, gameObject.transform.position.z + zPos);
            Transform closestParent = FindParent();
            cloneDrop.transform.SetParent(closestParent);
        }
    }
    private Transform FindParent()
    {
        Transform closestParent = null;
        float closestDistance = float.MaxValue;
        TerGen[] terGens = FindObjectsOfType<TerGen>();
        foreach (TerGen terGen in terGens)
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, terGen.parent.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestParent = terGen.parent;
            }
        }
        return closestParent;
    }
    private void OnDrawGizmos()
    {
        if (interactionTransform == null)
            interactionTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(interactionTransform.position, radius);
    }
}
