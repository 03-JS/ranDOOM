using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonHealth : MonoBehaviour
{
    public int i_health;
    public float f_painChance;
    [HideInInspector] public bool isDead;
    public bool canInfight;
    public float f_infightChance = 10f;
    public bool hasGibbingAnimation;
    public bool canBeResurrected;
    public AudioClip[] ac_deathSounds;
    public AudioClip ac_xDeathSound;

    // Demon death event variables
    public delegate void DemonDeath();
    public static event DemonDeath OnDemonDeath;

    private int i_maxHealth;
    private Rigidbody rigidBody;
    private Animator demonAnimator;
    private Collider demonCollider;
    private NavMeshAgent agent;
    private DemonAI demonAI;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        demonAnimator = GetComponentInChildren<Animator>();
        demonCollider = GetComponent<Collider>();
        demonAI = GetComponentInChildren<DemonAI>();
        i_maxHealth = i_health;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    public void TakeDamage(float value, bool useMultiplier, GameObject originator)
    {
        if (!isDead)
        {
            if (useMultiplier)
            {
                value *= ValueMultipliers.PlayerDamageMultiplier();
            }
            if (value > 0)
            {
                if (demonAI.go_projectile != null)
                {
                    if (originator != null)
                    {
                        if (originator.name != gameObject.name)
                        {
                            i_health -= (int)value;

                            // Sets a target that isn't the player when not aggro
                            SetTargetIfDormant(originator);
                            if (demonAI.isAggro)
                            {
                                // Infighting logic
                                Infight(originator);
                            }

                            // Generates a random value between 1 and 10 to determine if the demon will recieve pain
                            ReceivePain(originator);
                        }
                    }
                    else
                    {
                        i_health -= (int)value;

                        // Generates a random value between 0.1 and 10 to determine if the demon will recieve pain
                        ReceivePain(originator);
                    }
                }
                else
                {
                    if (originator != null)
                    {
                        // Sets a target that isn't the player when not aggro
                        SetTargetIfDormant(originator);
                        if (demonAI.isAggro)
                        {
                            // Infighting logic
                            Infight(originator);
                        }

                        // Generates a random value between 1 and 10 to determine if the demon will recieve pain
                        ReceivePain(originator);
                    }
                    i_health -= (int)value;
                }
                if (i_health <= 0)
                {
                    isDead = true;
                    rigidBody.isKinematic = true;
                    LevelStats.kills++;
                    if (OnDemonDeath != null)
                    {
                        OnDemonDeath();
                    }
                    if (i_health <= -i_maxHealth)
                    {
                        if (hasGibbingAnimation)
                        {
                            PlayXDeathAnim();
                        }
                        else
                        {
                            PlayDeathAnim();
                        }
                    }
                    else
                    {
                        PlayDeathAnim();
                    }
                }
                demonAI.isAggro = true;
            }
        }
    }

    public void PlayDeathAnim()
    {
        // demonCollider.isTrigger = true;
        demonCollider.gameObject.layer = 13;
        rigidBody.isKinematic = false;
        agent.enabled = false;
        demonAI.DisableAttackLight();
        demonAnimator.SetTrigger("Death");
        demonAnimator.ResetTrigger("Pain");
        AudioSource.PlayClipAtPoint(ac_deathSounds[Random.Range(0, ac_deathSounds.Length)], transform.position, AudioManager.f_globalSfxVolume);
        // demonCollider.enabled = false;
    }

    public void PlayXDeathAnim()
    {
        // demonCollider.isTrigger = true;
        demonCollider.gameObject.layer = 13;
        rigidBody.isKinematic = false;
        agent.enabled = false;
        demonAI.DisableAttackLight();
        demonAnimator.SetTrigger("XDeath");
        demonAnimator.ResetTrigger("Pain");
        AudioSource.PlayClipAtPoint(ac_xDeathSound, transform.position, AudioManager.f_globalSfxVolume);
        // demonCollider.enabled = false;
    }

    public void TriggerReviveAnimation()
    {
        demonAnimator.SetTrigger("Revive");
    }

    public void Revive()
    {
        isDead = false;
        i_health = i_maxHealth;
        // demonCollider.isTrigger = false;
        gameObject.layer = 9;
        rigidBody.isKinematic = true;
        agent.enabled = true;
    }

    public void Despawn()
    {
        // Destroy(gameObject);
        gameObject.SetActive(false);
    }

    private void Infight(GameObject originator)
    {
        if (canInfight && originator != null && originator.name != "Archvile(Clone)")
        {
            float f_randomInfightValue = Random.Range(0, 11);
            if (f_randomInfightValue < f_infightChance)
            {
                if (agent.isActiveAndEnabled)
                {
                    demonAI.SetTarget(originator.transform);
                }
            }
        }
    }

    private void ReceivePain(GameObject originator)
    {
        float f_randomPainValue = Random.Range(0.1f, 11f);
        if (f_randomPainValue < f_painChance)
        {
            demonAI.ReceivePain();
            if (canInfight && originator != null && originator.name != "Archvile(Clone)")
            {
                if (agent.isActiveAndEnabled)
                {
                    demonAI.SetTarget(originator.transform);
                }
            }
        }
    }

    private void SetTargetIfDormant(GameObject originator)
    {
        if (!demonAI.isAggro)
        {
            if (canInfight && originator != null && originator.name != "Archvile(Clone)")
            {
                if (agent.isActiveAndEnabled)
                {
                    demonAI.SetTarget(originator.transform);
                }
            }
        }
    }

}
