using UnityEngine;

namespace Interior.BedroomFurniture
{
    public class BedroomFurnitureInspector : MonoBehaviour
    {
        [Header("Orbit Inspection Settings")]
        public Transform targetFocus;
        public float distance = 4.5f;
        public float xSpeed = 120.0f;
        public float ySpeed = 80.0f;
        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        private float m_X = 35.0f;
        private float m_Y = 25.0f;

        private void Start()
        {
            if (targetFocus == null)
            {
                GameObject centerObj = GameObject.Find("QueenBed_HighPoly");
                if (centerObj != null) targetFocus = centerObj.transform;
                else targetFocus = transform;
            }

            Vector3 angles = transform.eulerAngles;
            m_X = angles.y;
            m_Y = angles.x;
        }

        private void LateUpdate()
        {
            if (targetFocus == null) return;

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                m_X += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                m_Y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                m_Y = ClampAngle(m_Y, yMinLimit, yMaxLimit);
            }

            distance -= Input.GetAxis("Mouse ScrollWheel") * 3.0f;
            distance = Mathf.Clamp(distance, 1.5f, 10.0f);

            Quaternion rotation = Quaternion.Euler(m_Y, m_X, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + targetFocus.position;

            transform.rotation = rotation;
            transform.position = position;
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F) angle += 360F;
            if (angle > 360F) angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
