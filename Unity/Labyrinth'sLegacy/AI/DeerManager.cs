using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerManager : MonoBehaviour
{
    public GameObject deer;
    public TerGen terGenScript;
    public int spawnHeight = 3;
    private void Start()
    {
        terGenScript = this.GetComponent<TerGen>();
    }
    //once dies activate this 
    public void DeerDead()//spawn the boar after death 
    {
        terGenScript.EnemySpawn(deer, spawnHeight);
    }
}
