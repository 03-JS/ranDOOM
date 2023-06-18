using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadDamagePowerup : MonoBehaviour
{
    private float f_duration = 30f;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        ValueMultipliers.IncreasePlayerDamageMultiplier(2);
        uiManager = FindObjectOfType<UIManager>();
        uiManager.EnablePowerupIcon("Berserk");
        Destroy(this, f_duration);
    }

    void OnDestroy()
    {
        ValueMultipliers.ResetPlayerDamageMultiplier();
        uiManager.DisablePowerupIcon("Berserk");
    }
}
