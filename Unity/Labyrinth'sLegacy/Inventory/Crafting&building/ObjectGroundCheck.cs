using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundCheck : MonoBehaviour
{
    public LayerMask groundLayer; // Layer that represents the ground
    private bool[] panelTouchingGround = new bool[4]; // Track each panel's ground contact

    public Transform[] panels;  // References to the 4 panel transforms (or their colliders)
    public bool isWallOrFloor;
    public bool isStairs;

    [Header("IfDestroyObject")]
    public GameObject[] drops;
    public int[] amount;

    // This method will check if all panels are touching the ground
    public bool AllPanelsTouchingGround()
    {
        int panelsTouchingGround = 0;

        // Loop through all panels and count how many are touching the ground
        for (int i = 0; i < panels.Length; i++)
        {
            if (IsTouchingGround(panels[i]))
            {
                panelsTouchingGround++;
            }
        }

        // If it's a wall or floor, only 2 out of 4 panels need to touch the ground
        if (isWallOrFloor || isStairs)
        {
            return panelsTouchingGround >= 2;
        }
        // Otherwise, all 4 panels need to touch the ground
        else
        {
            return panelsTouchingGround == panels.Length;
        }
    }


    // This method checks if a specific panel is touching the ground
    private bool IsTouchingGround(Transform panel)
    {
        Collider[] hitColliders = Physics.OverlapBox(panel.position, panel.localScale / 2, Quaternion.identity, groundLayer);
        
        return hitColliders.Length > 0;
    }
}
