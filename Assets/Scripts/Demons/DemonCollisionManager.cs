using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonCollisionManager : MonoBehaviour
{
    public bool isProjectile;
    public float f_collisionDamage = 25f;
    public float f_teleportFatigue = 1f;

    private NavMeshAgent agent;
    private DemonHealth demonHealth;
    private Animator demonAnimator;
    private float f_timer;
    private Rigidbody demonRigidbody;
    private Collider demonCollider;
    [HideInInspector] public bool canTeleport;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        demonHealth = GetComponent<DemonHealth>();
        demonAnimator = GetComponentInChildren<Animator>();
        demonRigidbody = GetComponent<Rigidbody>();
        demonCollider = GetComponent<Collider>();
        canTeleport = true;
    }

    void Update()
    {
        if (!canTeleport)
        {
            f_timer += Time.deltaTime;
            if (f_timer >= f_teleportFatigue) // The amount of time it takes for the demon to be able to teleport again in seconds
            {
                canTeleport = true;
                f_timer = 0;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isProjectile)
        {
            if (gameObject.GetComponentInChildren<DemonAI>().isAttacking)
            {
                if (collision.gameObject.CompareTag("Demon"))
                {
                    collision.gameObject.GetComponentInParent<DemonHealth>().TakeDamage(f_collisionDamage, false, gameObject);
                    Invoke("ChaseTargetAfterCollision", 0.5f);
                }
                else
                {
                    if (collision.gameObject.CompareTag("Player"))
                    {
                        if (collision.gameObject.layer == 16)
                        {
                            collision.gameObject.GetComponentInParent<PlayerHealthAndArmor>().TakeDamage(f_collisionDamage);
                        }
                        else
                        {
                            collision.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(f_collisionDamage);
                        }
                        Invoke("ChaseTargetAfterCollision", 0.5f);
                    }
                    else
                    {
                        if (collision.gameObject.CompareTag("Explosive Barrel"))
                        {
                            collision.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(f_collisionDamage);
                            Invoke("ChaseTargetAfterCollision", 0.5f);
                        }
                        else
                        {
                            ChaseTargetAfterCollision();
                        }
                    }
                }
                if (!collision.gameObject.CompareTag("Elevator"))
                {
                    agent.enabled = true;
                    demonRigidbody.velocity = Vector3.zero;
                    demonRigidbody.isKinematic = true;
                    // demonCollider.isTrigger = false;
                    gameObject.GetComponentInChildren<DemonAI>().ResetAttackState();
                    // Debug.Log("Collided with: " + collision.gameObject.name);
                }
            }
        }
    }

    private void ChaseTargetAfterCollision()
    {
        demonAnimator.SetTrigger("ChaseTarget");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Elevator")
        {
            if (other.gameObject.GetComponent<MovablePlatform>().isMoving)
            {
                agent.enabled = false;
                transform.SetParent(other.gameObject.transform);
                // transform.position = Vector3.MoveTowards(transform.position, GameObject.Find("Player").transform.position, f_agentSpeed * Time.deltaTime);
            }
            else
            {
                if (demonHealth != null)
                {
                    if (!demonHealth.isDead)
                    {
                        agent.enabled = true;
                        transform.SetParent(null);
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable")
        {
            if (other.gameObject.GetComponent<Door>().canDemonsOpen)
            {
                if (!demonHealth.isDead && demonHealth != null)
                {
                    other.gameObject.GetComponent<Door>().Open(false);
                }
            }
        }
    }
}
