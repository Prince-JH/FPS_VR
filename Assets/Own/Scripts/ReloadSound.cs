using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSound : MonoBehaviour
{
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void ReloadSoundPlay()
    {
        audio.Play();
    }    
    public void ReloadSoundStop()
    {
        audio.Stop();
    }
}
