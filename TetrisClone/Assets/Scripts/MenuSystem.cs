using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    public AudioClip StartGameClip;
        public void PlayAgain()
    {
        SceneManager.LoadScene("Game");
        

    }

    public void PlayStartSound()
    {
        audioSource.PlayOneShot(StartGameClip);

    }
}
