using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [HideInInspector] public bool isOn = false;
    public bool turnsOff;
    public bool activatesObjects;
    public GameObject[] objects;
    private float f_time = 0f;
    public float f_timeLimit = 3f;
    private float f_keyPresses = 0f;
    public Sprite[] sprites;
    public SpriteRenderer sr_spriteRenderer;
    public AudioClip ac_turnOnSound;
    public AudioClip ac_turnOffSound;

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            if (turnsOff)
            {
                f_time += Time.deltaTime;
            }
            if (f_time >= f_timeLimit && turnsOff)
            {
                isOn = false;
                f_keyPresses = 0;
                AudioSource.PlayClipAtPoint(ac_turnOffSound, transform.position, AudioManager.f_globalSfxVolume);
            }
            sr_spriteRenderer.sprite = sprites[1];
        }
        else
        {
            f_time = 0f;
            sr_spriteRenderer.sprite = sprites[0];
        }
    }

    public void TurnOn()
    {
        f_keyPresses++;
        if (f_keyPresses == 1)
        {
            AudioSource.PlayClipAtPoint(ac_turnOnSound, transform.position, AudioManager.f_globalSfxVolume);
        }
        isOn = true;
        if (activatesObjects)
        {
            foreach (GameObject gameObj in objects)
            {
                gameObj.SetActive(true);
            }
        }
    }
}
