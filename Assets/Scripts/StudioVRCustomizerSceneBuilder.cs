using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.VRCustomizer
{
    public class StudioVRCustomizerSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Customizable VR Studio Apartment")]
        public static void BuildVRStudioMenu()
        {
            GameObject builderObj = new GameObject("StudioVRCustomizerSceneBuilder");
            StudioVRCustomizerSceneBuilder builder = builderObj.AddComponent<StudioVRCustomizerSceneBuilder>();
            builder.BuildCustomizableStudioScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Customizable VR Studio");
        }
#endif

        [ContextMenu("Build Customizable VR Studio Scene")]
        public void BuildCustomizableStudioScene()
        {
            Debug.Log("[StudioVRCustomizerSceneBuilder] Constructing customizable VR Studio Apartment scene...");

            // 1. Setup Golden-Hour Sunlight (streaming through Balcony Door at X = -3)
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("GoldenHour_Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(20f, 75f, 0f);
            sunLight.color = new Color(1.0f, 0.88f, 0.72f); // Warm golden-hour tone
            sunLight.intensity = 1.55f;
            sunLight.shadows = LightShadows.Soft;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.92f, 0.84f, 0.75f);
            RenderSettings.ambientGroundColor = new Color(0.45f, 0.38f, 0.30f);
            RenderSettings.ambientIntensity = 1.2f;

            // 2. Instantiate Studio VR Layout Generator
            GameObject genObj = GameObject.Find("StudioVRLayoutGenerator");
            if (genObj == null) genObj = new GameObject("StudioVRLayoutGenerator");

            StudioVRLayoutGenerator generator = genObj.GetComponent<StudioVRLayoutGenerator>();
            if (generator == null) generator = genObj.AddComponent<StudioVRLayoutGenerator>();

            generator.GenerateVRStudioLayout();

            // 3. Instantiate Customizer Engine
            StudioVRCustomizer customizer = genObj.GetComponent<StudioVRCustomizer>();
            if (customizer == null) customizer = genObj.AddComponent<StudioVRCustomizer>();

            // 4. Create Warm Pendant Lights over Kitchen Counter & Nightstands
            CreatePendantLights();

            // 5. Create Interactive VR UI Customization Tablet Panel
            CreateCustomizationTabletPanel(customizer);

            // 6. Spawn VR Player in Living Room facing Kitchen/Dining area
            GameObject playerObj = GameObject.Find("VRStudioPlayer");
            if (playerObj == null) playerObj = new GameObject("VRStudioPlayer");

            // Position standing in living room facing kitchen/dining (X = 1.0, Z = -1.5)
            playerObj.transform.position = new Vector3(1.0f, 0f, -1.5f);
            playerObj.transform.rotation = Quaternion.Euler(0f, -60f, 0f);

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            StudioVRCustomizerPlayerCtrl playerCtrl = playerObj.GetComponent<StudioVRCustomizerPlayerCtrl>();
            if (playerCtrl == null) playerCtrl = playerObj.AddComponent<StudioVRCustomizerPlayerCtrl>();

            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }
            mainCam.transform.SetParent(playerObj.transform, false);
            mainCam.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            mainCam.transform.localRotation = Quaternion.identity;

            Debug.Log("[StudioVRCustomizerSceneBuilder] Customizable VR Studio Apartment created successfully!");
        }

        private Light FindSunLight()
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light l in lights)
            {
                if (l.type == LightType.Directional) return l;
            }
            return null;
        }

        private void CreatePendantLights()
        {
            GameObject container = GameObject.Find("PendantPointLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("PendantPointLights");

            Vector3[] pendantPositions = new Vector3[]
            {
                new Vector3(-1.95f, 2.1f, -0.3f), // Kitchen island pendant
                new Vector3(-0.15f, 1.8f, 2.85f), // Bed nightstand left pendant
                new Vector3(1.85f, 1.8f, 2.85f),  // Bed nightstand right pendant
                new Vector3(2.45f, 2.3f, 1.65f)   // Bathroom vanity pendant
            };

            foreach (Vector3 pos in pendantPositions)
            {
                GameObject pObj = new GameObject("PendantWarmLight");
                pObj.transform.SetParent(container.transform, false);
                pObj.transform.position = pos;

                Light l = pObj.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 4.5f;
                l.intensity = 1.4f;
                l.color = new Color(1.0f, 0.90f, 0.78f);
                l.shadows = LightShadows.Soft;
            }
        }

        private void CreateCustomizationTabletPanel(StudioVRCustomizer customizer)
        {
            GameObject tabletObj = GameObject.Find("VR_Customization_Tablet");
            if (tabletObj != null) DestroyImmediate(tabletObj);

            tabletObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tabletObj.name = "VR_Customization_Tablet";
            // Position on living room coffee table or mounted near entry
            tabletObj.transform.position = new Vector3(0.5f, 0.85f, -1.8f);
            tabletObj.transform.rotation = Quaternion.Euler(30f, 45f, 0f);
            tabletObj.transform.localScale = new Vector3(0.45f, 0.02f, 0.32f);

            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            Material tabletMat = new Material(litShader);
            tabletMat.name = "Mat_TabletUI";
            tabletMat.SetColor("_BaseColor", new Color(0.12f, 0.15f, 0.20f));
            tabletMat.SetColor("_Color", new Color(0.12f, 0.15f, 0.20f));
            tabletObj.GetComponent<MeshRenderer>().sharedMaterial = tabletMat;
        }
    }

    public class StudioVRCustomizerPlayerCtrl : MonoBehaviour
    {
        public float walkSpeed = 2.8f;
        public float eyeHeight = 1.65f;
        private CharacterController m_CC;
        private Camera m_Cam;
        private float m_Pitch = 0f;

        private void Start()
        {
            m_CC = GetComponent<CharacterController>();
            m_Cam = GetComponentInChildren<Camera>();

            if (m_Cam == null && Camera.main != null)
            {
                m_Cam = Camera.main;
                m_Cam.transform.SetParent(transform, false);
                m_Cam.transform.localPosition = new Vector3(0, eyeHeight, 0);
            }

            m_CC.height = 1.75f;
            m_CC.radius = 0.28f;
            m_CC.center = new Vector3(0, 0.875f, 0);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                float mx = Input.GetAxis("Mouse X") * 2.0f;
                float my = Input.GetAxis("Mouse Y") * 2.0f;

                transform.Rotate(Vector3.up * mx);
                m_Pitch -= my;
                m_Pitch = Mathf.Clamp(m_Pitch, -85f, 85f);
                if (m_Cam != null) m_Cam.transform.localRotation = Quaternion.Euler(m_Pitch, 0f, 0f);
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 move = transform.right * h + transform.forward * v;
            m_CC.Move(move * walkSpeed * Time.deltaTime + Vector3.down * 9.81f * Time.deltaTime);
        }
    }
}
