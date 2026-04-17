using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GobberManager : MonoBehaviour
{
    public GameObject Gobber;
    public TerGen terGenScript;
    public int spawnHeight = 3;

    private void Start()
    {
        terGenScript = this.GetComponent<TerGen>();
    }
    //once dies activate this 
    public void GobberDead()//spawn the boar after death 
    {
        terGenScript.EnemySpawn(Gobber, spawnHeight);
    }
}
