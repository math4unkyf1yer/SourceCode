using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeffEventTrigger : MonoBehaviour
{
    private JeffTalking jeffScript;

    // Start is called before the first frame update
    void Start()
    {
        jeffScript = GetComponent<JeffTalking>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        JeffGivingTrigger receive = other.gameObject.GetComponent<JeffGivingTrigger>();
        if (receive != null && receive.gaveInformation == false)
        {
            receive.gaveInformation = true;
            jeffScript.PlayDialogueSequence(receive.whichDialogue,1.8f,null,false);
        }
    }

    //will need 3 for the respawn and when trying to grab him
    public void specialEventTriggersDialogue()
    {
        Debug.Log("Not needed yet");
    }

    private void TriggerDialogue(int which)
    {
        jeffScript.SayDialogue(which,true);
    }
}
