using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZoomDialogue : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private bool isTypingDone = false;
    private AudioSource dialogueAudio;

    private void Start()
    {
        dialogueAudio = GetComponent<AudioSource>();
    }
    public void ShowDialogue(string sentence, string name)
    {
        dialogueBox.SetActive(true);
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueName.text = name.ToString();
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTypingDone = false;
        dialogueText.text = "";
        dialogueAudio.Play();
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        dialogueAudio.Stop();
        isTypingDone = true;
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }
    public bool IsTypingDone()
    {
        return isTypingDone;
    }
}
