using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCheck : MonoBehaviour
{
    private Rigidbody rb;
    private PlatformMove platmoveScript;
    public Collider colliders;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
        colliders = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        platmoveScript = GetComponent<PlatformMove>();
    }
    private void OnTriggerEnter(Collider other)
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(rb != null && !platmoveScript.platformRewinding)
        {
            if(rb.velocity.magnitude > 20)
            {
                rb.velocity = rb.velocity.normalized * 20f;
                
            }
        }

        if(gameObject.transform.position.y < -120)
        {
            transform.position = startPosition;
            rb.velocity = Vector3.zero;
        }
    }

    public void TurnOffCollider()
    {

    }
    public void TurnOnCollider()
    {

    }
}
