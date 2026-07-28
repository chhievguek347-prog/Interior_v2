using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.ScandiVR
{
    public class ScandiStudioVRSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Scandi VR Game-Ready Studio")]
        public static void BuildScandiVRMenu()
        {
            GameObject builderObj = new GameObject("ScandiStudioVRSceneBuilder");
            ScandiStudioVRSceneBuilder builder = builderObj.AddComponent<ScandiStudioVRSceneBuilder>();
            builder.BuildScandiVRScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Scandi VR Game-Ready Studio");
        }
#endif

        [ContextMenu("Build Scandi VR Scene")]
        public void BuildScandiVRScene()
        {
            Debug.Log("[ScandiStudioVRSceneBuilder] Constructing Scandinavian Studio VR Scene...");

            // 1. Natural Window Sunlight (streaming primarily through bedroom window at Z = +4.0)
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Natural Window Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(32f, 160f, 0f);
            sunLight.color = new Color(1.0f, 0.96f, 0.90f);
            sunLight.intensity = 1.45f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Trilight
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.88f, 0.92f, 0.98f);
            RenderSettings.ambientEquatorColor = new Color(0.92f, 0.88f, 0.82f);
            RenderSettings.ambientGroundColor = new Color(0.42f, 0.38f, 0.32f);
            RenderSettings.ambientIntensity = 1.2f;

            // 2. Instantiate Studio Generator
            GameObject genObj = GameObject.Find("ScandiStudioVRGenerator");
            if (genObj == null) genObj = new GameObject("ScandiStudioVRGenerator");

            ScandiStudioVRGenerator generator = genObj.GetComponent<ScandiStudioVRGenerator>();
            if (generator == null) generator = genObj.AddComponent<ScandiStudioVRGenerator>();

            generator.GenerateGameReadyScandiStudio();

            // 3. Player Rig Setup
            GameObject playerObj = GameObject.Find("ScandiVRPlayer");
            if (playerObj == null) playerObj = new GameObject("ScandiVRPlayer");

            playerObj.transform.position = new Vector3(0f, 0f, -3.5f); // Spawn at bottom center entrance
            playerObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            ScandiVRPlayerSetup playerSetup = playerObj.GetComponent<ScandiVRPlayerSetup>();
            if (playerSetup == null) playerSetup = playerObj.AddComponent<ScandiVRPlayerSetup>();

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

            Debug.Log("[ScandiStudioVRSceneBuilder] Scandinavian VR Studio Apartment scene created successfully!");
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
    }
}
