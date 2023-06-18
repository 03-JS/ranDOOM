using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public float f_textTimeLimit = 10f;
    public TextMeshProUGUI tmp_infoText;
    [Space]
    public TextMeshProUGUI tmp_armor;
    public TextMeshProUGUI tmp_armorBonus;
    public TextMeshProUGUI tmp_health;
    public TextMeshProUGUI tmp_healthBonus;
    public Image img_crosshair;
    public Image img_armorIcon;
    public Image img_healthIcon;
    public Image[] powerupIcons;
    [Space]
    public Image img_yellowKey;
    public Image img_yellowSkull;
    public Image img_blueKey;
    public Image img_blueSkull;
    public Image img_redKey;
    public Image img_redSkull;
    [Space]
    public TextMeshProUGUI tmp_shellBonus;
    public Image img_shellBonus;
    public TextMeshProUGUI tmp_bulletBonus;
    public Image img_bulletBonus;
    public TextMeshProUGUI tmp_cellBonus;
    public Image img_cellBonus;
    public TextMeshProUGUI tmp_rocketBonus;
    public Image img_rocketBonus;
    [Space]
    public TextMeshProUGUI tmp_shells;
    public Image img_shellsIcon;
    public TextMeshProUGUI tmp_cells;
    public Image img_cellsIcon;
    public TextMeshProUGUI tmp_bullets;
    public Image img_bulletsIcon;
    public TextMeshProUGUI tmp_rockets;
    public Image img_rocketsIcon;
    public TextMeshProUGUI tmp_fuel;
    public Image img_fuelIcon;

    private float f_time;
    private bool startUITimer;

    // Start is called before the first frame update
    void Start()
    {
        tmp_infoText.SetText("");

        foreach (Image powerupIcon in powerupIcons)
        {
            powerupIcon.enabled = false;
        }

        img_yellowKey.enabled = false;
        img_yellowSkull.enabled = false;
        img_blueKey.enabled = false;
        img_blueSkull.enabled = false;
        img_redKey.enabled = false;
        img_redSkull.enabled = false;

        img_shellBonus.enabled = false;
        img_cellBonus.enabled = false;
        img_bulletBonus.enabled = false;
        img_rocketBonus.enabled = false;
        img_armorIcon.enabled = false;

        tmp_armor.SetText("");
        tmp_armorBonus.SetText("");
        tmp_healthBonus.SetText("");
        tmp_shellBonus.SetText("");
        tmp_cellBonus.SetText("");
        tmp_bulletBonus.SetText("");
        tmp_rocketBonus.SetText("");
        img_bulletsIcon.enabled = false;
        img_rocketsIcon.enabled = false;
        img_cellsIcon.enabled = false;
        img_fuelIcon.enabled = false;
    }

    public void SetInfoText(string text)
    {
        startUITimer = true;
        f_time = 0;
        tmp_infoText.SetText(text);
    }

    // Update is called once per frame
    void Update()
    {
        if (startUITimer)
        {
            f_time += Time.deltaTime;
            if (f_time >= f_textTimeLimit)
            {
                startUITimer = false;
                f_time = 0;
                tmp_infoText.SetText("");
            }
        }
    }

    public void EnableShellsInfo()
    {
        tmp_shells.enabled = true;
        tmp_cells.enabled = false;
        tmp_rockets.enabled = false;
        tmp_bullets.enabled = false;
        tmp_fuel.enabled = false;

        img_shellsIcon.enabled = true;
        img_cellsIcon.enabled = false;
        img_bulletsIcon.enabled = false;
        img_rocketsIcon.enabled = false;
        img_fuelIcon.enabled = false;
    }

    public void EnableCellsInfo()
    {
        tmp_shells.enabled = false;
        tmp_cells.enabled = true;
        tmp_rockets.enabled = false;
        tmp_bullets.enabled = false;
        tmp_fuel.enabled = false;

        img_shellsIcon.enabled = false;
        img_cellsIcon.enabled = true;
        img_bulletsIcon.enabled = false;
        img_rocketsIcon.enabled = false;
        img_fuelIcon.enabled = false;
    }

    public void EnableBulletsInfo()
    {
        tmp_shells.enabled = false;
        tmp_cells.enabled = false;
        tmp_rockets.enabled = false;
        tmp_bullets.enabled = true;
        tmp_fuel.enabled = false;

        img_shellsIcon.enabled = false;
        img_cellsIcon.enabled = false;
        img_bulletsIcon.enabled = true;
        img_rocketsIcon.enabled = false;
        img_fuelIcon.enabled = false;
    }

    public void EnableRocketsInfo()
    {
        tmp_shells.enabled = false;
        tmp_cells.enabled = false;
        tmp_rockets.enabled = true;
        tmp_bullets.enabled = false;
        tmp_fuel.enabled = false;

        img_shellsIcon.enabled = false;
        img_cellsIcon.enabled = false;
        img_bulletsIcon.enabled = false;
        img_rocketsIcon.enabled = true;
        img_fuelIcon.enabled = false;
    }

    public void EnableFuelInfo()
    {
        tmp_shells.enabled = false;
        tmp_cells.enabled = false;
        tmp_rockets.enabled = false;
        tmp_bullets.enabled = false;
        tmp_fuel.enabled = true;

        img_shellsIcon.enabled = false;
        img_cellsIcon.enabled = false;
        img_bulletsIcon.enabled = false;
        img_rocketsIcon.enabled = false;
        img_fuelIcon.enabled = true;
    }

    public void DisableUI()
    {
        tmp_shells.enabled = false;
        tmp_cells.enabled = false;
        tmp_rockets.enabled = false;
        tmp_bullets.enabled = false;
        tmp_shellBonus.enabled = false;
        tmp_cellBonus.enabled = false;
        tmp_rocketBonus.enabled = false;
        tmp_bulletBonus.enabled = false;
        tmp_armorBonus.enabled = false;
        tmp_healthBonus.enabled = false;
        tmp_fuel.enabled = false;

        img_shellsIcon.enabled = false;
        img_shellBonus.enabled = false;
        img_cellsIcon.enabled = false;
        img_cellBonus.enabled = false;
        img_bulletsIcon.enabled = false;
        img_bulletBonus.enabled = false;
        img_rocketsIcon.enabled = false;
        img_rocketBonus.enabled = false;
        img_fuelIcon.enabled = false;

        img_blueKey.enabled = false;
        img_blueSkull.enabled = false;
        img_yellowKey.enabled = false;
        img_yellowSkull.enabled = false;
        img_redKey.enabled = false;
        img_redSkull.enabled = false;

        foreach (Image powerupIcon in powerupIcons)
        {
            powerupIcon.enabled = false;
        }
    }

    public void EnablePowerupIcon(string powerupName)
    {
        if (powerupName == "Berserk")
        {
            powerupIcons[0].enabled = true;
        }
        if (powerupName == "Partial Invincibility")
        {
            powerupIcons[1].enabled = true;
        }
        if (powerupName == "Rad Suit")
        {
            powerupIcons[2].enabled = true;
        }
    }

    public void DisablePowerupIcon(string powerupName)
    {
        if (powerupName == "Berserk")
        {
            if (powerupIcons[0] != null)
            {
                powerupIcons[0].enabled = false; 
            }
        }
        if (powerupName == "Partial Invincibility")
        {
            if (powerupIcons[1] != null)
            {
                powerupIcons[1].enabled = false; 
            }
        }
        if (powerupName == "Rad Suit")
        {
            if (powerupIcons[2] != null)
            {
                powerupIcons[2].enabled = false; 
            }
        }
    }
}
