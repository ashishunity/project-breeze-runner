using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;
  
  
    public void Start()
    {
        audioSource =GetComponent<AudioSource>();
    }
    public void Play(string type)
    {
        switch (type)
        {
            case "flip": audioSource.PlayOneShot(flipSound); break;
            case "match": audioSource.PlayOneShot(matchSound); break;
            case "mismatch": audioSource.PlayOneShot(mismatchSound); break;
            case "gameover": audioSource.PlayOneShot(gameOverSound); break;
        }
    }

}
