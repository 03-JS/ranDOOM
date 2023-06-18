using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public GameObject[] gameObjects;
    public int i_range = 2;
    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        int i_randomNumber = random.Next(0, i_range);
        Instantiate(gameObjects[i_randomNumber],
            transform.position, Quaternion.identity, transform);
    }
}
