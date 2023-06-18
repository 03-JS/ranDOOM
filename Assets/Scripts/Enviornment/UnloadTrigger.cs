using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadTrigger : MonoBehaviour
{
    public GameObject[] gameObjectsToDisable;
    public Collider[] collidersToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (GameObject obj in gameObjectsToDisable)
            {
                obj.SetActive(false);
            }
            foreach (Collider collider in collidersToDisable)
            {
                collider.enabled = false;
            }
            Destroy(this);
        }
    }
}
