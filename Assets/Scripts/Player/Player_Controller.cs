using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Input))]
public class Player_Controller : MonoBehaviour
{

    #region Class Variables
    Player_Input playerInput;
    CharacterController m_Player;
    Animator m_Anim;
    Weapon_Controller weaponController;
    Camera_Controller cameraController;
    #endregion

    #region Movement Variables
    [Header("Movement Variables")]
    [SerializeField] [Range(1, 10)] float walkSpeed = 3;
    [SerializeField] [Range(1, 20)] float sprintSpeed = 5;
    [SerializeField] [Range(1, 10)] float jumpSpeed = 5;
    [SerializeField] [Range(1, 10)] float crouchSpeed = 2;
    [SerializeField] [Range(1, 10)] float slideSpeed = 7;
    [SerializeField] [Range(1, 30)] float playerRotationSpeed = 20;
    [SerializeField] [Range(1, 20)] float crouchLerpAmount = 10;
    [SerializeField] [Range(1, 20)] float rotationLerpAmount = 10;
    [SerializeField] [Range(1, 20)] float speedLerpAmount = 5;

    [Header("Character Controller Variables")]
    [SerializeField] float originalCCHeight = 0;
    [SerializeField] float crouchedCCHeight = 0;
    [SerializeField] float originalCameraHeight = 0;
    [SerializeField] float crouchedCameraHeight = 0;
    [SerializeField] float m_Gravity = 9.8f;
    [SerializeField] float angleClampAmount = 30f;

    Vector3 desiredVelocity;

    float playerSpeed = 0;
    bool playerRotationStored = false;
    float lastYRotation = 0;

    [HideInInspector] public float yRotation = 0;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isCrouching;
    [HideInInspector] public bool isSliding;
    #endregion


    private void Start()
    {
        weaponController = GetComponent<Weapon_Controller>();
        cameraController = GetComponent<Camera_Controller>();
        playerInput = GetComponent<Player_Input>();
        m_Player = GetComponent<CharacterController>();
        m_Anim = weaponController.ActiveWeaponAnimator;
    }

    private void Update()
    {
        Crouching();
        Jumping();
        UpdateAnimations();
        UpdateMovement();
        UpdateRotation();
    }


    #region Movement
    private void UpdateMovement()
    {
        isSprinting = playerInput.SprintInput && !isCrouching && !weaponController.isAiming;

        if (m_Player.isGrounded)
        {
            Vector3 movementVector = (transform.forward * playerInput.VerticalVelocity + transform.right * playerInput.HorizontalVelocity).normalized;
            float desiredSpeed = isSliding ? slideSpeed : (isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed));
            playerSpeed = Mathf.Lerp(playerSpeed, desiredSpeed, Time.deltaTime * speedLerpAmount);

            if (isSliding)
            {
                desiredVelocity.x = m_Player.velocity.x;
                desiredVelocity.z = m_Player.velocity.z;
            }
            else
            {
                desiredVelocity.x = movementVector.x * playerSpeed;
                desiredVelocity.z = movementVector.z * playerSpeed;
            }

        }
        else
        {
            desiredVelocity.x = m_Player.velocity.x;
            desiredVelocity.z = m_Player.velocity.z;
            isSprinting = false;
        }

        m_Player.Move(desiredVelocity * Time.deltaTime);
    }
    #endregion

    #region Rotation
    private void UpdateRotation()
    {
         yRotation += playerInput.MouseX * playerRotationSpeed * Time.deltaTime;

         if (isSliding)
             yRotation = Mathf.Clamp(yRotation, lastYRotation - angleClampAmount, lastYRotation + angleClampAmount);

         transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, yRotation, 0), Time.deltaTime * rotationLerpAmount);
    }
    #endregion

    #region Animation
    private void UpdateAnimations()
    {
        if (weaponController.ActiveWeaponAnimator == null)
            return;

        if (m_Anim == null || m_Anim != weaponController.ActiveWeaponAnimator)
            m_Anim = weaponController.ActiveWeaponAnimator;

        m_Anim.SetFloat("Speed", new Vector3(desiredVelocity.x, 0, desiredVelocity.z).magnitude);
        m_Anim.SetBool("Sprinting", isSprinting);
        m_Anim.SetBool("Grounded", m_Player.isGrounded);
    }
    #endregion

    #region Jumping
    private void Jumping()
    {
        if (m_Player.isGrounded && !isCrouching && !isSliding)
        {
            desiredVelocity.y = -m_Gravity;
            if (playerInput.JumpInput)
            {
                desiredVelocity.y = jumpSpeed;
            }
        }
        else
        {
            desiredVelocity.y -= m_Gravity * Time.deltaTime;
        }
    }
    #endregion

    #region Crouching and Sliding
    void Crouching() {

        isCrouching = playerInput.CrouchInput && m_Player.isGrounded && !isSprinting;
        isSliding = playerInput.CrouchInput && m_Player.isGrounded && isSprinting;

        if (isSliding && !playerRotationStored)
        {
            lastYRotation = transform.eulerAngles.y;
            playerRotationStored = true;
        }

        if (!isSliding && playerRotationStored)
            playerRotationStored = false;

        if (isCrouching || isSliding)
        {
            if (m_Player.height != crouchedCCHeight)
            {
                m_Player.height = crouchedCCHeight;
                m_Player.center = new Vector3(m_Player.center.x, crouchedCCHeight/2f, m_Player.center.z);
            }
            if (m_Player.isGrounded && Camera.main.transform.localPosition.y != crouchedCameraHeight)
            {
                Vector3 desiredCamPos = Camera.main.transform.localPosition;
                desiredCamPos.y = crouchedCameraHeight;
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, desiredCamPos, crouchLerpAmount * Time.deltaTime);
            }
        }
        else if(!isCrouching && !isSliding)
        {
            if (m_Player.height != originalCCHeight)
            {
                m_Player.height = originalCCHeight;
                m_Player.center = new Vector3(m_Player.center.x, originalCCHeight / 2f, m_Player.center.z);
            }
            if (m_Player.isGrounded && Camera.main.transform.localPosition.y != originalCameraHeight)
            {
                Vector3 desiredCamPos = Camera.main.transform.localPosition;
                desiredCamPos.y = originalCameraHeight;
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, desiredCamPos, crouchLerpAmount * Time.deltaTime);
            }
        }
    }
    #endregion
}