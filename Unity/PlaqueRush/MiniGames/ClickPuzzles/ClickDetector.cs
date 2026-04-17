using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    private Camera camera1;
    // Start is called before the first frame update
    void Start()
    {
        camera1 = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            HandleClick(Input.mousePosition);
        }

        // --- Android (Touch) ---
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClick(Input.GetTouch(0).position);
        }
    }

    void HandleClick(Vector3 screenPosition)
    {
        Ray ray = camera1.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Clicked on: " + hit.collider.gameObject.name);

            // Example: if object has a custom script
            ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
            if (clickable != null)
            {
                clickable.OnClicked();
            }
        }
    }
}
