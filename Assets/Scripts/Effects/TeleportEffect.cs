using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : MonoBehaviour
{
    public AudioClip ac_soundEffect;
    public float duration = 1.4f;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(ac_soundEffect, transform.position, AudioManager.f_globalSfxVolume);
        Destroy(gameObject, duration);
    }
}
