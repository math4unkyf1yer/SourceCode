using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 70f; // How much speed to add
    public float boostDuration = 1f; // How long the boost lasts

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the bike has the "Player" tag
        {
            BikeController bike = GameObject.Find("Bike").GetComponent<BikeController>();
            if (bike != null)
            {
                bike.ActivateSpeedBoost(boostAmount, boostDuration);
            }
        }
    }
}
