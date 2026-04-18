using UnityEngine;

public class BoarManager : MonoBehaviour
{
    public GameObject Boar;
    public TerGen terGenScript;
    public int spawnHeight = 3;

    private void Start()
    {
        terGenScript = GetComponent<TerGen>();
    }

    public void BoarDead()
    {
        terGenScript.EnemySpawn(Boar, spawnHeight);
    }
}
