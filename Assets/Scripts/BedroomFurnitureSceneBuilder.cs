using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.BedroomFurniture
{
    public class BedroomFurnitureSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build High-Poly Bedroom Furniture Set")]
        public static void BuildFurnitureSetMenu()
        {
            GameObject builderObj = new GameObject("BedroomFurnitureSceneBuilder");
            BedroomFurnitureSceneBuilder builder = builderObj.AddComponent<BedroomFurnitureSceneBuilder>();
            builder.BuildFurnitureScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build High-Poly Bedroom Furniture Set");
        }
#endif

        [ContextMenu("Build Bedroom Furniture Scene")]
        public void BuildFurnitureScene()
        {
            Debug.Log("[BedroomFurnitureSceneBuilder] Constructing High-Poly Bedroom Furniture Set scene...");

            // 1. Setup Studio Main Light (Soft Directional Key Light)
            Light mainLight = FindMainLight();
            if (mainLight == null)
            {
                GameObject lightObj = new GameObject("Studio Key Light");
                mainLight = lightObj.AddComponent<Light>();
                mainLight.type = LightType.Directional;
            }
            mainLight.transform.rotation = Quaternion.Euler(42f, -35f, 0f);
            mainLight.color = new Color(1.0f, 0.95f, 0.88f);
            mainLight.intensity = 1.4f;
            mainLight.shadows = LightShadows.Soft;

            // Ambient Trilight
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.42f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Furniture Set Generator
            GameObject genObj = GameObject.Find("BedroomFurnitureSetGenerator");
            if (genObj == null) genObj = new GameObject("BedroomFurnitureSetGenerator");

            BedroomFurnitureSetGenerator generator = genObj.GetComponent<BedroomFurnitureSetGenerator>();
            if (generator == null) generator = genObj.AddComponent<BedroomFurnitureSetGenerator>();

            generator.GenerateHighPolyFurnitureSet();

            // 3. Setup Bedside Lamps Point Lights
            CreateBedsideLampLights();

            // 4. Setup Camera & Orbit Inspector
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            mainCam.transform.position = new Vector3(0.5f, 2.2f, -3.2f);
            mainCam.transform.rotation = Quaternion.Euler(25f, 0f, 0f);

            BedroomFurnitureInspector inspector = mainCam.GetComponent<BedroomFurnitureInspector>();
            if (inspector == null) inspector = mainCam.gameObject.AddComponent<BedroomFurnitureInspector>();

            Transform bedTrans = generator.transform.Find("HighPolyBedroomFurnitureSet/1_QueenBed_Detailed");
            if (bedTrans != null) inspector.targetFocus = bedTrans;

            Debug.Log("[BedroomFurnitureSceneBuilder] High-Poly Bedroom Furniture Set scene created successfully!");
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

        private void CreateBedsideLampLights()
        {
            GameObject container = GameObject.Find("FurnitureLampLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("FurnitureLampLights");

            Vector3[] positions = new Vector3[]
            {
                new Vector3(-0.55f, 0.75f, 1.7f),
                new Vector3(1.55f, 0.75f, 1.7f)
            };

            foreach (Vector3 p in positions)
            {
                GameObject lObj = new GameObject("LampWarmLight");
                lObj.transform.SetParent(container.transform, false);
                lObj.transform.position = p;

                Light l = lObj.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 3.5f;
                l.intensity = 1.1f;
                l.color = new Color(1.0f, 0.88f, 0.74f);
                l.shadows = LightShadows.Soft;
            }
        }
    }
}
