using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.WarmBedroom
{
    public class WarmBedroomSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Warm Modern Bedroom")]
        public static void BuildWarmBedroomMenu()
        {
            GameObject builderObj = new GameObject("WarmBedroomSceneBuilder");
            WarmBedroomSceneBuilder builder = builderObj.AddComponent<WarmBedroomSceneBuilder>();
            builder.BuildBedroomScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Warm Modern Bedroom");
        }
#endif

        [ContextMenu("Build Bedroom Scene")]
        public void BuildBedroomScene()
        {
            Debug.Log("[WarmBedroomSceneBuilder] Constructing Warm Modern Bedroom scene...");

            // 1. Setup Natural Window Sunlight (streaming from right window at X = 2)
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Window Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(25f, -110f, 0f);
            sunLight.color = new Color(1.0f, 0.94f, 0.86f);
            sunLight.intensity = 1.35f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.42f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Bedroom Generator
            GameObject genObj = GameObject.Find("WarmBedroomGenerator");
            if (genObj == null) genObj = new GameObject("WarmBedroomGenerator");

            WarmBedroomGenerator generator = genObj.GetComponent<WarmBedroomGenerator>();
            if (generator == null) generator = genObj.AddComponent<WarmBedroomGenerator>();

            generator.GenerateWarmBedroom();

            // 3. Create Ceiling Light & Bedside Soft Lights
            CreateBedroomSoftLights();

            // 4. Spawn Player at Wooden Door Exit facing Bedroom
            GameObject playerObj = GameObject.Find("WarmBedroomFPSPlayer");
            if (playerObj == null) playerObj = new GameObject("WarmBedroomFPSPlayer");

            playerObj.transform.position = new Vector3(-1.1f, 0f, -1.8f);
            playerObj.transform.rotation = Quaternion.Euler(0f, 25f, 0f);

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            WarmBedroomFPSController playerCtrl = playerObj.GetComponent<WarmBedroomFPSController>();
            if (playerCtrl == null) playerCtrl = playerObj.AddComponent<WarmBedroomFPSController>();

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

            Debug.Log("[WarmBedroomSceneBuilder] Warm Modern Bedroom scene created successfully!");
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

        private void CreateBedroomSoftLights()
        {
            GameObject container = GameObject.Find("BedroomSoftLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("BedroomSoftLights");

            // Ceiling Point Light
            GameObject ceilingLightObj = new GameObject("CeilingWarmLight");
            ceilingLightObj.transform.SetParent(container.transform, false);
            ceilingLightObj.transform.position = new Vector3(0f, 2.45f, 0f);

            Light cLight = ceilingLightObj.AddComponent<Light>();
            cLight.type = LightType.Point;
            cLight.range = 6.0f;
            cLight.intensity = 1.25f;
            cLight.color = new Color(1.0f, 0.92f, 0.82f);
            cLight.shadows = LightShadows.Soft;

            // Bedside Lamps
            Vector3[] lampPos = new Vector3[]
            {
                new Vector3(-0.55f, 0.65f, 1.7f),
                new Vector3(1.55f, 0.65f, 1.7f)
            };
            foreach (Vector3 p in lampPos)
            {
                GameObject lObj = new GameObject("BedsideLampLight");
                lObj.transform.SetParent(container.transform, false);
                lObj.transform.position = p;

                Light l = lObj.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 3.2f;
                l.intensity = 0.9f;
                l.color = new Color(1.0f, 0.88f, 0.74f);
                l.shadows = LightShadows.Soft;
            }
        }
    }
}
