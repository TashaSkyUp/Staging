using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input : MonoBehaviour {

    #region Axis Names and Keys
    [SerializeField] string horizontalAxis = "Horizontal";
    [SerializeField] string verticalAxis = "Vertical";
    [SerializeField] string mouseXAxis = "Mouse X";
    [SerializeField] string mouseYAxis = "Mouse Y";
    [SerializeField] string mouseScrollWheel = "Mouse ScrollWheel";
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode reloadKey = KeyCode.R;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] int primaryMouseButton = 0;
    [SerializeField] int secondaryMouseButton = 1;
    #endregion

    #region Input Booleans Variables
    float verticalVelocity = 0;
    float horizontalVelocity = 0;
    float mouseX = 0;
    float mouseY = 0;
    bool jumpInput = false;
    bool sprintInput = false;
    bool shootInput = false;
    bool aimInput = false;
    bool shootContinuosInput = false;
    bool reloadInput;
    float mouseScroll;
    bool crouchInput;
    #endregion

    #region Properties
    public float VerticalVelocity { get { return verticalVelocity; } }
    public float HorizontalVelocity { get { return horizontalVelocity; } }
    public float MouseX { get { return mouseX; } }
    public float MouseY { get { return mouseY; } }
    public bool JumpInput { get { return jumpInput; } }
    public bool SprintInput { get { return sprintInput; } }
    public bool ShootInput { get { return shootInput; } }
    public bool ShootContinuosInput { get { return shootContinuosInput; } }
    public bool AimInput { get { return aimInput; } }
    public bool ReloadInput { get { return reloadInput; } }
    public float MouseScroll { get { return mouseScroll; } }
    public bool CrouchInput { get { return crouchInput; } }
    #endregion


    private void Update()
    {
        GetInput();    
    }

    void GetInput() {
        verticalVelocity = Input.GetAxisRaw(verticalAxis);
        horizontalVelocity = Input.GetAxisRaw(horizontalAxis);
        sprintInput = Input.GetKey(sprintKey);
        jumpInput = Input.GetKeyDown(jumpKey);
        mouseX = Input.GetAxisRaw(mouseXAxis);
        mouseY = Input.GetAxisRaw(mouseYAxis);
        shootInput = Input.GetMouseButtonDown(primaryMouseButton);
        aimInput = Input.GetMouseButton(secondaryMouseButton);
        shootContinuosInput = Input.GetMouseButton(primaryMouseButton);
        reloadInput = Input.GetKeyDown(reloadKey);
        mouseScroll = Input.GetAxisRaw(mouseScrollWheel);
        crouchInput = Input.GetKey(crouchKey);
    }

}
