using UnityEngine;

public class GobberManager : MonoBehaviour
{
    public GameObject Gobber;
    public TerGen terGenScript;
    public int spawnHeight = 3;

    private void Start()
    {
        terGenScript = GetComponent<TerGen>();
    }

    public void GobberDead()
    {
        terGenScript.EnemySpawn(Gobber, spawnHeight);
    }
}
