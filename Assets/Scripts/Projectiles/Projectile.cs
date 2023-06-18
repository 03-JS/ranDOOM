using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool ignoreOriginatorOnly; // Tells the projectile to only ignore collisions between the originator and itself
    [Space]
    public float f_speed = 6f;
    public bool modifySpeed;
    public int i_damage;
    public float knockback = 500f;
    public bool followsTarget;
    public bool randomizeTracking;
    public bool useDamageMultiplier;
    public bool doesNotDamageDemons;
    public bool isExplosive;
    public float f_explopsionRadius;
    public int i_explosiveDamage;
    public float explosionForce = 20f;
    public bool explosionHasDmgFalloff;
    public bool explosionDamagesPlayer;
    public bool explosionDamagesDemons;
    public bool explosionDamagesBarrels;
    public bool createsTracers;
    public int i_tracerDamage;
    public float f_tracerDamageRate;
    public bool tracersDamagePlayer;
    public GameObject go_collisionEffect;
    public AudioClip ac_collisionSoundEffect;
    public GameObject go_explosionDamageEffect;

    private Rigidbody rigidBody;
    private Transform tr_target;
    private float f_tracerRange = Mathf.Infinity;
    private float f_time;
    private Collider projectileCollider;
    private Collider originatorsCollider;

    // Start is called before the first frame update
    void Start()
    {
        // tr_target = GameObject.Find("Player").transform;
        rigidBody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
        if (ignoreOriginatorOnly)
        {
            Physics.IgnoreCollision(projectileCollider, originatorsCollider, true);
        }
        if (randomizeTracking)
        {
            if (Random.Range(0, 4) == 0)
            {
                followsTarget = true;
            }
            else
            {
                followsTarget = false;
            }
        }
        if (modifySpeed)
        {
            f_speed *= ValueMultipliers.ProjectileSpeedMultiplier();
            // Debug.Log(gameObject.name + " speed: " + f_speed);
        }
        if (!followsTarget)
        {
            rigidBody.velocity = transform.TransformVector(Vector3.forward * f_speed);
        }
        Destroy(gameObject, 60f);
    }

    void Update()
    {
        if (createsTracers)
        {
            f_time += Time.deltaTime;
            if (f_time >= f_tracerDamageRate)
            {
                f_time = 0;
                CreateTracers();
            }
        }
        if (followsTarget)
        {
            if (tr_target.CompareTag("Demon"))
            {
                if (tr_target.GetComponent<DemonHealth>().isDead)
                {
                    rigidBody.velocity = transform.TransformVector(Vector3.forward * f_speed);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, tr_target.position, f_speed * 3f * Time.deltaTime);
                    transform.LookAt(tr_target);
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, tr_target.position, f_speed * 3f * Time.deltaTime);
                transform.LookAt(tr_target);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // if damages demons
        if (!doesNotDamageDemons)
        {
            if (collision.gameObject.tag == "Demon")
            {
                collision.gameObject.GetComponent<DemonHealth>().TakeDamage(i_damage, useDamageMultiplier, originatorsCollider.gameObject);
                if (isExplosive)
                {
                    collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, f_explopsionRadius);
                }
                else
                {
                    collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(knockback, transform.position, 10f);
                }
            }
        }
        if (collision.gameObject.tag == "Explosive Barrel")
        {
            collision.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(i_damage);
        }
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.layer == 16)
            {
                collision.gameObject.GetComponentInParent<PlayerHealthAndArmor>().TakeDamage(i_damage);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_damage);
            }
        }

        // Check if the projectile should create an explosion
        if (isExplosive)
        {
            CreateExplosion();
        }

        // Gets rid of the projectile
        Destroy(gameObject);

        // Creates the collision effect
        Destroy(Instantiate(go_collisionEffect, collision.GetContact(0).point, Quaternion.identity), 0.6f);

        // Plays the collision sound effect
        if (ac_collisionSoundEffect != null)
        {
            AudioSource.PlayClipAtPoint(ac_collisionSoundEffect, collision.GetContact(0).point, AudioManager.f_globalSfxVolume);
        }
    }

    public void SetOriginatorCollider(Collider collider)
    {
        originatorsCollider = collider;
    }

    public void SetTarget(Transform target)
    {
        tr_target = target;
    }

    private void CreateExplosion()
    {
        Vector3 v3_explosionOrigin = transform.position;
        Collider[] collidersInExplosion = Physics.OverlapSphere(v3_explosionOrigin, f_explopsionRadius);
        foreach (Collider collider in collidersInExplosion)
        {
            int layerMask = LayerMask.GetMask("Default", "Demon", "Player", "Cyberdemon");
            RaycastHit hitInfo;
            float f_distance = Vector3.Distance(v3_explosionOrigin, collider.transform.position);
            if (Physics.Raycast(v3_explosionOrigin, collider.transform.position - v3_explosionOrigin, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.collider == collider)
                {
                    if (explosionDamagesDemons)
                    {
                        if (collider.name != "Cyberdemon(Clone)")
                        {
                            if (collider.tag == "Demon")
                            {
                                if (explosionHasDmgFalloff)
                                {
                                    collider.GetComponent<DemonHealth>().TakeDamage(i_explosiveDamage - (f_distance * 20f), useDamageMultiplier, originatorsCollider.gameObject);
                                }
                                else
                                {
                                    collider.GetComponent<DemonHealth>().TakeDamage(i_explosiveDamage, useDamageMultiplier, originatorsCollider.gameObject);
                                }
                                // collider.GetComponent<Rigidbody>().isKinematic = false;
                                collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, f_explopsionRadius);
                                // collider.GetComponent<Rigidbody>().isKinematic = true;
                            }
                        }
                    }
                    if (explosionDamagesPlayer)
                    {
                        if (collider.tag == "Player")
                        {
                            if (explosionHasDmgFalloff)
                            {
                                collider.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_explosiveDamage - (f_distance * 20f));
                            }
                            else
                            {
                                collider.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_explosiveDamage);
                            }
                        }
                    }
                    if (explosionDamagesBarrels)
                    {
                        if (collider.tag == "Explosive Barrel")
                        {
                            if (explosionHasDmgFalloff)
                            {
                                collider.GetComponent<ExplosiveBarrel>().TakeDamage(i_explosiveDamage - (f_distance * 20f));
                            }
                            else
                            {
                                collider.GetComponent<ExplosiveBarrel>().TakeDamage(i_explosiveDamage);
                            }
                            collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, f_explopsionRadius);
                        }
                    }
                }
            }
        }
    }

    private void CreateTracers()
    {
        Collider[] collidersInTracerRange = Physics.OverlapSphere(transform.position, f_tracerRange);
        int layerMask = LayerMask.GetMask("Default", "Demon", "Player", "Cyberdemon");
        foreach (Collider collider in collidersInTracerRange)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.collider == collider)
                {
                    if (collider.tag == "Demon")
                    {
                        collider.GetComponent<DemonHealth>().TakeDamage(i_tracerDamage, true, originatorsCollider.gameObject);
                        collider.GetComponentInChildren<DemonAI>().ReceivePain();
                        if (collider.GetComponent<DemonHealth>().isDead)
                        {
                            // BFG Tracer Damage Effect
                            if (go_explosionDamageEffect != null)
                            {
                                Destroy(Instantiate(go_explosionDamageEffect, collider.transform.position, Quaternion.identity), 0.6f);
                            }
                        }
                    }
                    if (collider.tag == "Explosive Barrel")
                    {
                        collider.GetComponent<ExplosiveBarrel>().TakeDamage(i_tracerDamage);
                    }
                    if (tracersDamagePlayer)
                    {
                        if (collider.tag == "Player")
                        {
                            collider.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_tracerDamage);
                        }
                    }
                }
            }
        }
    }
}
