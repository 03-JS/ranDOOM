using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public float f_ammoBonusTimeLimit = 3f;

    public int i_fuel;
    public float f_fuelRechargeRate = 60f; // The seconds it takes for the chainsaw to regenerate a fuel can
    private List<string> str_keycards;
    public List<GameObject> go_obtainableWeapons;
    public List<GameObject> go_weapons;
    public GameObject go_chainsaw;
    // private int i_currentWeaponIndex;
    private int i_currentWeaponIndex;
    private GameObject go_currentlyEquippedWeapon;

    public int i_shells;
    private int i_shellsBonus;
    private float f_shellBonusTimer;
    private bool isShellBonusActive;
    private int i_maxShells = 8;

    public int i_cells;
    private int i_cellsBonus;
    private float f_cellBonusTimer;
    private bool isCellBonusActive;
    private int i_maxCells = 150;

    public int i_bullets;
    private int i_bulletsBonus;
    private float f_bulletBonusTimer;
    private bool isBulletBonusActive;
    private int i_maxBullets = 60;

    public int i_rockets;
    private int i_rocketsBonus;
    private float f_rocketBonusTimer;
    private bool isRocketBonusActive;
    private int i_maxRockets = 6;

    private UIManager uiManager;
    // private int[] i_recentWeaponsIndexes = { 1, 1 };
    private bool hadChainsawEquipped;

    private int i_ammoCapLevel;
    private float f_time;

    private int i_mouseWheelDelta = 0;
    private int i_previousWeaponIndex;

    private AudioManager audioManager;
    private PlayerHealthAndArmor player;

    void Start()
    {
        go_currentlyEquippedWeapon = go_weapons[0];
        i_currentWeaponIndex = 0;
        // i_previousWeaponIndex = -1;
        i_ammoCapLevel = 0;
        i_fuel = 1;
        // i_recentWeaponsIndexes[1] = 2;

        audioManager = FindObjectOfType<AudioManager>();

        player = FindObjectOfType<PlayerHealthAndArmor>();

        uiManager = FindObjectOfType<UIManager>();
        str_keycards = new List<string>();
        i_shells = 8;
        i_cells = 0;
        i_bullets = 0;
        i_rockets = 0;
        uiManager.tmp_shells.SetText("" + i_shells);

        uiManager.tmp_cells.SetText("" + i_cells);
        uiManager.tmp_cells.enabled = false;

        uiManager.tmp_bullets.SetText("" + i_bullets);
        uiManager.tmp_bullets.enabled = false; ;

        uiManager.tmp_rockets.SetText("" + i_rockets);
        uiManager.tmp_rockets.enabled = false;

        uiManager.tmp_fuel.SetText("" + i_fuel);
    }

    void Update()
    {
        AmmoBonusBehaviour();
        if (!player.isDead && !MenuInputManager.gameIsPaused)
        {
            CheckActiveWeapon();
            RechargeFuel();
            i_mouseWheelDelta += (int)Input.GetAxisRaw("Mouse ScrollWheel");
            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f)
            {
                if (i_mouseWheelDelta < 1)
                {
                    i_mouseWheelDelta = 1;
                }
                if (i_mouseWheelDelta > go_weapons.Count - 1)
                {
                    i_mouseWheelDelta = go_weapons.Count - 1;
                }
                SwitchToWeapon(i_mouseWheelDelta);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                SwitchToWeapon(0); // Shotgun
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                SwitchToWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                SwitchToWeapon(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                SwitchToWeapon(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                SwitchToWeapon(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                SwitchToWeapon(5);
            }
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick2Button2))
            {
                SwitchToWeapon(i_previousWeaponIndex);
            }
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick2Button3))
            {
                if (i_fuel > 0)
                {
                    SwitchToChainsaw();
                }
            }
        }
    }

    public void IncreaseAmmoCapacity()
    {
        i_maxShells += 4;
        if (i_maxShells > 24)
        {
            i_maxShells = 24;
        }
        i_maxBullets += 30;
        if (i_maxBullets > 180)
        {
            i_maxBullets = 180;
        }
        i_maxCells += 25;
        if (i_maxCells > 250)
        {
            i_maxCells = 250;
        }
        i_maxRockets += 2;
        if (i_maxRockets > 13)
        {
            i_maxRockets = 13;
        }
        i_ammoCapLevel++;
        if (i_ammoCapLevel > 4)
        {
            i_ammoCapLevel = 4;
        }
    }

    public void SetAmmoCapacityToMax()
    {
        i_maxShells = 24;
        i_maxCells = 250;
        i_maxBullets = 180;
        i_maxRockets = 13;
        i_ammoCapLevel = 4;
    }

    public void AddKeycard(GameObject key)
    {
        string[] keyName = key.name.Split(" ");
        string color = keyName[0];
        string type = keyName[1];
        if (color == "Yellow")
        {
            if (type == "Keycard")
            {
                uiManager.img_yellowKey.enabled = true;
            }
            else
            {
                uiManager.img_yellowSkull.enabled = true;
            }
        }
        if (color == "Blue")
        {
            if (type == "Keycard")
            {
                uiManager.img_blueKey.enabled = true;
            }
            else
            {
                uiManager.img_blueSkull.enabled = true;
            }
        }
        if (color == "Red")
        {
            if (type == "Keycard")
            {
                uiManager.img_redKey.enabled = true;
            }
            else
            {
                uiManager.img_redSkull.enabled = true;
            }
        }
        str_keycards.Add(color);
    }

    public bool HasKeycard(string str_keycard)
    {
        return str_keycards.Contains(str_keycard);
    }

    public void AddShells(int amount)
    {
        if (i_shells < i_maxShells)
        {
            isShellBonusActive = true;
            f_shellBonusTimer = 0;
            int dif = i_maxShells - i_shells;
            i_shells += amount;
            if (i_shells > i_maxShells)
            {
                i_shellsBonus += dif;
                i_shells = i_maxShells;
            }
            else
            {
                i_shellsBonus += amount;
            }
            uiManager.img_shellBonus.enabled = true;
            uiManager.tmp_shellBonus.SetText("+" + i_shellsBonus);
            uiManager.tmp_shells.SetText(i_shells + "");
        }
    }

    public void ConsumeShells(int amount)
    {
        i_shells -= amount;
        uiManager.tmp_shells.SetText(i_shells + "");
    }

    public bool isAtMaxShells()
    {
        return i_shells == i_maxShells;
    }

    public void AddCells(int amount)
    {
        if (i_cells < i_maxCells)
        {
            isCellBonusActive = true;
            f_cellBonusTimer = 0;
            int dif = i_maxCells - i_cells;
            i_cells += amount;
            if (i_cells > i_maxCells)
            {
                i_cellsBonus += dif;
                i_cells = i_maxCells;
            }
            else
            {
                i_cellsBonus += amount;
            }
            uiManager.tmp_cellBonus.SetText("+" + i_cellsBonus);
            uiManager.img_cellBonus.enabled = true;
            uiManager.tmp_cells.SetText("" + i_cells);
        }
    }

    public void ConsumeCells(int amount)
    {
        i_cells -= amount;
        uiManager.tmp_cells.SetText(i_cells + "");
    }

    public bool isAtMaxCells()
    {
        return i_cells == i_maxCells;
    }

    public void AddBullets(int amount)
    {
        if (i_bullets < i_maxBullets)
        {
            isBulletBonusActive = true;
            f_bulletBonusTimer = 0;
            int dif = i_maxBullets - i_bullets;
            i_bullets += amount;
            if (i_bullets > i_maxBullets)
            {
                i_bulletsBonus += dif;
                i_bullets = i_maxBullets;
            }
            else
            {
                i_bulletsBonus += amount;
            }
            uiManager.tmp_bulletBonus.SetText("+" + i_bulletsBonus);
            uiManager.img_bulletBonus.enabled = true;
            uiManager.tmp_bullets.SetText("" + i_bullets);
        }
    }

    public void ConsumeBullets(int amount)
    {
        i_bullets -= amount;
        uiManager.tmp_bullets.SetText(i_bullets + "");
    }

    public bool isAtMaxBullets()
    {
        return i_bullets == i_maxBullets;
    }

    public void AddRockets(int amount)
    {
        if (i_rockets < i_maxRockets)
        {
            isRocketBonusActive = true;
            f_rocketBonusTimer = 0;
            int dif = i_maxRockets - i_rockets;
            i_rockets += amount;
            if (i_rockets > i_maxRockets)
            {
                i_rocketsBonus += dif;
                i_rockets = i_maxRockets;
            }
            else
            {
                i_rocketsBonus += amount;
            }
            uiManager.tmp_rocketBonus.SetText("+" + i_rocketsBonus);
            uiManager.img_rocketBonus.enabled = true;
            uiManager.tmp_rockets.SetText("" + i_rockets);
        }
    }

    public void ConsumeRockets(int amount)
    {
        i_rockets -= amount;
        uiManager.tmp_rockets.SetText(i_rockets + "");
    }

    public bool isAtMaxRockets()
    {
        return i_rockets == i_maxRockets;
    }

    public void AmmoBonusBehaviour()
    {
        if (isShellBonusActive)
        {
            f_shellBonusTimer += Time.deltaTime;
            if (f_shellBonusTimer >= f_ammoBonusTimeLimit)
            {
                uiManager.tmp_shellBonus.SetText("");
                uiManager.img_shellBonus.enabled = false;
                i_shellsBonus = 0;
                isShellBonusActive = false;
            }
        }
        if (isCellBonusActive)
        {
            f_cellBonusTimer += Time.deltaTime;
            if (f_cellBonusTimer >= f_ammoBonusTimeLimit)
            {
                uiManager.tmp_cellBonus.SetText("");
                uiManager.img_cellBonus.enabled = false;
                i_cellsBonus = 0;
                isCellBonusActive = false;
            }
        }
        if (isBulletBonusActive)
        {
            f_bulletBonusTimer += Time.deltaTime;
            if (f_bulletBonusTimer >= f_ammoBonusTimeLimit)
            {
                uiManager.tmp_bulletBonus.SetText("");
                uiManager.img_bulletBonus.enabled = false;
                i_bulletsBonus = 0;
                isBulletBonusActive = false;
            }
        }
        if (isRocketBonusActive)
        {
            f_rocketBonusTimer += Time.deltaTime;
            if (f_rocketBonusTimer >= f_ammoBonusTimeLimit)
            {
                uiManager.tmp_rocketBonus.SetText("");
                uiManager.img_rocketBonus.enabled = false;
                i_rocketsBonus = 0;
                isRocketBonusActive = false;
            }
        }
    }

    private void SwitchToWeapon(int index)
    {
        if (index < go_weapons.Count)
        {
            // Debug.Log("Current weapon index: " + i_currentWeaponIndex);
            // Debug.Log("Previous weapon index: " + i_previousWeaponIndex);
            if (i_currentWeaponIndex != index)
            {
                i_previousWeaponIndex = i_currentWeaponIndex;
            }
            i_currentWeaponIndex = index;
            if (i_previousWeaponIndex != i_currentWeaponIndex || hadChainsawEquipped)
            {
                go_chainsaw.SetActive(false);
                if (i_previousWeaponIndex >= 0)
                {
                    go_weapons[i_previousWeaponIndex].SetActive(false);
                }
                go_currentlyEquippedWeapon = go_weapons[i_currentWeaponIndex];
                go_currentlyEquippedWeapon.SetActive(true);
                hadChainsawEquipped = false;
                /*
                if (i_nextWeaponIndex != 0)
                {
                    Debug.Log("Current weapon index: " + i_currentWeaponIndex);
                    Debug.Log("Next weapon index: " + i_nextWeaponIndex);
                    if (!hadChainsawEquipped)
                    {
                        if (i_currentWeaponIndex != 0)
                        {
                            i_recentWeaponsIndexes[0] = i_currentWeaponIndex;
                        }
                        Debug.Log("Weapon 1: " + i_recentWeaponsIndexes[0]);
                        if (i_nextWeaponIndex != 0)
                        {
                            i_recentWeaponsIndexes[1] = i_nextWeaponIndex;
                            if (i_recentWeaponsIndexes[1] == i_recentWeaponsIndexes[0])
                            {
                                i_recentWeaponsIndexes[1] = i_previousWeaponIndex;
                            }
                            i_previousWeaponIndex = i_recentWeaponsIndexes[1];
                        }
                        Debug.Log("Weapon 2: " + i_recentWeaponsIndexes[1]);
                    }
                    hadChainsawEquipped = false;
                }
                */
            }
        }
    }

    private void SwitchToChainsaw()
    {
        for (int i = 0; i < go_weapons.Count; i++)
        {
            go_weapons[i].SetActive(false);
        }
        go_chainsaw.SetActive(true);
        hadChainsawEquipped = true;
    }

    private void CheckActiveWeapon()
    {
        if (go_currentlyEquippedWeapon.name.ToUpper() == "SHOTGUN" || go_currentlyEquippedWeapon.name.ToUpper() == "SUPER SHOTGUN")
        {
            uiManager.EnableShellsInfo();
        }
        if (go_currentlyEquippedWeapon.name.ToUpper() == "CHAINGUN")
        {
            uiManager.EnableBulletsInfo();
        }
        if (go_currentlyEquippedWeapon.name.ToUpper() == "PLASMA RIFLE" || go_currentlyEquippedWeapon.name.ToUpper() == "BFG 9000")
        {
            uiManager.EnableCellsInfo();
        }
        if (go_currentlyEquippedWeapon.name.ToUpper() == "ROCKET LAUNCHER")
        {
            uiManager.EnableRocketsInfo();
        }
        if (go_chainsaw.activeInHierarchy)
        {
            uiManager.EnableFuelInfo();
        }
    }

    public void AddWeapon(string name)
    {
        foreach (GameObject weapon in go_obtainableWeapons)
        {
            if (weapon.name == name)
            {
                if (!go_weapons.Contains(weapon))
                {
                    go_weapons.Add(weapon);
                }
            }
        }
    }

    public void AddAllWeapons()
    {
        foreach (GameObject weapon in go_obtainableWeapons)
        {
            if (!go_weapons.Contains(weapon))
            {
                go_weapons.Add(weapon);
            }
        }
    }

    public int GetAmmoCapLevel()
    {
        return i_ammoCapLevel;
    }

    private void RechargeFuel()
    {
        if (i_fuel == 0)
        {
            f_time += Time.deltaTime;
            if (f_time >= f_fuelRechargeRate)
            {
                Debug.Log("Fuel added");
                audioManager.PlaySfx(9);
                i_fuel = 1;
                f_time = 0;
                uiManager.tmp_fuel.SetText(i_fuel + "");
            }
        }
        else
        {
            f_time = 0;
        }
    }

    public void AddFuel(int amount)
    {
        if (!IsAtMaxFuel())
        {
            audioManager.PlaySfx(9);
        }
        i_fuel += amount;
        if (i_fuel > 3)
        {
            i_fuel = 3;
        }
        uiManager.tmp_fuel.SetText(i_fuel + "");
    }

    public bool IsAtMaxFuel()
    {
        return i_fuel == 3;
    }

    public void DisableCurrentlyEquippedWeapon()
    {
        go_currentlyEquippedWeapon.SetActive(false);
        go_chainsaw.SetActive(false);
    }

}
