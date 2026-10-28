using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Pohyb + gravitace")]
    public float moveSpeed = 5f;
    float YSpeed = 0f;
    public float gravity = -15f;

    [Header("Rotace")]
    public float rotationSpeed = 10f;

    [Header("Dodge/úhyb")]
    public float dodgeSpeed = 15f;
    public float dodgeDuration = 0.25f;
    public float dodgeBaseCooldown = 1f; //cooldown mezi úhyby
    float dodgeCooldownTimer = 0f;
    bool isDodging;
    public bool IsDodging => isDodging;
    float dodgeTimer; //do konce úhybu
    Vector3 dodgeDirection;

    CharacterController controller;
    InputAction moveAction;
    InputAction dodgeAction;
    PlayerCombat combat;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
        dodgeAction = InputSystem.actions.FindAction("Dodge");
        combat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        //cooldown úhybu
        if (dodgeCooldownTimer > 0f)
        {
            dodgeCooldownTimer -= Time.deltaTime;
        }

        //gravitace (vždy)
        if (controller.isGrounded)
        {
            YSpeed = -7f;
        }
        else
        {
            YSpeed += gravity * Time.deltaTime;
        }

        //úhyb právì probíhá
        if (isDodging)
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0f)
            {
                isDodging = false;
                dodgeCooldownTimer = dodgeBaseCooldown;
            }
            else
            {
                Vector3 dodgeMove = dodgeDirection * dodgeSpeed;
                dodgeMove.y = YSpeed;
                controller.Move(dodgeMove * Time.deltaTime);
                return; // bìhem úhybu žádná normální chùze ani otáèení
            }
        }

        //ètení vstupu
        Vector2 input = moveAction.ReadValue<Vector2>() * moveSpeed;

        //spouštìní úhybu
        if (dodgeAction.triggered && controller.isGrounded && dodgeCooldownTimer <= 0f && !isDodging)
        {
            isDodging = true;
            dodgeTimer = dodgeDuration;
            dodgeDirection = transform.forward;
            combat.CancelAttackLock(); //úhyb pøeruší attack lock
        }

        //attack lock = žádná chùze ani otáèení
        if (combat.IsAttackLocked)
        {
            controller.Move(new Vector3(0f, YSpeed, 0f) * Time.deltaTime);
            return;
        }

        //chùze
        Vector3 move = new Vector3(input.x, YSpeed, input.y);
        controller.Move(move * Time.deltaTime);

        //otáèení
        Vector3 targetDirection = new Vector3(input.x, 0f, input.y);
        if (targetDirection.sqrMagnitude > 0.01f)
        {
            Quaternion cil = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, cil, rotationSpeed * Time.deltaTime);
        }
    }
}