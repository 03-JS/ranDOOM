using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController ccont_controller;
    public float f_speed = 7f;
    public float momentumDamping = 3f;
    public float f_speedIncrease = 2f;
    public float f_gravity = -19.62f;
    public float f_fallHeightLimit = -100f;
    public float f_teleportFatigue = 3f;
    [HideInInspector] public bool canTeleport;
    public float f_toggleSprint = 0f;
    public Animator cameraAnimator;

    private float f_timer;
    private float inputMagnitude;
    private Vector3 v3_initalPosition;
    // private Vector3 lastPosition;
    private Quaternion q_initialRotation;
    private Vector3 v3_velocity;
    private Vector3 v3_move;
    private PlayerHealthAndArmor player;

    // Static variables
    public static bool controlsAreSmooth = true;
    public static bool headBob = true;

    // Start is called before the first frame update
    void Start()
    {
        // FindObjectOfType<AudioManager>().PlaySong();
        v3_initalPosition = transform.position;
        q_initialRotation = transform.rotation;
        canTeleport = true;
        player = GetComponent<PlayerHealthAndArmor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isDead)
        {
            if (transform.position.y < f_fallHeightLimit)
            {
                transform.position = v3_initalPosition;
                transform.rotation = q_initialRotation;
                return;
            }
            if (ccont_controller.isGrounded)
            {
                v3_velocity.y = 0;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Joystick2Button5))
            {
                f_toggleSprint++;
                if (f_toggleSprint == 1)
                {
                    f_speed *= f_speedIncrease;
                }
                else
                {
                    f_speed /= f_speedIncrease;
                    f_toggleSprint = 0;
                }
            }
            float f_x = 0f;
            float f_z = 0f;
            if (controlsAreSmooth)
            {
                f_x = Input.GetAxis("Horizontal");
                f_z = Input.GetAxis("Vertical");
            }
            else
            {
                f_x = Input.GetAxisRaw("Horizontal");
                f_z = Input.GetAxisRaw("Vertical");
            }
            if (Input.GetKey(KeyCode.W) ||
                Input.GetKey(KeyCode.A) ||
                Input.GetKey(KeyCode.S) ||
                Input.GetKey(KeyCode.D))
            {
                v3_move = transform.right * f_x + transform.forward * f_z;
                inputMagnitude = v3_move.magnitude;
                v3_move.Normalize();
                if (headBob)
                {
                    cameraAnimator.SetBool("isWalking", true);
                }
            }
            else
            {
                v3_move = Vector3.Lerp(v3_move, Vector3.zero, momentumDamping * Time.deltaTime);
            }
            // Debug.Log("Gravity pull: " + v3_move.y);
            // Debug.Log("X Axis: " + f_x);
            // Debug.Log("Z Axis: " + f_z);
            ccont_controller.Move(v3_move * inputMagnitude * f_speed * Time.deltaTime);
            if (headBob)
            {
                // Debug.Log("Player velocity magnitude: " + ccont_controller.velocity.magnitude);
                if (ccont_controller.velocity.magnitude >= 7f)
                {
                    cameraAnimator.SetBool("isWalking", true);
                }
                else
                {
                    cameraAnimator.SetBool("isWalking", false);
                }
            }
            v3_velocity.y += f_gravity * Time.deltaTime;
            ccont_controller.Move(v3_velocity * Time.deltaTime);

            // Teleportation shenanigans
            if (!canTeleport)
            {
                f_timer += Time.deltaTime;
                if (f_timer >= f_teleportFatigue) // The amount of time it takes for the player to be able to teleport again in seconds
                {
                    canTeleport = true;
                    f_timer = 0;
                }
            }
            // lastPosition = transform.position;
        }
    }

    /*
    public static void ChangeControlsFeel(bool smooth)
    {
        controlsAreSmooth = smooth;
    }
    */

    public void StopAllMomentum()
    {
        v3_move *= 0;
        inputMagnitude = 0;
    }

}
