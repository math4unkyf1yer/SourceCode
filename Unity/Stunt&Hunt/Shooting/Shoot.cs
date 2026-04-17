using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;

public class Shoot : MonoBehaviour
{
    public LayerMask enemyLayer;
    public Transform cameraTransform;  // Reference to the player's camera
    public Transform gunStartPoint;    // Where the gun is located
    public GameObject head;
    public GameObject forearm;
    public Transform lookatHead;
    public Transform lookAtWeapon;
    public int gunRange = 100;         // Range of the gun
    public int bodyDamage = 1;
    public int headDamage = 2;
    public int amountOfBullet;
    private int amountOfBulletShown;
    private float sniperReload = 1f;
    private bool reloading;
    public bool canShoot = true;
    public LineRenderer shootLine;     // LineRenderer component to show the shot

    public TextMeshProUGUI bulletText;
    private Shotgun shotgun;
    private GunBroadCast gunBroadcastScript;

    public AudioClip sniperShoot;       //Audio for Sniper
    public AudioClip shotgunShoot;       //Audio for Shotgun
    public AudioClip gunEmpty;       //Audio for Shotgun
    public AudioClip gunReload;       //Audio for Shotgun
    public AudioSource gunSource;       //AudioSource of the gun

    //Particle Effects
    [SerializeField] ParticleSystem _flashPart; 
    [SerializeField] ParticleSystem _bulletPart; 

    //UI Animations
    [SerializeField] Animation _emptyAnim; 


    void Start () {
        shotgun = GetComponent<Shotgun>();
        amountOfBulletShown = amountOfBullet - 1;
        bulletText.text =  amountOfBullet.ToString();
        gunBroadcastScript = GetComponent<GunBroadCast>();

        //Define AudioSource
        gunSource = GetComponent<AudioSource>();

        //Set amount of bullets to 0
        amountOfBullet = 3;
        bulletText.text = amountOfBullet.ToString();

    }
    private void Update()
    {
       
        head.transform.LookAt(lookatHead);
        
        if (Input.GetMouseButtonDown(0) && reloading == false && amountOfBullet > 0 && canShoot == true || Input.GetAxis("RT") > 0.1f && reloading == false && amountOfBullet > 0 && canShoot == true)  // Example: Left mouse click to shoot
        {
            // bulletText.text = "0/" + amountOfBulletShown.ToString();
            bulletText.text = amountOfBullet.ToString();
            gunBroadcastScript.BroadcastGunshot(gameObject.transform.position);
            reloading = true;

            //Particle for bullet and gunfire
            _flashPart.Play();
            _bulletPart.Play();
            
            ShootRaycast();
            StartCoroutine(waitForReload());
        }
        else if (Input.GetMouseButtonDown(0) && reloading == false && amountOfBullet <= 0 && canShoot == true || Input.GetAxis("RT") > 0.1f && reloading == false && amountOfBullet <= 0 && canShoot == true)
        {
            //Gun is empty
            gunSource.clip = gunEmpty;
            gunSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
            gunSource.Play();

            _emptyAnim.Play("Bullet_Shake");
        }

        if (amountOfBullet < 0)
        {
            amountOfBullet = 0;
            amountOfBulletShown = 0;
        }
    }

    public void IncreaseAmmo(int bullet)
    {
        amountOfBullet += bullet;
        amountOfBulletShown = amountOfBullet - 1;
        // bulletText.text = "1/" + amountOfBulletShown.ToString();
        bulletText.text = amountOfBullet.ToString();

    }

    IEnumerator waitForReload()
    {
        yield return new WaitForSeconds(sniperReload);
        amountOfBullet -= 1;
        amountOfBulletShown = amountOfBullet - 1;
        bulletText.text = amountOfBullet.ToString();
        reloading = false;

        gunSource.clip = gunReload;
        gunSource.pitch = 1.0f;
        gunSource.Play();

    }
    public void waitForRealoadDead()
    {
       // amountOfBullet -= 1;
       // amountOfBulletShown = amountOfBullet - 1;
       // bulletText.text = "1/" + amountOfBullet.ToString();
        reloading = false;
    }

    public void ShootRaycast()
    {
        RaycastHit hit;

        // Start the ray from the camera and shoot forward
        Vector3 shootDirection = cameraTransform.forward;


        // Check if you're in first or third person view

        if(shotgun.isSniper == true && canShoot == true) {
            // Perform the raycast

            //Play Audio for Sniper
            gunSource.clip = sniperShoot;
            gunSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
            gunSource.Play();

            if (Physics.Raycast(gunStartPoint.position, shootDirection, out hit, gunRange, enemyLayer))
            {
                Debug.Log("Hit " + hit.collider.tag);

                // Show the line from the gun to the hit point
                shootLine.SetPosition(0, gunStartPoint.position);
                shootLine.SetPosition(1, hit.point); // Set the end point at the hit location

                

                if (hit.collider.tag == "Enemy")
                {
                    HitBox hitboxEnemy = hit.collider.gameObject.GetComponentInParent<HitBox>();
                    hitboxEnemy.TakeDamage(bodyDamage);
                }
                else if (hit.collider.tag == "EnemyHead")
                {
                    HitBox hitboxEnemy = hit.collider.gameObject.GetComponentInParent<HitBox>();
                    hitboxEnemy.TakeDamage(headDamage);
                }

            }
            else
            {
                // If the ray doesn't hit anything, set the line to end at max range
                shootLine.SetPosition(0, gunStartPoint.position);
                shootLine.SetPosition(1, gunStartPoint.position + shootDirection * gunRange);
            }

            // Activate the line for the shot duration
            shootLine.enabled = true;

            // Optionally, hide the line after a brief time (e.g., 0.1 seconds)
            StartCoroutine(HideShootLine());

        }
        else if (shotgun.isSniper == false) {
            
            if (Physics.Raycast(cameraTransform.position, shootDirection, out hit, gunRange, enemyLayer))
            {
                
                Debug.Log("Hit " + hit.collider.tag);

                // Show the line from the gun to the hit point
                shootLine.SetPosition(0, gunStartPoint.position);
                shootLine.SetPosition(1, hit.point); // Set the end point at the hit location


            }
            else
            {
                // If the ray doesn't hit anything, set the line to end at max range
                shootLine.SetPosition(0, cameraTransform.position);
                shootLine.SetPosition(1, cameraTransform.position + shootDirection * gunRange);
            }

            // Activate the line for the shot duration
            shootLine.enabled = true;

            // Optionally, hide the line after a brief time (e.g., 0.1 seconds)
            StartCoroutine(HideShootLine());
        }

    }

    private IEnumerator HideShootLine()//hide after afew sec
    {
        // Wait for 0.1 seconds before disabling the LineRenderer
        yield return new WaitForSeconds(0.5f);
        shootLine.enabled = false;
    }
    public void HideShootLineDead()
    {
        shootLine.enabled = false;
    }

    // Get number of bullets to show in pause menu
    public string GetNumBullets() {
        return bulletText.text;
    }
}
