using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseStatCount : MonoBehaviour
{
    public bool increaseItems;
    public bool increaseSecrets;
    
    // Start is called before the first frame update
    void Start()
    {
        if (increaseItems)
        {
            LevelStats.items++;
        }
        if (increaseSecrets)
        {
            LevelStats.secrets++;
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
