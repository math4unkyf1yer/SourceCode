using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GunBroadCast : MonoBehaviour
{
    public static event Action<Vector3> OnGunshotFired;

    public  void BroadcastGunshot(Vector3 position)
    {
        OnGunshotFired?.Invoke(position);
    }
}
