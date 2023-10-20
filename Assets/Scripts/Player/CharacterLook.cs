using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterLook : MonoBehaviour
{
    public Transform t_player;

    private float f_xRotation = 0f;
    private PlayerHealthAndArmor player;

    // Static variables
    public static float f_globalMouseSensitivity = 5f;
    public static int i_globalFOV = 90;
    private static Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            playerCamera = GetComponent<Camera>();
            playerCamera.fieldOfView = i_globalFOV;
            player = FindObjectOfType<PlayerHealthAndArmor>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isDead && !MenuInputManager.gameIsPaused)
        {
            float f_yAxis = Input.GetAxis("Mouse Y") * f_globalMouseSensitivity;
            float f_xAxis = Input.GetAxis("Mouse X") * f_globalMouseSensitivity;
            f_xRotation -= f_yAxis;
            f_xRotation = Mathf.Clamp(f_xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(f_xRotation, 0f, 0f);
            t_player.Rotate(Vector3.up * f_xAxis);
        }
    }

    public static void ChangeMouseSensitivity(float value)
    {
        f_globalMouseSensitivity = value;
        // Debug.Log("Mouse sensitivity: " + f_globalMouseSensitivity);
    }

    public static void ChangeFOV(int value)
    {
        i_globalFOV = value;
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = i_globalFOV;
        }
    }
}
