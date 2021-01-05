using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSound : MonoBehaviour
{
    private AudioSource audio;
    public bool isPlay;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(isPlay)
        {
            audio.Play();
            isPlay = false;
        }
    }
}
