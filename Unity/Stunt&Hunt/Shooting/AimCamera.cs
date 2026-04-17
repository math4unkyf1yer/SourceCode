using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class AimCamera : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform bike;
    public Transform playerBody;
    public Transform shoulderNotMoving;
    public float maxAngle = 90f;
    private float lastValidX;
    public Transform PlayerObj;
    public Rigidbody rb;
    public Transform combatLookAt;
    public CinemachineFreeLook freeLookCam;
    public CinemachineFreeLook freeLookCamMouse;

    private string lastControlScheme = "Mouse";
    private float inputCheckDelay = 1f;
    private float inputTimer = 0f;

    //mouse camera
    public GameObject thirdpersonMouse;
    //xbox Camera
    public GameObject thirdPersonXbox;

    public float rotationSpeed;
    public float lookSensitivity = 2f;

    public CameraStyle currentStyle;

    private Vector2 lookInput;
    private PlayerControlls controls;
    public enum CameraStyle
    {
        basic,
        Combat,
        TopDown
    }
    private void Awake()
    {
        controls = new PlayerControlls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        var mouseMoved = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f;
        var gamepadUsed = Gamepad.current != null && (
            Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.01f ||
            Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.01f ||
            Gamepad.current.buttonSouth.wasPressedThisFrame
        );

        if (mouseMoved)
        {
            SetCameraForMouse();
        }
        else if (gamepadUsed)
        {
            SetCameraForGamepad();
        }

        // Your existing camera style rotation logic can stay here...
    }
    private void SetCameraForMouse()
    {
        if (!thirdpersonMouse.activeSelf)
        {
            thirdpersonMouse.SetActive(true);
            thirdPersonXbox.SetActive(false);
            currentStyle = CameraStyle.Combat;
        }
    }

    private void SetCameraForGamepad()
    {
        if (!thirdPersonXbox.activeSelf)
        {
            thirdPersonXbox.SetActive(true);
            thirdpersonMouse.SetActive(false);
            currentStyle = CameraStyle.basic;
        }
    }

    private void FixedUpdate()
    {
        if (player == null || PlayerObj == null || orientation == null)
            return; // Skip update if references are missing

        if (currentStyle == CameraStyle.Combat)
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            Vector3 dirCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirCombatLookAt.normalized;
            PlayerObj.forward = dirCombatLookAt.normalized;
            playerBody.forward = dirCombatLookAt.normalized;
        }
        if (currentStyle == CameraStyle.basic)
        {

            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            Vector3 dirCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirCombatLookAt.normalized;
            PlayerObj.forward = dirCombatLookAt.normalized;
            playerBody.forward = dirCombatLookAt.normalized;
        }
    }

    private void LateUpdate()
    {
        if (bike == null)
            return;

        // Which camera are we using?
        CinemachineFreeLook activeCam = currentStyle == CameraStyle.Combat && thirdPersonXbox.activeSelf
            ? freeLookCam
            : freeLookCamMouse;

        if (activeCam == null)
            return;

        // Calculate current direction from bike to camera
        Vector3 camDir = bike.position - activeCam.State.FinalPosition;
        camDir.y = 0;
        camDir.Normalize();

        Vector3 bikeForward = bike.forward;
        bikeForward.y = 0;
        bikeForward.Normalize();

        float angle = Vector3.SignedAngle(bikeForward, camDir, Vector3.up);

        if (Mathf.Abs(angle) > maxAngle)
        {
            float clampedAngle = Mathf.Clamp(angle, -maxAngle, maxAngle);
            float delta = clampedAngle - angle;

            // Translate that angle correction into XAxis value correction
            activeCam.m_XAxis.Value += delta * Time.deltaTime * 5f; // smooth correction
        }
        else
        {
            lastValidX = activeCam.m_XAxis.Value; // store current if valid
        }

    }
}
