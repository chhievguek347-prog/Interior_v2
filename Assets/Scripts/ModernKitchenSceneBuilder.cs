using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.ModernKitchen
{
    public class ModernKitchenSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Modern Kitchen")]
        public static void BuildModernKitchenMenu()
        {
            GameObject builderObj = new GameObject("ModernKitchenSceneBuilder");
            ModernKitchenSceneBuilder builder = builderObj.AddComponent<ModernKitchenSceneBuilder>();
            builder.BuildKitchenScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Modern Kitchen");
        }
#endif

        [ContextMenu("Build Kitchen Scene")]
        public void BuildKitchenScene()
        {
            Debug.Log("[ModernKitchenSceneBuilder] Constructing Modern Kitchen scene...");

            // 1. Setup Natural Window Sunlight (streaming from right window at X = 1.75)
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Window Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(28f, -120f, 0f);
            sunLight.color = new Color(1.0f, 0.95f, 0.88f);
            sunLight.intensity = 1.4f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.40f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Kitchen Generator
            GameObject genObj = GameObject.Find("ModernKitchenGenerator");
            if (genObj == null) genObj = new GameObject("ModernKitchenGenerator");

            ModernKitchenGenerator generator = genObj.GetComponent<ModernKitchenGenerator>();
            if (generator == null) generator = genObj.AddComponent<ModernKitchenGenerator>();

            generator.GenerateModernKitchen();

            // 3. Create Warm Pendant Point Lights
            CreatePendantLights();

            // 4. Spawn Player at Entrance Doorway facing Kitchen & Dining Area
            GameObject playerObj = GameObject.Find("ModernKitchenFPSPlayer");
            if (playerObj == null) playerObj = new GameObject("ModernKitchenFPSPlayer");

            playerObj.transform.position = new Vector3(0.3f, 0f, -1.5f);
            playerObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            ModernKitchenFPSController playerCtrl = playerObj.GetComponent<ModernKitchenFPSController>();
            if (playerCtrl == null) playerCtrl = playerObj.AddComponent<ModernKitchenFPSController>();

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

            Debug.Log("[ModernKitchenSceneBuilder] Modern Kitchen scene created successfully!");
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
            GameObject container = GameObject.Find("KitchenPendantLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("KitchenPendantLights");

            Vector3[] lightPos = new Vector3[]
            {
                new Vector3(0.75f, 2.05f, -0.5f), // Dining table pendant
                new Vector3(-0.8f, 2.05f, 0.4f)   // Counter island pendant
            };

            foreach (Vector3 p in lightPos)
            {
                GameObject lObj = new GameObject("PendantWarmLight");
                lObj.transform.SetParent(container.transform, false);
                lObj.transform.position = p;

                Light l = lObj.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 4.5f;
                l.intensity = 1.35f;
                l.color = new Color(1.0f, 0.90f, 0.78f);
                l.shadows = LightShadows.Soft;
            }
        }
    }
}
