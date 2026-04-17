using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public List<GameObject> walls = new List<GameObject>();
    // Start is called before the first frame update
    

    public void HideAtIndex(int index)
    {
        if (walls == null || walls.Count == 0)
            return;

        // Hide current
        HideObject(index);

        // Hide previous
        HideObject(index - 1);

        // Hide next
        HideObject(index + 1);
    }

    void HideObject(int index)
    {
        if (index >= 0 && index < walls.Count && walls[index] != null)
        {
            walls[index].SetActive(false);
        }
    }
}
