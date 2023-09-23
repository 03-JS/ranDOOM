using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] songs;
    public Sound[] classicSongs;
    public Sound[] soundEffects;
    public Sound[] menuSoundEffects;
    public Sound[] weaponSoundEffects;
    public Sound[] reloadSoundEffects;
    private Sound songPlaying;

    // Static variables. These are required if there is more than one scene involved
    public static float f_globalRemadeMusicVolume = 0.25f; // The param for remade music
    public static float f_globalClassicMusicVolume = 0.5f; // The param for classic music
    public static float f_globalSfxVolume = 1f; // The param for almost all sfx
    public static float f_globalWeaponSfxVolume = 0.5f; // The param for all weapons sfx
    public static float f_globalElevatorMoveSfxVolume = 1000000f; // A special param for the elevators since the sfx volume is very low
    public static bool isMusicClassic = false;

    void Awake()
    {
        foreach (Sound song in songs)
        {
            song.audioSource = gameObject.AddComponent<AudioSource>();
            song.audioSource.clip = song.acl_audio;
            song.audioSource.volume = f_globalRemadeMusicVolume;
            song.audioSource.loop = song.b_loop;
            song.audioSource.ignoreListenerPause = true;
        }
        foreach (Sound classicSong in classicSongs)
        {
            classicSong.audioSource = gameObject.AddComponent<AudioSource>();
            classicSong.audioSource.clip = classicSong.acl_audio;
            classicSong.audioSource.volume = f_globalClassicMusicVolume;
            classicSong.audioSource.loop = classicSong.b_loop;
            classicSong.audioSource.ignoreListenerPause = true;
        }
        foreach (Sound sfx in soundEffects)
        {
            sfx.audioSource = gameObject.AddComponent<AudioSource>();
            sfx.audioSource.clip = sfx.acl_audio;
            sfx.audioSource.volume = f_globalSfxVolume;
            sfx.audioSource.loop = sfx.b_loop;
            // sfx.audioSource.ignoreListenerPause = true;
        }
        foreach (Sound menuSfx in menuSoundEffects)
        {
            menuSfx.audioSource = gameObject.AddComponent<AudioSource>();
            menuSfx.audioSource.clip = menuSfx.acl_audio;
            menuSfx.audioSource.volume = f_globalSfxVolume;
            menuSfx.audioSource.loop = menuSfx.b_loop;
            menuSfx.audioSource.ignoreListenerPause = true;
        }
        foreach (Sound weaponSfx in weaponSoundEffects)
        {
            weaponSfx.audioSource = gameObject.AddComponent<AudioSource>();
            weaponSfx.audioSource.clip = weaponSfx.acl_audio;
            weaponSfx.audioSource.volume = f_globalWeaponSfxVolume;
            weaponSfx.audioSource.loop = weaponSfx.b_loop;
            // weaponSfx.audioSource.ignoreListenerPause = true;
        }
        foreach (Sound reloadSfx in reloadSoundEffects)
        {
            reloadSfx.audioSource = gameObject.AddComponent<AudioSource>();
            reloadSfx.audioSource.clip = reloadSfx.acl_audio;
            reloadSfx.audioSource.volume = f_globalWeaponSfxVolume;
            reloadSfx.audioSource.loop = reloadSfx.b_loop;
            // reloadSfx.audioSource.ignoreListenerPause = true;
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            PlayMenuIntro();
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "Intermission")
            {
                PlayIntermissionMusic();
            }
            else
            {
                if (isMusicClassic)
                {
                    PlayClassicSong();
                }
                else
                {
                    PlaySong();
                }
            }
        }
    }

    private void PlayMenuIntro()
    {
        menuSoundEffects[0].audioSource.Play();
    }

    private void PlayIntermissionMusic()
    {
        if (isMusicClassic)
        {
            classicSongs[11].audioSource.Play();
        }
        else
        {
            songs[12].audioSource.Play();
        }
    }

    public void PlaySong()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            StopSong();
            System.Random random = new System.Random();
            int i_randomNumber = random.Next(0, songs.Length);
            songs[i_randomNumber].audioSource.Play();
            songPlaying = songs[i_randomNumber];
        }
    }

    // This is only used for the IDMUS cheat
    public void PlaySong(int index)
    {
        StopSong();
        try
        {
            Debug.Log("Playing remade music");
            songs[index].audioSource.Play();
            FindObjectOfType<UIManager>().SetInfoText("Now playing: " + songs[index].ToString());
            songPlaying = songs[index];
        }
        catch (Exception)
        {
            FindObjectOfType<UIManager>().SetInfoText("Track not found. Playing a random selection instead");
            PlaySong();
        }
    }

    public void StopSong()
    {
        if (songPlaying != null)
        {
            songPlaying.audioSource.Stop();
        }
    }

    public void PlayClassicSong()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            StopSong();
            System.Random random = new System.Random();
            int i_randomNumber = random.Next(0, classicSongs.Length);
            classicSongs[i_randomNumber].audioSource.Play();
            songPlaying = classicSongs[i_randomNumber];
        }
    }

    public void PlayClassicSong(int index)
    {
        StopSong();
        try
        {
            Debug.Log("Playing classic music");
            classicSongs[index].audioSource.Play();
            FindObjectOfType<UIManager>().SetInfoText("Now playing: " + classicSongs[index].ToString());
            songPlaying = classicSongs[index];
        }
        catch (Exception)
        {
            FindObjectOfType<UIManager>().SetInfoText("Track not found. Playing a random selection instead");
            PlayClassicSong();
        }
    }

    public void PlaySfx(int index)
    {
        soundEffects[index].audioSource.Play();
    }

    public void StopSfx(int index)
    {
        if (soundEffects[index].audioSource != null)
        {
            soundEffects[index].audioSource.Stop();
        }
    }

    public void PlayWeaponSfx(int index)
    {
        weaponSoundEffects[index].audioSource.Play();
    }

    public void StopWeaponSfx(int index)
    {
        if (weaponSoundEffects[index].audioSource != null)
        {
            weaponSoundEffects[index].audioSource.Stop();
        }
    }

    public void PlayUninterruptedWeaponSfx(int index)
    {
        if (!weaponSoundEffects[index].audioSource.isPlaying)
        {
            weaponSoundEffects[index].audioSource.Play();
        }
    }

    public void PlayMenuSfx(int index)
    {
        menuSoundEffects[index].audioSource.Play();
    }

    // This exists just in case I want to switch the style manually instead of automatically
    public void SetMusicStyle(string style)
    {
        if (style == "Classic")
        {
            isMusicClassic = true;
            PlayClassicSong();
        }
        else
        {
            isMusicClassic = false;
            PlaySong();
        }
    }

    public void SwitchMusicStyle(bool showInfoMessage)
    {
        if (isMusicClassic)
        {
            isMusicClassic = false;
            if (showInfoMessage)
            {
                FindObjectOfType<UIManager>().SetInfoText("Music style is now set to remake");
            }
            PlaySong();
        }
        else
        {
            isMusicClassic = true;
            if (showInfoMessage)
            {
                FindObjectOfType<UIManager>().SetInfoText("Music style is now set to classic");
            }
            PlayClassicSong();
        }
    }

    public void ChangeMusicVolume(float value)
    {
        f_globalClassicMusicVolume = value / 100;
        f_globalClassicMusicVolume /= 2;
        // Debug.Log("Classic music volume: " + f_globalClassicMusicVolume);
        f_globalRemadeMusicVolume = value / 100;
        f_globalRemadeMusicVolume /= 4;
        // Debug.Log("Remade music volume: " + f_globalRemadeMusicVolume);
        if (isMusicClassic)
        {
            foreach (Sound classicSong in classicSongs)
            {
                // f_globalClassicMusicVolume = classicSong.audioSource.volume;
                classicSong.audioSource.volume = f_globalClassicMusicVolume;
            }
        }
        else
        {
            foreach (Sound song in songs)
            {
                // f_globalRemadeMusicVolume = song.audioSource.volume;
                song.audioSource.volume = f_globalRemadeMusicVolume;
            }
        }

    }

    public void ChangeSfxVolume(float value)
    {
        f_globalSfxVolume = value / 100;
        f_globalWeaponSfxVolume = value / 100;
        f_globalWeaponSfxVolume /= 2;
        f_globalElevatorMoveSfxVolume = value * 10000;
        // Debug.Log("Sfx volume: " + f_globalSfxVolume);
        foreach (Sound sfx in soundEffects)
        {
            sfx.audioSource.volume = f_globalSfxVolume;
        }
        foreach (Sound menuSfx in menuSoundEffects)
        {
            menuSfx.audioSource.volume = f_globalSfxVolume;
        }
        foreach (Sound weaponSfx in weaponSoundEffects)
        {
            weaponSfx.audioSource.volume = f_globalWeaponSfxVolume;
        }
        foreach (Sound reloadSfx in reloadSoundEffects)
        {
            reloadSfx.audioSource.volume = f_globalWeaponSfxVolume;
        }
    }

    // A method used only by the SSG
    public void PlayReloadSFX(int index)
    {
        reloadSoundEffects[index].audioSource.Play();
    }

}
