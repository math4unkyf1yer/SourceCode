using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuScript : MonoBehaviour
{
    public GameObject menuPage;
    public GameObject controlPage;
    public bool escape;
    private Inventory inventoryScript;
    private Tutorial tutoScript;
    private Attack attackScript;
    private PlayerMovement playerMovementScript;
  
    private ThirdPersonCam thirdPersonScript;

    #region Singleton
    public static menuScript Intance;
    private void Awake()
    {
        Intance = this;
    }
    #endregion
    private void Start()
    {
        inventoryScript = Inventory.Intance;
        GameObject player = GameObject.Find("Player");
        tutoScript = player.GetComponent<Tutorial>();
        attackScript = player.GetComponent<Attack>();
        playerMovementScript = PlayerMovement.instance;
        thirdPersonScript = ThirdPersonCam.Intance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) &&inventoryScript.inventoryOpen == false && !attackScript.onCraftPage)
        {
            if(tutoScript.currentStep == 0)
            {
                tutoScript.currentStep++;
            }
            if(escape == false)
            {
                thirdPersonScript.StopCameraMovement();
                playerMovementScript.TurnMovementOff = true;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                escape = true;
                menuPage.SetActive(true);
            }
            else
            {
                thirdPersonScript.StartCameraMovement();
                playerMovementScript.TurnMovementOff = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                escape = false;
                CloseControl();
                menuPage.SetActive(false);
            }
        }
    }

    public void ClickControl()
    {
        controlPage.SetActive(true);
    }
    public void CloseControl()
    {
        if (controlPage.activeSelf)
        {
            controlPage.SetActive(false);
        }
    }
     
}
