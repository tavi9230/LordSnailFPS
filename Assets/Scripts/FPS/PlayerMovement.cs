using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PlayerAnimationController animationController;
    private CharacterController controller;
    private PlayerController playerController;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 forward, right;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animationController = GetComponent<PlayerAnimationController>();
        controller = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (!playerController.State.Exists(s => s == StateEnum.Attacking))
        {
            forward = Camera.main.transform.forward;
            forward.y = 0;
            forward = Vector3.Normalize(forward);

            right = Quaternion.Euler(0, 90, 0) * forward;

            float x = Input.GetAxis("HorizontalIsometric");
            float y = Input.GetAxis("Jump");
            float z = Input.GetAxis("VerticalIsometric");

            Vector3 rightMovement = right * playerController.Stats.TotalSpeed * Time.deltaTime * x;
            Vector3 upMovement = forward * playerController.Stats.TotalSpeed * Time.deltaTime * z;

            //Vector3 move = transform.right * x + transform.forward * z;
            Vector3 move = rightMovement + upMovement;

            controller.Move(move);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
            animationController.SetMove(x, 0, z);
        }
    }
}
