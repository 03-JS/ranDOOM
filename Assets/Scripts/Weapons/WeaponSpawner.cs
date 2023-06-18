using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public List<GameObject> gol_weapons;
    public int i_range = 5;

    // Start is called before the first frame update
    public void SpawnWeapon(GameObject spawnPoint)
    {
        if (gol_weapons.Count > 0)
        {
            System.Random random = new System.Random();
            int i_randomNumber = random.Next(0, i_range);
            GameObject weapon = gol_weapons[i_randomNumber];
            Instantiate(weapon,
                new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), Quaternion.identity, spawnPoint.transform);
            gol_weapons.Remove(weapon);
            i_range -= 1;
        }
    }
}
