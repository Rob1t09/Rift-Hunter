using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    float YSpeed = 0f;
    public float gravity = -15f;
    

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
            YSpeed += gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3(input.x, YSpeed, input.y);

        controller.Move(move * Time.deltaTime);

        Console.WriteLine($"Move: {move}, YSpeed: {YSpeed}, IsGrounded: {controller.isGrounded}");
    }
}
