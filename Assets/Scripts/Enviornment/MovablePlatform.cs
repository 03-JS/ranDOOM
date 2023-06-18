using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    private Vector3 v3_higherDestination;
    private Vector3 v3_lowerDestination;
    private bool entityInTrigger;
    private bool shouldStoppingSoundPlay;
    private float f_timeBetweenSfx;
    private float f_timesPlayed;
    // private float f_time;
    private GameObject go_switch;
    [HideInInspector] public bool isMoving;

    public bool requiresSwitch;
    public GameObject[] go_switches;
    public bool doesntGoUp;
    public bool doesntGoDown;
    // public bool isAutomated;
    public bool isReversed;
    public bool requiresActivation;
    public bool canDemonsActivate;
    public float f_lowerY = 0f;
    public float f_higherY = 0f;
    public float f_risingSpeed = 1f;
    public float f_loweringSpeed = 1f;
    public float f_sfxPlayTime = 0.25f;
    // public float f_timeLimit = 5f;
    public AudioClip ac_sound;
    public AudioClip ac_stoppingSound;

    // Start is called before the first frame update
    void Start()
    {
        entityInTrigger = false;
        v3_lowerDestination = new Vector3(transform.position.x, f_lowerY, transform.position.z);
        v3_higherDestination = new Vector3(transform.position.x, f_higherY, transform.position.z);
        shouldStoppingSoundPlay = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (canDemonsActivate)
        {
            if (other.tag == "Demon")
            {
                // Debug.Log("Demon in trigger");
                entityInTrigger = true;
                if (!requiresActivation)
                {
                    other.transform.SetParent(gameObject.transform);
                }
            }
        }
        if (other.tag == "Player")
        {
            entityInTrigger = true;
            if (!requiresActivation)
            {
                other.gameObject.transform.SetParent(gameObject.transform);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!requiresActivation)
        {
            entityInTrigger = false;
        }
        other.gameObject.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (requiresSwitch)
        {
            ChechIfSwitchIsOn();
            if (go_switch != null)
            {
                if (go_switch.GetComponent<Switch>().isOn)
                {
                    if (isReversed)
                    {
                        GoDown();
                    }
                    else
                    {
                        GoUp();
                    }
                }
                else
                {
                    if (isReversed)
                    {
                        GoUp();
                    }
                    else
                    {
                        GoDown();
                    }
                }
            }
        }
        else
        {
            if (requiresActivation)
            {
                if (entityInTrigger)
                {
                    if (isReversed)
                    {
                        GoDown();
                    }
                    else
                    {
                        GoUp();
                    }
                }
                else
                {
                    if (isReversed)
                    {
                        GoUp();
                    }
                    else
                    {
                        GoDown();
                    }
                }
            }
        }

        if (transform.position == v3_higherDestination || transform.position == v3_lowerDestination)
        {
            PlayStoppingSound();
        }
        else
        {
            f_timesPlayed = 0;
            shouldStoppingSoundPlay = true;
        }
    }

    private void GoUp()
    {
        if (!doesntGoUp)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, v3_higherDestination, f_risingSpeed * Time.deltaTime);
            if (transform.position != v3_higherDestination)
            {
                f_timeBetweenSfx += Time.deltaTime;
                if (f_timeBetweenSfx >= f_sfxPlayTime)
                {
                    f_timeBetweenSfx = 0;
                    AudioSource.PlayClipAtPoint(ac_sound, transform.position, AudioManager.f_globalElevatorMoveSfxVolume);
                }
            }
            else
            {
                // f_time = 0;
                isMoving = false;
            }
        }
    }

    private void GoDown()
    {
        if (!doesntGoDown)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, v3_lowerDestination, f_loweringSpeed * Time.deltaTime);
            if (transform.position != v3_lowerDestination)
            {
                f_timeBetweenSfx += Time.deltaTime;
                if (f_timeBetweenSfx >= f_sfxPlayTime)
                {
                    f_timeBetweenSfx = 0;
                    AudioSource.PlayClipAtPoint(ac_sound, transform.position, AudioManager.f_globalElevatorMoveSfxVolume);
                }
            }
            else
            {
                // f_time = 0;
                isMoving = false;
            }
        }
    }

    private void PlayStoppingSound()
    {
        if (f_timesPlayed <= 2)
        {
            f_timesPlayed++;
        }
        if (f_timesPlayed == 1 && ac_stoppingSound != null && shouldStoppingSoundPlay)
        {
            AudioSource.PlayClipAtPoint(ac_stoppingSound, transform.position, AudioManager.f_globalSfxVolume);
        }
    }

    private void ChechIfSwitchIsOn()
    {
        foreach (GameObject sw in go_switches)
        {
            if (sw != null)
            {
                if (sw.GetComponent<Switch>().isOn)
                {
                    go_switch = sw;
                }
            }
        }
    }
}
