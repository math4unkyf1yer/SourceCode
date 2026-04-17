using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarManager : MonoBehaviour
{
    public GameObject Boar;
    public TerGen terGenScript;
    public int spawnHeight = 3;

    private void Start()
    {
        terGenScript = this.GetComponent<TerGen>();
    }
    //once dies activate this 
    public void BoarDead()//spawn the boar after death 
    {
        terGenScript.EnemySpawn(Boar,spawnHeight);
    }
}
