using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
    public GameObject directionalLight;
    public GameObject cameras;
    public GameObject menuTerrain;
    public GameObject canvas;
    public GameObject start;
    public GameObject player;
    public GameObject loadScreen;
    public GameObject eventSystem;
    public GameObject sceneManager;
    public void OnClick()
    {
        SceneManager.LoadScene(0);
        loadScreen.SetActive(true);
        StartCoroutine(waitAbit());
    }

    IEnumerator waitAbit()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(directionalLight);
        Destroy(menuTerrain);
        Destroy(start);
        Destroy(player);
        Destroy(cameras);
        Destroy(canvas);
        Destroy(sceneManager);
        Destroy(eventSystem);
    }
}
