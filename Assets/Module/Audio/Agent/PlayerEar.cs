using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEar : MonoBehaviour
{
    public AudioReverbZone reverbZone;
    public AudioListener listener;

    private void Awake()
    {
        reverbZone = GetComponent<AudioReverbZone>();
        listener = GetComponent<AudioListener>();
    }
}
