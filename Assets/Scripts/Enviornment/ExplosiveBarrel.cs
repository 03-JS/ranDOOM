using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public int i_health = 40;
    public int i_damage = 128;
    public float explosionForce = 20f;
    public float f_explosionRadius = 5f;
    public float f_destructionDelay = 0.04f;
    public AudioClip ac_explosionSfx;

    private Animator barrelAnimator;
    private int i_explosionCount = 0;
    private Rigidbody rigidBody;

    void Start()
    {
        barrelAnimator = gameObject.GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    public void TakeDamage(float amount)
    {
        i_health -= (int) amount;
        if (i_health <= 0 && i_explosionCount == 0)
        {
            i_explosionCount++;
            barrelAnimator.SetTrigger("Explosion");
            Destroy(gameObject, f_destructionDelay);
        }
    }

    public void CreateExplosion()
    {
        Vector3 v3_explosionOrigin = transform.position;
        Collider[] collidersInExplosion = Physics.OverlapSphere(v3_explosionOrigin, f_explosionRadius);
        foreach (Collider collider in collidersInExplosion)
        {
            int layerMask = LayerMask.GetMask("Default", "Demon", "Player", "Cyberdemon");
            RaycastHit hitInfo;
            float f_distance = Vector3.Distance(v3_explosionOrigin, collider.transform.position);
            if (Physics.Raycast(v3_explosionOrigin, collider.transform.position - v3_explosionOrigin, out hitInfo, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.collider == collider)
                {
                    if (collider.gameObject.tag == "Demon")
                    {
                        collider.GetComponent<DemonHealth>().TakeDamage(i_damage - (f_distance * 20f), false, null);
                        collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, v3_explosionOrigin, f_explosionRadius);
                    }
                    if (collider.gameObject.tag == "Player")
                    {
                        collider.GetComponent<PlayerHealthAndArmor>().TakeDamage(i_damage - (f_distance * 20f));
                    }
                    if (collider.gameObject.tag == "Explosive Barrel" && gameObject != collider.gameObject)
                    {
                        collider.GetComponent<ExplosiveBarrel>().TakeDamage(i_damage - (f_distance * 20f));
                        collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, v3_explosionOrigin, f_explosionRadius);
                    }
                }
            }
        }
    }

    public void PlayExplosionSfx()
    {
        AudioSource.PlayClipAtPoint(ac_explosionSfx, transform.position, AudioManager.f_globalSfxVolume);
    }

}
