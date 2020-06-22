using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float gravity = -19.62f;
    public float jumpHeight = 1.5f;
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

        RotateArms();

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

            Vector3 move = rightMovement + upMovement;

            controller.Move(move);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
            //animationController.SetIsMoving(x != 0 || z != 0);
        }
    }

    void RotateArms()
    {
        // TODO: Apply rotation to attack hand
        //if (playerController.State.Exists(s => s == StateEnum.IsChargingLeftAttack))
        //{
        //    var body = playerController.gameObject.transform.GetChild(playerController.gameObject.transform.childCount - 1);
        //    float mouseX = Input.GetAxis("Mouse X") * 100 * Time.deltaTime;
        //    float mouseY = Input.GetAxis("Mouse Y") * 100 * Time.deltaTime;
        //    float xRotation = 0f;
        //    float yRotation = 0f;
        //    xRotation -= mouseX;
        //    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //    yRotation -= mouseY;
        //    yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        //    var leftShoulder = body.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        //    Debug.Log(leftShoulder.transform.rotation.z + mouseY);
        //    leftShoulder.Rotate(0, 0, leftShoulder.transform.rotation.z + mouseY);
        //}
    }
}
