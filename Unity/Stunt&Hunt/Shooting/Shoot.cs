using System.Collections;
using UnityEngine;
using TMPro;

public class Shoot : MonoBehaviour
{
    public LayerMask enemyLayer;
    public Transform cameraTransform;
    public Transform gunStartPoint;
    public GameObject head;
    public GameObject forearm;
    public Transform lookatHead;
    public Transform lookAtWeapon;
    public int gunRange = 100;
    public int bodyDamage = 1;
    public int headDamage = 2;
    public int amountOfBullet;
    public bool isSniper = true;
    private float sniperReload = 1f;
    private bool reloading;
    public bool canShoot = true;
    public LineRenderer shootLine;

    public TextMeshProUGUI bulletText;
    private GunBroadCast gunBroadcastScript;

    public AudioClip sniperShoot;
    public AudioClip shotgunShoot;
    public AudioClip gunEmpty;
    public AudioClip gunReload;
    public AudioSource gunSource;

    [SerializeField] ParticleSystem _flashPart;
    [SerializeField] ParticleSystem _bulletPart;
    [SerializeField] Animation _emptyAnim;

    void Start()
    {
        amountOfBullet = 3;
        bulletText.text = amountOfBullet.ToString();
        gunBroadcastScript = GetComponent<GunBroadCast>();
        gunSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        head.transform.LookAt(lookatHead);

        bool firePressed = Input.GetMouseButtonDown(0) || Input.GetAxis("RT") > 0.1f;

        if (firePressed && !reloading && canShoot)
        {
            if (amountOfBullet > 0)
            {
                bulletText.text = amountOfBullet.ToString();
                gunBroadcastScript.BroadcastGunshot(gameObject.transform.position);
                reloading = true;
                _flashPart.Play();
                _bulletPart.Play();
                ShootRaycast();
                StartCoroutine(waitForReload());
            }
            else
            {
                gunSource.clip = gunEmpty;
                gunSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
                gunSource.Play();
                _emptyAnim.Play("Bullet_Shake");
            }
        }

        if (amountOfBullet < 0)
            amountOfBullet = 0;
    }

    public void IncreaseAmmo(int bullet)
    {
        amountOfBullet += bullet;
        bulletText.text = amountOfBullet.ToString();
    }

    IEnumerator waitForReload()
    {
        yield return new WaitForSeconds(sniperReload);
        amountOfBullet -= 1;
        bulletText.text = amountOfBullet.ToString();
        reloading = false;
        gunSource.clip = gunReload;
        gunSource.pitch = 1.0f;
        gunSource.Play();
    }

    public void waitForReloadDead()
    {
        reloading = false;
    }

    public void ShootRaycast()
    {
        RaycastHit hit;
        Vector3 shootDirection = cameraTransform.forward;
        Vector3 rayOrigin = isSniper ? gunStartPoint.position : cameraTransform.position;

        if (isSniper)
        {
            gunSource.clip = sniperShoot;
            gunSource.pitch = UnityEngine.Random.Range(0.8f, 1.3f);
            gunSource.Play();
        }

        if (Physics.Raycast(rayOrigin, shootDirection, out hit, gunRange, enemyLayer))
        {
            shootLine.SetPosition(0, gunStartPoint.position);
            shootLine.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Enemy"))
            {
                HitBox hitboxEnemy = hit.collider.gameObject.GetComponentInParent<HitBox>();
                hitboxEnemy.TakeDamage(bodyDamage);
            }
            else if (hit.collider.CompareTag("EnemyHead"))
            {
                HitBox hitboxEnemy = hit.collider.gameObject.GetComponentInParent<HitBox>();
                hitboxEnemy.TakeDamage(headDamage);
            }
        }
        else
        {
            shootLine.SetPosition(0, gunStartPoint.position);
            shootLine.SetPosition(1, gunStartPoint.position + shootDirection * gunRange);
        }

        shootLine.enabled = true;
        StartCoroutine(HideShootLine());
    }

    private IEnumerator HideShootLine()
    {
        yield return new WaitForSeconds(0.5f);
        shootLine.enabled = false;
    }

    public void HideShootLineDead()
    {
        shootLine.enabled = false;
    }

    public string GetNumBullets()
    {
        return bulletText.text;
    }
}
