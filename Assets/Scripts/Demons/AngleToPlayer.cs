using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleToPlayer : MonoBehaviour
{
    private float f_angle;
    private int i_lastIndex;
    private Animator spriteAnimator;
    private SpriteRenderer spriteRenderer;
    private Transform t_playerTransform;
    private DemonHealth demonHealth;

    // Start is called before the first frame update
    void Start()
    {
        demonHealth = GetComponent<DemonHealth>();
        t_playerTransform = FindObjectOfType<CharacterMovement>().transform;
        spriteAnimator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((demonHealth == null || !demonHealth.isDead))
        {
            Vector3 v3_playerPosition = t_playerTransform.position;
            v3_playerPosition.y = transform.position.y;
            f_angle = Vector3.SignedAngle(v3_playerPosition - transform.position, transform.forward, Vector3.up);
            Vector3 v3_tempScale = Vector3.one;
            // Flips the sprite
            if (f_angle < -22.5f)
            {
                v3_tempScale.x *= -1f;
            }
            spriteRenderer.transform.localScale = v3_tempScale;
            i_lastIndex = GetIndex(f_angle);
            spriteAnimator.SetFloat("spriteRotation", i_lastIndex);
        }
    }

    private int GetIndex(float f_angle)
    {
        // Front
        if (f_angle > -22.5f && f_angle < 22.6f)
        {
            return 0;
        }
        if (f_angle >= 22.5f && f_angle < 67.5f)
        {
            return 7;
        }
        if (f_angle >= 67.5f && f_angle < 112.5f)
        {
            return 6;
        }
        if (f_angle >= 112.5f && f_angle < 157.5f)
        {
            return 5;
        }

        // Back
        if (f_angle <= -157.5 || f_angle >= 157.5f)
        {
            return 4;
        }
        if (f_angle >= -157.4f && f_angle < -112.5f)
        {
            return 3;
        }
        if (f_angle >= -112.5f && f_angle < -67.5f)
        {
            return 2;
        }
        if (f_angle >= -67.5f && f_angle <= -22.5f)
        {
            return 1;
        }
        return i_lastIndex;
    }
}
