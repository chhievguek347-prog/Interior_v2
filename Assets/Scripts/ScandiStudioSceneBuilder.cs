using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.Scandi
{
    public class ScandiStudioSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Scandinavian VR Studio")]
        public static void BuildScandiStudioMenu()
        {
            GameObject builderObj = new GameObject("ScandiStudioSceneBuilder");
            ScandiStudioSceneBuilder builder = builderObj.AddComponent<ScandiStudioSceneBuilder>();
            builder.BuildStudioScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Scandinavian VR Studio");
        }
#endif

        [ContextMenu("Build Scandinavian Studio Scene")]
        public void BuildStudioScene()
        {
            Debug.Log("[ScandiStudioSceneBuilder] Constructing game-ready Scandinavian VR Studio Apartment...");

            // 1. Natural Window Sunlight Setup (streaming from bedroom window at Z = +4)
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Natural Window Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(22f, 160f, 0f);
            sunLight.color = new Color(1.0f, 0.95f, 0.88f);
            sunLight.intensity = 1.45f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.82f, 0.88f, 0.96f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.42f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Studio Apartment Generator
            GameObject studioObj = GameObject.Find("ScandiStudioGenerator");
            if (studioObj == null)
            {
                studioObj = new GameObject("ScandiStudioGenerator");
            }
            ScandiStudioGenerator generator = studioObj.GetComponent<ScandiStudioGenerator>();
            if (generator == null) generator = studioObj.AddComponent<ScandiStudioGenerator>();

            generator.GenerateStudioApartment();

            // 3. Add Interior Warm Soft Lights over Bed, Kitchen, and Living sofa
            CreateInteriorSoftLights();

            // 4. Instantiate VR & Desktop Player Controller
            GameObject playerObj = GameObject.Find("ScandiVRPlayer");
            if (playerObj == null)
            {
                playerObj = new GameObject("ScandiVRPlayer");
            }
            playerObj.transform.position = new Vector3(0f, 0f, -3.2f); // Entrance spawn
            playerObj.transform.rotation = Quaternion.identity;

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            ScandiVRPlayerController vrCtrl = playerObj.GetComponent<ScandiVRPlayerController>();
            if (vrCtrl == null) vrCtrl = playerObj.AddComponent<ScandiVRPlayerController>();

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

            Debug.Log("[ScandiStudioSceneBuilder] Scandinavian VR Studio Apartment build completed successfully!");
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

        private void CreateInteriorSoftLights()
        {
            GameObject container = GameObject.Find("InteriorSoftLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("InteriorSoftLights");

            Vector3[] lightPositions = new Vector3[]
            {
                new Vector3(1.75f, 2.5f, 2.85f),  // Bedroom soft warmth
                new Vector3(-2.68f, 2.5f, -1.25f), // Kitchen soft light
                new Vector3(1.75f, 2.5f, -1.5f),  // Living area soft light
                new Vector3(-1.35f, 2.5f, 1.55f)   // Bathroom vanity light
            };

            foreach (Vector3 pos in lightPositions)
            {
                GameObject pObj = new GameObject("WarmSoftLight");
                pObj.transform.SetParent(container.transform, false);
                pObj.transform.position = pos;

                Light l = pObj.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 5.0f;
                l.intensity = 1.1f;
                l.color = new Color(1.0f, 0.92f, 0.82f);
                l.shadows = LightShadows.Soft;
            }
        }
    }
}
