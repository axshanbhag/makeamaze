using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float sprintSpeed = 12f;
    public float jumpForce = 7f;
    public float gravityStrength = 10f;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 45f;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchSpeedModifier = 3f;

    private Vector3 velocity = Vector3.zero;
    private float pitchRotation = 0;
    private CharacterController controller;

    private bool isMovementEnabled = true;

    // List to hold collected items
    private List<string> collectedItems = new List<string>();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraLook();
        HandleCrouch();
    }

    private void HandleMovement()
    {
        Vector3 forwardDirection = transform.TransformDirection(Vector3.forward);
        Vector3 rightDirection = transform.TransformDirection(Vector3.right);

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float movementSpeedX = isMovementEnabled ? (isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float movementSpeedY = isMovementEnabled ? (isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        float verticalSpeed = velocity.y;
        velocity = (forwardDirection * movementSpeedX) + (rightDirection * movementSpeedY);

        if (Input.GetButton("Jump") && isMovementEnabled && controller.isGrounded)
        {
            velocity.y = jumpForce;
        }
        else
        {
            velocity.y = verticalSpeed;
        }

        if (!controller.isGrounded)
        {
            velocity.y -= gravityStrength * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCameraLook()
    {
        if (isMovementEnabled)
        {
            pitchRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
            pitchRotation = Mathf.Clamp(pitchRotation, -verticalLookLimit, verticalLookLimit);

            float yawRotation = Input.GetAxis("Mouse X") * lookSensitivity;
            transform.rotation *= Quaternion.Euler(0, yawRotation, 0);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.R) && isMovementEnabled)
        {
            controller.height = crouchingHeight;
            walkSpeed = crouchSpeedModifier;
            sprintSpeed = crouchSpeedModifier;
        }
        else
        {
            controller.height = standingHeight;
            walkSpeed = 6f;
            sprintSpeed = 12f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            string collectedItem = other.gameObject.name;
            collectedItems.Add(collectedItem); 
            Debug.Log("Collected: " + collectedItem); 

            Destroy(other.gameObject);

            Debug.Log("All Collected Items so far:");
            for (int i = 0; i < collectedItems.Count; i++) 
            {
                Debug.Log((i + 1) + ": " + collectedItems[i]);
        }
}



}
