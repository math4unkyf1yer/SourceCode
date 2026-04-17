using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AiGameStart()
    {
        SceneManager.LoadScene("AIGame");
    }
    public void PlayerGameStart()
    {
        Debug.Log("start the game");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
