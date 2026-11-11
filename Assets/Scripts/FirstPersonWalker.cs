using UnityEngine;
using UnityEngine.InputSystem;

namespace PocketWorld
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonWalker : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField, Min(0.1f)] private float walkSpeed = 4f;
        [SerializeField, Min(0.1f)] private float runSpeed = 7f;
        [SerializeField, Min(0f)] private float jumpHeight = 1.1f;
        [SerializeField, Min(0.01f)] private float mouseSensitivity = 0.12f;
        [SerializeField] private float gravity = -22f;

        private CharacterController controller;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private float verticalSpeed;
        private float pitch;
        private bool skipLook;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        private void Start() => ReleaseCursor();

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) ReleaseCursor();
            else if (Application.isFocused && mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                skipLook = true;
            }

            bool controlling = Application.isFocused && Cursor.lockState == CursorLockMode.Locked;
            if (controlling && mouse != null && playerCamera != null)
            {
                // Mouse delta already represents a frame's motion; do not multiply by deltaTime.
                Vector2 look = skipLook ? Vector2.zero : mouse.delta.ReadValue();
                skipLook = false;
                transform.Rotate(0f, look.x * mouseSensitivity, 0f);
                pitch = Mathf.Clamp(pitch - look.y * mouseSensitivity, -85f, 85f);
                playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }

            Vector2 input = Vector2.zero;
            bool running = false;
            if (controlling && keyboard != null)
            {
                input.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
                input.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
                running = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            }

            if (controller.isGrounded && verticalSpeed < 0f) verticalSpeed = -2f;
            if (controlling && keyboard != null && controller.isGrounded && keyboard.spaceKey.wasPressedThisFrame)
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            verticalSpeed += gravity * Time.deltaTime;
            Vector3 direction = transform.right * input.x + transform.forward * input.y;
            Vector3 velocity = Vector3.ClampMagnitude(direction, 1f) * (running ? runSpeed : walkSpeed);
            velocity.y = verticalSpeed;
            CollisionFlags flags = controller.Move(velocity * Time.deltaTime);
            if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f) verticalSpeed = 0f;
            if (transform.position.y < -20f) Respawn();
        }

        private void Respawn()
        {
            controller.enabled = false;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            controller.enabled = true;
            verticalSpeed = 0f;
            pitch = 0f;
            if (playerCamera != null) playerCamera.transform.localRotation = Quaternion.identity;
        }

        private static void ReleaseCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnApplicationFocus(bool focused) { if (!focused) ReleaseCursor(); }
        private void OnDisable() => ReleaseCursor();

        private void OnGUI()
        {
            GUI.Box(new Rect(16, 16, 540, 58), "WORLD WALK\nWASD / Arrows: Move   Shift: Run   Space: Jump   Mouse: Look");
            if (Cursor.lockState != CursorLockMode.Locked)
                GUI.Box(new Rect(Screen.width / 2f - 170, Screen.height / 2f - 30, 340, 60),
                    "Click to explore\nEsc: release mouse");
            else
                GUI.Label(new Rect(Screen.width / 2f - 5, Screen.height / 2f - 10, 20, 20), "+");
        }
    }
}
