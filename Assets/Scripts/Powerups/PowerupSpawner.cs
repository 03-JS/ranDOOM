using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        int i_randomNumber = random.Next(0, gameObjects.Length);
        Instantiate(gameObjects[i_randomNumber],
            transform.position, Quaternion.identity, transform);
    }
}
