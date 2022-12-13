using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public bool CanMove { get; private set;} = true;

    [Header("Movement Parameters")]
    [SerializeField] private float walkspeed = 3.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedx = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedy = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector3 currentInput;

    private float rotationX = 0;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponentInChildren<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleLookInput();

            ApplyFinalMovements();
        }
        
    }

    private void HandleMovementInput ()
    {
        currentInput = new Vector2(walkspeed * Input.GetAxis("Vertical"), walkspeed * Input.GetAxis("Horizontal"));
        
        float moveDirectionY = moveDirection.y;
        moveDirection = (transfrom.TransformDirection(Vector3.forward) * currentInput.x) + (transfrom.TransformDirection(Vector3.right) * currentInput.y);
    }

    private void HandleLookInput ()
    {
            
    }

    private void ApplyFinalMovements ()
    {
            
    }
}
