using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    public AudioClip clickSound; // Assign the sound effect in the Inspector
    private AudioSource audioSource;

    private void Awake()
    {
        // Add or get an AudioSource component
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Ensure it's 2D sound
        audioSource.volume = 1f; // Adjust the volume as needed
    }

    private void Start()
    {
        // Get the button component and add a listener
        Button button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }
}
