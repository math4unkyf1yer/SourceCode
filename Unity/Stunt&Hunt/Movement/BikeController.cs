using System.Collections;
using UnityEngine;
using TMPro;

public class BikeController : MonoBehaviour
{
    RaycastHit hit;
    float moveInput, steerInput, rayLength, rayLengthUp, currentVelocityOffset;
    private TextMeshProUGUI speedText;
    public float maxSpeed, acceleration, steerStrenght, tiltAngle, gravity, bikeTiltIncrement = 0.09f, zTitlAngle = 45, hadleRotVal = 30f, handleRotSpeed = .15f;
    public float airReturnSpeed = 2f;
    [Range(1, 10)]
    public float brakeFactor;
    public float speedCheck;
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

    private bool isBoosting = false;
    private float originalMaxSpeed;

    private bool isflipping;
    private bool isStalling = false;
    public bool canstall;

    public bool cantCrash;
    private bool isRespawning = false;

    private Score scoreScript;
    private Shoot shootScript;
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

    void Start()
    {
        playerAnimation.enabled = false;
        cantCrash = true;
        initialHandleLocalRotation = handle.transform.localRotation;

        if (sphereRb != null) sphereRb.transform.parent = null;
        if (bikeBody != null) bikeBody.transform.parent = null;

        sphereRb.drag = 0.1f;
        sphereRb.angularDrag = 1f;

        SaveSpawnPoint = GameObject.Find("SaveSpawnPoint")?.transform;

        GameObject speedTextObj = GameObject.Find("SpeedText");
        if (speedTextObj != null)
            speedText = speedTextObj.GetComponent<TextMeshProUGUI>();

        GameObject gun = GameObject.Find("Ak47Holder");
        if (gun != null)
            shootScript = gun.GetComponent<Shoot>();

        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
            scoreScript = gameManager.GetComponent<Score>();

        SphereCollider sc = sphereRb.GetComponent<SphereCollider>();
        rayLength = sc.radius + 4f;
        rayLengthUp = sc.radius + 44f;
        playerObjMaterial = playerObj.GetComponent<Renderer>();

        playerObjMaterial.material = newMat;
        StartCoroutine(Immortal());
    }

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
            PerformBackflip();

        if (Physics.Raycast(sphereRb.position, Vector3.down, out _, rayLength + 2, deadLayer))
            TriggerCrash();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        bool grounded = Grounded();
        inAir = !grounded && !isStalling;

        if (grounded && !isStalling)
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
        float currentSpeed = sphereRb.velocity.magnitude;
        float targetSpeed = maxSpeed;
        float accelerationRate = acceleration * Time.fixedDeltaTime;

        if (moveInput < 0)
        {
            targetSpeed = 0;
            accelerationRate *= brakeFactor;
        }

        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate);

        if (OnSlope())
        {
            sphereRb.AddForce(transform.forward * acceleration * 20f, ForceMode.Acceleration);
        }
        else
        {
            sphereRb.velocity = transform.forward * newSpeed;
        }

        if (newSpeed <= 40f && !cantCrash && !canstall)
            Stalling();
    }

    bool OnSlope()
    {
        if (Grounded())
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            return slopeAngle > 5f && slopeAngle < 50f;
        }
        return false;
    }

    void Stalling()
    {
        if (isStalling) return;

        isStalling = true;
        shootScript.canShoot = false;

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
        if (cantCrash) return;

        shootScript.canShoot = false;
        shootScript.HideShootLineDead();
        shootScript.waitForReloadDead();
        crashEffect.SetActive(true);
        bikeBody.gameObject.SetActive(false);
        crashAudio.Play();
        StartCoroutine(SpawnBack());
    }

    IEnumerator SpawnBack()
    {
        if (isRespawning) yield break;
        isRespawning = true;

        playerAnimation.SetTrigger("Falling");
        yield return new WaitForSeconds(2f);
        playerObjMaterial.material = newMat;
        playerAnimation.enabled = false;

        cantCrash = true;
        bikeBody.transform.position = SaveSpawnPoint.position;
        bikeBody.transform.rotation = SaveSpawnPoint.rotation;

        sphereRb.transform.position = SaveSpawnPoint.position;
        sphereRb.transform.rotation = SaveSpawnPoint.rotation;

        sphereRb.velocity = Vector3.zero;
        sphereRb.angularVelocity = Vector3.zero;

        bikeBody.velocity = Vector3.zero;
        bikeBody.angularVelocity = Vector3.zero;

        handle.transform.localRotation = Quaternion.identity;

        bikeBody.gameObject.SetActive(true);
        crashEffect.SetActive(false);

        isStalling = false;
        isdead = false;
        isRespawning = false;
        shootScript.canShoot = true;
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
        float turnAmount = steerInput * steerStrenght * Time.fixedDeltaTime;
        transform.Rotate(0, turnAmount, 0, Space.World);

        Quaternion steer = Quaternion.Euler(0f, 0f, hadleRotVal * steerInput);
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
        float zRot = -zTitlAngle * steerInput;

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
        float xRot = bikeBody.transform.eulerAngles.x;
        float zRot = Mathf.LerpAngle(bikeBody.transform.eulerAngles.z, 0f, Time.fixedDeltaTime * 3f);

        Quaternion newRotation = Quaternion.Euler(xRot, transform.eulerAngles.y, zRot);
        bikeBody.MoveRotation(newRotation);
    }

    public void ActivateSpeedBoost(float boostAmount, float duration)
    {
        if (!isBoosting)
        {
            isBoosting = true;
            originalMaxSpeed = maxSpeed;
            maxSpeed += boostAmount;
            StartCoroutine(GradualSpeedBoost(boostAmount, duration));
        }
    }

    private IEnumerator GradualSpeedBoost(float boostAmount, float duration)
    {
        float elapsedTime = 0f;
        float initialSpeed = sphereRb.velocity.magnitude;
        float targetSpeed = initialSpeed + boostAmount;

        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newSpeed = Mathf.Lerp(initialSpeed, targetSpeed, elapsedTime / (duration / 2));
            sphereRb.velocity = transform.forward * newSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(duration / 2);

        elapsedTime = 0f;
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
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && !isflipping)
            StartCoroutine(trickRoutine());
    }

    IEnumerator trickRoutine()
    {
        isflipping = true;
        shootScript.canShoot = false;
        playerAnimation.enabled = true;
        playerAnimation.SetTrigger("Trick");

        yield return new WaitForSeconds(.8f);

        if (scoreScript != null)
            scoreScript.score += 5;

        playerAnimation.enabled = false;
        isflipping = false;
        shootScript.IncreaseAmmo(1);
        trickAudio.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
        trickAudio.Play();
        shootScript.canShoot = true;
    }

    public bool CanFlip()
    {
        return !Physics.Raycast(sphereRb.position, Vector3.down, out _, rayLengthUp, drivableLayer);
    }

    public bool Grounded()
    {
        return Physics.Raycast(sphereRb.position, Vector3.down, out hit, rayLength, drivableLayer);
    }

    public void Gravity()
    {
        sphereRb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }

    private void TriggerCrash()
    {
        if (isdead) return;
        isdead = true;
        SaveSpawnPoint.position = outsideMapSpawn.transform.position;
        SaveSpawnPoint.rotation = outsideMapSpawn.transform.rotation;
        DestroyBike();
    }

    public void CheckCollision()
    {
        if (speedCheck >= 130)
            TriggerCrash();
    }

    public void FlyCrash()
    {
        TriggerCrash();
    }
}
