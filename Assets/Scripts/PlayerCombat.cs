using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public WeaponStatsMelee melee;
    public LayerMask enemyLayer;

    float meleeCooldownTimer;
    float attackLockTimer;
    public bool IsAttackLocked => attackLockTimer > 0f;

    InputAction attackAction;
    InputAction rangedAction;
    PlayerController playerController;


    void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        rangedAction = InputSystem.actions.FindAction("Ranged");
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (meleeCooldownTimer > 0f)
        {
            meleeCooldownTimer -= Time.deltaTime;
        }
        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;
        }


        if (playerController.IsDodging) return;
        if (attackAction.triggered && meleeCooldownTimer <= 0 && !IsAttackLocked)
        {
            MeleeAttack();
        }
        if (rangedAction.triggered)
        {
            RangedAttack();
        }
    }

    public void CancelAttackLock()
    {
        attackLockTimer = 0f;
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
        
        meleeCooldownTimer = melee.cooldown;
        attackLockTimer = melee.attackLockTime;
        Transform target = GetCursorTarget();
        if (target != null)
        {
            //delka cesty k targetu
            Vector3 directionToTarget = target.position - transform.position;

            if (directionToTarget.magnitude > melee.range)
            {

                return;
            }

            //otoèení na primary target
            directionToTarget.y = 0; //ignoruju y, chci jen horizontální úhel
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(directionToTarget);
            }
        }


        Collider[] enemies = Physics.OverlapSphere(transform.position, melee.range, enemyLayer);
        foreach (Collider enemy in enemies)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            directionToEnemy.y = 0; //ignoruju y, chci jen horizontální úhel
            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
            if (angleToEnemy <= melee.arc / 2f)
            {
                Health health = enemy.GetComponent<Health>();
                if (health != null)
                {
                    if (enemy.transform == target)
                    {
                        health.TakeDamage(melee.damage);
                    }
                    else
                    {
                        health.TakeDamage(melee.damage * melee.secondaryDamageMultiplier);
                    }
                }
            }
        }

    }

    void RangedAttack()
    {
        // Implementovat ranged attack logiku
        Transform target = GetCursorTarget();
        if (target != null)
        {

        }
    }
}