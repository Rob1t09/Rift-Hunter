using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerCombat : MonoBehaviour
{
    public WeaponStatsMelee melee;
    public WeaponStatsRanged ranged;
    public LayerMask enemyLayer;

    float meleeCooldownTimer;
    float attackLockTimer;
    float rangedCooldownTimer;
    float rangedAttackLockTimer;
    public bool IsAttackLocked => attackLockTimer > 0f || rangedAttackLockTimer > 0f;

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
        //casovace pro cooldowny a locky
        if (meleeCooldownTimer > 0f)
        {
            meleeCooldownTimer -= Time.deltaTime;
        }
        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;
        }
        if (rangedCooldownTimer > 0f)
        {
            rangedCooldownTimer -= Time.deltaTime;
        }
        if (rangedAttackLockTimer > 0f)
        {
            rangedAttackLockTimer -= Time.deltaTime;
        }


        if (playerController.IsDodging) return;
        if (attackAction.IsPressed() && meleeCooldownTimer <= 0 && !IsAttackLocked)
        {
            MeleeAttack();
        }
        if (rangedAction.IsPressed() && rangedCooldownTimer <= 0 && !IsAttackLocked)
        {
            RangedAttack();
        }
    }

    public void CancelAttackLock()
    {
        attackLockTimer = 0f;
        rangedAttackLockTimer = 0f;
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
            Vector3 flatDirection = target.position - transform.position;
            flatDirection.y = 0f; //ignoruju y, chci jen horizontální úhel

            //kontrola, jestli je target v dosahu
            if (closestToTarget.magnitude > melee.range)
            {
                return;
            }

            //otoèení na primary target
            if (flatDirection.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(flatDirection);
            }
        }


        Collider[] enemies = Physics.OverlapSphere(transform.position, melee.range + 1f, enemyLayer);
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

        rangedCooldownTimer = ranged.cooldown;
        rangedAttackLockTimer = ranged.attackLockTime;
        Vector3 chest = transform.position + Vector3.up;
        Vector3 direction = transform.forward;
        Transform target = GetCursorTarget();
        if (target != null)
        {
            Collider targetCollider = target.GetComponent<Collider>();
            Vector3 aimPoint = targetCollider.ClosestPoint(chest);
            direction = aimPoint - chest;

            Vector3 flat = direction;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(flat);
            }
        }

        Vector3 spawnPoint = chest + transform.forward * 0.5f;

        for (int i = 0; i < ranged.projectileAmount; i++)
        {
            float offset = (i-(ranged.projectileAmount-1) / 2f) * ranged.spreadAngle;
            Quaternion spreadRotation = Quaternion.AngleAxis(offset, Vector3.up) * Quaternion.LookRotation(direction);
            GameObject projectile = Instantiate(ranged.projectilePrefab, spawnPoint, spreadRotation);
            projectile.GetComponent<Projectile>().Init(ranged.damage, ranged.projectileSpeed);
        }
    }
}