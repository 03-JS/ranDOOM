using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelStats.time = Time.timeSinceLevelLoad;
        SceneManager.LoadScene("Intermission");
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
