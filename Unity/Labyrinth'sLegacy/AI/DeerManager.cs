using UnityEngine;

public class DeerManager : MonoBehaviour
{
    public GameObject deer;
    public TerGen terGenScript;
    public int spawnHeight = 3;

    private void Start()
    {
        terGenScript = GetComponent<TerGen>();
    }

    public void DeerDead()
    {
        terGenScript.EnemySpawn(deer, spawnHeight);
    }
}
