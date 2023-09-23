using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStats : MonoBehaviour
{
    public static int secrets = 0;
    public static int maxKills = 0;
    public static int items = 0;

    public static int secretsFound = 0;
    public static int kills = 0;
    public static int itemsFound = 0;

    public static float time;
    public static string level;

    // Start is called before the first frame update
    void Start()
    {
        level = SceneManager.GetActiveScene().name;
        DemonSpawner[] demonSpawners = FindObjectsOfType<DemonSpawner>(true);
        maxKills = demonSpawners.GetLength(0);
        //foreach (DemonSpawner spawnerScript in demonSpawners)
        //{
        //    GameObject spawner = spawnerScript.gameObject;
        //    //if (!spawner.activeInHierarchy)
        //    //{
        //    //    spawner.SetActive(true);
        //    //}
        //}
    }

    public static void ResetStats()
    {
        secrets = 0;
        maxKills = 0;
        items = 0;
        secretsFound = 0;
        kills = 0;
        itemsFound = 0;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Max kills: " + maxKills);
        //Debug.Log("Items: " + items);
        //Debug.Log("Secrets: " + secrets);
        //Debug.Log("Kills: " + kills);
        //Debug.Log("Items found: " + itemsFound);
        //Debug.Log("Secrets found: " + secretsFound);
    }
}
