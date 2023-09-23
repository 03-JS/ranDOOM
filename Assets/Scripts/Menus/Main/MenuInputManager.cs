using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuInputManager : MonoBehaviour
{
    public int index;
    public GameObject previousMenu;
    [Space]
    public Button[] buttons;
    // public TMP_InputField[] inputFields;
    [Space]
    public bool enablesHiddenObjects;
    public bool playsAudioCue;
    public GameObject[] objects;
    public static bool gameIsPaused;

    private AudioManager audioManager;
    private int activeButtonIndex;
    private int previousButtonIndex;
    private KeyCode pauseKeyCode;
    private int i_timesEscapeHasBeenPressed = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        // activeButtonIndex = -1;
        // previousButtonIndex = 1;
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            pauseKeyCode = KeyCode.Joystick1Button3;
        }
        else
        {
            pauseKeyCode = KeyCode.Joystick1Button7;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(pauseKeyCode))
        {
            GoBack();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            SelectButton(1);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            SelectButton(-1);
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (activeButtonIndex >= 0 && activeButtonIndex < buttons.Length)
            {
                if (buttons[activeButtonIndex].gameObject.activeInHierarchy)
                {
                    buttons[activeButtonIndex].GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }

    void OnEnable()
    {
        activeButtonIndex = -1;
        previousButtonIndex = 1;
    }

    public void SelectLevel(string str_levelName)
    {
        SceneManager.LoadScene(str_levelName);
    }

    public void RandomlySelectLevel()
    {
        SceneManager.LoadScene(Random.Range(2, SceneManager.sceneCountInBuildSettings));
    }

    public void EnableHiddenObjects()
    {
        if (enablesHiddenObjects)
        {
            if (playsAudioCue)
            {
                audioManager.PlayMenuSfx(4); // Secret sound effect
            }
            foreach (GameObject gameObj in objects)
            {
                gameObj.SetActive(true);
            }
        }
    }

    private void DisableHiddenObjects()
    {
        if (enablesHiddenObjects)
        {
            foreach (GameObject gameObj in objects)
            {
                gameObj.SetActive(false);
            }
        }
    }

    public void GoBack()
    {
        // activeButtonIndex = -1;
        if (index >= 2)
        {
            transform.parent.gameObject.SetActive(false);
            previousMenu.SetActive(true);
            audioManager.PlayMenuSfx(2);
        }
        else
        {
            if (index == 1)
            {
                Quit();
                audioManager.PlayMenuSfx(2);
            }
            else
            {
                if (i_timesEscapeHasBeenPressed == 0)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
    }

    public void ChangeMenu(GameObject menu)
    {
        audioManager.PlayMenuSfx(1);
        activeButtonIndex = -1;
        transform.parent.gameObject.SetActive(false);
        menu.SetActive(true);
    }

    private void SelectButton(int number)
    {
        activeButtonIndex += number;
        if (previousButtonIndex >= buttons.Length)
        {
            previousButtonIndex = 0;
        }
        if (activeButtonIndex >= buttons.Length)
        {
            activeButtonIndex = 0;
        }
        if (activeButtonIndex < 0)
        {
            activeButtonIndex = buttons.Length - 1;
        }
        RecursiveButtonSelect();
    }

    private void RecursiveButtonSelect()
    {
        if (buttons[activeButtonIndex].gameObject.activeInHierarchy)
        {
            audioManager.PlayMenuSfx(3);
            // Debug.Log(previousButtonIndex);
            if (previousButtonIndex >= buttons.Length)
            {
                previousButtonIndex = buttons.Length - 1;
            }
            if (buttons[previousButtonIndex].GetComponent<MenuSkullBehaviour>())
            {
                buttons[previousButtonIndex].GetComponent<MenuSkullBehaviour>().DisableSkull();
            }
            if (buttons[activeButtonIndex].GetComponent<MenuSkullBehaviour>())
            {
                buttons[activeButtonIndex].GetComponent<MenuSkullBehaviour>().EnableSkull();
            }
            previousButtonIndex = activeButtonIndex;
        }
        else
        {
            activeButtonIndex--;
            if (activeButtonIndex < 0)
            {
                return;
            }
            RecursiveButtonSelect();
        }
    }

    public void PauseGame()
    {
        activeButtonIndex = -1;
        previousButtonIndex = 1;
        audioManager.PlayMenuSfx(1);
        gameIsPaused = true;
        i_timesEscapeHasBeenPressed++;
        Time.timeScale = 0;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        EnableHiddenObjects();
    }

    public void ResumeGame()
    {
        audioManager.PlayMenuSfx(2);
        gameIsPaused = false;
        i_timesEscapeHasBeenPressed = 0;
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        DisableHiddenObjects();
    }

    public void Quit()
    {
        audioManager.PlayMenuSfx(2);
        Application.Quit();
        // UnityEditor.EditorApplication.isPlaying = false;
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1;
        audioManager.PlayMenuSfx(1);
        SceneManager.LoadScene("Main Menu");
        gameIsPaused = false;
        AudioListener.pause = false;
    }

}
