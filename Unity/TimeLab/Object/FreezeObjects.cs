using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeObjects : MonoBehaviour
{
    private Rigidbody rb;
    private RigidbodyConstraints originalConstrain;
    private Vector3 storedVelocity;
    private Vector3 storeAngularVelocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FreezeObj()
    {
        if (rb != null)
        {
            storedVelocity = rb.velocity;
            storeAngularVelocity = rb.angularVelocity;
            originalConstrain = rb.constraints;

            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    public void UnFreezeObj()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = storedVelocity;
            rb.angularVelocity = storeAngularVelocity;
        }
    }
}
