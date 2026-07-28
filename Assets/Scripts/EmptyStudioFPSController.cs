using UnityEngine;

namespace Interior.EmptyShell
{
    [RequireComponent(typeof(CharacterController))]
    public class EmptyStudioFPSController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 3.0f;
        public float runSpeed = 5.0f;
        public float mouseSensitivity = 2.0f;
        public float eyeHeight = 1.65f;

        private CharacterController m_CC;
        private Camera m_PlayerCam;
        private float m_Pitch = 0f;

        private void Start()
        {
            m_CC = GetComponent<CharacterController>();
            m_PlayerCam = GetComponentInChildren<Camera>();

            if (m_PlayerCam == null && Camera.main != null)
            {
                m_PlayerCam = Camera.main;
                m_PlayerCam.transform.SetParent(transform, false);
                m_PlayerCam.transform.localPosition = new Vector3(0, eyeHeight, 0);
            }

            m_CC.height = 1.75f;
            m_CC.radius = 0.28f;
            m_CC.center = new Vector3(0, 0.875f, 0);

            LockCursor(true);
        }

        private void Update()
        {
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
                float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
                float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

                transform.Rotate(Vector3.up * mx);
                m_Pitch -= my;
                m_Pitch = Mathf.Clamp(m_Pitch, -85f, 85f);

                if (m_PlayerCam != null)
                {
                    m_PlayerCam.transform.localRotation = Quaternion.Euler(m_Pitch, 0f, 0f);
                }
            }

            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 move = transform.right * h + transform.forward * v;
            m_CC.Move(move * speed * Time.deltaTime + Vector3.down * 9.81f * Time.deltaTime);
        }

        private void LockCursor(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !state;
        }
    }
}
