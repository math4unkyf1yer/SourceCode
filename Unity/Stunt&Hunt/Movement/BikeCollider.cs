using UnityEngine;

public class BikeCollider : MonoBehaviour
{
    public BikeController bikeControllerScript;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            if (bikeControllerScript.inAir)
            {
                bikeControllerScript.FlyCrash();
                return;
            }
            bikeControllerScript.CheckCollision();
        }
        if (collision.gameObject.CompareTag("Floor"))
        {
            bikeControllerScript.FlyCrash();
        }
    }
}
