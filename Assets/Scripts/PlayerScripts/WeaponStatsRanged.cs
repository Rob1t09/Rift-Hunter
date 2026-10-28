using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class WeaponStatsRanged
{
    public string weaponName;
    public GameObject projectilePrefab;
    public float damage;
    public float cooldown;
    public float attackLockTime;
    public float projectileSpeed;
    public int projectileAmount = 1;
    public float spreadAngle = 15f;
}
