using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BikeController : MonoBehaviour
{
    RaycastHit hit;
    float moveInput, steerInput , rayLenght,rayLenghtUp, currentVelocityOffset;
    private TextMeshProUGUI speedText;
    public float maxSpeed, acceleration, steerStrenght, tiltAngle, gravity, bikeTiltIncrement = 0.09f, zTitlAngle = 45, hadleRotVal = 30f, handleRotSpeed = .15f;
    public float airReturnSpeed = 2f;
    [Range(1,10)]
    public float brakeFactor;
    public float speedCheck;//use for if the bike blows up;
    public bool inAir;
    private Quaternion initialHandleLocalRotation;

    //Spawn
    public GameObject spawnPosition;
    private Transform SaveSpawnPoint;
    public Transform outsideMapSpawn;
    public GameObject crashEffect;
    public bool isdead;

    [HideInInspector] public Vector3 velocity2;
    public GameObject handle;

    public Rigidbody sphereRb, bikeBody, player;
    public GameObject biketrickHolder;

    public LayerMask drivableLayer;
    public LayerMask deadLayer;

    //increase speed for speed boost
    private bool isBoosting = false;
    private float originalMaxSpeed;

    //for backflip 
    private bool isflipping;
    private bool isStalling = false;
    public bool canstall;

    public bool cantCrash;

    private SpawnBikeBack spawnScript;
    private Shoot shootScript;
    private Score scoreScript;
    public Animator playerAnimation;

    //Material
    public Material oldMat;
    public Material newMat;
    public GameObject playerObj;
    private Renderer playerObjMaterial;

    //Audio Setup
    public AudioSource trickAudio;
    public AudioSource crashAudio;
    public AudioSource idleAudio;
    public AudioSource fallAudio;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimation.enabled = false;
        cantCrash = true;
        Debug.developerConsoleVisible = true;
        initialHandleLocalRotation = handle.transform.localRotation;

        // Detach rigidbodies from parent
        if (sphereRb != null) sphereRb.transform.parent = null;
        if (bikeBody != null) bikeBody.transform.parent = null;

        sphereRb.drag = 0.1f;
        sphereRb.angularDrag = 1f;

        // Get components safely
        spawnScript = GameObject.Find("SaveSpawnPoint")?.GetComponent<SpawnBikeBack>();
        SaveSpawnPoint = GameObject.Find("SaveSpawnPoint")?.transform;

        GameObject speedTextObj = GameObject.Find("SpeedText");
        if (speedTextObj != null)
            speedText = speedTextObj.GetComponent<TextMeshProUGUI>();

        GameObject gun = GameObject.Find("Ak47Holder");
        if (gun != null)
            shootScript = gun.GetComponent<Shoot>();

        rayLenght = sphereRb.GetComponent<SphereCollider>().radius + 4f;
        rayLenghtUp = sphereRb.GetComponent<SphereCollider>().radius + 44f;
        playerObjMaterial = playerObj.GetComponent<Renderer>();

        playerObjMaterial.material = newMat;
        StartCoroutine(Immortal());
    }

    // Update is called once per frame
    void Update()
    {
      
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
       
        transform.position = sphereRb.transform.position;

        speedCheck = sphereRb.velocity.magnitude;
        speedText.text = speedCheck.ToString();

        velocity2 = bikeBody.transform.InverseTransformDirection(bikeBody.velocity);
        currentVelocityOffset = velocity2.z / maxSpeed;

        if (CanFlip())
        {
            PerformBackflip();
        }
        if (Physics.Raycast(sphereRb.position, Vector3.down, out hit, rayLenght + 2, deadLayer))
        {
            outsideMap();
        }
    }

    private void FixedUpdate()
    {      
        Movement();
    }

    void Movement()
    {
        if (Grounded() && !isStalling)
        {        
           Acceleration();
           Rotation();
           BikeTilt();

        }
        else
        {
            Gravity();
            BikeTiltAirborne();
        }
    }
    void Acceleration()
    {
        float currentSpeed = sphereRb.velocity.magnitude; // Get current speed
        float targetSpeed = maxSpeed; // Default target speed
        float accelerationRate = acceleration * Time.fixedDeltaTime; // Acceleration factor

        if (moveInput < 0) // Pressing 'S' or down arrow
        {
            targetSpeed = 0; // Decelerate to stop
            accelerationRate *= brakeFactor; // Apply brake factor
        }

        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate); // Smooth transition

        if (OnSlope())
        {
            // Add force in the forward direction to push the bike uphill
            sphereRb.AddForce(transform.forward * acceleration * 20f, ForceMode.Acceleration);
        }
        else
        {
            
            sphereRb.velocity = transform.forward * newSpeed;
        }

        if (newSpeed <= 40f && cantCrash == false && canstall == false) // If stopped, trigger game over
        {
            Debug.Log("Hello");
            //falling over 
            Stalling();
        }
    }
    bool OnSlope()
    {
        if (Grounded())
        {
           float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            return slopeAngle > 5f && slopeAngle < 50f; // tweak range for what's considered a ramp
        }
        return false;
    }

    void Stalling()
    {
        if (isStalling) return;

        isStalling = true;
        shootScript.canShoot = false;

        // Stop movement
        sphereRb.velocity = Vector3.zero;
        sphereRb.angularVelocity = Vector3.zero;

        moveInput = 0;
        steerInput = 0;

        SaveSpawnPoint.position = outsideMapSpawn.transform.position; 
        SaveSpawnPoint.rotation = outsideMapSpawn.transform.rotation;

        playerAnimation.enabled = true;
        StartCoroutine(SpawnBack());

        if (!fallAudio.isPlaying)
        {
            fallAudio.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
            fallAudio.Play();
        }

    }
    void DestroyBike()
    {
        if(cantCrash == false)
        {
            // Stop movement
            shootScript.canShoot = false;
            shootScript.HideShootLineDead();
            shootScript.waitForRealoadDead();
            crashEffect.SetActive(true);
            bikeBody.gameObject.SetActive(false);
            crashAudio.Play();
            StartCoroutine(SpawnBack());
        }
    }
    IEnumerator SpawnBack()
    {
        playerAnimation.SetTrigger("Falling");
        yield return new WaitForSeconds(2f);
        playerObjMaterial.material = newMat;
        playerAnimation.enabled = false;
        // Reset position and rotation
        cantCrash = true;
        bikeBody.transform.position = SaveSpawnPoint.position;
        bikeBody.transform.rotation = SaveSpawnPoint.rotation;

        sphereRb.transform.position = SaveSpawnPoint.position;
        sphereRb.transform.rotation = SaveSpawnPoint.rotation;

        // Reset velocity
        sphereRb.velocity = Vector3.zero;
        sphereRb.angularVelocity = Vector3.zero;

        bikeBody.velocity = Vector3.zero;
        bikeBody.angularVelocity = Vector3.zero;

        // Reset any rotation on the handle if needed
        handle.transform.localRotation = Quaternion.identity;

        // Reactivate bike visuals & physics
        bikeBody.gameObject.SetActive(true);
        crashEffect.SetActive(false);

        isStalling = false;
        isdead = false;
        shootScript.canShoot = true;
        // Make bike invincible for a few seconds again
        StartCoroutine(Immortal());
    }
    IEnumerator Immortal()
    {
        yield return new WaitForSeconds(4f);
        playerObjMaterial.material = oldMat;
        cantCrash = false;
    }

    void Rotation()
    {
        // Tilt-based steering: The more the player tilts, the sharper the turn
        float turnAmount = steerInput * steerStrenght * Time.fixedDeltaTime;
        transform.Rotate(0, turnAmount, 0, Space.World);

        // Rotation around Z simulating lean during steering
        Quaternion steer = Quaternion.Euler(0f, 0f, hadleRotVal * steerInput);

        // Combine in this specific order: baseline → steer → tilt
        Quaternion targetRot = initialHandleLocalRotation * steer;

        handle.transform.localRotation = Quaternion.Slerp(
            handle.transform.localRotation,
            targetRot,
            handleRotSpeed
        );
    }

    void BikeTilt()
    {
        float xRot = (Quaternion.FromToRotation(bikeBody.transform.up, hit.normal) * bikeBody.transform.rotation).eulerAngles.x;
        float zRot = -zTitlAngle * steerInput; // Tilt the bike based on steering input

        Quaternion targetRot = Quaternion.Slerp(
            bikeBody.transform.rotation,
            Quaternion.Euler(xRot, transform.eulerAngles.y, zRot),
            bikeTiltIncrement
        );
        

        Quaternion newRotation = Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, targetRot.eulerAngles.z);
        bikeBody.MoveRotation(newRotation);
    }
    void BikeTiltAirborne()
    {
        // Keep X rotation, reset Z rotation smoothly
        float xRot = bikeBody.transform.eulerAngles.x;
        float zRot = Mathf.LerpAngle(bikeBody.transform.eulerAngles.z, 0f, Time.fixedDeltaTime * 3f); // You can tweak speed

        Quaternion newRotation = Quaternion.Euler(xRot, transform.eulerAngles.y, zRot);
        bikeBody.MoveRotation(newRotation);
    }
    public void ActivateSpeedBoost(float boostAmount, float duration)
    {
        if (!isBoosting)
        {
            isBoosting = true;
            originalMaxSpeed = maxSpeed;
            maxSpeed += boostAmount; // Temporarily increase max speed

            StartCoroutine(GradualSpeedBoost(boostAmount, duration)); // Apply gradual speed increase
        }
    }

    private IEnumerator GradualSpeedBoost(float boostAmount, float duration)
    {
        float elapsedTime = 0f;
        float initialSpeed = sphereRb.velocity.magnitude;
        float targetSpeed = initialSpeed + boostAmount;

        // Increase speed over half of the duration (1 sec)
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newSpeed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / (duration / 2));
            sphereRb.velocity = transform.forward * newSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(duration / 2); // Maintain speed for a brief moment

        elapsedTime = 0f;
        // Gradually return speed back to normal over the remaining time
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newSpeed = Mathf.Lerp(targetSpeed, originalMaxSpeed, elapsedTime / (duration / 2));
            sphereRb.velocity = transform.forward * newSpeed;
            yield return null;
        }

        maxSpeed = originalMaxSpeed;
        isBoosting = false;
    }

    void PerformBackflip()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isflipping || Input.GetKeyDown(KeyCode.Joystick1Button0) && !isflipping)
        {
            StartCoroutine(trickRoutine());
        }
    }
    IEnumerator trickRoutine()
    {
        isflipping = true;
        shootScript.canShoot = false;
        playerAnimation.enabled = true;
        playerAnimation.SetTrigger("Trick");

        yield return new WaitForSeconds(.8f);

        scoreScript = GameObject.Find("GameManager").GetComponent<Score>();
        scoreScript.score += 5;

        playerAnimation.enabled = false;
        isflipping = false;
        // Reward the player
        shootScript.IncreaseAmmo(1);
        trickAudio.pitch = UnityEngine.Random.Range(0.8f, 1.3f);    //Temporary
        trickAudio.Play();
        shootScript.canShoot = true;
        // Reset
    }

    public bool CanFlip()
    {
        if (Physics.Raycast(sphereRb.position, Vector3.down, out hit, rayLenghtUp, drivableLayer))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool Grounded()
    {
        if (Physics.Raycast(sphereRb.position, Vector3.down, out hit, rayLenght, deadLayer))
        {
            isdead = false;
            cantCrash = false;
            outsideMap();
            return true;
        }
        if (Physics.Raycast(sphereRb.position,Vector3.down,out hit, rayLenght, drivableLayer))
        {
            return true;
        }
        else { return false; }
    }
    public void Gravity()
    {
        sphereRb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }

    public void CheckCollision()
    {
        if(speedCheck >= 130 && isdead == false)
        {
            isdead = true;
            SaveSpawnPoint.position = outsideMapSpawn.transform.position;
            SaveSpawnPoint.rotation = outsideMapSpawn.transform.rotation;
            DestroyBike();
        }
    }
    public void FlyCrash()
    {
        if(isdead == false)
        {
            isdead = true;
            SaveSpawnPoint.position = outsideMapSpawn.transform.position;
            SaveSpawnPoint.rotation = outsideMapSpawn.transform.rotation;
            DestroyBike();
        }
    }
    private void outsideMap()
    {
        if(isdead == false)
        {
            isdead = true;
            SaveSpawnPoint.position = outsideMapSpawn.transform.position;
            SaveSpawnPoint.rotation = outsideMapSpawn.transform.rotation;
            DestroyBike();
        }
    }

}
