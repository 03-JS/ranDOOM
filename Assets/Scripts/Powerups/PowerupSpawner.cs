using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public GameObject[] powerups;
    public bool respawns;
    public GameObject respawnEffect;

    private bool shouldRespawn;
    private bool useRespawnEffect;

    // Start is called before the first frame update
    void Start()
    {
        useRespawnEffect = false;
        SpawnPowerup();
    }

    void Update()
    {
        if (respawns)
        {
            if (shouldRespawn && transform.childCount == 0)
            {
                shouldRespawn = false;
                useRespawnEffect = true;
                Invoke("SpawnPowerup", Random.Range(10f, 61f));
            }
        }
    }

    private void SpawnPowerup()
    {
        System.Random random = new System.Random();
        int i_randomNumber = random.Next(0, powerups.Length);
        Instantiate(powerups[i_randomNumber],
            transform.position, Quaternion.identity, transform);
        if (respawns)
        {
            shouldRespawn = true;
            if (useRespawnEffect)
            {
                Destroy(Instantiate(respawnEffect, transform.position, Quaternion.identity), 1f);
            }
        }
    }
}
