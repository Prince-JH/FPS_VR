using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSound : MonoBehaviour
{
    private AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void SoundPlay()
    {
        audio.Play();
    }
    public void SoundStop()
    {
        audio.Stop();
    }
}
