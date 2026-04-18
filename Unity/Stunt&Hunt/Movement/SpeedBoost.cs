using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 70f;
    public float boostDuration = 1f;
    private BikeController bike;

    private void Start()
    {
        GameObject bikeObj = GameObject.Find("Bike");
        if (bikeObj != null)
            bike = bikeObj.GetComponent<BikeController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bike != null)
            bike.ActivateSpeedBoost(boostAmount, boostDuration);
    }
}
