using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonCraftandSkills : MonoBehaviour
{
    public GameObject craftPage;
    public GameObject skillsPage;
    //craftPage
    public void OnClickCraft()
    {
        //open page 
        if (craftPage.activeSelf)
        {
            Debug.Log("Already open");
        }
        else
        {
            craftPage.gameObject.SetActive(true);
            skillsPage.gameObject.SetActive(false);
        }
    }
    public void OnClickSkills()
    {
        //open page 
        if (skillsPage.activeSelf)
        {
            Debug.Log("Already open");
        }
        else
        {
            skillsPage.gameObject.SetActive(true);
            craftPage.gameObject.SetActive(false);
        }
    }

    public void ShowSkillsUi()
    {
        //show the skills if I want 
    }
}
