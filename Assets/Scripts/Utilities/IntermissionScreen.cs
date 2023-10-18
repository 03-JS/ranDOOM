using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntermissionScreen : MonoBehaviour
{
    public float maxTimeBetweenRows;
    public TextMeshProUGUI tmp_levelTitle;
    public TextMeshProUGUI tmp_subtitle;
    public TextMeshProUGUI tmp_time;
    public TextMeshProUGUI[] tmp_percents;
    public GameObject text;
    public string[] levels;
    public string[] enduranceTrials;

    private string nextLevel;
    private AudioManager audioManager;
    private int killsP = 0;
    private int itemsP = 0;
    private int secretsP = 0;
    private int[] percents;
    private int count = 0;
    private int index = 0;
    private int boomCount = 0;
    private float time = 0f;

    private bool shouldStatsAnimPlay;
    private bool shouldTimeAnimPlay;
    private bool animHasFinished = false;
    private bool listenToInput = true;

    private float maxDelay = 0.025f;
    private float delay = 0f;
    private float maxSoundDelay = 0.1f;
    private float soundDelay = 0f;
    private float timeBetweenRows = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        shouldStatsAnimPlay = true;
        // animHasFinished = false;
        if (LevelStats.maxKills > 0)
        {
            killsP = (LevelStats.kills * 100) / LevelStats.maxKills;
        }
        else
        {
            killsP = 100;
        }
        if (LevelStats.items > 0)
        {
            itemsP = (LevelStats.itemsFound * 100) / LevelStats.items;
        }
        else
        {
            itemsP = 100;
        }
        if (LevelStats.secrets > 0)
        {
            secretsP = (LevelStats.secretsFound * 100) / LevelStats.secrets;
        }
        else
        {
            secretsP = 100;
        }

        percents = new int[3];
        percents[0] = killsP;
        percents[1] = itemsP;
        percents[2] = secretsP;

        if (LevelStats.level != null)
        {
            tmp_levelTitle.SetText(LevelStats.level.ToUpper());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (listenToInput)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
            {
                if (animHasFinished)
                {
                    PrepareNextLevel();
                }
                else
                {
                    shouldStatsAnimPlay = false;
                    shouldTimeAnimPlay = false;
                }
            }
        }

        if (shouldStatsAnimPlay)
        {
            PlayStatsAnimation();
        }
        else
        {
            if (shouldTimeAnimPlay)
            {
                PlayTimeAnimation();
            }
            else
            {
                SkipAllAnimations();
            }
        }
    }

    private void PlayStatsAnimation()
    {
        if (count < percents[index])
        {
            tmp_percents[index].gameObject.SetActive(true);
            delay += Time.deltaTime;
            if (delay >= maxDelay)
            {
                count += 1;
                tmp_percents[index].SetText(count + "%");
                delay = 0;
            }
            soundDelay += Time.deltaTime;
            if (soundDelay >= maxSoundDelay)
            {
                PlayWeaponSFX();
                soundDelay = 0;
            }
        }
        else
        {
            if (!tmp_percents[index].gameObject.activeInHierarchy)
            {
                tmp_percents[index].gameObject.SetActive(true);
            }

            if (boomCount < 1)
            {
                boomCount++;
                audioManager.PlayMenuSfx(5);
            }

            timeBetweenRows += Time.deltaTime;

            if (timeBetweenRows >= maxTimeBetweenRows)
            {
                if (index < 2)
                {
                    index++;
                }
                else
                {
                    shouldStatsAnimPlay = false;
                    shouldTimeAnimPlay = true;
                    delay = 0;
                    soundDelay = 0;
                    boomCount = 0;
                }
                timeBetweenRows = 0;
                count = 0;
                boomCount = 0;
            }
        }
    }

    private void PlayTimeAnimation()
    {
        if (time < LevelStats.time)
        {
            tmp_time.gameObject.SetActive(true);
            delay += Time.deltaTime;
            if (delay >= maxDelay)
            {
                time += 1;
                tmp_time.SetText(TimeSpan.FromSeconds(time).Minutes + ":" + TimeSpan.FromSeconds(time).Seconds);
                delay = 0;
            }
            soundDelay += Time.deltaTime;
            if (soundDelay >= maxSoundDelay)
            {
                PlayWeaponSFX();
                soundDelay = 0;
            }
        }
        else
        {
            if (boomCount < 1)
            {
                boomCount++;
                audioManager.PlayMenuSfx(5);
            }
            shouldTimeAnimPlay = false;
            animHasFinished = true;
            LevelStats.ResetStats();
        }
    }

    private void SkipAllAnimations()
    {
        if (!animHasFinished)
        {
            for (int i = 0; i < tmp_percents.Length; i++)
            {
                tmp_percents[i].gameObject.SetActive(true);
                tmp_percents[i].SetText(percents[i] + "%");
            }
            tmp_time.gameObject.SetActive(true);
            tmp_time.SetText(TimeSpan.FromSeconds(LevelStats.time).Minutes + ":" + TimeSpan.FromSeconds(LevelStats.time).Seconds);
            audioManager.PlayMenuSfx(5);
            animHasFinished = true;
            LevelStats.ResetStats();
        }
    }

    private void PlayWeaponSFX()
    {
        audioManager.PlayWeaponSfx(0);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(nextLevel);
    }

    public void PrepareNextLevel()
    {
        listenToInput = false;
        if (tmp_levelTitle.text.Contains("ENDURANCE TRIAL"))
        {
            nextLevel = enduranceTrials[UnityEngine.Random.Range(0, enduranceTrials.Length)];
            if (enduranceTrials.Length > 1)
            {
                while (nextLevel == LevelStats.level)
                {
                    nextLevel = enduranceTrials[UnityEngine.Random.Range(0, enduranceTrials.Length)];
                }
            }
        }
        else
        {
            nextLevel = levels[UnityEngine.Random.Range(0, levels.Length)];
            if (levels.Length > 1)
            {
                while (nextLevel == LevelStats.level)
                {
                    nextLevel = levels[UnityEngine.Random.Range(0, levels.Length)];
                }
            }
        }
        // Debug.Log(nextLevel);
        tmp_levelTitle.SetText("ENTERING");
        tmp_subtitle.SetText(nextLevel.ToUpper());
        text.SetActive(false);
        audioManager.PlaySfx(1);
        Invoke("LoadScene", 2f);
    }
}
