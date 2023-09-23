using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleporter : MonoBehaviour
{
    public Transform destination;
    public GameObject go_teleportEffect;

    void OnTriggerEnter(Collider other)
    {
        if (destination != null)
        {
            if (other.name == "Player")
            {
                if (other.gameObject.GetComponent<CharacterMovement>().canTeleport)
                {
                    other.gameObject.GetComponent<CharacterMovement>().canTeleport = false;
                    other.transform.position = destination.position;
                    other.transform.rotation = destination.rotation;
                    other.gameObject.GetComponent<CharacterMovement>().StopAllMomentum();
                    Destroy(Instantiate(go_teleportEffect, new Vector3(destination.position.x, destination.position.y + 1, destination.position.z), Quaternion.identity), 1.6f);
                }
            }
            if (other.CompareTag("Demon"))
            {
                if (other.gameObject.GetComponent<DemonCollisionManager>().canTeleport)
                {
                    Destroy(Instantiate(go_teleportEffect, new Vector3(other.transform.position.x, other.transform.position.y + 1, other.transform.position.z), Quaternion.identity), 1.6f);
                    other.GetComponent<NavMeshAgent>().enabled = false;
                    other.gameObject.GetComponent<DemonCollisionManager>().canTeleport = false;
                    other.transform.position = destination.position;
                    other.GetComponent<NavMeshAgent>().enabled = true;
                    Destroy(Instantiate(go_teleportEffect, new Vector3(destination.position.x, destination.position.y + 1, destination.position.z), Quaternion.identity), 1.6f);
                }
            }
        }
    }
}
