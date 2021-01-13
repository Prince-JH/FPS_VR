using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSound : MonoBehaviour
{
    private AudioSource audio;
    [HideInInspector]
    public bool canPlay = true;
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void DeadSoundPlay()
    {
        audio.Play();
        canPlay = false;
    }
    public void DeadSoundStop()
    {
        audio.Stop();
    }
}
