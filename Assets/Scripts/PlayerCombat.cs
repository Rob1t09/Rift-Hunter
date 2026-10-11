using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public WeaponStatsMelee melee;
    public LayerMask enemyLayer;

    float meleeCooldownTimer;


    InputAction attackAction;
    InputAction rangedAction;


    void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        rangedAction = InputSystem.actions.FindAction("Ranged");
    }

    private void Update()
    {
        if (meleeCooldownTimer > 0f)
        {
            meleeCooldownTimer -= Time.deltaTime;
        }

        if (attackAction.triggered && meleeCooldownTimer <= 0)
        {
            MeleeAttack();
        }
        if (rangedAction.triggered)
        {
            RangedAttack();
        }
    }


    void MeleeAttack()
    {
        // Implementovat melee attack logiku
        Debug.Log("Melee attack");
        meleeCooldownTimer = melee.cooldown;
    }

    void RangedAttack()
    {
        // Implementovat ranged attack logiku
        Debug.Log("Ranged attack");
    }
}

