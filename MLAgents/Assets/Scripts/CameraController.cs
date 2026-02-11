using UnityEngine;
using UnityEngine.InputSystem; // Still required for New Input System

public class SimpleCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float sprintMultiplier = 2.5f;

    [Header("Rotation Settings")]
    public float lookSpeed = 90f; // Degrees per second (since arrow keys are constant speed)
    
    // Internal trackers to keep rotation smooth
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Snap the internal variables to the current camera angle so it doesn't jump
        Vector3 startRot = transform.localEulerAngles;
        rotationX = startRot.x;
        rotationY = startRot.y;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // ------------------------------------------
        // 1. LOOK (Arrow Keys)
        // ------------------------------------------
        float lookInputX = 0f; // Left/Right
        float lookInputY = 0f; // Up/Down

        // Read Arrow Keys
        if (keyboard.rightArrowKey.isPressed) lookInputX = 1f;
        if (keyboard.leftArrowKey.isPressed) lookInputX = -1f;
        
        if (keyboard.upArrowKey.isPressed) lookInputY = 1f;
        if (keyboard.downArrowKey.isPressed) lookInputY = -1f;

        // Apply Rotation Logic
        rotationY += lookInputX * lookSpeed * Time.deltaTime;
        rotationX -= lookInputY * lookSpeed * Time.deltaTime; // Subtract to look up naturally

        // Clamp Up/Down so you can't break your neck (flip upside down)
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        // Apply to Camera
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        // ------------------------------------------
        // 2. MOVEMENT (WASD + Q/E)
        // ------------------------------------------
        Vector3 moveDir = Vector3.zero;

        if (keyboard.wKey.isPressed) moveDir += transform.forward;
        if (keyboard.sKey.isPressed) moveDir -= transform.forward;
        if (keyboard.aKey.isPressed) moveDir -= transform.right;
        if (keyboard.dKey.isPressed) moveDir += transform.right;
        
        // Fly Up/Down
        if (keyboard.eKey.isPressed) moveDir += Vector3.up;
        if (keyboard.qKey.isPressed) moveDir -= Vector3.up;

        // Sprint
        float currentSpeed = moveSpeed;
        if (keyboard.leftShiftKey.isPressed)
        {
            currentSpeed *= sprintMultiplier;
        }

        transform.position += moveDir * currentSpeed * Time.deltaTime;
    }
}