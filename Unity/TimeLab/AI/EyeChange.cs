using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeChange : MonoBehaviour
{
    public GameObject[] eyes;
    public bool eyeIdle = true;
    public bool talking;
    public GameObject eyeHolder;
    public Transform camera2;
    public float viewDistance = 20f;

    private LineRenderer laserLine;

    private void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        InvokeRepeating("EyeIdle", 0, 5f);
    }

    void EyeIdle()
    {
        if (eyeIdle)
        {
            if (eyes[0].activeSelf)
            {
                eyes[1].SetActive(true);
                eyes[0].SetActive(false);

            }
            else
            {
                eyes[0].SetActive(true);
                eyes[1].SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (talking)
        {
            eyeIdle = false;
            eyes[3].SetActive(true);
            eyes[1].SetActive(false);
            eyes[0].SetActive(false);
        }
        else if (!talking && !eyeIdle)
        {
            eyeIdle = true;
            eyes[3].SetActive(false);
            eyes[0].SetActive(true);
        }
    }
    public void ShootingEye()
    {
        eyeIdle = false;
        talking = false;
        eyes[3].SetActive(false);
        eyes[2].SetActive(true);
    }
    public void Shooting()
    {
        if (laserLine != null)
        {
            laserLine.enabled = true;

            laserLine.SetPosition(0, eyeHolder.transform.position);
            laserLine.SetPosition(1, camera2.transform.position);
            Debug.Log("suppose to be shooting");
        }
    }
}
