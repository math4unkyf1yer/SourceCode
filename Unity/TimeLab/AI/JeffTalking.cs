using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeffTalking : MonoBehaviour
{
    public string[] dialogueLines;
    public string[] deadDialogue;

    public string whoIsTalking = "Jeff";
    private List<int> dialogueUsed = new List<int>();

    private ZoomDialogue dialogueManager;
    private EyeChange eyeScript;
    private TriggerCutscene triggerCutScript;

    [Header("Only LastCutscene   need to time with dialogue")]
    public Animator playerCutAnimator;
    public GameObject orbEffect;
    public GameObject leftOrbEffect;
    public AudioSource leftArm;
    public AudioSource rightArm;
    public AudioSource main;

    private void Start()
    {
        eyeScript = GetComponent<EyeChange>();
        dialogueManager = FindObjectOfType<ZoomDialogue>();
    }

    // Call this to play a specific line by index
    public void SayDialogue(int index, bool oneTime, float duration = 10f)
    {
        if (index < 0 || index >= dialogueLines.Length)
        {
            Debug.LogWarning("Dialogue index out of range!");
            return;
        }
        // If oneTime is true and we've already said this line, exit
        if (oneTime && dialogueUsed.Contains(index))
        {
            Debug.Log($"Dialogue index {index} has already been used.");
            return;
        }

        dialogueManager.ShowDialogue(dialogueLines[index], whoIsTalking);
        eyeScript.talking = true;
        Invoke(nameof(HideDialogue), duration);

        // If oneTime, record this index as used
        if (oneTime)
        {
            dialogueUsed.Add(index);
        }
    }

    public void DeadDialogue(int index,float duration = 5f)
    {
        if (index < 0 || index >= dialogueLines.Length)
        {
            Debug.LogWarning("Dialogue index out of range!");
            return;
        }
        dialogueManager.ShowDialogue(deadDialogue[index], whoIsTalking);
        eyeScript.talking = true;
        Invoke(nameof(HideDialogue), duration);
    }

    private void HideDialogue()
    {
        eyeScript.talking = false;
        dialogueManager.HideDialogue();
    }

    //for the cutscens
    public void PlayDialogueSequence(int[] indices, float waitBetweenLines = 0.5f,TriggerCutscene cutsceneScript = null,bool lastcutScene = false)
    {
        triggerCutScript = cutsceneScript;
        StartCoroutine(PlaySequence(indices, waitBetweenLines,lastcutScene));
    }

    private IEnumerator PlaySequence(int[] indices, float waitBetweenLines,bool lastCut)
    {
        foreach (int index in indices)
        {
            if (index < 0 || index >= dialogueLines.Length)
            {
                Debug.LogWarning("Dialogue index out of range!");
                continue;
            }
            eyeScript.talking = true;
            dialogueManager.ShowDialogue(dialogueLines[index], whoIsTalking);

            if(lastCut == true && index == dialogueLines.Length - 2)
            {
                Debug.Log("second to last");
                LastCutsceneAnimation();
            }

            // Wait until the typewriter finishes
            yield return new WaitUntil(() => dialogueManager.IsTypingDone());

            // Wait some time to let the player read
            if(lastCut == true && index == dialogueLines.Length - 1)
            {
                Debug.Log("last dialogue");
                orbEffect.SetActive(true);
                rightArm.Play();
                leftOrbEffect.SetActive(true);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(waitBetweenLines);
            }

            // Hide and clear
            eyeScript.talking = false;
            dialogueManager.HideDialogue();
        }
        if(triggerCutScript != null)
        {
            eyeScript.ShootingEye();
            StartCoroutine(triggerCutScript.Wait());
        }
    }

    void LastCutsceneAnimation()
    {
        playerCutAnimator.SetBool("LastCut", true);
    }
}
