using UnityEngine;

namespace Interior.Environment
{
    public class TreeOrbitCamera : MonoBehaviour
    {
        [Header("Target & Focus")]
        public Transform target;
        public Vector3 targetOffset = new Vector3(0f, 3.5f, 0f);

        [Header("Distance & Zoom")]
        public float distance = 12f;
        public float minDistance = 3f;
        public float maxDistance = 25f;
        public float zoomSensitivity = 4f;

        [Header("Orbit Speed")]
        public float xSpeed = 120f;
        public float ySpeed = 80f;
        public float yMinLimit = -10f;
        public float yMaxLimit = 80f;

        [Header("Auto Rotation")]
        public bool autoRotate = true;
        public float autoRotateSpeed = 8f;

        private float m_CurrentX = 45f;
        private float m_CurrentY = 20f;
        private bool m_UserInteracting = false;
        private float m_InteractionTimer = 0f;

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            m_CurrentX = angles.y;
            m_CurrentY = angles.x;
        }

        private void LateUpdate()
        {
            Vector3 focusPoint = target != null ? target.position + targetOffset : targetOffset;

            // Handle Mouse Orbit (Right click or Left click drag)
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                m_CurrentX += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                m_CurrentY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                m_UserInteracting = true;
                m_InteractionTimer = 0f;
            }
            else
            {
                m_InteractionTimer += Time.deltaTime;
                if (m_InteractionTimer > 2.5f)
                {
                    m_UserInteracting = false;
                }
            }

            // Auto-rotate when user is idle
            if (autoRotate && !m_UserInteracting)
            {
                m_CurrentX += autoRotateSpeed * Time.deltaTime;
            }

            // Clamp vertical rotation angle
            m_CurrentY = Mathf.Clamp(m_CurrentY, yMinLimit, yMaxLimit);

            // Handle Scroll Zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                distance -= scroll * zoomSensitivity;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }

            // Compute Position and Rotation
            Quaternion rotation = Quaternion.Euler(m_CurrentY, m_CurrentX, 0f);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + focusPoint;

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}
