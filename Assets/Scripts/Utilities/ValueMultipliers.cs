using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueMultipliers : MonoBehaviour
{
    private static int i_playerDamageMultiplier = 1;
    private static int i_projectileSpeedMultiplier = 1;

    public static void IncreasePlayerDamageMultiplier(int value)
    {
        i_playerDamageMultiplier = value;
    }

    public static void ResetPlayerDamageMultiplier()
    {
        i_playerDamageMultiplier = 1;
    }

    public static int PlayerDamageMultiplier()
    {
        return i_playerDamageMultiplier;
    }

    public static void IncreaseProjectileSpeedMultiplier(int value)
    {
        i_projectileSpeedMultiplier = value;
    }

    public static void ResetProjectileSpeedMultiplier()
    {
        i_projectileSpeedMultiplier = 1;
    }

    public static int ProjectileSpeedMultiplier()
    {
        return i_projectileSpeedMultiplier;
    }
}
