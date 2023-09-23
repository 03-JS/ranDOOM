using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class DemonAI : MonoBehaviour
{
    public string tier;
    public Transform lineOfSightOrigin;
    public float f_reactionTime;
    public float f_detectionRange;
    public bool attacksOnWakeup;
    public bool noPredefinedTarget;
    public bool alwaysFollowsTarget;
    public bool flies;
    public float f_lungeSpeed = 10f;
    // public bool canSeeThroughEnemies
    public float f_timeBetweenAttacks;
    public bool attacksIfTargetIsClose;
    public bool hasMeleeAttack;
    public bool canWhiffMeleeAttack;
    public int i_meleeDamage;
    public AudioClip ac_meleeAttackSound;
    public bool hasRangedAttack;
    public int i_rangedDamage;
    public float f_bulletSpread = 0.1f;
    public int i_bulletsPerShot = 1;
    public AudioClip ac_rangedAttackSound;
    public GameObject go_poofEffect;
    public GameObject go_bloodEffect;
    public AudioClip ac_painSound;
    public GameObject go_projectile;
    public GameObject go_attackLight;
    public Transform[] tr_attackOrigins;
    public AudioClip ac_projectileAttackSound;
    public AudioClip ac_attackSound;
    public bool canResurrectDemons;
    public float f_resurrectionRange = 10f;
    public GameObject go_flames;
    public float f_flameDuration = 0.4f;
    public Animator demonAnimator;
    public AudioClip[] sightingSounds;
    public AudioClip ac_roamingSound;
    public AudioClip ac_walkingSound;
    [HideInInspector] public bool isAggro;
    [HideInInspector] public bool isAttacking;

    private AudioSource audioSource;
    private bool canSeeThroughEnemies;
    private float f_meleeDistance = 2;
    private float f_distanceToTarget;
    private bool isTargetInSight;
    private bool isInPain;
    private float f_time;
    private int i_timesSightingSoundHasPlayed;
    private float f_timeBetweenRoamingSounds;
    private float f_timeTargetHasBeenOOS; // The time the demon's target has been out of sight
    private bool isReviving;
    private GameObject go_instantiatedFlame;
    private Transform tr_destination;
    private NavMeshAgent agent;
    private DemonHealth demonHealth;
    private GameObject go_playerCamera;
    private Transform tr_playerShootingPoint;
    private Transform tr_originalLOS; // Preserves the initial lineOfSightOrigin value
    private Rigidbody demonRigidbody;
    private SpriteRenderer demonRenderer;
    // private Collider demonCollider;

    // Start is called before the first frame update
    void Start()
    {
        demonHealth = GetComponentInParent<DemonHealth>();
        if (!noPredefinedTarget)
        {
            tr_destination = GameObject.Find("Player").transform;
        }
        agent = GetComponentInParent<NavMeshAgent>();
        f_timeBetweenRoamingSounds = Random.Range(5, 15);
        go_playerCamera = GameObject.Find("Player Camera");
        tr_playerShootingPoint = GameObject.Find("Projectile Shooting Point").transform;
        tr_originalLOS = lineOfSightOrigin;
        demonRigidbody = GetComponentInParent<Rigidbody>();
        demonRenderer = GetComponent<SpriteRenderer>();
        // demonCollider = GetComponentInParent<Collider>();
        audioSource = GetComponent<AudioSource>();

        f_time = Random.Range(0, f_timeBetweenAttacks - 1f);
        f_time -= f_reactionTime;

        // When a demon becomes aggro it will attack instantly instead of waiting for the next attack window as long as the target is in sight
        if (attacksOnWakeup)
        {
            f_time = f_timeBetweenAttacks;
        }

        // Debug.Log(transform.parent.name + " time: " + f_time);
    }

    // Update is called once per frame
    void Update()
    {
        if (!demonHealth.isDead)
        {
            if (!isAggro)
            {
                float f_distanceToTarget = Vector3.Distance(transform.parent.position, tr_destination.position);
                if (f_distanceToTarget <= f_detectionRange)
                {
                    isAggro = true;
                }
            }

            // Chase the player if the demon target is dead
            if (tr_destination.CompareTag("Demon"))
            {
                canSeeThroughEnemies = false;
                if (!tr_destination.gameObject.activeSelf || tr_destination.GetComponent<DemonHealth>().isDead)
                {
                    if (f_timeBetweenAttacks > 0 && !canResurrectDemons)
                    {
                        if (!isAttacking)
                        {
                            SetTarget(GameObject.Find("Player").transform);
                            if (sightingSounds.Length > 0)
                            {
                                // AudioSource.PlayClipAtPoint(sightingSounds[Random.Range(0, sightingSounds.Length)], transform.position, AudioManager.f_globalSfxVolume);
                                audioSource.clip = sightingSounds[Random.Range(0, sightingSounds.Length)];
                                audioSource.volume = AudioManager.f_globalSfxVolume;
                                audioSource.Play();
                            }
                        }
                    }
                    else
                    {
                        f_time = -f_reactionTime /* / 1.5f */;
                        SetTarget(GameObject.Find("Player").transform);
                        // AudioSource.PlayClipAtPoint(sightingSounds[Random.Range(0, sightingSounds.Length)], transform.position, AudioManager.f_globalSfxVolume);
                        audioSource.clip = sightingSounds[Random.Range(0, sightingSounds.Length)];
                        audioSource.volume = AudioManager.f_globalSfxVolume;
                        audioSource.Play();
                    }
                }
            }

            SightCheck();

            // Aggro behaviour
            if (isAggro && !isInPain)
            {
                // Plays a wakeup sound effect
                if (i_timesSightingSoundHasPlayed == 0 && sightingSounds.Length > 0)
                {
                    // AudioSource.PlayClipAtPoint(sightingSounds[Random.Range(0, sightingSounds.Length)], transform.position, AudioManager.f_globalSfxVolume);
                    audioSource.clip = sightingSounds[Random.Range(0, sightingSounds.Length)];
                    audioSource.volume = AudioManager.f_globalSfxVolume;
                    audioSource.Play();
                    i_timesSightingSoundHasPlayed++;
                }

                // Chases the target if it isn't visible or the demon isn't attacking
                if (!isAttacking)
                {
                    if (f_time <= f_timeBetweenAttacks)
                    {
                        f_time += Time.deltaTime;
                    }
                    ChaseTarget();
                }

                // Calculates the melee attack range
                if (tr_destination != null)
                {
                    if (tr_destination.CompareTag("Demon"))
                    {
                        f_meleeDistance = (tr_destination.gameObject.GetComponent<NavMeshAgent>().radius + agent.radius) * 5;
                    }
                    else
                    {
                        f_meleeDistance = (tr_destination.gameObject.GetComponent<CharacterController>().radius + agent.radius) * 3;
                    }
                }

                // Calculates the distance to the target
                if (attacksIfTargetIsClose || hasMeleeAttack)
                {
                    if (tr_destination != null)
                    {
                        f_distanceToTarget = Vector3.Distance(transform.parent.position, tr_destination.position);
                        if (f_distanceToTarget <= f_meleeDistance && f_reactionTime > 0)
                        {
                            f_time = 0;
                        }
                    }
                    if (flies)
                    {
                        f_distanceToTarget = Vector3.Distance(new Vector3(transform.parent.position.x, tr_destination.position.y, transform.parent.position.z), tr_destination.position);
                    }
                }
                else
                {
                    f_distanceToTarget = f_meleeDistance + 1;
                }

                if (!alwaysFollowsTarget)
                {
                    // Sets the animation triggers once the time is higher or equal to the time between attacks or if the player is inside the instant attack window
                    if ((f_time >= f_timeBetweenAttacks || f_distanceToTarget <= f_meleeDistance) && isTargetInSight && !isAttacking && !canResurrectDemons)
                    {
                        StopChasingTarget();
                        isAttacking = true;
                        if (hasMeleeAttack)
                        {
                            TryMeleeAttack();
                        }
                        else
                        {
                            if (hasRangedAttack)
                            {
                                demonAnimator.SetTrigger("RangedAttack");
                                if (flies)
                                {
                                    LaunchTowardsTarget();
                                }
                            }
                            else
                            {
                                demonAnimator.SetTrigger("ProjectileAttack");
                            }
                        }

                    }
                }
                else
                {
                    if (f_distanceToTarget <= f_meleeDistance && isTargetInSight && !isAttacking)
                    {
                        StopChasingTarget();
                        isAttacking = true;
                        demonAnimator.SetTrigger("MeleeAttack");
                    }
                }

                // Special attack behaviour for the Archvile. They can resurrect demons even if the player is not visible and can also alternate between two attacks
                if (canResurrectDemons)
                {
                    if ((f_time >= f_timeBetweenAttacks || f_distanceToTarget <= f_meleeDistance) && !isAttacking)
                    {
                        CheckForDeadDemons();
                    }
                    if (isAttacking)
                    {
                        UpdateFlamePosition();
                    }
                }
            }

            // Roaming sound logic
            if (ac_roamingSound != null)
            {
                if (!isTargetInSight)
                {
                    if (Time.time >= f_timeBetweenRoamingSounds)
                    {
                        f_timeBetweenRoamingSounds = Time.time + Random.Range(5, 15);
                        // AudioSource.PlayClipAtPoint(ac_roamingSound, transform.position, AudioManager.f_globalSfxVolume);
                        audioSource.clip = ac_roamingSound;
                        audioSource.volume = AudioManager.f_globalSfxVolume;
                        audioSource.Play();
                    }
                }
            }

            // Stop chasing the player if it is dead
            if (tr_destination.CompareTag("Player"))
            {
                // Debug.Log(transform.parent.name + " atack time (towards player): " + f_time);
                canSeeThroughEnemies = true;
                if (tr_destination.gameObject.GetComponent<PlayerHealthAndArmor>().isDead)
                {
                    StopChasingTarget();
                    isAttacking = false;
                    isTargetInSight = false;
                }
            }
        }
    }

    // Checks if the target is in the demon's line of sight
    private void SightCheck()
    {
        RaycastHit hitInfo;
        int layerMask = LayerMask.GetMask("Default", "Player", "Demon", "Cyberdemon");
        if (!isAggro)
        {
            if (Physics.Raycast(lineOfSightOrigin.position, tr_destination.position - lineOfSightOrigin.position, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.collider.gameObject == transform.parent.gameObject)
                {
                    isTargetInSight = false;
                    return;
                }
            }
        }
        if (canSeeThroughEnemies)
        {
            layerMask = LayerMask.GetMask("Default", "Player");
        }
        if (tr_destination.CompareTag("Demon"))
        {
            lineOfSightOrigin = transform.parent;
        }
        else
        {
            lineOfSightOrigin = tr_originalLOS;
        }
        if (Physics.Raycast(lineOfSightOrigin.position, tr_destination.position - lineOfSightOrigin.position, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.gameObject == tr_destination.gameObject)
            {
                isAggro = true;
                isTargetInSight = true;
                f_timeTargetHasBeenOOS = 0;
            }
            else
            {
                isTargetInSight = false;
                if (f_time >= 0 && f_reactionTime > 0)
                {
                    if (f_timeTargetHasBeenOOS >= 2)
                    {
                        f_time = -f_reactionTime /* / 1.5f */;
                        // Debug.Log(transform.parent.name + " time before attacking: " + f_time);
                    }
                    f_timeTargetHasBeenOOS += Time.deltaTime;
                }
            }
            // Debug.Log(f_time);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        tr_destination = newTarget;
    }

    private void ChaseTarget()
    {
        demonAnimator.SetBool("isChasing", true);
        if (agent.isActiveAndEnabled && !isInPain)
        {
            agent.destination = tr_destination.position;
            agent.isStopped = false;
        }
    }

    private void StopChasingTarget()
    {
        demonAnimator.SetBool("isChasing", false);
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }
    }

    private void TryMeleeAttack()
    {
        if (f_distanceToTarget <= f_meleeDistance)
        {
            demonAnimator.SetTrigger("MeleeAttack");
        }
        else
        {
            if (hasRangedAttack)
            {
                demonAnimator.SetTrigger("RangedAttack");
                if (flies)
                {
                    LaunchTowardsTarget();
                }
            }
            else
            {
                demonAnimator.SetTrigger("ProjectileAttack");
            }
        }
    }

    public void Shoot()
    {
        RaycastHit hitInfo;
        int layerMask = LayerMask.GetMask("Default", "Demon", "Player", "Cyberdemon");
        Vector3 v3_direction = tr_attackOrigins[0].forward;
        for (int i = 0; i < i_bulletsPerShot; i++)
        {
            v3_direction.x += Random.Range(-f_bulletSpread, f_bulletSpread);
            v3_direction.z += Random.Range(-f_bulletSpread, f_bulletSpread);
            if (Physics.Raycast(tr_attackOrigins[0].position, v3_direction, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.collider.tag == "Demon")
                {
                    Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                    hitInfo.collider.gameObject.GetComponent<DemonHealth>().TakeDamage(i_rangedDamage, false, transform.parent.gameObject);
                }
                if (hitInfo.collider.tag == "Player")
                {
                    Destroy(Instantiate(go_bloodEffect, hitInfo.point, Quaternion.identity), 0.5f);
                    hitInfo.collider.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_rangedDamage);
                }
                if (hitInfo.collider.tag == "Explosive Barrel")
                {
                    Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.identity), 0.5f);
                    hitInfo.collider.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(i_rangedDamage);
                }
                else
                {
                    if (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Demon")
                    {
                        Destroy(Instantiate(go_poofEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 0.5f);
                    }
                }
            }
        }
        // AudioSource.PlayClipAtPoint(ac_rangedAttackSound, transform.position, AudioManager.f_globalSfxVolume);
        audioSource.clip = ac_rangedAttackSound;
        audioSource.volume = AudioManager.f_globalSfxVolume;
        audioSource.Play();
    }

    public void ShootProjectile()
    {
        foreach (Transform origin in tr_attackOrigins)
        {
            GameObject instantiatedProjectile = Instantiate(go_projectile, origin.position, origin.rotation);
            instantiatedProjectile.GetComponent<Projectile>().SetOriginatorCollider(GetComponentInParent<Collider>());
            instantiatedProjectile.GetComponent<Projectile>().SetTarget(tr_destination);
        }
        // AudioSource.PlayClipAtPoint(ac_projectileAttackSound, transform.position, AudioManager.f_globalSfxVolume);
        audioSource.clip = ac_projectileAttackSound;
        audioSource.volume = AudioManager.f_globalSfxVolume;
        audioSource.Play();
    }

    public void SpawnLostSoul()
    {
        GameObject lostSoul = Instantiate(go_projectile, tr_attackOrigins[0].position, tr_attackOrigins[0].rotation);
        Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), lostSoul.GetComponent<Collider>(), true);
        // Physics.IgnoreCollision(transform.parent.GetComponent<Collider>(), lostSoul.GetComponentInChildren<Collider>(), true);
        lostSoul.GetComponentInChildren<DemonAI>().SetTarget(tr_destination);
    }

    public void SpawnLostSouls()
    {
        foreach (Transform origin in tr_attackOrigins)
        {
            GameObject lostSoul = Instantiate(go_projectile, origin.position, origin.rotation);
            lostSoul.GetComponentInChildren<DemonAI>().SetTarget(tr_destination);
        }
    }

    public void MeleeAttack()
    {
        float f_distanceToTarget = Vector3.Distance(transform.position, tr_destination.position);
        if (f_distanceToTarget <= f_meleeDistance)
        {
            if (tr_destination.CompareTag("Player"))
            {
                tr_destination.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_meleeDamage);
                if (ac_meleeAttackSound != null)
                {
                    // AudioSource.PlayClipAtPoint(ac_meleeAttackSound, transform.position, AudioManager.f_globalSfxVolume);
                    audioSource.clip = ac_meleeAttackSound;
                    audioSource.volume = AudioManager.f_globalSfxVolume;
                    audioSource.Play();
                }
            }
            if (tr_destination.CompareTag("Demon"))
            {
                tr_destination.gameObject.GetComponent<DemonHealth>().TakeDamage(i_meleeDamage, false, transform.parent.gameObject);
                if (ac_meleeAttackSound != null)
                {
                    // AudioSource.PlayClipAtPoint(ac_meleeAttackSound, transform.position, AudioManager.f_globalSfxVolume);
                    audioSource.clip = ac_meleeAttackSound;
                    audioSource.volume = AudioManager.f_globalSfxVolume;
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (!canWhiffMeleeAttack)
            {
                ShootProjectile();
            }
        }
    }

    // Attack animations call this function at the end. This allows me to make longer & shorter attacks more easily using animations and nothing else
    public void ResetAttackState()
    {
        if (f_reactionTime <= 0)
        {
            f_time = Random.Range(0, f_timeBetweenAttacks);
        }
        if (hasMeleeAttack && !flies)
        {
            f_time -= f_distanceToTarget / 10;
        }
        // Debug.Log(transform.parent.name + " time: " + f_time);
        isAttacking = false;
    }

    public void ReceivePain()
    {
        if (!demonHealth.isDead)
        {
            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }
            DisableAttackLight();
            demonAnimator.SetTrigger("Pain");
            isAttacking = false;
            isInPain = true;
            demonRigidbody.isKinematic = true;
        }
    }

    // Pain animations call this at the end to reset the bool's value
    public void ResetPainState()
    {
        if (!demonHealth.isDead)
        {
            if (agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
            }
            isInPain = false;
            int randomRetaliateChance = Random.Range(0, 3);
            // Debug.Log(transform.parent.name + " retaliate chance: " + randomRetaliateChance);
            if (randomRetaliateChance == 1)
            {
                f_time = f_timeBetweenAttacks;
            }
            else
            {
                ResetAttackState();
                if (f_reactionTime > 0)
                {
                    f_time = -f_reactionTime / 1.5f;
                }
            }
        }
    }

    public void PlayPainSound()
    {
        // AudioSource.PlayClipAtPoint(ac_painSound, transform.position, AudioManager.f_globalSfxVolume);
        audioSource.clip = ac_painSound;
        audioSource.volume = AudioManager.f_globalSfxVolume;
        audioSource.Play();
    }

    public void LookAtTarget()
    {
        Vector3 v3_playerPosition = GameObject.Find("Player").transform.position;
        v3_playerPosition.y = transform.parent.position.y;
        transform.parent.gameObject.transform.LookAt(tr_destination.position);
        transform.LookAt(v3_playerPosition);
    }

    public void LookAtPlayer()
    {
        Vector3 v3_playerPosition = GameObject.Find("Player").transform.position;
        v3_playerPosition.y = transform.parent.position.y;
        transform.parent.LookAt(v3_playerPosition);
    }

    public void LookOppositeToPlayer()
    {
        Vector3 v3_playerPosition = GameObject.Find("Player").transform.position;
        v3_playerPosition.y = transform.parent.position.y;
        transform.parent.LookAt(2 * transform.parent.position - v3_playerPosition);
    }

    public void PlayWalkingSound()
    {
        // AudioSource.PlayClipAtPoint(ac_walkingSound, transform.position, AudioManager.f_globalSfxVolume);
        audioSource.clip = ac_walkingSound;
        audioSource.volume = AudioManager.f_globalSfxVolume;
        audioSource.Play();
    }

    public void PlayAttackSfx()
    {
        if (transform.parent.name == "Mancubus(Clone)")
        {
            AudioSource.PlayClipAtPoint(ac_attackSound, transform.position, AudioManager.f_globalSfxVolume);
        }
        else
        {
            audioSource.clip = ac_attackSound;
            audioSource.volume = AudioManager.f_globalSfxVolume;
            audioSource.Play();
        }
    }

    // Checks if there are any dead demons that can be resurrected in the designated range and sets the attack triggers
    private void CheckForDeadDemons()
    {
        bool areAnyDemonsDead = false;
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, f_resurrectionRange);
        foreach (Collider collider in collidersInRange)
        {
            if (collider.tag == "Demon")
            {
                if (collider.gameObject.GetComponent<DemonHealth>().isDead && collider.gameObject.GetComponent<DemonHealth>().canBeResurrected)
                {
                    areAnyDemonsDead = true;
                    break;
                }
            }
        }
        if (areAnyDemonsDead)
        {
            if (isTargetInSight)
            {
                if (Random.Range(0, 2) == 0)
                {
                    demonAnimator.SetTrigger("ResurrectDemons");
                    areAnyDemonsDead = false;
                }
                else
                {
                    demonAnimator.SetTrigger("FlameAttack");
                }
                StopChasingTarget();
                isAttacking = true;
            }
            else
            {
                StopChasingTarget();
                isAttacking = true;
                demonAnimator.SetTrigger("ResurrectDemons");
                areAnyDemonsDead = false;
            }
        }
        else
        {
            if (isTargetInSight)
            {
                StopChasingTarget();
                isAttacking = true;
                demonAnimator.SetTrigger("FlameAttack");
            }
        }
    }

    // The archvile revive attack animation calls this function to trigger the demons revive animation
    public void ReviveDemons()
    {
        // Cast a sphere to get the demons (dead or alive) that are nearby and trigger the revive animation
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, f_resurrectionRange);
        foreach (Collider collider in collidersInRange)
        {
            if (collider.tag == "Demon")
            {
                if (collider.gameObject.GetComponent<DemonHealth>().canBeResurrected && collider.gameObject.GetComponent<DemonHealth>().isDead && !collider.gameObject.GetComponentInChildren<DemonAI>().isReviving)
                {
                    collider.gameObject.GetComponent<DemonHealth>().TriggerReviveAnimation();
                    collider.gameObject.GetComponentInChildren<DemonAI>().isReviving = true;
                }
            }
        }
    }

    // Revive animations call this function at the end
    public void Revive()
    {
        demonAnimator.SetBool("isChasing", false);
        demonHealth.Revive();
        // tr_destination = GameObject.Find("Player").transform;
        f_time = Random.Range(0, f_timeBetweenAttacks - 1f);
        f_time -= f_reactionTime;
        f_timeBetweenRoamingSounds = Random.Range(5, 15);
        isAttacking = false;
        isInPain = false;
        isReviving = false;
    }

    public void SpawnFlames()
    {
        AudioSource.PlayClipAtPoint(ac_rangedAttackSound, tr_destination.position, AudioManager.f_globalSfxVolume);
        if (tr_destination.gameObject.CompareTag("Player"))
        {
            go_instantiatedFlame = Instantiate(go_flames, tr_playerShootingPoint.position, Quaternion.identity);
        }
        else
        {
            go_instantiatedFlame = Instantiate(go_flames, tr_destination.position, Quaternion.identity);
        }
        go_instantiatedFlame.transform.SetParent(tr_destination);
        Destroy(go_instantiatedFlame, f_flameDuration);
    }

    public void PerformFlameAttack()
    {
        if (isTargetInSight)
        {
            if (tr_destination.CompareTag("Player"))
            {
                tr_destination.gameObject.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_rangedDamage);
            }
            else
            {
                tr_destination.gameObject.GetComponent<DemonHealth>().TakeDamage(i_rangedDamage, false, transform.parent.gameObject);
            }
            if (ac_attackSound != null)
            {
                AudioSource.PlayClipAtPoint(ac_attackSound, tr_destination.position, AudioManager.f_globalSfxVolume);
                // audioSource.clip = ac_attackSound;
                // audioSource.volume = AudioManager.f_globalSfxVolume;
                // audioSource.Play();
            }
        }
    }

    private void UpdateFlamePosition()
    {
        if (go_instantiatedFlame != null)
        {
            if (!isTargetInSight)
            {
                go_instantiatedFlame.transform.SetParent(null);
            }
            else
            {
                if (tr_destination.CompareTag("Player"))
                {
                    go_instantiatedFlame.transform.SetParent(go_playerCamera.transform);
                    go_instantiatedFlame.transform.position = tr_playerShootingPoint.position;
                }
                else
                {
                    go_instantiatedFlame.transform.SetParent(tr_destination);
                    go_instantiatedFlame.transform.position = tr_destination.position;
                }
            }
        }
    }

    public void LaunchTowardsTarget()
    {
        agent.enabled = false;
        // AudioSource.PlayClipAtPoint(ac_attackSound, transform.position, AudioManager.f_globalSfxVolume);
        audioSource.clip = ac_attackSound;
        audioSource.volume = AudioManager.f_globalSfxVolume;
        audioSource.Play();
        LookAtTarget();
        demonRigidbody.isKinematic = false;
        demonRigidbody.velocity = transform.parent.TransformVector(Vector3.forward * f_lungeSpeed);
        // demonCollider.isTrigger = true;
        demonAnimator.ResetTrigger("ChaseTarget");
    }

    public void Despawn()
    {
        demonHealth.Despawn();
    }

    public void DestroyDemon()
    {
        Destroy(transform.parent.gameObject);
    }

    public void EnableAttackLight()
    {
        if (demonRenderer.material.name == "Sprite Material (Instance)")
        {
            if (go_attackLight != null)
            {
                go_attackLight.SetActive(true);
            }
        }
    }

    public void DisableAttackLight()
    {
        if (go_attackLight != null)
        {
            go_attackLight.SetActive(false);
        }
    }

}
