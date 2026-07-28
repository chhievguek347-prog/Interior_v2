using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.LivingFurniture
{
    public class LivingFurnitureSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build High-Poly Living Room Furniture Set")]
        public static void BuildLivingFurnitureSetMenu()
        {
            GameObject builderObj = new GameObject("LivingFurnitureSceneBuilder");
            LivingFurnitureSceneBuilder builder = builderObj.AddComponent<LivingFurnitureSceneBuilder>();
            builder.BuildLivingFurnitureScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build High-Poly Living Room Furniture Set");
        }
#endif

        [ContextMenu("Build Living Room Furniture Scene")]
        public void BuildLivingFurnitureScene()
        {
            Debug.Log("[LivingFurnitureSceneBuilder] Constructing High-Poly Living Room Furniture Set scene...");

            // 1. Setup Studio Main Light (Soft Directional Key Light)
            Light mainLight = FindMainLight();
            if (mainLight == null)
            {
                GameObject lightObj = new GameObject("Studio Key Light");
                mainLight = lightObj.AddComponent<Light>();
                mainLight.type = LightType.Directional;
            }
            mainLight.transform.rotation = Quaternion.Euler(40f, -45f, 0f);
            mainLight.color = new Color(1.0f, 0.96f, 0.90f);
            mainLight.intensity = 1.4f;
            mainLight.shadows = LightShadows.Soft;

            // Ambient Trilight
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.42f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Furniture Set Generator
            GameObject genObj = GameObject.Find("LivingFurnitureSetGenerator");
            if (genObj == null) genObj = new GameObject("LivingFurnitureSetGenerator");

            LivingFurnitureSetGenerator generator = genObj.GetComponent<LivingFurnitureSetGenerator>();
            if (generator == null) generator = genObj.AddComponent<LivingFurnitureSetGenerator>();

            generator.GenerateHighPolyLivingFurnitureSet();

            // 3. Setup Floor Lamp Point Light
            CreateFloorLampLight();

            // 4. Setup Camera & Orbit Inspector
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            mainCam.transform.position = new Vector3(1.75f, 2.4f, -4.5f);
            mainCam.transform.rotation = Quaternion.Euler(25f, 0f, 0f);

            LivingFurnitureInspector inspector = mainCam.GetComponent<LivingFurnitureInspector>();
            if (inspector == null) inspector = mainCam.gameObject.AddComponent<LivingFurnitureInspector>();

            Transform sofaTrans = generator.transform.Find("HighPolyLivingFurnitureSet/1_Sofa3Seater_Detailed");
            if (sofaTrans != null) inspector.targetFocus = sofaTrans;

            Debug.Log("[LivingFurnitureSceneBuilder] High-Poly Living Room Furniture Set scene created successfully!");
        }

        private Light FindMainLight()
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light l in lights)
            {
                if (l.type == LightType.Directional) return l;
            }
            return null;
        }

        private void CreateFloorLampLight()
        {
            GameObject container = GameObject.Find("LivingLampLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("LivingLampLights");

            GameObject lObj = new GameObject("FloorLampWarmLight");
            lObj.transform.SetParent(container.transform, false);
            lObj.transform.position = new Vector3(2.6f, 1.65f, 0.4f);

            Light l = lObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.range = 4.8f;
            l.intensity = 1.3f;
            l.color = new Color(1.0f, 0.90f, 0.78f);
            l.shadows = LightShadows.Soft;
        }
    }
}
