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
            Collider targetCollider = target.GetComponent<Collider>();
            Vector3 closest = targetCollider.ClosestPoint(transform.position);
            Vector3 closestToTarget = closest - transform.position;
            closestToTarget.y = 0f; //ignoruju y, chci jen horizontální úhel
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f; //ignoruju y, chci jen horizontální úhel

            //kontrola, jestli je target v dosahu
            if (closestToTarget.magnitude > melee.range)
            {
                return;
            }

            //otoèení na primary target
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(directionToTarget);
            }
        }


        Collider[] enemies = Physics.OverlapSphere(transform.position, melee.range + 1f, enemyLayer);
        Debug.Log("Nalezeno: " + enemies.Length);
        foreach (Collider enemy in enemies)
        {
            //nejbližší bod na collideru, a velký enemy nevypadne z oblouku
            Vector3 closestPoint = enemy.ClosestPoint(transform.position);
            Vector3 directionToEnemy = closestPoint - transform.position;
            directionToEnemy.y = 0f; //ignoruju y, chci jen horizontální úhel

            if (directionToEnemy.magnitude > melee.range)
            {
                continue; 
            }

            float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);
            Debug.Log($"{enemy.name} úhel: {angleToEnemy}");
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