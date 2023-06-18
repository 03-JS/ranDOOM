using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSpawner : MonoBehaviour
{
    public GameObject[] fodderDemons;
    public GameObject[] extendedFodderDemons;
    public GameObject[] heavyDemons;
    public GameObject[] extendedHeavyDemons;
    // public GameObject[] flyingHeavyDemons;
    public GameObject[] thinSuperHeavyDemons;
    public GameObject[] wideSuperHeavyDemons;
    public float f_delay = 0f;
    public bool dontDestroyAfterSpawn;
    public bool spawnsFodder;
    public bool spawnsExtendedFodder;
    public bool spawnsHeavy;
    public bool spawnsExtendedHeavy;
    // public bool spawnsFlyingHeavy;
    public bool spawnsThinSuperHeavy;
    public bool spawnsWideSuperHeavy;
    public bool respawns;
    [Space]
    public bool spawnsAggro;
    public bool lookAtTarget;
    public bool lookOppositeToTarget;
    [Space]
    public bool useTeleportEffect;
    public GameObject go_teleportEffect;

    private GameObject demon;
    private bool shouldRespawn;

    void OnEnable()
    {
        Invoke("SpawnDemon", f_delay);
    }

    void Update()
    {
        if (demon != null)
        {
            if (respawns)
            {
                if (demon.GetComponent<DemonHealth>().isDead && shouldRespawn)
                {
                    shouldRespawn = false;
                    Invoke("SpawnDemon", Random.Range(13f, 26f));
                }
            }
        }
    }

    private void SpawnDemon()
    {
        if (spawnsFodder)
        {
            demon = Instantiate(fodderDemons[Random.Range(0, fodderDemons.Length)], transform.position, transform.rotation, transform.parent.transform);
        }
        if (spawnsExtendedFodder)
        {
            demon = Instantiate(extendedFodderDemons[Random.Range(0, extendedFodderDemons.Length)], transform.position, transform.rotation, transform.parent.transform);
        }
        if (spawnsHeavy)
        {
            demon = Instantiate(heavyDemons[Random.Range(0, heavyDemons.Length)], transform.position, transform.rotation, transform.parent.transform);
        }
        if (spawnsExtendedHeavy)
        {
            demon = Instantiate(extendedHeavyDemons[Random.Range(0, extendedHeavyDemons.Length)], transform.position, transform.rotation, transform.parent.transform);
        }
        if (spawnsWideSuperHeavy)
        {
            demon = Instantiate(wideSuperHeavyDemons[Random.Range(0, wideSuperHeavyDemons.Length)], transform.position, transform.rotation, transform.parent.transform);
        }
        if (spawnsThinSuperHeavy)
        {
            demon = Instantiate(thinSuperHeavyDemons[Random.Range(0, thinSuperHeavyDemons.Length)], transform.position, transform.rotation, transform.parent.transform);
        }
        if (spawnsAggro)
        {
            demon.GetComponentInChildren<DemonAI>().isAggro = true;
        }
        if (lookAtTarget)
        {
            demon.GetComponentInChildren<DemonAI>().LookAtPlayer();
        }
        if (lookOppositeToTarget)
        {
            demon.GetComponentInChildren<DemonAI>().LookOppositeToPlayer();
        }
        if (useTeleportEffect || respawns)
        {
            Destroy(Instantiate(go_teleportEffect, new Vector3(demon.transform.position.x, demon.transform.position.y + 1f, demon.transform.position.z), Quaternion.identity), 1f);
        }
        if (respawns)
        {
            shouldRespawn = true;
        }
        if (!respawns && !dontDestroyAfterSpawn)
        {
            Destroy(gameObject);
        }
    }

    public void StopRespawn()
    {
        CancelInvoke();
    }

    public GameObject GetDemon()
    {
        return demon;
    }

}
