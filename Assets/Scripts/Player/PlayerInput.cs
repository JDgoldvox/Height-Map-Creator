using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerInput : MonoBehaviour
{
    public PlayerControls controls;

    private InputAction move;
    private InputAction look;
    [SerializeField] private float moveSpeed = 5;
    private Rigidbody rb;

    //camera vars
    private Camera mainCamera;
    public float mouseSensitivity = 0.2f;
    public float cameraVerticalRotation = 0f;

    private bool freeze = false;

    [SerializeField] private GameObject pauseBar1;
    [SerializeField] private GameObject pauseBar2;

    void Awake()
    {
        controls = new PlayerControls();
        mainCamera = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        move = controls.Player.Move; //player action map inside input actions
        move.Enable();

        look = controls.Player.Look;
        look.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //freeze mechanic (stops all movement)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            freeze = !freeze;
        }

        if (freeze)
        {
            pauseBar1.SetActive(true);
            pauseBar2.SetActive(true);

            return;
        }

        pauseBar1.SetActive(false);
        pauseBar2.SetActive(false);

        //camera
        Vector2 lookInput = look.ReadValue<Vector2>() * mouseSensitivity;

        cameraVerticalRotation -= lookInput.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        mainCamera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        this.gameObject.transform.Rotate(Vector3.up * lookInput.x);

        //movement
        Vector2 moveInput = move.ReadValue<Vector2>();
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0; // Ignore vertical component for forward movement
        right.y = 0; // Ignore vertical component for right movement

        //vector maths, adding vectors = new destination, multiplying a vector gives you a bigger arrow
        Vector3 desiredMoveDirection = (forward * moveInput.y + right * moveInput.x) * moveSpeed * Time.deltaTime;

        // Apply movement
        transform.position += desiredMoveDirection;

        //up and down
        if (Keyboard.current.eKey.IsPressed())
        {
            var pos = rb.transform.position;
            rb.transform.position = new Vector3(pos.x, pos.y + 1.0f * Time.deltaTime * moveSpeed, pos.z);
        }

        if (Keyboard.current.qKey.IsPressed())
        {
            var pos = rb.transform.position;
            rb.transform.position = new Vector3(pos.x, pos.y - 1.0f * Time.deltaTime * moveSpeed, pos.z);
        }
    }
}