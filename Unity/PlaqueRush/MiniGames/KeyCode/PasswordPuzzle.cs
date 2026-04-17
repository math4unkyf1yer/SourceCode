using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PasswordPuzzle : MonoBehaviour
{
    [Header("Password Settings")]
    [SerializeField] int[] randomNumbers;
    [SerializeField] int[] playerNumbers;
    [SerializeField] TextMeshProUGUI[] randomNbUI;
    [SerializeField] TextMeshProUGUI[] playerNbUI;

    [Header("Audio Settings")]
    [SerializeField] AudioSource buttonPressSound;
    [SerializeField] AudioSource wrongCodeSound;
    [SerializeField] AudioSource correctCodeSound;

    int currentIndex;
    int howManyGood;
    bool canClick = true;
    private PuzzleManager puzzleManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        puzzleManagerScript = GetComponentInParent<PuzzleManager>();
        AssignRandomeNb();
    }

    void AssignRandomeNb()
    {
        for (int i = 0; i < randomNumbers.Length; i++)
        {
            randomNumbers[i] = Random.Range(0, 10);
            ShowNumber(i, true);
        }
    }

    //click the buttons function
    public void AddNumberBt(int value)
    {
        if (canClick)
        {
            // Play button press sound
            if (buttonPressSound != null)
            {
                buttonPressSound.Play();
            }

            if (currentIndex < playerNumbers.Length)
            {
                playerNumbers[currentIndex] = value;
                ShowNumber(currentIndex, false);
                currentIndex++;
            }
            if (currentIndex > 3)// all slots are fill
            {
                CheckifSuccess();
            }
        }
    }

    void ShowNumber(int index, bool random)
    {
        //want to show 
        if (random)
        {
            randomNbUI[index].text = randomNumbers[index].ToString();
        }
        else
        {
            playerNbUI[index].text = playerNumbers[index].ToString();
        }
    }

    void HideNumbers()
    {
        for (int i = 0; i < playerNbUI.Length; i++)
        {
            playerNbUI[i].text = "".ToString();
        }
    }

    void CheckifSuccess()
    {
        for (int i = 0; i < currentIndex; i++)
        {
            if (playerNumbers[i] == randomNumbers[i])
            {
                howManyGood++;
            }
            else
            {
                howManyGood = 0;

                // Play wrong code sound
                if (wrongCodeSound != null)
                {
                    wrongCodeSound.Play();
                }

                ErasePlayerNumber();
                break;
            }
        }
        if (howManyGood == playerNumbers.Length)
        {
            Success();
        }
    }

    void Success()
    {
        canClick = false;
        currentIndex = 0;

        // Play correct code sound
        if (correctCodeSound != null)
        {
            correctCodeSound.Play();
        }

        howManyGood = 0;
        Restart();

        //puzzle manager finish 
        puzzleManagerScript.FinishActivePuzzle();
    }

    public void Restart()
    {
        ErasePlayerNumber();
        AssignRandomeNb();
        canClick = true;
    }

    void ErasePlayerNumber()
    {
        // once upon again if anything it in erase it 
        for (int i = 0; i < playerNumbers.Length; i++)
        {
            playerNumbers[i] = -1;
        }
        currentIndex = 0;
        HideNumbers();
    }
}