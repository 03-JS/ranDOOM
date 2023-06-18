using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageResistPowerup : MonoBehaviour
{
    private float f_duration = 30f;
    private UIManager uiManager;
    public static bool playerHasDamageResistance;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        uiManager.EnablePowerupIcon("Partial Invincibility");
        playerHasDamageResistance = true;
        Destroy(this, f_duration);
    }

    void OnDestroy()
    {
        uiManager.DisablePowerupIcon("Partial Invincibility");
        playerHasDamageResistance = false;
    }
}
