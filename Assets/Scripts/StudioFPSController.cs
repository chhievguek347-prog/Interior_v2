using UnityEngine;

namespace Interior.Studio
{
    [RequireComponent(typeof(CharacterController))]
    public class StudioFPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 3.5f;
        public float runSpeed = 6.0f;
        public float gravity = -9.81f;
        public float eyeHeight = 1.65f;

        [Header("Mouse Look Settings")]
        public float mouseSensitivity = 2.0f;
        public float upDownLookLimit = 85.0f;

        private CharacterController m_CharacterController;
        private Camera m_PlayerCamera;
        private float m_VerticalRotation = 0f;
        private Vector3 m_Velocity;

        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_PlayerCamera = GetComponentInChildren<Camera>();

            if (m_PlayerCamera == null && Camera.main != null)
            {
                m_PlayerCamera = Camera.main;
                m_PlayerCamera.transform.SetParent(transform, false);
                m_PlayerCamera.transform.localPosition = new Vector3(0, eyeHeight, 0);
            }

            m_CharacterController.height = 1.8f;
            m_CharacterController.radius = 0.3f;
            m_CharacterController.center = new Vector3(0, 0.9f, 0);

            // Lock cursor for FPS controls
            LockCursor(true);
        }

        private void Update()
        {
            // Handle Cursor Locking toggle
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LockCursor(false);
            }
            if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor(true);
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                HandleMouseLook();
            }

            HandleMovement();
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Horizontal rotation (Player Body Yaw)
            transform.Rotate(Vector3.up * mouseX);

            // Vertical rotation (Camera Pitch)
            m_VerticalRotation -= mouseY;
            m_VerticalRotation = Mathf.Clamp(m_VerticalRotation, -upDownLookLimit, upDownLookLimit);

            if (m_PlayerCamera != null)
            {
                m_PlayerCamera.transform.localRotation = Quaternion.Euler(m_VerticalRotation, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;
            m_CharacterController.Move(move * currentSpeed * Time.deltaTime);

            // Apply gravity
            if (m_CharacterController.isGrounded && m_Velocity.y < 0)
            {
                m_Velocity.y = -2f;
            }
            m_Velocity.y += gravity * Time.deltaTime;
            m_CharacterController.Move(m_Velocity * Time.deltaTime);
        }

        private void LockCursor(bool lockState)
        {
            Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockState;
        }
    }
}
