using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tp : MonoBehaviour
{
    public Transform positionB;
    public float rangex = 2f,rangez = 7f;
    public float maxRangeZ, minRangeZ, maxRangeX,minRangeX;
    private float x, z,y;
    public LayerMask layer;
    public bool verticalTp;
    public bool notRandom;
    public bool horizontalTp;
    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 6)
        {
            if (other.GetComponent<PlatformMove>())
            {
                PlatformMove platMove = other.GetComponent<PlatformMove>();

                if (platMove.platformRewinding)
                {
                    
                    return;
                }
            }
            x = other.transform.position.x;
            z = other.transform.position.z;
            y = other.transform.position.y;
            if (!verticalTp && !notRandom)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;

                // Generate random X and Z offsets
                float randomZ = Random.Range(minRangeZ,maxRangeZ);
                float randomX = Random.Range(minRangeX, maxRangeX);
                /*if (randomZ > maxRangeZ)
                {
                    randomZ = maxRangeZ;
                }
                else if (randomZ < minRangeZ)
                {
                    randomZ = minRangeZ;
                }*/

                other.gameObject.transform.position = new Vector3(randomX, positionB.position.y, randomZ);
            }
            else if(verticalTp)
            {
                other.gameObject.transform.position = new Vector3(positionB.position.x, positionB.position.y, positionB.position.z);
            }
            else
            {
                other.gameObject.transform.position = new Vector3(x, positionB.position.y, z);
            }
        }
    }
}
