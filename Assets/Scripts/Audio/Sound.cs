using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip acl_audio;
    public AudioSource audioSource;
    public bool b_loop = true;

    public override string ToString()
    {
        return acl_audio.name;
    }

}
