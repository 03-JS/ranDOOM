using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] healthItems;
    public GameObject[] armorItems;
    public GameObject[] ammoItems;
    public GameObject respawnEffect;
    public bool spawnsHealth;
    public bool spawnsArmor;
    public bool spawnsAmmo;

    private bool shouldRespawn;
    private bool useRespawnEffect;

    // Start is called before the first frame update
    void Start()
    {
        useRespawnEffect = false;
        SpawnItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRespawn && transform.childCount == 0)
        {
            shouldRespawn = false;
            useRespawnEffect = true;
            Invoke("SpawnItem", Random.Range(10f, 31f));
        }
    }

    private void SpawnItem()
    {
        GameObject item = null;
        if (spawnsHealth)
        {
            item = Instantiate(healthItems[Random.Range(0, healthItems.Length)], transform.position, Quaternion.identity, transform);
        }
        if (spawnsArmor)
        {
            item = Instantiate(armorItems[Random.Range(0, armorItems.Length)], transform.position, Quaternion.identity, transform);
        }
        if (spawnsAmmo)
        {
            item = Instantiate(ammoItems[Random.Range(0, ammoItems.Length)], transform.position, Quaternion.identity, transform);
        }
        if (item != null)
        {
            string[] splitItemName = item.name.Split("(");
            item.name = splitItemName[0];
        }
        if (useRespawnEffect)
        {
            Destroy(Instantiate(respawnEffect, transform.position, Quaternion.identity), 1f);
        }
        shouldRespawn = true;
    }
}
