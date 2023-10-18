using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cube;
    public Transform[] targets;
    public float delay = 8f;
    public AudioClip spawnCubeSound;
    public Material defaultMaterial;

    private float time;
    private int cubesSpawned = 0;
    private Transform previousTarget;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= delay)
        {
            SpawnCube();
            time = 0;
        }
    }

    private void SpawnCube()
    {
        AudioSource.PlayClipAtPoint(spawnCubeSound, transform.position, AudioManager.f_globalSfxVolume);
        GameObject instantiatedCube = Instantiate(cube, transform.position, transform.rotation);
        instantiatedCube.GetComponent<Cube>().SetTarget(SelectTarget());
        instantiatedCube.GetComponent<Cube>().SetDefaultMaterial(defaultMaterial);
        cubesSpawned++;
        switch (cubesSpawned)
        {
            case 10:
                delay--;
                break;
            case 25:
                delay--;
                break;
            case 40:
                delay--;
                break;
            case 55:
                delay--;
                break;
            default:
                break;
        }
    }

    private Transform SelectTarget()
    {
        Transform target = targets[Random.Range(0, targets.Length)];
        if (targets.Length > 1)
        {
            while (target == previousTarget)
            {
                target = targets[Random.Range(0, targets.Length)];
            }
        }
        previousTarget = target;
        return target;
    }
}
