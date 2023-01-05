using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam_3Person : MonoBehaviour
{
    [Header("Reference")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public float rotationSpeed;
    public Transform combatLookAt;
    public CameraStyle currentStyle;
    [SerializeField] GameObject combatCam, thirdPersonCam, topDownCam;
    public bool canChange;

    public enum CameraStyle
    {
        Basic,
        Combat,
        TopDown
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        RotateCam();
        if(canChange)
        {
            SwitchCameraStyle(currentStyle);
            canChange = false;
        }
        
    }

    void RotateCam()
    {
        //Rotation orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if(currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.TopDown)
        {
            Rotate_BasicMode();
        }
        else if(currentStyle == CameraStyle.Combat)
        {
            Rotate_CombatMode();
        }
    }

    void Rotate_CombatMode()
    {
        Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
        orientation.forward = dirToCombatLookAt.normalized;

        playerObj.forward = dirToCombatLookAt.normalized;

    }

    void  Rotate_BasicMode()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);

    }
    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCam.SetActive(false);
        thirdPersonCam.SetActive(false);
        topDownCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
        if (newStyle == CameraStyle.TopDown) topDownCam.SetActive(true);

        currentStyle = newStyle;
    }

}
 