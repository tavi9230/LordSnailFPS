using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 80f;

    private PlayerController playerController;
    private UIManager uiManager;
    private Transform playerHead;
    private Transform fov;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        playerHead = GameObject.Find("Player").transform.GetChild(2).GetChild(0).GetChild(0).transform;
        fov = GameObject.Find("Player").transform.GetChild(1).transform;
        playerController = GetComponentInParent<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (uiManager.PlayerInfoUI.activeSelf == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseX;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            //transform.localRotation = Quaternion.Euler(0f, -xRotation, 0f);
            Vector3 rot = Vector3.up * mouseY;
            Vector3 rotation = new Vector3(rot.x, 0, rot.z);
            //playerHead.localRotation = Quaternion.Euler(yRotation, -xRotation, 0f);
            //fov.localRotation = Quaternion.Euler(0f, -xRotation, 0f);
            //fov.GetChild(0).localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
