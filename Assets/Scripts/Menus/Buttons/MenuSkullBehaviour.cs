using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuSkullBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject img_skullIcon;
    // private string str_button;
    // private string str_previousButton;

    void OnDisable()
    {
        img_skullIcon.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        /*
        str_button = eventData.pointerEnter.name;
        if (str_button != str_previousButton)
        {
            
        }
        
        img_skullIcon.SetActive(true);
        FindObjectOfType<AudioManager>().PlayMenuSfx(3); // Elevator stop sound effect
        */
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // str_previousButton = str_button;
        // img_skullIcon.SetActive(false);
    }

    public void EnableSkull()
    {
        img_skullIcon.SetActive(true);
    }

    public void DisableSkull()
    {
        img_skullIcon.SetActive(false);
    }

}
