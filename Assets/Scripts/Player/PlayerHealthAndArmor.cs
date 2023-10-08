using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthAndArmor : MonoBehaviour
{
    public int i_maxHealth;
    public int i_maxArmor;
    public int i_health = 100;
    public int i_armor = 0;
    public float f_bonusTimeLimit;

    public GameObject go_painEffect;
    public Animator painAnimator;
    public Animator playerCamAnimator;

    [HideInInspector] public bool isDead;
    private bool isInvulnerable;
    private bool isGod;
    private int i_healthBonus;
    private int i_armorBonus;
    private float f_armorTimer;
    private float f_healthTimer;
    private bool isArmorTimerActive;
    private bool isHealthTimerActive;
    private CharacterController characterController;
    private Collider blocker;

    private UIManager uiManager;
    private PlayerInventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        uiManager.tmp_health.SetText("" + i_health);
        inventory = FindObjectOfType<PlayerInventory>();
        characterController = FindObjectOfType<CharacterController>();
        blocker = GameObject.Find("Blocker").GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isArmorTimerActive)
        {
            f_armorTimer += Time.deltaTime;
            if (f_armorTimer >= f_bonusTimeLimit)
            {
                f_armorTimer = 0;
                isArmorTimerActive = false;
                i_armorBonus = 0;
                uiManager.tmp_armorBonus.SetText("");
            }
        }
        if (isHealthTimerActive)
        {
            f_healthTimer += Time.deltaTime;
            if (f_healthTimer >= f_bonusTimeLimit)
            {
                f_healthTimer = 0;
                isHealthTimerActive = false;
                i_healthBonus = 0;
                uiManager.tmp_healthBonus.SetText("");
            }
        }
        //if (i_armor == 0)
        //{
        //    uiManager.img_armorIcon.enabled = false;
        //    uiManager.tmp_armor.SetText("");
        //}
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick2Button4))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void AddHealth(int amount)
    {
        if (i_health < i_maxHealth)
        {
            isHealthTimerActive = true;
            f_healthTimer = 0;
            int i_dif = i_maxHealth - i_health;
            i_health += amount;
            if (i_health > i_maxHealth)
            {
                i_healthBonus += i_dif;
                i_health = i_maxHealth;
            }
            else
            {
                i_healthBonus += amount;
            }
            uiManager.tmp_health.SetText(i_health + "");
            uiManager.tmp_healthBonus.SetText("+" + i_healthBonus);
        }
    }

    public void SubstractHealth(int amount)
    {
        if (i_health > 0)
        {
            i_health -= amount;
            uiManager.tmp_health.SetText(i_health + "");
        }
    }

    public void AddArmor(int amount)
    {
        if (i_armor < i_maxArmor)
        {
            isArmorTimerActive = true;
            f_armorTimer = 0;
            int i_dif = i_maxArmor - i_armor;
            i_armor += amount;
            if (i_armor > i_maxArmor)
            {
                i_armorBonus += i_dif;
                i_armor = i_maxArmor;
            }
            else
            {
                i_armorBonus += amount;
            }
            uiManager.tmp_armor.SetText(i_armor + "");
            uiManager.tmp_armorBonus.SetText("+" + i_armorBonus);
            uiManager.img_armorIcon.enabled = true;
        }
    }

    public void SubstractArmor(int amount)
    {
        if (i_armor > 0)
        {
            i_armor -= amount;
            if (i_armor <= 0)
            {
                i_armor = 0;
                uiManager.img_armorIcon.enabled = false;
                uiManager.tmp_armor.SetText("");
            }
            else
            {
                uiManager.tmp_armor.SetText(i_armor + "");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if ((!isInvulnerable && !isGod) && !isDead)
        {
            if (DamageResistPowerup.playerHasDamageResistance)
            {
                amount /= 2;
            }
            if (i_armor > 0)
            {
                int i_remainingDamage = (int)amount - i_armor;
                SubstractArmor((int)amount);
                if (i_remainingDamage > 0)
                {
                    SubstractHealth(i_remainingDamage);
                    if (i_health <= 0)
                    {
                        if (i_health <= -50)
                        {
                            PlayXDeathSound();
                        }
                        else
                        {
                            PlayDeathSound();
                        }
                        Death();
                    }
                }
                ReceivePain();
            }
            else
            {
                SubstractHealth((int)amount);
                if (i_health <= 0)
                {
                    if (i_health <= -50)
                    {
                        PlayXDeathSound();
                    }
                    else
                    {
                        PlayDeathSound();
                    }
                    Death();
                }
                else
                {
                    ReceivePain();
                }
            }
        }
    }

    private void ReceivePain()
    {
        if (Random.Range(0, 1) == 0)
        {
            FindObjectOfType<AudioManager>().PlaySfx(6); // Pain sfx
        }
        painAnimator.SetTrigger("Pain");
    }

    public bool IsAtMaxHealth()
    {
        return i_health == i_maxHealth;
    }

    public bool IsAtMaxArmor()
    {
        return i_armor == i_maxArmor;
    }

    public bool IsDead()
    {
        return i_health == 0;
    }

    private void PlayDeathSound()
    {
        FindObjectOfType<AudioManager>().PlaySfx(7); // Normal death SFX
    }

    private void PlayXDeathSound()
    {
        FindObjectOfType<AudioManager>().PlaySfx(8); // XDeath sound effect
    }

    private void Death()
    {
        characterController.enabled = false;
        blocker.enabled = false;
        isDead = true;
        playerCamAnimator.SetTrigger("Death");
        painAnimator.SetTrigger("Death");
        uiManager.img_healthIcon.enabled = false;
        uiManager.tmp_health.SetText("");
        uiManager.img_crosshair.enabled = false;
        uiManager.DisableUI();
        inventory.DisableCurrentlyEquippedWeapon();
        LevelStats.ResetStats();
    }

    public void SetGodMode(bool value)
    {
        if (value == true)
        {
            uiManager.SetInfoText("Degreelessness Mode On");
        }
        else
        {
            uiManager.SetInfoText("Degreelessness Mode Off");
        }
        isGod = value;
    }

    public void MakePlayerInvulnerable(bool value)
    {
        isInvulnerable = value;
        // Debug.Log("Is player invul?: " + isInvulnerable);
    }
}
