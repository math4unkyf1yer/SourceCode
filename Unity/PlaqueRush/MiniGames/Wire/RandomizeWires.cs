using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeWires : MonoBehaviour
{
    // wire start
    public GameObject[] wireStart;
    //wire ends
    public GameObject[] wireEnd;
    //wire start position
    public Transform[] wirePosStart;
    //wire end position
    public Transform[] wirePosEnd;

    private void Start()
    {
        //randomize wire start and end 
        ShuffleArrays(wireStart);
        ShuffleArrays(wireEnd);

        // Move wires to shuffled positions
        PlaceWires(wireStart, wirePosStart);
        PlaceWires(wireEnd, wirePosEnd);
    }

    void ShuffleArrays(GameObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            //swapping
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void PlaceWires(GameObject[] wires, Transform[] positions)
    {
        for (int i = 0; i < wires.Length && i < positions.Length; i++)
        {
            wires[i].transform.position = positions[i].position;
            wires[i].transform.rotation = positions[i].rotation; // optional: keep orientation
        }
    }


}
