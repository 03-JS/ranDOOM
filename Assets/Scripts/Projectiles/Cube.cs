using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float speed;
    public GameObject[] demons;
    public GameObject spawnEffect;
    public AudioClip spawnSFX;

    private Material defaultMaterial;
    private Transform target;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (transform.position.Equals(target.position))
            {
                SpawnDemon();
                Destroy(gameObject);
                Destroy(Instantiate(spawnEffect, target.position, Quaternion.identity), 1.18f);
                AudioSource.PlayClipAtPoint(spawnSFX, target.position, AudioManager.f_globalSfxVolume);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetDefaultMaterial(Material material)
    {
        defaultMaterial = material;
        gameObject.GetComponentInChildren<SpriteRenderer>().material = defaultMaterial;
    }

    private void SpawnDemon()
    {
        LevelStats.maxKills++;
        GameObject demon = Instantiate(demons[Random.Range(0, demons.Length)], target.position, Quaternion.identity);
        demon.GetComponentInChildren<DemonAI>().isAggro = true;
        demon.GetComponentInChildren<SpriteRenderer>().material = defaultMaterial;
    }
}
