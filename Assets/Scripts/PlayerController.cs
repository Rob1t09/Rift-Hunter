using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [Header("Pohyb + gravitace")]
    public float MoveSpeed = 5f;
    float YSpeed = 0f;
    public float Gravity = -15f;

    [Header("Rotace")]
    public float RotationSpeed = 10f;

    [Header("dodge/úhyb")]
    public float DodgeSpeed = 10f;
    public float DodgeDuration = 1f;
    public float DodgeBaseCooldown = 1f; //cooldown mezi úhyby
    float DodgeCooldownTimer = 0f;
    bool isDodging;
    float DodgeTimer; //do konce úhybu
    Vector3 DodgeDirection;



    CharacterController controller;
    InputAction moveAction;
    InputAction dodgeAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
        dodgeAction = InputSystem.actions.FindAction("Dodge");
    }

    void Update()
    {
        //priorita
        if (DodgeCooldownTimer > 0f)
        {
            DodgeCooldownTimer -= Time.deltaTime;
        }

        //gravitace (vždy)
        if (controller.isGrounded)
        {
            YSpeed = -2f;
        }
        else
        {
            YSpeed += Gravity * Time.deltaTime;
        }


        //úhyb právì probíhá
        if (isDodging)
        {
            DodgeTimer -= Time.deltaTime;
            if (DodgeTimer <= 0f)
            {
                isDodging = false;
                DodgeCooldownTimer = DodgeBaseCooldown;
            }
            else
            {
                Vector3 dodgeMove = DodgeDirection * DodgeSpeed;
                dodgeMove.y = YSpeed;
                controller.Move(dodgeMove * Time.deltaTime);
                return; // bìhem úhybu žádná normální chùze ani otáèení
            }
        }







        //ètení vstupu
        Vector2 input = moveAction.ReadValue<Vector2>() * MoveSpeed;

        //spouštìní úhybu
        if (dodgeAction.triggered && controller.isGrounded && DodgeCooldownTimer <= 0f && !isDodging)
        {
            isDodging = true;
            DodgeTimer = DodgeDuration;
            DodgeDirection = transform.forward;
        }

        //chùze
        Vector3 move = new Vector3(input.x, YSpeed, input.y);
        controller.Move(move * Time.deltaTime);

        //otáèení
        Vector3 TargetDirection = new Vector3(input.x, 0f, input.y);
        if (TargetDirection.sqrMagnitude > 0.01f)
        {
            Quaternion cil = Quaternion.LookRotation(TargetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, cil, RotationSpeed * Time.deltaTime);
        }
    }
}
