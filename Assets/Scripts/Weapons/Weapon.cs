using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform tr_fireOrigin;
    public GameObject muzzleLight;
    public bool hasProjectile;
    public GameObject go_projectile;
    public bool usesShells;
    public bool usesBullets;
    public bool usesPlasma;
    public bool usesRockets;
    public bool isFullAuto;
    public int i_damage = 0;
    public float knockback = 500f;
    public int i_ammoPerShot = 1;
    public float f_rateOfFire;
    public float f_verticalBulletSpread;
    public float f_horizontalBulletSpread;
    public float f_chargeTimeLimit = 0.07f;
    public GameObject go_poofEffect;
    public GameObject go_bloodEffect;
    public Animator weaponAnimator;

    private AudioManager audioManager;
    private UIManager uiManager;
    private PlayerInventory inventory;
    private PlayerHealthAndArmor player;
    private float f_nextTimeToFire = 0f;
    private float f_chargeTime = 0f;
    private bool isCharging;
    private bool shouldInterrupt;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        uiManager = FindObjectOfType<UIManager>();
        inventory = FindObjectOfType<PlayerInventory>();
        player = FindObjectOfType<PlayerHealthAndArmor>();
    }

    void Update()
    {
        if (!player.isDead && !MenuInputManager.gameIsPaused)
        {
            if (isFullAuto)
            {
                if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.Joystick2Button1)) && Time.time >= f_nextTimeToFire)
                {
                    f_nextTimeToFire = Time.time + f_rateOfFire;
                    Shoot();
                }
            }
            else
            {
                if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Joystick2Button1)) && Time.time >= f_nextTimeToFire)
                {
                    f_nextTimeToFire = Time.time + f_rateOfFire;
                    Shoot();
                }
            }
            if (isCharging)
            {
                f_chargeTime += Time.deltaTime;
                if (f_chargeTime >= f_chargeTimeLimit)
                {
                    isCharging = false;
                    f_chargeTime = 0;
                    FireBFGProjectile();
                }
            }
        }
    }

    private void Shoot()
    {
        if (hasProjectile)
        {
            if (usesPlasma)
            {
                if (gameObject.name.ToUpper() == "BFG 9000")
                {
                    if (inventory.i_cells >= i_ammoPerShot)
                    {
                        shouldInterrupt = false;
                        weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
                        weaponAnimator.SetTrigger("Shoot");
                        PlayFireSound();
                        isCharging = true;
                        inventory.ConsumeCells(i_ammoPerShot);
                    }
                    else
                    {
                        audioManager.PlaySfx(5);
                        uiManager.SetInfoText("Not enough cells left");
                    }
                }
                else
                {
                    if (inventory.i_cells >= i_ammoPerShot)
                    {
                        shouldInterrupt = false;
                        weaponAnimator.SetFloat("randomMuzzleValue", Random.Range(0, 2));
                        weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
                        weaponAnimator.SetTrigger("Shoot");
                        PlayFireSound();
                        inventory.ConsumeCells(i_ammoPerShot);
                        Instantiate(go_projectile, tr_fireOrigin.position, tr_fireOrigin.rotation).GetComponent<Projectile>().SetOriginatorCollider(player.gameObject.GetComponent<CharacterController>());
                    }
                    else
                    {
                        audioManager.PlaySfx(5);
                        uiManager.SetInfoText("No cells left");
                    }
                }
            }
            if (usesRockets)
            {
                if (inventory.i_rockets >= i_ammoPerShot)
                {
                    shouldInterrupt = false;
                    weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
                    weaponAnimator.SetTrigger("Shoot");
                    PlayFireSound();
                    inventory.ConsumeRockets(i_ammoPerShot);
                    Instantiate(go_projectile, tr_fireOrigin.position, tr_fireOrigin.rotation).GetComponent<Projectile>().SetOriginatorCollider(player.gameObject.GetComponent<CharacterController>());
                }
                else
                {
                    audioManager.PlaySfx(5);
                    uiManager.SetInfoText("No rockets left");
                }
            }
        }
        else
        {
            if (usesShells)
            {
                if (gameObject.name.ToUpper() == "SHOTGUN")
                {
                    if (inventory.i_shells >= i_ammoPerShot)
                    {
                        shouldInterrupt = false;
                        weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
                        weaponAnimator.SetTrigger("Shoot");
                        PlayFireSound();
                        inventory.ConsumeShells(i_ammoPerShot);
                        Vector3 v3_shootDirection = tr_fireOrigin.transform.forward;
                        RaycastHit hitInfo;
                        int layerMask = LayerMask.GetMask("Default", "Demon", "Cyberdemon");
                        for (int i = 0; i < 6; i++)
                        {
                            if (i <= 2)
                            {
                                v3_shootDirection.x += Random.Range(-f_horizontalBulletSpread + 0.05f, f_horizontalBulletSpread - 0.05f);
                                v3_shootDirection.z += Random.Range(-f_horizontalBulletSpread + 0.05f, f_horizontalBulletSpread - 0.05f);
                                v3_shootDirection.y += Random.Range(-f_verticalBulletSpread, f_verticalBulletSpread);
                            }
                            else
                            {
                                v3_shootDirection.x += Random.Range(-f_horizontalBulletSpread, f_horizontalBulletSpread);
                                v3_shootDirection.z += Random.Range(-f_horizontalBulletSpread, f_horizontalBulletSpread);
                                v3_shootDirection.y += Random.Range(-f_verticalBulletSpread, f_verticalBulletSpread);
                            }
                            if (Physics.Raycast(tr_fireOrigin.position, v3_shootDirection, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                            {
                                if (!hitInfo.transform.gameObject.tag.Equals("Demon") && !hitInfo.transform.gameObject.tag.Equals("Player"))
                                {
                                    Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                                }
                                if (hitInfo.transform.gameObject.tag == "Demon")
                                {
                                    Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                                    hitInfo.transform.gameObject.GetComponent<DemonHealth>().TakeDamage(i_damage, true, player.gameObject);
                                }
                                if (hitInfo.transform.gameObject.tag == "Player")
                                {
                                    Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                                    hitInfo.transform.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_damage);
                                }
                                if (hitInfo.transform.gameObject.tag == "Explosive Barrel")
                                {
                                    Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                                    hitInfo.transform.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(i_damage);
                                }
                            }
                        }
                    }
                    else
                    {
                        audioManager.PlaySfx(5);
                        uiManager.SetInfoText("No shells left");
                    }
                }
                else
                {
                    if (inventory.i_shells >= i_ammoPerShot)
                    {
                        shouldInterrupt = false;
                        weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
                        weaponAnimator.SetTrigger("Shoot");
                        PlayFireSound();
                        inventory.ConsumeShells(i_ammoPerShot);
                        Vector3 v3_shootDirection = tr_fireOrigin.transform.forward;
                        RaycastHit hitInfo;
                        int layerMask = LayerMask.GetMask("Default", "Demon", "Cyberdemon");
                        for (int i = 0; i < 20; i++)
                        {
                            // Makes the first 10 pellets have less spread to make the gun feel more accurate
                            if (i <= 9)
                            {
                                v3_shootDirection.x += Random.Range(-f_horizontalBulletSpread + 0.05f, f_horizontalBulletSpread - 0.05f);
                                v3_shootDirection.z += Random.Range(-f_horizontalBulletSpread + 0.05f, f_horizontalBulletSpread - 0.05f);
                                v3_shootDirection.y += Random.Range(-f_verticalBulletSpread, f_verticalBulletSpread);
                            }
                            else
                            {
                                v3_shootDirection.x += Random.Range(-f_horizontalBulletSpread, f_horizontalBulletSpread);
                                v3_shootDirection.z += Random.Range(-f_horizontalBulletSpread, f_horizontalBulletSpread);
                                v3_shootDirection.y += Random.Range(-f_verticalBulletSpread, f_verticalBulletSpread);
                            }
                            if (Physics.Raycast(tr_fireOrigin.position, v3_shootDirection, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                            {
                                if (!hitInfo.transform.gameObject.tag.Equals("Demon") && !hitInfo.transform.gameObject.tag.Equals("Player"))
                                {
                                    Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                                }
                                if (hitInfo.transform.gameObject.tag == "Demon")
                                {
                                    Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                                    hitInfo.transform.gameObject.GetComponent<DemonHealth>().TakeDamage(i_damage, true, player.gameObject);
                                }
                                if (hitInfo.transform.gameObject.tag == "Player")
                                {
                                    Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                                    hitInfo.transform.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_damage);
                                }
                                if (hitInfo.transform.gameObject.tag == "Explosive Barrel")
                                {
                                    Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                                    hitInfo.transform.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(i_damage);
                                }
                            }
                        }
                    }
                    else
                    {
                        audioManager.PlaySfx(5);
                        uiManager.SetInfoText("Not enough shells left");
                    }
                }
            }
            if (usesBullets)
            {
                if (inventory.i_bullets >= i_ammoPerShot)
                {
                    shouldInterrupt = false;
                    weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
                    weaponAnimator.SetTrigger("Shoot");
                    PlayFireSound();
                    inventory.ConsumeBullets(i_ammoPerShot);
                    Vector3 v3_shootDirection = tr_fireOrigin.transform.forward;
                    RaycastHit hitInfo;
                    int layerMask = LayerMask.GetMask("Default", "Demon", "Cyberdemon");
                    if (Physics.Raycast(tr_fireOrigin.position, v3_shootDirection, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        if (!hitInfo.transform.gameObject.tag.Equals("Demon") && !hitInfo.transform.gameObject.tag.Equals("Player"))
                        {
                            Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                        }
                        if (hitInfo.transform.gameObject.tag == "Demon")
                        {
                            Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                            hitInfo.transform.gameObject.GetComponent<DemonHealth>().TakeDamage(i_damage, true, player.gameObject);
                        }
                        if (hitInfo.transform.gameObject.tag == "Player")
                        {
                            Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                            hitInfo.transform.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_damage);
                        }
                        if (hitInfo.transform.gameObject.tag == "Explosive Barrel")
                        {
                            Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                            hitInfo.transform.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(i_damage);
                        }
                    }
                }
                else
                {
                    audioManager.PlaySfx(5);
                    uiManager.SetInfoText("No bullets left");
                }
            }
        }
    }

    private void FireBFGProjectile()
    {
        Instantiate(go_projectile, tr_fireOrigin.position, tr_fireOrigin.rotation).GetComponent<Projectile>().SetOriginatorCollider(player.gameObject.GetComponent<CharacterController>());
    }

    public void PlayFireSound()
    {
        if (usesShells)
        {
            if (gameObject.name.ToUpper() == "SHOTGUN")
            {
                audioManager.PlayWeaponSfx(5); // Shotgun sound effect
            }
            else
            {
                audioManager.PlayWeaponSfx(3); // SSG sound effect
            }
        }
        if (usesBullets)
        {
            audioManager.PlayWeaponSfx(0); // Chaingun sound effect
        }
        if (usesRockets)
        {
            audioManager.PlayWeaponSfx(4); // RL sound effect
        }
        if (usesPlasma)
        {
            if (gameObject.name.ToUpper() == "PLASMA RIFLE")
            {
                audioManager.PlayWeaponSfx(1); // Plasma Rifle sound effect
            }
            else
            {
                audioManager.PlayWeaponSfx(2); // BFG sound effect
            }
        }
    }

    void OnEnable()
    {
        if (gameObject.name.ToUpper() != "BFG 9000")
        {
            f_nextTimeToFire = Time.time + 0.15f;
        }
    }

    void OnDisable()
    {
        if (weaponAnimator != null)
        {
            shouldInterrupt = true;
            weaponAnimator.SetBool("shouldInterrupt", shouldInterrupt);
            DisableLight();
        }
    }

    // These 3 methods only exist because of the SSG. They are used by some of its animations
    public void PlayOpeningSound()
    {
        audioManager.PlayReloadSFX(0);
    }

    public void PlayLoadSound()
    {
        audioManager.PlayReloadSFX(1);
    }

    public void PlayClosingSound()
    {
        audioManager.PlayReloadSFX(2);
    }

    public void EnableLight()
    {
        // muzzleLight.SetActive(true);
    }

    public void DisableLight()
    {
        // muzzleLight.SetActive(false);
    }

    public void SetShellsValue()
    {
        if (inventory.i_shells < 1)
        {
            Invoke("StopShotgunSFX", 0.25f);
            weaponAnimator.SetTrigger("NoReload");
        }
        if (inventory.i_shells < 2 && gameObject.name == "Super Shotgun")
        {
            weaponAnimator.SetTrigger("NoReload");
        }
    }

    private void StopShotgunSFX()
    {
        audioManager.StopWeaponSfx(5);
    }

}
