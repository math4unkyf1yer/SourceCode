using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct TransformSnapshot
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformSnapshot(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}
public class PositionTracker : MonoBehaviour
{
    private Queue<TransformSnapshot> transformHistory = new Queue<TransformSnapshot>();
    // private Queue<Quaternion> rotationHistory - new 
    public float recordInterval = 0.1f;
    private float timer;


    public List<TransformSnapshot> GetFullHistory()
    {
        return new List<TransformSnapshot>(transformHistory);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= recordInterval)
        {
            timer = 0f;
            transformHistory.Enqueue(new TransformSnapshot(transform.position, transform.rotation));

            // Keep only 8 seconds of history (5 / 0.1 = 100 frames)
            if (transformHistory.Count > 80)
            {
                transformHistory.Dequeue();
            }
        }
    }
    public Vector3 GetPositionFiveSecondsAgo()
    {
        if (transformHistory.Count > 0)
            return transformHistory.Peek().position;
        else
            return transform.position;
    }

    public Quaternion GetRotationFiveSecondsAgo()
    {
        if (transformHistory.Count > 0)
            return transformHistory.Peek().rotation;
        else
            return transform.rotation;
    }
}

