using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : MonoBehaviour
{
    public Transform tr_hittingPoint;
    public float f_range = 2f;
    public int i_damage = 20;
    public float f_damageRate = 0.07f;
    public Animator chainsawAnimator;
    public GameObject go_poofEffect;
    public GameObject go_bloodEffect;

    private float f_nextTimeToDamage = 0f;
    private float f_nextTimeUntilIdleSound;
    private float f_idleSoundRate;
    private AudioManager audioManager;
    private UIManager uiManager;
    private PlayerInventory inventory;
    private PlayerHealthAndArmor player;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        player = FindObjectOfType<PlayerHealthAndArmor>();
        uiManager = FindObjectOfType<UIManager>();
        f_nextTimeUntilIdleSound = 0f;
        f_idleSoundRate = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuInputManager.gameIsPaused)
        {
            // Attacking
            if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Joystick2Button1))
            {
                if (Time.time >= f_nextTimeToDamage)
                {
                    f_nextTimeToDamage = Time.time + f_damageRate;
                    Saw();
                }
            }
            else
            {
                chainsawAnimator.SetBool("shouldInterrupt", true);
                audioManager.StopWeaponSfx(7);
                audioManager.StopWeaponSfx(8);
                player.MakePlayerInvulnerable(false);
                if (Time.time >= f_nextTimeUntilIdleSound && inventory.i_fuel > 0)
                {
                    f_nextTimeUntilIdleSound = Time.time + f_idleSoundRate;
                    audioManager.PlaySfx(9);
                }
            }
        }
    }

    private void Saw()
    {
        if (inventory.i_fuel > 0)
        {
            chainsawAnimator.SetBool("shouldInterrupt", false);
            chainsawAnimator.SetTrigger("Saw");
            audioManager.StopSfx(9);
            RaycastHit hitInfo;
            int layerMask = LayerMask.GetMask("Default", "Demon");
            if (Physics.Raycast(tr_hittingPoint.transform.position, tr_hittingPoint.transform.forward, out hitInfo, f_range, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.transform.gameObject.tag == "Demon")
                {
                    if (hitInfo.transform.gameObject.GetComponentInChildren<DemonAI>().tier == "Fodder")
                    {
                        audioManager.PlayWeaponSfx(8);
                        DamageDemon(hitInfo, 1);
                    }
                    else
                    {
                        if (hitInfo.transform.gameObject.GetComponentInChildren<DemonAI>().tier == "Heavy")
                        {
                            if (inventory.i_fuel >= 3)
                            {
                                audioManager.PlayWeaponSfx(8);
                                DamageDemon(hitInfo, 3);
                            }
                            else
                            {
                                uiManager.SetInfoText("You need 3 charges to kill a heavy demon");
                                audioManager.PlayWeaponSfx(7);
                            }
                        }
                        else
                        {
                            uiManager.SetInfoText("You can't chainsaw a super heavy demon");
                            audioManager.PlayWeaponSfx(7);
                        }
                    }
                }
                else
                {
                    if (hitInfo.collider.CompareTag("Explosive Barrel"))
                    {
                        hitInfo.collider.GetComponent<ExplosiveBarrel>().TakeDamage(i_damage);
                    }
                    player.MakePlayerInvulnerable(false);
                    audioManager.PlayWeaponSfx(7);
                    Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                }
            }
            else
            {
                player.MakePlayerInvulnerable(false);
                audioManager.PlayWeaponSfx(7);
            }
        }
        else
        {
            uiManager.SetInfoText("No fuel left");
            // audioManager.PlaySfx(5);
        }
    }

    void OnEnable()
    {
        audioManager = FindObjectOfType<AudioManager>();
        f_nextTimeUntilIdleSound += 0.2f;
        f_nextTimeToDamage = 0;
    }

    void OnDisable()
    {
        audioManager.StopWeaponSfx(7);
        audioManager.StopWeaponSfx(8);
        audioManager.StopSfx(9);
    }

    private void GiveAmmo()
    {
        if (inventory.GetAmmoCapLevel() == 0)
        {
            inventory.AddShells(10);
            inventory.AddCells(75);
            inventory.AddBullets(30);
            inventory.AddRockets(4);
        }
        if (inventory.GetAmmoCapLevel() == 1)
        {
            inventory.AddShells(10);
            inventory.AddCells(90);
            inventory.AddBullets(50);
            inventory.AddRockets(5);
        }
        if (inventory.GetAmmoCapLevel() == 2)
        {
            inventory.AddShells(10);
            inventory.AddCells(105);
            inventory.AddBullets(60);
            inventory.AddRockets(5);
        }
        if (inventory.GetAmmoCapLevel() == 3)
        {
            inventory.AddShells(15);
            inventory.AddCells(120);
            inventory.AddBullets(80);
            inventory.AddRockets(6);
        }
        if (inventory.GetAmmoCapLevel() == 4)
        {
            inventory.AddShells(15);
            inventory.AddCells(135);
            inventory.AddBullets(90);
            inventory.AddRockets(6);
        }
    }

    private void DamageDemon(RaycastHit hitInfo, int fuel)
    {
        Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
        hitInfo.transform.gameObject.GetComponent<DemonHealth>().TakeDamage(i_damage, true, inventory.gameObject);
        hitInfo.transform.gameObject.GetComponentInChildren<DemonAI>().ReceivePain();
        player.MakePlayerInvulnerable(true);
        if (hitInfo.transform.gameObject.GetComponent<DemonHealth>().isDead)
        {
            // Debug.Log("Consumed " + fuel + " fuel can");
            GiveAmmo();
            inventory.i_fuel -= fuel;
            uiManager.tmp_fuel.SetText(inventory.i_fuel + "");
            player.MakePlayerInvulnerable(false);
        }
    }

}
