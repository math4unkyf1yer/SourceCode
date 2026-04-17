using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopScript : MonoBehaviour
{
    public float speedPoop = 20f;
    private Vector3 oldPlayerPs;
    public int BlundgeDamage = 7;
    private Stats statsScript;
    private void Start()
    {
        GameObject player =GameObject.Find("Player");

        statsScript = player.GetComponent<Stats>();
        // Apply an initial force towards the player
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (statsScript.transform.position - transform.position).normalized;
            rb.AddForce(direction * speedPoop, ForceMode.Impulse);
        }
    }
    

    // Method to set the target position after instantiation
    public void SetTargetPosition(Vector3 position)
    {
        oldPlayerPs = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerObj")
        {
            //Take damage
            statsScript.TakebleugDamage(BlundgeDamage);
            Destroy(gameObject);
        }
        else if(other.gameObject.name != "SceneCollider" && other.gameObject.name != "Gobbers(Clone)")
        {
            Destroy(gameObject);
        }      
    }
}
