using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTrigger : MonoBehaviour
{
    public GameObject[] gameObjectsToEnable;
    public Collider[] collidersToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (GameObject obj in gameObjectsToEnable)
            {
                obj.SetActive(true);
            }
            foreach (Collider collider in collidersToEnable)
            {
                collider.enabled = true;
            }
            Destroy(this);
        }
    }
}
