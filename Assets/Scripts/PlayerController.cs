using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    float YSpeed = 0f;
    public float Gravity = -15f;

    public float RotationSpeed = 10f; 



    CharacterController controller;
    InputAction moveAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>() * MoveSpeed;
        
        
        if (controller.isGrounded)
        {
            YSpeed = -2f;
        }
        else
        {
            YSpeed += Gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3(input.x, YSpeed, input.y);

        controller.Move(move * Time.deltaTime);

        Vector3 TargetAngle = new Vector3(input.x, 0f, input.y);
        if (TargetAngle.sqrMagnitude > 0.01f)
        {
            Quaternion cil = Quaternion.LookRotation(TargetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, cil, RotationSpeed * Time.deltaTime);
        }
    }
}
