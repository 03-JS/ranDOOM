using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ValuesManager : MonoBehaviour
{
    // public string str_mainMenuName = "MainMenu";
    // public GameObject[] cnv_menus;
    // public List<GameObject> cnv_importantMenus;
    // public GameObject go_hiddenMenuButton;
    public TextMeshProUGUI tmp_movement;
    public TextMeshProUGUI tmp_actions;
    public TextMeshProUGUI tmp_inputButtonText;
    public TextMeshProUGUI tmp_smoothCtrlsButtonText;
    public TextMeshProUGUI tmp_headBobButtonText;
    public TextMeshProUGUI tmp_musicStyleButtonText;
    public TMP_InputField tmpi_musicValue;
    public TMP_InputField tmpi_sfxValue;
    public TMP_InputField tmpi_sensitivityValue;
    public TMP_InputField tmpi_fovValue;

    private AudioManager audioManager;
    private int i_timesInputButtonPressed = 0;
    private string[] str_mnkControls = { "W: Move forward\nS: Move Backwards\nA: Move to the left\nD: Move to the right\n\nLeft Ctrl to sprint", "E to interact with doors & switches\n\n1-6 to switch between weapons\n\nQ to switch between the last 2 used weapons\n\nC to switch to the Chainsaw\n\nLeft Mouse button to use/fire a weapon\n\nESC to pause the game" };
    private string[] str_controllerControls = { "", "" };

    private static string defaultMusicText = "100";
    private static string defaultSfxText = "100";
    private static string defaultMusicStyleText = "Music Style: <color=yellow>Remade</color>";
    private static string defaultSensitivityText = "5";
    private static string defaultFOVText = "90";
    private static string defaultCtrlsFeelText = "Smooth Controls:<color=yellow> On</color>";
    private static string defaultHeadBobText = "Headbob:<color=yellow> On</color>";

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        /*
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ApplyFOVChange();
            ApplySensitivitySettings();
            ApplySoundSettings();
            AudioManager.isMusicClassic = true;
            CharacterMovement.controlsAreSmooth = false;
            CharacterMovement.headBob = false;
        }
        */
        tmpi_musicValue.text = defaultMusicText + "";
        tmpi_sfxValue.text = defaultSfxText;
        tmp_musicStyleButtonText.SetText(defaultMusicStyleText);
        tmp_smoothCtrlsButtonText.SetText(defaultCtrlsFeelText);
        tmpi_fovValue.text = defaultFOVText;
        tmpi_sensitivityValue.text = defaultSensitivityText;
        tmp_headBobButtonText.SetText(defaultHeadBobText);
        tmp_movement.text = str_mnkControls[0]; // Movement controls text
        tmp_actions.text = str_mnkControls[1]; // Action controls text
    }

    public void ChangeControlsInput()
    {
        audioManager.PlayMenuSfx(3);
        if (i_timesInputButtonPressed == 0)
        {
            i_timesInputButtonPressed++;
            tmp_inputButtonText.text = "Input:\n<color=yellow>Controller</color>";
            tmp_movement.text = str_controllerControls[0]; // Movement controls text
            tmp_actions.text = str_controllerControls[1]; // Action controls text
        }
        else
        {
            i_timesInputButtonPressed = 0;
            tmp_inputButtonText.text = "Input:\n<color=yellow>Mouse and Keyboard</color>";
            tmp_movement.text = str_mnkControls[0]; // Movement controls text
            tmp_actions.text = str_mnkControls[1]; // Action controls text
        }
    }

    public void ApplySoundSettings()
    {
        try
        {
            float sfxVolume = float.Parse(tmpi_sfxValue.text);
            float musicVolume = float.Parse(tmpi_musicValue.text);
            audioManager.ChangeMusicVolume(musicVolume);
            audioManager.ChangeSfxVolume(sfxVolume);
            defaultMusicText = (int) musicVolume + "";
            defaultSfxText = (int) sfxVolume + "";
        }
        catch (System.Exception)
        {
            Debug.Log("An error ocurred while trying to parse a value from the sound settings");
        }

    }

    public void ApplySensitivitySettings()
    {
        float f_sensitivity = float.Parse(tmpi_sensitivityValue.text);
        CharacterLook.ChangeMouseSensitivity(f_sensitivity);
        defaultSensitivityText = "" + f_sensitivity;
    }

    public void ApplyControlsFeelChange()
    {
        audioManager.PlayMenuSfx(3);
        if (!CharacterMovement.controlsAreSmooth)
        {
            // i_timesSmoothCtrlsButtonPressed++;
            tmp_smoothCtrlsButtonText.text = "Smooth Controls:<color=yellow> On</color>";
            CharacterMovement.controlsAreSmooth = true;
        }
        else
        {
            // i_timesSmoothCtrlsButtonPressed = 0;
            tmp_smoothCtrlsButtonText.text = "Smooth Controls:<color=yellow> Off</color>";
            CharacterMovement.controlsAreSmooth = false;
        }
        defaultCtrlsFeelText = tmp_smoothCtrlsButtonText.text;
    }
    public void ApplyFOVChange()
    {
        int i_FOV = int.Parse(tmpi_fovValue.text);
        if (i_FOV > 120)
        {
            i_FOV = 120;
            tmpi_fovValue.text = "120";
        }
        if (i_FOV < 10)
        {
            i_FOV = 10;
            tmpi_fovValue.text = "10";
        }
        CharacterLook.ChangeFOV(i_FOV);
        defaultFOVText = tmpi_fovValue.text;
    }

    public void ApplyBobChange()
    {
        audioManager.PlayMenuSfx(3);
        if (!CharacterMovement.headBob)
        {
            tmp_headBobButtonText.text = "Headbob:<color=yellow> On</color>";
            CharacterMovement.headBob = true;
        }
        else
        {
            CharacterMovement.headBob = false;
            tmp_headBobButtonText.text = "Headbob:<color=yellow> Off</color>";
        }
        defaultHeadBobText = tmp_headBobButtonText.text;
    }

    public void ChangeMusicStyle()
    {
        audioManager.PlayMenuSfx(3);
        audioManager.SwitchMusicStyle(false); // Changes the music style without showing the info message
        if (AudioManager.isMusicClassic)
        {
            tmp_musicStyleButtonText.SetText("Music Style: <color=yellow>Classic</color>");
            defaultMusicStyleText = "Music Style: <color=yellow>Classic</color>";
        }
        else
        {
            tmp_musicStyleButtonText.SetText("Music Style: <color=yellow>Remade</color>");
            defaultMusicStyleText = "Music Style: <color=yellow>Remade</color>";
        }
    }
}
