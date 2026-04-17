using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Selecting : MonoBehaviour
{
    public Camera cam; // Assign your camera (usually the Main Camera)
    public LineRenderer lineRenderer;
    public float lineLength = 14f;
    public LayerMask raycastLayers;
    private LayerMask obstacleLayers;
    private OutlineHighlighter lastHighlighted;
    private OutlineHighlighter highligtedCl;
    private bool selectedObject;

    public PostProcessVolume grayscaleVolume;
    public bool isGrayscale = false;

    private bool isRewinding = false;
    private Coroutine rewindCoroutine = null;
    private GameObject rewindingObject = null;
    private ColorGrading colorGrading;

    [Header("Swap")]
    public bool swap;
    private GameObject player;

    private string saveTypeRb;

    [Header("Freeze")]
    public bool freeze;
    public bool freezeActivated;
    private Coroutine freezeCoruntine = null;
    FreezeObjects[] objectsFreeze;
    AudioSource[] objectAudio;
    [Header("Grabbing")]
    public GameObject objectOn;
    public GameObject grabbedObject;
    public bool isGrabbing;
    public bool cannotGrabble = false;
    private Rigidbody grabbedRigid;
    private Vector3 previousPos;
    private Vector3 currentVelocity;
    private float grabDistance;
    public float maxThrowSpeed = 10f;
    private float maxDistance;
    private float minDistance = 3f;
    public float scrollSpeed = 2f;
    private PlatformMove platformScript;

    [Header("Beam")]
    public float beamOffseyrs = 1.5f;
    public LineRenderer beamRenderer;
    public GameObject hand;
    public GameObject beamEffect;
    public GameObject rewindEffect;
    public GameObject swapEffect;
    public GameObject freezeEffect;

    [Header("Cooldowns")]
    public CoolDownAbility coolDownScript;
    private bool isOnCoolDown = false;

    [Header("Audio")]
    public AudioClip rewindAudio;
    public AudioClip swapAudio;
    public AudioClip freezeAudio;
    public AudioSource handsAudio; 
    private AudioSource audioSource;

    [Header("Animation")]
    public Animator playerAnimation;
    // Start is called before the first frame update
    void Start()
    {
        obstacleLayers = LayerMask.GetMask("Default", "Walls");
        objectsFreeze = FindObjectsOfType<FreezeObjects>();
        objectAudio = FindObjectsOfType<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        maxDistance = lineLength;
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        // Set line renderer to have 2 points
        lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        // Get the starting position (camera position)
        Vector3 startPos = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        // Get the direction the camera is facing and multiply by lineLength
        Vector3 endPos = startPos + direction * lineLength;

        StopTragectory();

        if (freeze == true)
        {
            if (freezeActivated == false && Input.GetMouseButtonDown(1) && !isGrayscale && !isOnCoolDown)
            {
                freezeCoruntine = StartCoroutine(Freeze());
                StartCoroutine(Freeze());
            }
            else if (freezeActivated == true && Input.GetMouseButtonDown(1) && isGrayscale && !isOnCoolDown)
            {
                StopFreeze();
            }
        }

       int ignoreLayer = LayerMask.NameToLayer("TriggerZone");
       int combinedMask = raycastLayers|obstacleLayers;

        //  Use RaycastAll instead of single Raycast
        Ray ray = new Ray(startPos, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, lineLength, combinedMask);

        // Sort hits by distance so we check closest first
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        bool blockedByObstacle = false;
        bool foundTarget = false;

        foreach (var hit in hits)
        {
            int hitLayer = hit.collider.gameObject.layer;

            //  If we hit an obstacle (like wall on Default layer), block everything beyond
            if ((obstacleLayers & (1 << hitLayer)) != 0)
            {
                blockedByObstacle = true;
                break; // Stop checking further
            }

            //  If this hit is a grabbable object and we’re not blocked
            if (!blockedByObstacle && ((1 << hitLayer) & raycastLayers) != 0 && !selectedObject && grabbedObject == null)
            {
                foundTarget = true;
                endPos = hit.point;

                OutlineHighlighter highlighter = hit.collider.GetComponent<OutlineHighlighter>();

                if (highlighter != null)
                {
                    if (lastHighlighted != null && lastHighlighted != highlighter)
                        lastHighlighted.Unhighlight();

                    highlighter.Highlight();
                    lastHighlighted = highlighter;
                    highligtedCl = highlighter;
                }

                GameObject target = hit.collider.gameObject;

                if (target.CompareTag("notGrab"))
                {
                    cannotGrabble = true;
                }
                else
                {
                    cannotGrabble = false;
                }

                //  Grab logic
                if (Input.GetMouseButtonDown(0) && !isGrayscale && !cannotGrabble)
                {
                    grabbedObject = target;
                    grabbedRigid = grabbedObject.GetComponent<Rigidbody>();

                    if (grabbedRigid != null)
                    {
                        grabbedRigid.useGravity = false;
                        grabbedRigid.velocity = Vector3.zero;
                        grabbedRigid.angularVelocity = Vector3.zero;
                    }

                    grabDistance = Vector3.Distance(cam.transform.position, grabbedObject.transform.position);
                    previousPos = grabbedObject.transform.position;
                }

                //  Rewind trigger logic
                if (Input.GetMouseButtonDown(1) && !isGrayscale && !isOnCoolDown)
                {
                    grabbedObject = target;
                    grabbedRigid = grabbedObject.GetComponent<Rigidbody>();
                    if (swap == true)
                    {
                        if (!grabbedObject.CompareTag("notGrab"))
                        {
                            lastHighlighted.selectedHighlight("purple");
                            target.layer = LayerMask.NameToLayer("Highlighted");
                            StartCoroutine(SwapStart(target, lastHighlighted));
                        }
                        else { grabbedObject = null; }
                    }
                    else if (!freeze && !swap)
                    {
                        lastHighlighted.selectedHighlight("yellow");
                        target.layer = LayerMask.NameToLayer("Highlighted");
                        StartCoroutine(ApplyGrayscale(lastHighlighted, target));
                    }
                }

                break; //  Stop after processing first valid target
            }
        }

        // If no valid target was found, clear highlight
        if (!foundTarget)
        {
            ClearLastHighlight();
        }

        // Continue with grabbing movement logic
        Grabbing();

        // Update the Line Renderer
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void Grabbing()
    {
        if(grabbedObject != null)
        {
            if (grabbedObject.CompareTag("notGrab") || grabbedObject == objectOn)
            {
                //remove line renderer 
                isGrabbing = false;
                BeamDeactive();
                return; // Prevent grabbing
            }
            if (Input.GetMouseButton(0) && !cannotGrabble)
            {
                if (cannotGrabble == true)
                    return;
                BeamActive();
                LookBeam();
                isGrabbing = true;
                if(grabbedObject.gameObject.tag == "Platform" && grabbedRigid.isKinematic)
                {
                    grabbedRigid.isKinematic = false;
                }
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                int maxSpeed = 20;

                grabDistance += scroll * scrollSpeed;              // Update distance
                grabDistance = Mathf.Clamp(grabDistance, minDistance, maxDistance);

                Vector3 targetPos = cam.transform.position + cam.transform.forward * grabDistance;
                Vector3 toTarget = targetPos - grabbedRigid.position;

                // Calculate a desired velocity with smoothing
                // Smooth it by limiting max acceleration or by interpolating velocity

                // Option 1: Simple proportional velocity (like a spring)
                float smoothTime = 0.1f; // smaller = snappier, bigger = smoother
                Vector3 desiredVelocity = toTarget / smoothTime;
                currentVelocity = desiredVelocity;

                // Clamp to max speed
                desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);

                grabbedRigid.velocity = desiredVelocity;

                previousPos = grabbedRigid.position;
                
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (grabbedRigid != null)
                {

                    isGrabbing = false;
                    if(grabbedRigid.gameObject.tag == "objGravity" || grabbedRigid.gameObject.tag == "PlatformG")
                    {
                        grabbedRigid.useGravity = true;
                        grabbedRigid.velocity = Vector3.ClampMagnitude(currentVelocity, maxThrowSpeed);
                    }
                    if (grabbedRigid.gameObject.tag == "Platform")
                    {
                            grabbedRigid.isKinematic = true;                        
                    }
                    grabbedRigid = null;
                    
                    BeamDeactive();
                }
                grabbedObject = null;
            }
        }
    }

    void LookBeam()
    {
        Vector3 start = hand.transform.position + hand.transform.forward * beamOffseyrs;
        Vector3 end = grabbedObject.transform.position;

        int pointCount = 15;
        beamRenderer.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // Add curve using a sine wave (curve up or down)
            Vector3 dragOffset = transform.forward * Mathf.Sin(t * Mathf.PI) * 1.5f;
            point += dragOffset;

            beamRenderer.SetPosition(i, point);
        }
    }
    public void BeamActive()
    {
        beamEffect.SetActive(true);
        if (!handsAudio.isPlaying)
        {
            handsAudio.Play();
        }
        playerAnimation.SetBool("Grab", true);
        beamRenderer.enabled = true;
    }
    public void BeamDeactive()
    {
        beamEffect.SetActive(false);
        handsAudio.Stop();
        playerAnimation.SetBool("Grab", false);
        beamRenderer.enabled = false;
    }
    void ClearLastHighlight()
    {
        if (lastHighlighted != null)
        {
            lastHighlighted.Unhighlight();
            lastHighlighted = null;
        }
    }

    IEnumerator ApplyGrayscale(OutlineHighlighter outlineScript, GameObject hitObject)
    {
        playerAnimation.SetBool("Power", true);
        selectedObject = true;
        isGrayscale = true;
        isRewinding = true;
        rewindingObject = hitObject;
        grayscaleVolume.enabled = true;
        rewindEffect.SetActive(true);
        //the sounds 
        audioSource.clip = rewindAudio;
        audioSource.Play();
        //make it kinematic 
        if (grabbedRigid.useGravity)
        {
            saveTypeRb = "Gravity";
            grabbedRigid.useGravity = false;
            grabbedRigid.isKinematic = true;
        }
        else
        {
            saveTypeRb = "Kinematic";
        }
        TrailRenderer objTrailRenderer = hitObject.GetComponent<TrailRenderer>();
        LineRenderer objLineRend = hitObject.GetComponent<LineRenderer>();

        PositionTracker tracker = hitObject.GetComponent<PositionTracker>();
        List<TransformSnapshot> history = null;

        checkIfPlatform(hitObject);

        if (tracker != null)
        {
            history = tracker.GetFullHistory();
        }

        if (history != null && history.Count > 0)
        {
            if (objTrailRenderer != null)
                objTrailRenderer.enabled = false;
            // Move back through the recorded positions/rotations
            rewindCoroutine = StartCoroutine(ReplayTrajectory(hitObject, history, tracker.recordInterval));
            yield return rewindCoroutine;
            objLineRend.enabled = false;
            if (objTrailRenderer != null)
                objTrailRenderer.enabled = true;
            isRewinding = false;
        }
        outlineScript.UnselectedHighlight();
        var rb = rewindingObject.GetComponent<Rigidbody>();
   //     StopVelocity(rb);
        hitObject.layer = LayerMask.NameToLayer("MoveObject");
        grayscaleVolume.enabled = false;
        isGrayscale = false;
        selectedObject = false;
        if(objTrailRenderer != null)
            objTrailRenderer.enabled = true;
        rewindingObject = null;
        if (platformScript != null)
        {
            platformScript.platformRewinding = false;
            platformScript = null;
        }
        audioSource.Stop();
        CheckGravOrKin();
        playerAnimation.SetBool("Power", false);
        rewindEffect.SetActive(false);
        StartCoroutine(CooldownRewind());
    }
    void CheckGravOrKin()
    {
        if(saveTypeRb == "Gravity")
        {
            grabbedRigid.useGravity = true;
            grabbedRigid.isKinematic = false;
        }
        else
        {
            //nothing
        }
        grabbedObject = null;
        grabbedRigid = null;
    }
    void checkIfPlatform(GameObject hitObject)
    {
        if (hitObject.GetComponent<PlatformMove>() != null)
            platformScript = hitObject.GetComponent<PlatformMove>();

        if (platformScript != null)
        {
            platformScript.StartRewind();
        }
    }
    IEnumerator ReplayTrajectory(GameObject obj, List<TransformSnapshot> history, float interval)
    {
        LineRenderer objLineRend = obj.GetComponent<LineRenderer>();
        if (objLineRend == null) yield break;

        objLineRend.enabled = true;
        objLineRend.positionCount = history.Count;

        // Set all positions up front
        for (int i = 0; i < history.Count; i++)
        {
            objLineRend.SetPosition(i, history[i].position);
        }
        Rigidbody rbobj = obj.GetComponent<Rigidbody>();
        // Play history in reverse (from newest to oldest)
        for (int i = history.Count - 1; i > 0; i--)
        {
            Vector3 startPos = history[i].position;
            Quaternion startRot = history[i].rotation;
            Vector3 endPos = history[i - 1].position;
            Quaternion endRot = history[i - 1].rotation;

            float distance = Vector3.Distance(startPos, endPos);
            SpeedCheck speedScript;
            if (obj.GetComponent<SpeedCheck>() != null)
            {
                speedScript = obj.GetComponent<SpeedCheck>();
                            if(distance> 7)
            {
                speedScript.colliders.enabled = false;
            }
            else
            {
                speedScript.colliders.enabled = true;
            }
            }


            float elapsed = 0f;
            while (elapsed < interval)
            {
                if (!isRewinding) yield break;
                float t = elapsed / interval;
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
                Quaternion currentRot = Quaternion.Slerp(startRot, endRot, t);
                if(rbobj != null && obj.tag != "Platform" && obj.tag != "notGrab")
                {
                    rbobj.MovePosition(currentPos);
                    rbobj.MoveRotation(currentRot);
                }
                else
                {
                    obj.transform.position = currentPos;
                    obj.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                }
                elapsed += Time.deltaTime;

                yield return null;
            }
        }
    }

    void StopTragectory()
    {
        if (Input.GetMouseButtonDown(1) && isRewinding && rewindCoroutine != null && isOnCoolDown == false)
        {
            rewindEffect.SetActive(false);
            highligtedCl.UnselectedHighlight();
            isRewinding = false;
            playerAnimation.SetBool("Power", false);

            if (rewindCoroutine != null)
            {
                StopCoroutine(rewindCoroutine);
                rewindCoroutine = null;
            }

            if (rewindingObject != null)
            {
                // Reset object visuals/state if needed
                var objTrail = rewindingObject.GetComponent<TrailRenderer>();
                var objLine = rewindingObject.GetComponent<LineRenderer>();
                if (objLine != null) objLine.enabled = false;
                if (objTrail != null) objTrail.enabled = true;
                // 🆕 Reset Rigidbody velocity
                var rb = rewindingObject.GetComponent<Rigidbody>();
            //    StopVelocity(rb);
                rewindingObject.layer = LayerMask.NameToLayer("MoveObject");
            }

            grayscaleVolume.enabled = false;
            isGrayscale = false;
            selectedObject = false;
            rewindingObject = null;
            if (platformScript != null)
            {
                platformScript.platformRewinding = false;
                platformScript = null;
            }
            audioSource.Stop();
            CheckGravOrKin();
            StartCoroutine(CooldownRewind());


        }
    }

    // SWAP POWERS

    IEnumerator SwapStart(GameObject hitobject, OutlineHighlighter outlineScript)
    {
        audioSource.clip = swapAudio;
        audioSource.Play();
        swapEffect.SetActive(true);
        playerAnimation.SetBool("Power", true);
        // Cache both positions BEFORE the wait
        Vector3 objectPos = hitobject.transform.position;
        Vector3 playerPos = player.transform.position;

        // Optional: stop object movement if it has Rigidbody
        Rigidbody hitRb = hitobject.GetComponent<Rigidbody>();

        if (hitRb != null)
        {
            hitRb.velocity = Vector3.zero;
            hitRb.angularVelocity = Vector3.zero;
            hitRb.useGravity = false;
            hitRb.isKinematic = true;
        }

        selectedObject = true;
        isGrayscale = true;
        grayscaleVolume.enabled = true;

        yield return new WaitForSeconds(0.4f);

        // Swap positions
        hitRb.position = playerPos;
        player.transform.position = objectPos;

        // Reset object
        if (hitRb != null)
        {
            hitRb.useGravity = true;
            hitRb.isKinematic = false; // Re-enable physics
        }

        grabbedObject = null;
        selectedObject = false;
        grayscaleVolume.enabled = false;
        isGrayscale = false;

        outlineScript.UnselectedHighlight();
        hitobject.layer = LayerMask.NameToLayer("MoveObject");
        playerAnimation.SetBool("Power", false);
        swapEffect.SetActive(false);
        StartCoroutine(CooldownRewind());
    }

    private bool freezeCancelRequested = false;
    //Freeze Power 
    [System.Obsolete]
    IEnumerator Freeze()
    {
        //audio
        audioSource.clip = freezeAudio;
        audioSource.ignoreListenerPause = true;
        audioSource.PlayOneShot(freezeAudio);
        //grayscale
        isGrabbing = false;
        playerAnimation.updateMode = AnimatorUpdateMode.UnscaledTime;

        playerAnimation.SetBool("Power", true);
        ParticleSystem[] allPS = freezeEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in allPS)
        {
            var main = ps.main;
            main.useUnscaledTime = true; // make it ignore Time.timeScale
        }
        freezeEffect.SetActive(true);
        freezeCancelRequested = false;
        yield return StartCoroutine(FreezeObjectsRoutine(true));
        yield return StartCoroutine(FreezeAudio(true));
        isGrayscale = true;
        grayscaleVolume.enabled = true;
        freezeActivated = true;
        //pause time
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // also scale physics
        Physics.autoSimulation = false; // disable automatic physics
        //player can move
        float freezeTime = 6f;
        float timer = 0f;
        while (timer < freezeTime && !freezeCancelRequested)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return StartCoroutine(FreezeObjectsRoutine(false));
        yield return StartCoroutine(FreezeAudio(false));
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Physics.autoSimulation = true; // restore normal physics behavior
        freezeActivated = false;
        grayscaleVolume.enabled = false;
        isGrayscale = false;
        grabbedObject = null;
        freezeEffect.SetActive(false);
        playerAnimation.SetBool("Power", false);
        foreach (var plate in FindObjectsOfType<WeightNeeded>())
        {
            plate.OnFreezeEnd();
        }
        StartCoroutine(CooldownRewind());
    }



    [System.Obsolete]
   public void StopFreeze()
    {
        if(freezeCoruntine != null)
        {
            StartCoroutine(FreezeObjectsRoutine(false));
            StartCoroutine(FreezeAudio(false));
            foreach (var plate in FindObjectsOfType<WeightNeeded>())
            {
                plate.OnFreezeEnd();
            }
            freezeActivated = false;
            freezeCancelRequested = true;
            freezeCoruntine = null;
            Time.timeScale = 1f;
            Physics.autoSimulation = true; // restore normal physics behavior
            grayscaleVolume.enabled = false;
            isGrayscale = false;
            grabbedObject = null;
            freezeEffect.SetActive(false);
            playerAnimation.SetBool("Power", false);
            StartCoroutine(CooldownRewind());
        }
    }

    IEnumerator FreezeObjectsRoutine(bool freeze)
    {
        int batchSize = 10;
        for (int i = 0; i < objectsFreeze.Length; i++)
        {
            if (freeze)
                objectsFreeze[i].FreezeObj();
            else
                objectsFreeze[i].UnFreezeObj();

            if (i % batchSize == 0)
                yield return null; // wait one frame to spread load
        }
    }
    IEnumerator FreezeAudio(bool freeze)
    {
        int batchSize = 10;
        for (int i = 0; i < objectAudio.Length; i++)
        {
            if (freeze)
            {
                if (objectAudio[i] != audioSource) // don’t disable freezeAudio
                    objectAudio[i].enabled = false;
            }
            else
                objectAudio[i].enabled = true;

            if (objectAudio[i].gameObject.tag == "Player")
                objectAudio[i].enabled = true;

            if (i % batchSize == 0)
                yield return null; // wait one frame to spread load
        }
    }
    /*  void StopVelocity(Rigidbody rb)
      {
          if (rb != null)
          {
              rb.velocity = Vector3.zero;
              rb.angularVelocity = Vector3.zero;
              rb.useGravity = true;
          }
      }*/
    IEnumerator CooldownRewind()
    {
        coolDownScript.SetMaxFill();
        isOnCoolDown = true;
        if(!freeze)
            yield return StartCoroutine(coolDownScript.CoolDownRoutine(3f));
        else
            yield return StartCoroutine(coolDownScript.CoolDownRoutine(7f));
        isOnCoolDown = false;
    }
}
