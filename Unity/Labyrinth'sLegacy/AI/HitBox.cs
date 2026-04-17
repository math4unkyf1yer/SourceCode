using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public string Name;
    public GameObject[] objectDrop;
    public GameObject rareObjectDrop;
    public int[] amountDrop;
    public int[] max;
    public int[] min;
    public int health;
    public int knockBackDef;
    private int knockBackOg;
    public int strenght;
    public float deathTime;
    public bool dieByFallDamage;
    GameObject objectT;
    //need to add resistence
    public int slashResistence;
    public int blugResistence;
    public int pierceResistence;
    TerGen tergenChosen;
    BoarController boar;
    DeerController deer;
    GobberController gobber;
    public GameObject knockEffect;
    public GameObject deadEffect;
    public GameObject bleedEffect;
    public GameObject enemyBody;

    public AudioClip destroyAudio;
    public AudioClip hitAudios;
    AudioSource hitAudioClone;


    IEnumerator StopBleeding()
    {
        yield return new WaitForSeconds(0.5f);
        bleedEffect.SetActive(false);
    }
    public void SlashDamage(int slashDamage, int knockbackdam)
    {
        DamageAudio();
        if (bleedEffect != null)
        {
            bleedEffect.SetActive(true);
            StartCoroutine(StopBleeding());
        }
        slashDamage = slashDamage - slashResistence;
        if (slashDamage <= 0)
            slashDamage = 1;
        health -= slashDamage;
        knockBackOg = knockBackDef;
        knockBackDef -= knockbackdam;
        if (health <= 0)
        {
            StartCoroutine(Destroyg());
        }
    }
    public void PierceDamage(int pierceDamage,int knockbackdam)
    {
        DamageAudio();
        if (bleedEffect != null)
        {
            bleedEffect.SetActive(true);
            StartCoroutine(StopBleeding());
        }
        pierceDamage = pierceDamage - pierceResistence;
        if (pierceDamage <= 0)
            pierceDamage = 1;
        health -= pierceDamage;
        knockBackOg = knockBackDef;
        knockBackDef -= knockbackdam;
        if (health <= 0)
        {
            StartCoroutine(Destroyg());
        }
    }
    public void BludDamge(int blugDamage,int knockbackdam)
    {
        DamageAudio();
        if (bleedEffect != null)
        {
            bleedEffect.SetActive(true);
            StartCoroutine(StopBleeding());
        }
        blugDamage = blugDamage - blugResistence;
        if (blugDamage <= 0)
            blugDamage = 1;
        health -= blugDamage;
        knockBackOg = knockBackDef;
        knockBackDef -= knockbackdam;
        if(knockBackDef <= 0)
        {
            Debug.Log("Broke");
            BrokeDef();
        }
        if (health <= 0)
        {
            StartCoroutine(Destroyg());
        }
    }

    void DamageAudio()
    {
        AudioSource hitAudio = GetComponent<AudioSource>();
        hitAudioClone = hitAudio;
        hitAudio.clip = hitAudios;
        if(hitAudio != null)
        {
            hitAudio.Play();
            
        }
    }
    
    //Add knock Back
    void BrokeDef()
    {
        if(name == "Boar(Clone)")
        {
            boar = GetComponent<BoarController>();
            boar.enabled = false;
            StartCoroutine(defBack(name));
        }else if(name == "Deer(Clone)")
        {
            deer = GetComponent<DeerController>();
            deer.enabled = false;
            StartCoroutine(defBack(name));
        }
        else if(name == "Gobbers(Clone)")
        {
            gobber = GetComponent<GobberController>();
            gobber.enabled = false;
            StartCoroutine(defBack(name));
        }
    }
    IEnumerator defBack(string name)
    {
        knockEffect.gameObject.SetActive(true);
        //knock back Effect
        yield return new WaitForSeconds(1.8f);
        knockEffect.gameObject.SetActive(false);
        if (name == "Boar(Clone)")
            boar.enabled = true;
        if(name == "Deer(Clone)")
            deer.enabled = true;
        if(name == "Gobbers(Clone)")
            gobber.enabled = true;
        knockBackDef = knockBackOg;
    }
     IEnumerator Destroyg()
    {
        if (enemyBody != null)
        {
            enemyBody.gameObject.SetActive(false);
        }
        objectT = this.gameObject;
        Rigidbody rb = objectT.GetComponent<Rigidbody>();
        BoarController bScript = objectT.GetComponent<BoarController>();
        if(bScript != null)
        {
            bScript.boarStop = true;
            //spawn another boar
        }
        rb.isKinematic = false;
        rb.useGravity = true;
        //dead effect
        if(deadEffect != null)
            deadEffect.gameObject.SetActive(true);

        //audio
        hitAudioClone.clip = destroyAudio;
        hitAudioClone.Play();
        yield return new WaitForSeconds(deathTime);
        Dead(objectT);
    }
    public void Dead(GameObject objectT)
    {
        GameObject player = GameObject.Find("Player");
        Stats statsScript = player.GetComponent<Stats>();
        Tutorial tutoScript = player.GetComponent<Tutorial>();
        if(tutoScript.currentStep == 11 && gameObject.name == "Boar(Clone)" && !dieByFallDamage)
        {
            tutoScript.currentStep++;
        }
        if (tutoScript.currentStep == 12 && gameObject.name == "Deer(Clone)" && !dieByFallDamage)
        {
            tutoScript.currentStep++;
        }
        if (tutoScript.currentStep == 13 && gameObject.name == "Gobbers(Clone)" && !dieByFallDamage)
        {
            tutoScript.currentStep++;
        }
        if (statsScript.fighting)
        {
            statsScript.fighting = false;
        }
        if (!dieByFallDamage)
        {
            for (int j = 0; j < objectDrop.Length; j++)
            {
                amountDrop[j] = Random.Range(min[j], max[j]);
                for (int i = 0; i < amountDrop[j]; i++)
                {
                    // Instantiate the object drop
                    GameObject objectDropClone = Instantiate(objectDrop[j]);

                    // Set a small random offset to prevent overlap
                    Vector3 randomOffset = new Vector3(
                        Random.Range(-1f, 1f),  // Small random offset in X
                        2,                        // Slight upward offset in Y
                        Random.Range(-1f, 1f));  // Small random offset in Z

                    // Set the position to the object's current position plus the offset
                    objectDropClone.transform.position = this.transform.position + randomOffset;

                    // Set the parent if necessary
                    Transform closestParent = FindParent();
                    objectDropClone.transform.SetParent(closestParent);
                }
            }
        }

        GetTerGen();
        EnemyCheck();
        // Destroy the original object
        Destroy(objectT);
    }

    void EnemyCheck()
    {
        if (this.gameObject.name == "Boar(Clone)")
        {
            BoarController boarControllerScript = gameObject.GetComponent<BoarController>();
            // Destroy the patrol points (startPositions)
            foreach (Transform patrolPoint in boarControllerScript.startPositions)
            {
                if (patrolPoint != null)
                {
                    Destroy(patrolPoint.gameObject);
                }
            }
            BoarManager boarManagerScript = tergenChosen.GetComponent<BoarManager>();
            boarManagerScript.BoarDead();
        }
        if (this.gameObject.name == "Deer(Clone)")
        {
            DeerController deerControllerScript = gameObject.GetComponent<DeerController>();
            // Destroy the patrol points (startPositions)
            foreach (Transform patrolPoint in deerControllerScript.startPositions)
            {
                if (patrolPoint != null)
                {
                    Destroy(patrolPoint.gameObject);
                }
            }
            DeerManager deerScriptManager = tergenChosen.GetComponent<DeerManager>();
            deerScriptManager.DeerDead();
        }
        if (this.gameObject.name == "Gobbers(Clone)")
        {
            GobberController gobberControllerScript = gameObject.GetComponent<GobberController>();
            // Destroy the patrol points (startPositions)
            foreach (Transform patrolPoint in gobberControllerScript.startPositions)
            {
                if (patrolPoint != null)
                {
                    Destroy(patrolPoint.gameObject);
                }
            }
            GobberManager GobberManagerScript = tergenChosen.GetComponent<GobberManager>();
            GobberManagerScript.GobberDead();
        }
    }
    private void GetTerGen()
    {
        float closestDistance = float.MaxValue;
        TerGen[] terGens = FindObjectsOfType<TerGen>();
        foreach (TerGen terGen in terGens)
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, terGen.parent.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                tergenChosen = terGen;
            }
        }
    }
    private Transform FindParent()
    {
        Transform closestParent = null;
        float closestDistance = float.MaxValue;
        TerGen[] terGens = FindObjectsOfType<TerGen>();
        foreach (TerGen terGen in terGens)
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, terGen.parent.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestParent = terGen.parent;
            }
        }
        return closestParent;
    }
}
