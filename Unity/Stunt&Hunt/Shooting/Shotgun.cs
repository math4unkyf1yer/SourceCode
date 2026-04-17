using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Shotgun : Shoot
{
    public GameObject shotgunReticle;
    public GameObject sniperReticle;
    // [System.NonSerialized] public bool isSniper = true;
    public bool isSniper = true;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isSniper) {
            Debug.Log("Reached first if statement");
            // FirstPersonController();
            shotgunReticle.SetActive(true);
            sniperReticle.SetActive(false);
            isSniper = false;
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isSniper){

            Debug.Log("Reached Second if statement");
            // ThirdPersonController();
            shotgunReticle.SetActive(false);
            sniperReticle.SetActive(true);
            isSniper = true;
        }
        
        
    }
 
}
