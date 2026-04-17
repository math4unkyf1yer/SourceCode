using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private bool timerActivate;
    [SerializeField] float currenttimer;
    [SerializeField] float maxtimer;
    [SerializeField] Slider timerSlider;

    private Menu menuScript; //restart if timer runs out
    private PuzzleManager puzzleManager;

    private void Start()
    {
        menuScript = GetComponent<Menu>();
        puzzleManager = GetComponent<PuzzleManager>();
    }
    public void startTimer()
    {
        SetMaxSliderValue();
        currenttimer = maxtimer;
        gameObject.SetActive(true);
        timerActivate = true;
    }
    public void StopTimer(bool Lose)
    {
        timerActivate = false;
        maxtimer -= 3.0f;
        SetMaxSliderValue();
        if(Lose == true)
        {
            Debug.Log("timer ran out ");
            menuScript.OpenLosePage();
            puzzleManager.ClosePages();
        }
    }

    private void Update()
    {
        if (timerActivate)
        {
            TimerRunning();
        }
    }

    void TimerRunning()
    {
        if(currenttimer > 0)
        {
            currenttimer -= Time.deltaTime;
            //set the slider
            setSliderValue(currenttimer);
        }
        else
        {
            //lose timer ran out 
            StopTimer(true);
        }
    }

    //Slider Functions
    void SetMaxSliderValue()
    {
        timerSlider.maxValue = maxtimer;
        timerSlider.value = maxtimer;
    }
    void setSliderValue(float tvalue)
    {
        timerSlider.value = tvalue;
    }
}
