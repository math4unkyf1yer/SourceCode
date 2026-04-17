using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeCollider : MonoBehaviour
{
    public BikeController bikeControllerScript;

    private void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            if (bikeControllerScript.inAir)
            {
                bikeControllerScript.FlyCrash();
                return;
            }
            Debug.Log(collision.gameObject.tag);
            bikeControllerScript.CheckCollision();
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            bikeControllerScript.FlyCrash();
        }
    }
}
