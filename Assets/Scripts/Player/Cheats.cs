using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    public GameObject[] keysInTheLevel;
    public float f_inputTimeLimit = 5f;

    private string str_pattern;
    private string str_number = "";
    private float f_timer;
    private bool hasSequenceStarted;
    private int iddqdCount = 0;

    private AudioManager audioManager;
    private UIManager uiManager;
    private PlayerHealthAndArmor player;
    private PlayerInventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<PlayerHealthAndArmor>();
        inventory = FindObjectOfType<PlayerInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            if (!player.isDead)
            {
                ManageCheatInput();
            }
        }
        else
        {
            ManageCheatInput();
        }
        if (hasSequenceStarted)
        {
            f_timer += Time.deltaTime;
            if (f_timer > f_inputTimeLimit)
            {
                hasSequenceStarted = false;
                f_timer = 0;
            }
        }
    }

    public void ManageCheatInput()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            hasSequenceStarted = true;
            f_timer = 0;
            str_number = "";
            str_pattern = "I";
        }
        if (hasSequenceStarted && f_timer <= f_inputTimeLimit)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                str_pattern += "D";
            }
            if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                str_pattern += "K";
            }
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                str_pattern += "F";
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                str_pattern += "A";
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                str_pattern += "M";
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                str_pattern += "U";
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                str_pattern += "S";
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                str_pattern += "T";
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                str_pattern += "Y";
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                str_pattern += "C";
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                str_pattern += "H";
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                str_pattern += "E";
            }
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                str_pattern += "Q";
            }
            ApplyCheatEffect();
        }
    }

    public void ApplyCheatEffect()
    {
        // Debug.Log(str_pattern);
        if (str_pattern == "IDCHEATS")
        {
            FindObjectOfType<MenuInputManager>().EnableHiddenObjects();
            hasSequenceStarted = false;
            f_timer = 0;
        }
        if (SceneManager.GetActiveScene().name != "Main Menu" && !MenuInputManager.gameIsPaused)
        {
            if (str_pattern == "IDFA")
            {
                player.AddArmor(1000);
                player.AddHealth(1000);
                inventory.SetAmmoCapacityToMax();
                inventory.AddShells(24);
                inventory.AddCells(250);
                inventory.AddBullets(180);
                inventory.AddRockets(13);
                inventory.AddFuel(3);
                inventory.AddAllWeapons();
                uiManager.SetInfoText("Ammo (no keys) added");
                hasSequenceStarted = false;
                f_timer = 0;
            }
            if (str_pattern == "IDKFA")
            {
                player.AddArmor(1000);
                player.AddHealth(1000);
                inventory.SetAmmoCapacityToMax();
                inventory.AddShells(24);
                inventory.AddCells(250);
                inventory.AddBullets(180);
                inventory.AddRockets(13);
                inventory.AddFuel(3);
                inventory.AddAllWeapons();
                if (keysInTheLevel[0] != null)
                {
                    inventory.AddKeycard(keysInTheLevel[0]);
                }
                if (keysInTheLevel[1] != null)
                {
                    inventory.AddKeycard(keysInTheLevel[1]);
                }
                if (keysInTheLevel[2] != null)
                {
                    inventory.AddKeycard(keysInTheLevel[2]);
                }
                uiManager.SetInfoText("Very Happy Ammo Added");
                hasSequenceStarted = false;
                f_timer = 0;
            }
            if (str_pattern == "IDMUS")
            {
                str_number = ManageNumberInput();
                if (str_number != "") // this is the same as saying the player pressed a number key
                {
                    f_timer = 0;
                    int number = int.Parse(str_number);
                    if (str_number.Length >= 2)
                    {
                        if (AudioManager.isMusicClassic)
                        {
                            audioManager.PlayClassicSong(number - 1);
                        }
                        else
                        {
                            audioManager.PlaySong(number - 1);
                        }
                        hasSequenceStarted = false;
                        f_timer = 0;
                        str_number = "";
                    }
                }
            }
            /*
            if (str_pattern == "IDMST")
            {
                audioManager.SwitchMusicStyle(true);
                hasSequenceStarted = false;
                f_timer = 0;
            }
            */
            if (str_pattern == "IDDQD")
            {
                iddqdCount++;
                if (iddqdCount == 1)
                {
                    player.SetGodMode(true);
                }
                else
                {
                    iddqdCount = 0;
                    player.SetGodMode(false);
                }
                hasSequenceStarted = false;
                f_timer = 0;
            }
        }
    }

    public string ManageNumberInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            str_number += "0";
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            str_number += "1";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            str_number += "2";
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            str_number += "3";
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            str_number += "4";
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            str_number += "5";
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            str_number += "6";
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            str_number += "7";
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            str_number += "8";
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            str_number += "9";
        }
        return str_number;
    }
}
