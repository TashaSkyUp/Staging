using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    Player_Input playerInput;

    [SerializeField] [Range(1, 30)] float lookSensitivity = 25;
    [SerializeField] [Range(1, 20)] float rotationLerpAmount = 10;
    [SerializeField] float maxXClampAmount = 70f;
    [SerializeField] float minXClampAmount = -80;
    [SerializeField] bool invertMouse = false;

    Camera playerCam;

    [HideInInspector] public float xRotation = 0;

    private void Start()
    {
        playerInput = GetComponent<Player_Input>();
        playerCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float xRot = invertMouse ? playerInput.MouseY : -playerInput.MouseY;
        xRotation += xRot * lookSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, minXClampAmount, maxXClampAmount);
        playerCam.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation, Quaternion.Euler(xRotation, 0, 0), Time.deltaTime * rotationLerpAmount);
    }
}
