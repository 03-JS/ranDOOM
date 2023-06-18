using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public List<GameObject> triggerDemons;
    public GameObject[] affectedObjects;
    public Collider[] affectedColliders;

    void OnEnable()
    {
        DemonHealth.OnDemonDeath += EnableOrDisableObjects;
    }

    void OnDisable()
    {
        DemonHealth.OnDemonDeath -= EnableOrDisableObjects;
    }

    private void EnableOrDisableObjects()
    {
        foreach (GameObject demonSpawner in triggerDemons)
        {
            GameObject demon = demonSpawner.GetComponent<DemonSpawner>().GetDemon();
            if (demon != null)
            {
                if (!demon.GetComponent<DemonHealth>().isDead)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        foreach (GameObject obj in affectedObjects)
        {
            if (obj.GetComponent<DemonSpawner>())
            {
                obj.GetComponent<DemonSpawner>().StopRespawn();
            }
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
        foreach (Collider collider in affectedColliders)
        {
            if (collider.enabled)
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = true;
            }
        }
        gameObject.SetActive(false);
    }

}
