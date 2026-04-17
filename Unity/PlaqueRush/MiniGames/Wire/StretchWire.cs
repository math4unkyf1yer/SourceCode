using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchWire : MonoBehaviour
{
    public RectTransform startPoint;      // where the wire starts
    public RectTransform targetImage;     // the draggable image

    private RectTransform rectTransform; // its transform


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Direction from start to target
        Vector2 dir = targetImage.anchoredPosition - startPoint.anchoredPosition;
        float distance = dir.magnitude;

        // Stretch the wire along X
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);

        // Position the wire at the start point
        rectTransform.anchoredPosition = startPoint.anchoredPosition;

        // Rotate the wire to point toward the target
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }


}
