using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeightNeeded : MonoBehaviour
{
    public int weightNeeded;
    private int currentWeight;
    private bool doorOpen;

    private Selecting powerScript;

    public GameObject doors;
    private Animator doorAnimator;
    private AudioSource doorAudio;
    public AudioClip openDoor;
    public AudioClip closeDoor;
    private Collider doorCollider;

    public float moveDuration = 1f;
    private Coroutine moveCoroutine;

    private Vector3 originalDoorPosition;
    private Vector3 targetOpenPosition;

    private bool isMoving = false;

    // Keep track of objects currently on plate
    private HashSet<Weight> weightsOnPlate = new HashSet<Weight>();

    //Text 
    public TextMeshPro[] weightText;

    private void Start()
    {
        powerScript = GameObject.Find("Selecting").GetComponent<Selecting>();
        weightText[0].text = "0/" + weightNeeded.ToString();
        weightText[1].text = "0/" + weightNeeded.ToString();
        if (doors != null)
        {
            doorAnimator = doors.GetComponent<Animator>();
            doorAudio = doors.GetComponent<AudioSource>();
            doorCollider = doors.GetComponent<Collider>();

            originalDoorPosition = doors.transform.position;
            targetOpenPosition = originalDoorPosition + new Vector3(0, 6f, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Weight weightScript = collision.gameObject.GetComponent<Weight>();
        if (weightScript != null)
        {
            weightsOnPlate.Add(weightScript);
            if (!powerScript.freezeActivated)
                UpdateCurrentWeightAndCheck();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Weight weightScript = collision.gameObject.GetComponent<Weight>();
        if (weightScript != null)
        {
            weightsOnPlate.Remove(weightScript);
            if (!powerScript.freezeActivated)
                UpdateCurrentWeightAndCheck();
        }
    }

    private void Update()
    {
        // Detect when freeze ends
        if (!powerScript.freezeActivated && doorOpen && currentWeight < weightNeeded)
        {
            
            CloseDoor();
            Debug.Log(doorOpen);
        }
        else if (!powerScript.freezeActivated && !doorOpen && currentWeight >= weightNeeded)
        {
            MaxWeightAchieve();
            Debug.Log(doorOpen);
        }
    }

    // Called when freeze ends
    public void OnFreezeEnd()
    {
        UpdateCurrentWeightAndCheck();
    }

    private void UpdateCurrentWeightAndCheck()
    {
        currentWeight = 0;
        foreach (var w in weightsOnPlate)
        {
            if (w != null)
            {
                currentWeight += w.weight;
            }
        }
        weightText[0].text = currentWeight + "/" + weightNeeded.ToString();
        weightText[1].text = currentWeight + "/" + weightNeeded.ToString();
        if(currentWeight >= weightNeeded)
        {
            weightText[0].color = Color.green;
            weightText[0].color = Color.green;
        }
        else
        {
            weightText[0].color = Color.red;
            weightText[0].color = Color.red;
        }
    }

    void CloseDoor()
    {
        doorOpen = false;

        if (doors.tag == "notGrab")
        {
            Rigidbody rb = doors.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
        else if (doors.tag == "Laser")
        {
            doors.SetActive(true);
        }
        else
        {
            if (doorAudio)
            {
                doorAudio.clip = closeDoor;
                doorAudio.Play();
            }
            if (doorAnimator) doorAnimator.SetBool("isDoorOpen", false);
            if (doorCollider) doorCollider.enabled = true;
        }
    }

    void MaxWeightAchieve()
    {
        doorOpen = true;

        if (doors.tag == "notGrab")
        {
            Rigidbody rb = doors.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            if (doors.transform.position.y < targetOpenPosition.y - 0.01f)
            {
                StartMoveDoor(targetOpenPosition);
            }
        }
        else if (doors.tag == "Laser")
        {
            doors.SetActive(false);
        }
        else
        {
            if (doorAudio) {
                doorAudio.clip = openDoor;
                doorAudio.Play();
            } 
            if (doorAnimator) doorAnimator.SetBool("isDoorOpen", true);
            if (doorCollider) doorCollider.enabled = false;
        }
    }

    void StartMoveDoor(Vector3 targetPosition)
    {
        if (isMoving) return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveDoorSmoothly(targetPosition));
    }

    IEnumerator MoveDoorSmoothly(Vector3 targetPos)
    {
        isMoving = true;
        Vector3 startPos = doors.transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            doors.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        doors.transform.position = targetPos;
        isMoving = false;
    }
}
