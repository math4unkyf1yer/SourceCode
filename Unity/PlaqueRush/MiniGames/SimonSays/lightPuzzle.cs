using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lightPuzzle : MonoBehaviour
{
    [Header("Light Settings")]
    public Image[] lightColor;
    public int[] orderNb;
    private int maxNb = 4;
    private int nbTimedone;
    public GameObject blockClick;

    [Header("Audio Settings")]
    [SerializeField] AudioSource lightFlickerSound;
    [SerializeField] AudioSource buttonPressSound;
    [SerializeField] AudioSource correctCodeSound;
    [SerializeField] AudioSource wrongCodeSound;

    private PuzzleManager puzzleManagerScript;
    int whichbuttonsave;
    Color lastColor;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManagerScript = GetComponentInParent<PuzzleManager>();
    }

    public void StartPuzzle()
    {
        StartCoroutine(ShowColor());
    }

    IEnumerator ShowColor()
    {
        int i = Random.Range(0, lightColor.Length);
        orderNb[nbTimedone] = i;
        nbTimedone++;

        // Play light flicker sound
        if (lightFlickerSound != null)
        {
            lightFlickerSound.Play();
        }

        Color currentColor = lightColor[i].color;
        Color startColor = currentColor;
        currentColor.a = 1f;
        lightColor[i].color = currentColor;

        //Wait a bit 
        yield return new WaitForSeconds(0.7f);
        lightColor[i].color = startColor;

        if (nbTimedone == maxNb)
        {
            Debug.Log("Stop loop");
            yield return new WaitForSeconds(0.2f);
            blockClick.SetActive(false);
            nbTimedone = 0;
        }
        else
        {
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(ShowColor());
        }
    }

    public void ClickLightButton(int whichButtton)
    {
        // Play button press sound
        if (buttonPressSound != null)
        {
            buttonPressSound.Play();
        }

        if (whichButtton == orderNb[nbTimedone])
        {
            nbTimedone++;
            StartCoroutine(showClickColor(whichButtton));

            //if all correct you did it 
            if (nbTimedone == maxNb)
            {
                Debug.Log("You Win");

                // Play correct code sound
                if (correctCodeSound != null)
                {
                    correctCodeSound.Play();
                }

                lightColor[whichbuttonsave].color = lastColor;
                
                nbTimedone = 0;
                puzzleManagerScript.FinishActivePuzzle();
            }
        }
        else
        {
            // Play wrong code sound
            if (wrongCodeSound != null)
            {
                wrongCodeSound.Play();
            }

            //restart the loop 
            nbTimedone = 0;
            blockClick.SetActive(true);
            StartCoroutine(ShowColor());
        }
    }


    IEnumerator showClickColor(int whichButton) // just to make it look better 
    {
        Color currentColor = lightColor[whichButton].color;
        Color startColor = currentColor;
        lastColor = currentColor;
        whichbuttonsave = whichButton;
        currentColor.a = 1;
        lightColor[whichButton].color = currentColor;
        yield return new WaitForSeconds(0.2f);
        lightColor[whichButton].color = startColor;
    }
}