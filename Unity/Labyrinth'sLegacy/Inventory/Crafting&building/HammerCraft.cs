using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerCraft : MonoBehaviour
{
    public GameObject workbenchPage;
    public GameObject constructionPage;

    #region Singleton
    public static HammerCraft Intance;
    private void Awake()
    {
        Intance = this;
    }
    #endregion
    private void Start()
    {
        OpenConstruction();
    }
    public void OpenWorkbench()
    {
        if (workbenchPage.activeSelf == false)
        {
            workbenchPage.SetActive(true);
            constructionPage.SetActive(false);
        }
    }
    public void OpenConstruction()
    {
        if (constructionPage.activeSelf == false)
        {
            constructionPage.SetActive(true);
            workbenchPage.SetActive(false);
        }
    }
    public void ClosePage()//for closing page  
    {
        constructionPage.SetActive(true);
        workbenchPage.SetActive(false);
    }
}
