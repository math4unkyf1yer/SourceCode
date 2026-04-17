using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMenu : MonoBehaviour
{

    public string targetTag;
    public string DownTargetTag;
    private GameObject[] objects;
    private GameObject[] downObjects;
    private bool isOpenSide;
    private bool isOpenDown;


    private void Start()
    {
        objects = GameObject.FindGameObjectsWithTag(targetTag);
        downObjects = GameObject.FindGameObjectsWithTag(DownTargetTag);
        
        ToggleObjects(false);
        ToggleDownObjects(false);
    }
    public void ToggleObjects(bool toggle)
    {
        if (objects == null) { return; }

            foreach (GameObject obj in objects)
            {
                obj.SetActive(toggle);
            }
      //  CloseWallNotTarget();
    }
    public void ToggleDownObjects(bool toggle)
    {
        if (downObjects == null) { return; }

        foreach (GameObject obj in downObjects)
        {
            obj.SetActive(toggle);
        }
        //  CloseWallNotTarget();
    }

    public void MenuOpenOrClose()
    {
        
        if (isOpenSide)
        {
            objects = GameObject.FindGameObjectsWithTag(targetTag);
            isOpenSide = false;
            ToggleObjects(isOpenSide);
        }
        else
        {
            isOpenSide = true;
            ToggleObjects(isOpenSide);
            if (isOpenDown)
            {
                isOpenDown = false;
                downObjects = GameObject.FindGameObjectsWithTag(DownTargetTag);
                ToggleDownObjects(isOpenDown);
            }
        }
    }
    public void MenuDownOpen()
    {
        if (isOpenDown)
        {
            downObjects = GameObject.FindGameObjectsWithTag(DownTargetTag);
            isOpenDown = false;
            ToggleDownObjects(isOpenDown);
        }
        else
        {
            isOpenDown = true;
            ToggleDownObjects(isOpenDown);

            if (isOpenSide)
            {
                isOpenSide = false;
                objects = GameObject.FindGameObjectsWithTag(targetTag);
                ToggleObjects(isOpenSide);
            }
        }
    }

}
