using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public Transform cameraTransform;   // Main camera
    public Transform armBone;            // Arm or hand bone to rotate
    public Transform powerArmBone;

    public float minPitch = -45f;
    public float maxPitch = 45f;

    private Quaternion initialLocalRotation;
    private Quaternion initialLocalRotationPArm;

    private Selecting powerScript;

    void Start()
    {
        initialLocalRotation = armBone.localRotation;
        initialLocalRotationPArm = powerArmBone.localRotation;
        powerScript = GameObject.Find("Selecting").GetComponent<Selecting>();
    }

    void LateUpdate()
    {
        if (powerScript.isGrabbing)
        {
            Vector3 cameraEuler = cameraTransform.localEulerAngles;
            float pitch = cameraEuler.x;
            if (pitch > 180f) pitch -= 360f;
            float clampedPitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Create a rotation around X axis for pitch
            Quaternion pitchRotation = Quaternion.Euler(0, -clampedPitch, 0f);

            // Apply pitch rotation as additive to the animated rotation
            armBone.localRotation = initialLocalRotation * pitchRotation;
        }
        if (powerScript.isGrayscale)
        {
            Vector3 cameraEuler = cameraTransform.localEulerAngles;
            float pitch = cameraEuler.x;
            if (pitch > 180f) pitch -= 360f;
            float clampedPitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Create a rotation around X axis for pitch
            Quaternion pitchRotation = Quaternion.Euler(0f, clampedPitch -8f, 0f);

            // Apply pitch rotation as additive to the animated rotation
            powerArmBone.localRotation = initialLocalRotationPArm * pitchRotation;
        }
    }
}
