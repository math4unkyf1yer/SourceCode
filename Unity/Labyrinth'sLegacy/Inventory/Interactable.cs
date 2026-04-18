using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    public Transform interactionTransform;

    private Transform player;
    private Tutorial tutoScript;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        tutoScript = GameObject.Find("Player").GetComponent<Tutorial>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.position, interactionTransform.position);
        if (distance < radius)
            Interact();
    }

    public virtual void Interact()
    {
        if (tutoScript.currentStep == 2 && interactionTransform.name == "WoodPick(Clone)")
            tutoScript.currentStep = 3;

        if (tutoScript.currentStep == 3 && interactionTransform.name == "StonePick(Clone)")
            tutoScript.currentStep = 4;
    }

    private void OnDrawGizmos()
    {
        if (interactionTransform == null)
            interactionTransform = transform;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(interactionTransform.position, radius);
    }
}
