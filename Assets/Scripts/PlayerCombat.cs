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

    Transform GetCursorTarget()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 100f, enemyLayer))
        {
            return hit.collider.transform;
        }
        else
        {
            return null;
        }
    }


    void MeleeAttack()
    {
        // Implementovat melee attack logiku
        Debug.Log("Melee attack");
        meleeCooldownTimer = melee.cooldown;
        Transform target = GetCursorTarget();
        if (target == null) return;
        Debug.Log("Melee attack on target: " + target.name);


    }

    void RangedAttack()
    {
        // Implementovat ranged attack logiku
        Debug.Log("Ranged attack");
        Transform target = GetCursorTarget();
        if (target == null) return;
        Debug.Log("Ranged attack on target: " + target.name);
    }
}

