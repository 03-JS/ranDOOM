using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private float f_time;
    public float f_timeLimit = 6f;
    public float f_lowerY = 0f;
    public float f_higherY = 0f;
    public float f_speed = 0f;
    public bool requiresKey;
    public bool requiresRedKey;
    public bool requiresBlueKey;
    public bool requiresYellowKey;
    public bool doesntClose;
    public bool canDemonsOpen;
    public bool activatesObjects;
    public GameObject[] objects;
    public AudioClip ac_openingSound;
    public AudioClip ac_closingSound;
    private float f_timesClosingSoundHasPlayed;
    private float f_timesOpeningSoundHasPlayed;
    private Vector3 v3_lowerDestination;
    private Vector3 v3_higherDestination;
    private Vector3 v3_initialPosition;
    private bool shouldDoorClose;
    private bool shouldDoorOpen;

    // Note to self: Doors are fine as they are right now. Don't change them
    // Start is called before the first frame update
    void Start()
    {
        v3_lowerDestination = new Vector3(transform.position.x, f_lowerY, transform.position.z);
        v3_higherDestination = new Vector3(transform.position.x, f_higherY, transform.position.z);
        v3_initialPosition = transform.position;
    }

    void Update()
    {
        if (shouldDoorOpen)
        {
            OpeningSequence();
        }
        if (HasOpened())
        {
            shouldDoorOpen = false;
            shouldDoorClose = true;
        }
        if (shouldDoorClose && !HasClosed())
        {
            ClosingSequence();
        }
        if (HasClosed())
        {
            shouldDoorClose = false;
        }
    }

    public void OpeningSequence()
    {
        PlayOpeningSound();
        f_time = 0;
        f_timesClosingSoundHasPlayed = 0;
        transform.position = Vector3.MoveTowards(transform.position, v3_higherDestination, f_speed * Time.deltaTime);
    }

    public void ClosingSequence()
    {
        if (v3_initialPosition != transform.position && !doesntClose)
        {
            f_time += Time.deltaTime;
        }
        else
        {
            f_time = 0;
        }
        if (!doesntClose)
        {
            if (f_time >= f_timeLimit)
            {
                PlayClosingSound();
                f_timesOpeningSoundHasPlayed = 0;
                transform.position = Vector3.MoveTowards(transform.position, v3_lowerDestination, f_speed * Time.deltaTime);
            }
        }
    }

    public bool HasClosed()
    {
        return transform.position == v3_lowerDestination;
    }

    public bool HasOpened()
    {
        return transform.position == v3_higherDestination;
    }

    private void PlayOpeningSound()
    {
        f_timesOpeningSoundHasPlayed++;
        if (f_timesOpeningSoundHasPlayed == 1)
        {
            AudioSource.PlayClipAtPoint(ac_openingSound, transform.position, AudioManager.f_globalSfxVolume);
        }
    }

    private void PlayClosingSound()
    {
        f_timesClosingSoundHasPlayed++;
        if (f_timesClosingSoundHasPlayed == 1)
        {
            AudioSource.PlayClipAtPoint(ac_closingSound, transform.position, AudioManager.f_globalSfxVolume);
        }
    }

    public void Open(bool showMessage)
    {
        if (requiresKey)
        {
            if (requiresYellowKey)
            {
                OpenIfHasKey("Yellow", showMessage);
            }
            if (requiresBlueKey)
            {
                OpenIfHasKey("Blue", showMessage);
            }
            if (requiresRedKey)
            {
                OpenIfHasKey("Red", showMessage);
            }
        }
        else
        {
            shouldDoorOpen = true;
            if (activatesObjects)
            {
                foreach (GameObject gameObj in objects)
                {
                    if (gameObj != null)
                    {
                        gameObj.SetActive(true);
                    }
                }
            }
        }

    }

    private void OpenIfHasKey(string str_keyColor, bool showMessage)
    {
        if (FindObjectOfType<PlayerInventory>().HasKeycard(str_keyColor))
        {
            shouldDoorOpen = true;
            if (activatesObjects)
            {
                foreach (GameObject gameObj in objects)
                {
                    gameObj.SetActive(true);
                }
            }
        }
        else
        {
            if (showMessage)
            {
                FindObjectOfType<UIManager>().SetInfoText("You need a " + str_keyColor + " key to open this door");
                FindObjectOfType<AudioManager>().PlaySfx(5); // Unf sound
            }
        }
    }
}
