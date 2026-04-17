using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform playerpos;

    float xRotation;
    float yRotation;
    public bool cameraMove;
    public bool menuOpen;
    public GameObject player;

    private Selecting powerScript;
    // Start is called before the first frame update
    void Start()
    {
        powerScript = GameObject.Find("Selecting").GetComponent<Selecting>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null && !menuOpen)
        {
            // Mouse input
            float mouseX = Input.GetAxis("Mouse X") * sensX * Time.unscaledDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.unscaledDeltaTime;

            // Controller input (Right Stick)
            //    float controllerX = Input.GetAxisRaw("Right Stick Horizontal") * sensX * Time.deltaTime;
            //     float controllerY = Input.GetAxisRaw("Right Stick Vertical") * sensY * Time.deltaTime;


            // Combine inputs (prioritize controller if active)
            //   float finalX = (Mathf.Abs(controllerX) > 0.1f) ? controllerX : mouseX;
            //    float finalY = (Mathf.Abs(controllerY) > 0.1f) ? controllerY : mouseY;

            // Apply rotations
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            Quaternion targetRotation = Quaternion.Euler(xRotation, yRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * 15f);

            // Rotate Player (left/right)
            playerpos.rotation = Quaternion.Euler(0, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

    }
}
