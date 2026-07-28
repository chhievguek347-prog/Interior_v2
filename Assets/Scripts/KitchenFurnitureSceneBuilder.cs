using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.KitchenFurniture
{
    public class KitchenFurnitureSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build High-Poly Kitchen Furniture Set")]
        public static void BuildKitchenFurnitureSetMenu()
        {
            GameObject builderObj = new GameObject("KitchenFurnitureSceneBuilder");
            KitchenFurnitureSceneBuilder builder = builderObj.AddComponent<KitchenFurnitureSceneBuilder>();
            builder.BuildKitchenFurnitureScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build High-Poly Kitchen Furniture Set");
        }
#endif

        [ContextMenu("Build Kitchen Furniture Scene")]
        public void BuildKitchenFurnitureScene()
        {
            Debug.Log("[KitchenFurnitureSceneBuilder] Constructing High-Poly Kitchen Furniture Set scene...");

            // 1. Setup Studio Main Key Light
            Light mainLight = FindMainLight();
            if (mainLight == null)
            {
                GameObject lightObj = new GameObject("Studio Key Light");
                mainLight = lightObj.AddComponent<Light>();
                mainLight.type = LightType.Directional;
            }
            mainLight.transform.rotation = Quaternion.Euler(35f, -120f, 0f);
            mainLight.color = new Color(1.0f, 0.96f, 0.90f);
            mainLight.intensity = 1.45f;
            mainLight.shadows = LightShadows.Soft;

            // Ambient Trilight
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.40f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Furniture Set Generator
            GameObject genObj = GameObject.Find("KitchenFurnitureSetGenerator");
            if (genObj == null) genObj = new GameObject("KitchenFurnitureSetGenerator");

            KitchenFurnitureSetGenerator generator = genObj.GetComponent<KitchenFurnitureSetGenerator>();
            if (generator == null) generator = genObj.AddComponent<KitchenFurnitureSetGenerator>();

            generator.GenerateHighPolyKitchenFurnitureSet();

            // 3. Setup Pendant Warm Point Lights
            CreatePendantLights();

            // 4. Setup Camera & Orbit Inspector
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            mainCam.transform.position = new Vector3(-1.2f, 2.2f, -3.2f);
            mainCam.transform.rotation = Quaternion.Euler(25f, 0f, 0f);

            KitchenFurnitureInspector inspector = mainCam.GetComponent<KitchenFurnitureInspector>();
            if (inspector == null) inspector = mainCam.gameObject.AddComponent<KitchenFurnitureInspector>();

            Transform counterTrans = generator.transform.Find("HighPolyKitchenFurnitureSet/1_L_CountertopAndCabinets");
            if (counterTrans != null) inspector.targetFocus = counterTrans;

            Debug.Log("[KitchenFurnitureSceneBuilder] High-Poly Kitchen Furniture Set scene created successfully!");
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

        private void CreatePendantLights()
        {
            GameObject container = GameObject.Find("KitchenPendantWarmLights");
            if (container != null) DestroyImmediate(container);

            container = new GameObject("KitchenPendantWarmLights");

            Vector3[] positions = new Vector3[]
            {
                new Vector3(0.75f, 2.0f, -0.5f),
                new Vector3(-0.8f, 2.0f, 0.4f)
            };

            foreach (Vector3 p in positions)
            {
                GameObject lObj = new GameObject("PendantLightSource");
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
