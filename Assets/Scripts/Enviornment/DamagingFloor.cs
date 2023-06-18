using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingFloor : MonoBehaviour
{
    public int i_damage;
    public float f_timeBetweenDamage;

    private float f_timer;
    private bool isPlayerOnTrigger;
    private PlayerHealthAndArmor player;

    // Update is called once per frame
    void Update()
    {
        if (isPlayerOnTrigger)
        {
            f_timer += Time.deltaTime;
            if (f_timer >= f_timeBetweenDamage)
            {
                player.TakeDamage(i_damage);
                f_timer = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnTrigger = true;
            player = other.GetComponent<PlayerHealthAndArmor>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnTrigger = false;
            f_timer = 0;
        }
    }

}
