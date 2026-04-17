using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    Transform player;
    public Transform interactionTransform;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(player.position, interactionTransform.position);
        if(distance < radius)
        {
            Interact();
        }
    }
    public virtual void Interact()
    {
        Tutorial tutoScript = GameObject.Find("Player").GetComponent<Tutorial>();
        if(tutoScript.currentStep == 2 && interactionTransform.name == "WoodPick(Clone)")
        {
            tutoScript.currentStep = 3;
        }
        if (tutoScript.currentStep == 3 && interactionTransform.name == "StonePick(Clone)")
        {
            tutoScript.currentStep = 4;
        }
        Debug.Log("Interacting with"+ interactionTransform.name);
    }
    private void OnDrawGizmos()
    {
        if (interactionTransform == null)
            interactionTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(interactionTransform.position, radius);
    }
}
