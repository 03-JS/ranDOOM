using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollisionsManager : MonoBehaviour
{
    private bool isNearDoor;
    private PlayerInventory inventory;
    private AudioManager audioManager;
    private PlayerHealthAndArmor healthAndArmor;
    private UIManager uiManager;
    private GameObject go_door;

    void Start()
    {
        inventory = FindObjectOfType<PlayerInventory>();
        audioManager = FindObjectOfType<AudioManager>();
        healthAndArmor = FindObjectOfType<PlayerHealthAndArmor>();
        uiManager = FindObjectOfType<UIManager>();
    }

    // Switch detection
    void OnTriggerStay(Collider other)
    {
        if (other.name.ToLower().Contains("switch"))
        {
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Joystick2Button0))
            {
                other.gameObject.GetComponent<Switch>().TurnOn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.tag != "Enviornment" && other.gameObject.name != "Powerup Spawner")
        {
            Debug.Log(other.gameObject.name);
        }*/

        string[] itemName = other.gameObject.name.Split("(");
        // Debug.Log(itemName[0]);

        // Door detection
        if (other.name.ToLower().Contains("door"))
        {
            go_door = other.gameObject;
            isNearDoor = true;
        }

        // Sound effects & powerup behaviour
        if (other.gameObject.tag == "Powerup")
        {
            if (other.name == "Megasphere(Clone)")
            {
                if (!healthAndArmor.IsAtMaxArmor())
                {
                    uiManager.SetInfoText("Picked up a " + itemName[0]);
                    PickUpPowerup(other.gameObject);
                }
                if (!healthAndArmor.IsAtMaxHealth())
                {
                    uiManager.SetInfoText("Picked up a " + itemName[0]);
                    PickUpPowerup(other.gameObject);
                }
                healthAndArmor.AddArmor(150);
                healthAndArmor.AddHealth(200);
            }
            else
            {
                if (other.name == "Soul Sphere(Clone)")
                {
                    if (!healthAndArmor.IsAtMaxHealth())
                    {
                        uiManager.SetInfoText("Picked up a " + itemName[0]);
                        PickUpPowerup(other.gameObject);
                    }
                    healthAndArmor.AddHealth(200);
                }
                else
                {
                    if (other.name == "Partial Invincibility(Clone)")
                    {
                        uiManager.SetInfoText("Picked up a " + itemName[0]);
                        gameObject.AddComponent<DamageResistPowerup>();
                        PickUpPowerup(other.gameObject);
                    }
                    else
                    {
                        if (other.name == "Backpack")
                        {
                            uiManager.SetInfoText("Your ammo capacity has been upgraded");
                            LevelStats.itemsFound++;
                            inventory.IncreaseAmmoCapacity();
                            PickUpPowerup(other.gameObject);
                        }
                        else
                        {
                            if (other.name == "Berserk(Clone)")
                            {
                                uiManager.SetInfoText("Berserk!");
                                gameObject.AddComponent<QuadDamagePowerup>();
                                PickUpPowerup(other.gameObject);
                                healthAndArmor.AddHealth(100);
                            }
                        }
                    }
                }
            }
        }

        // Weapons
        if (other.tag == "Weapon")
        {
            uiManager.SetInfoText("You got the " + itemName[0]);
            PickUpWeapon(other.gameObject);
            inventory.AddWeapon(itemName[0]);
            LevelStats.itemsFound++;
            if (itemName[0] == "Shotgun" || itemName[0] == "Super Shotgun")
            {
                inventory.AddShells(8);
            }
            if (itemName[0] == "Plasma Rifle")
            {
                inventory.AddCells(90);
            }
            if (itemName[0] == "BFG 9000")
            {
                inventory.AddCells(150);
            }
            if (itemName[0] == "Chaingun")
            {
                inventory.AddBullets(60);
            }
            if (itemName[0] == "Rocket Launcher")
            {
                inventory.AddRockets(4);
            }
        }

        // Pickups
        if (other.tag == "Pickup")
        {
            if (itemName[0] == "Armor" || itemName[0] == "Armor Bonus")
            {
                if (!healthAndArmor.IsAtMaxArmor())
                {
                    PickUpItem(other.gameObject);
                }
            }
            else
            {
                if (itemName[0] == "Health Bonus" || itemName[0] == "Stimpack" || itemName[0] == "Medkit")
                {
                    if (!healthAndArmor.IsAtMaxHealth())
                    {
                        PickUpItem(other.gameObject);
                    }
                }
                else
                {
                    if (itemName[0] == "Shells")
                    {
                        if (!inventory.isAtMaxShells())
                        {
                            inventory.AddShells(8);
                            PickUpItem(other.gameObject);
                        }
                    }
                    else
                    {
                        if (itemName[0] == "Bullets")
                        {
                            if (!inventory.isAtMaxBullets()) // && inventory.hasWeapon()
                            {
                                inventory.AddBullets(30);
                                PickUpItem(other.gameObject);
                            }
                        }
                        else
                        {
                            if (itemName[0] == "Cells")
                            {
                                if (!inventory.isAtMaxCells()) // && inventory.hasWeapon()
                                {
                                    inventory.AddCells(30);
                                    PickUpItem(other.gameObject);
                                }
                            }
                            else
                            {
                                if (itemName[0] == "Rocket")
                                {
                                    if (!inventory.isAtMaxRockets()) // && inventory.hasWeapon()
                                    {
                                        inventory.AddRockets(1);
                                        PickUpItem(other.gameObject);
                                    }
                                }
                                else
                                {
                                    if (itemName[0] == "Chainsaw")
                                    {
                                        if (!inventory.IsAtMaxFuel())
                                        {
                                            uiManager.SetInfoText("Picked up some " + itemName[0] + " fuel");
                                            inventory.AddFuel(1);
                                            Destroy(other.gameObject);
                                        }
                                    }
                                    else
                                    {
                                        PickUpItem(other.gameObject);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

        // Health
        if (itemName[0] == "Health Bonus")
        {
            healthAndArmor.AddHealth(10);
        }
        if (itemName[0] == "Stimpack")
        {
            healthAndArmor.AddHealth(20);
        }
        if (itemName[0] == "Medkit")
        {
            healthAndArmor.AddHealth(50);
        }

        // Armor
        if (itemName[0] == "Armor Bonus")
        {
            healthAndArmor.AddArmor(5);
        }
        if (itemName[0] == "Armor")
        {
            healthAndArmor.AddArmor(50);
        }

        // Keycard management
        if (other.tag == "Key")
        {
            // Keycards don't have any parenthesis in their names, that's why I made a new array just for them
            string[] keyName = other.name.Split(" ");
            uiManager.SetInfoText("Picked up a " + keyName[0] + " " + keyName[1]);
            if (keyName[0] == "Yellow" || keyName[0] == "Blue" || keyName[0] == "Red")
            {
                inventory.AddKeycard(other.gameObject);
            }
            PickUpItem(other.gameObject);
            LevelStats.itemsFound++;
        }

        // Secrets
        if (other.name == "Secret Trigger")
        {
            uiManager.SetInfoText("A secret is revealed!");
            audioManager.PlaySfx(3); // "A secret is revealed!" sound effect
            Destroy(other.gameObject);
            LevelStats.secretsFound++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.ToLower().Contains("door"))
        {
            isNearDoor = false;
        }
    }

    void Update()
    {
        if (go_door != null)
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick2Button0)) && isNearDoor)
            {
                go_door.GetComponent<Door>().Open(true);
            }
        }

    }

    public void PickUpItem(GameObject item)
    {
        audioManager.PlaySfx(2); // Pickup sound effect
        Destroy(item);
    }

    public void PickUpPowerup(GameObject powerup)
    {
        audioManager.PlaySfx(0); // Powerup sound effect
        Destroy(powerup);
    }

    public void PickUpWeapon(GameObject weapon)
    {
        audioManager.PlaySfx(1); // Weapon pickup sound effect
        Destroy(weapon);
    }

}