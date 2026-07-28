using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.DualZone
{
    public class DualZoneSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Dual-Zone Studio Apartment")]
        public static void BuildDualZoneMenu()
        {
            GameObject builderObj = new GameObject("DualZoneSceneBuilder");
            DualZoneSceneBuilder builder = builderObj.AddComponent<DualZoneSceneBuilder>();
            builder.BuildDualZoneScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Dual-Zone Studio Apartment");
        }
#endif

        [ContextMenu("Build Dual-Zone Studio Scene")]
        public void BuildDualZoneScene()
        {
            Debug.Log("[DualZoneSceneBuilder] Constructing Dual-Zone Studio Apartment scene...");

            // 1. Setup Sun & Ambient Lighting
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Directional Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(30f, -45f, 0f);
            sunLight.color = new Color(1.0f, 0.96f, 0.88f);
            sunLight.intensity = 1.35f;
            sunLight.shadows = LightShadows.Soft;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.82f, 0.88f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.88f, 0.84f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.38f, 0.32f, 0.25f);
            RenderSettings.ambientIntensity = 1.15f;

            // 2. Instantiate Dual-Zone Generator
            GameObject genObj = GameObject.Find("DualZoneStudioGenerator");
            if (genObj == null) genObj = new GameObject("DualZoneStudioGenerator");

            DualZoneStudioGenerator generator = genObj.GetComponent<DualZoneStudioGenerator>();
            if (generator == null) generator = genObj.AddComponent<DualZoneStudioGenerator>();

            generator.GenerateDualZoneStudio();

            // 3. Spawn Player at Front Entrance Door facing Zone A (Living/Kitchen)
            GameObject playerObj = GameObject.Find("DualZoneFPSPlayer");
            if (playerObj == null) playerObj = new GameObject("DualZoneFPSPlayer");

            playerObj.transform.position = new Vector3(-2.2f, 0f, -2.0f); // Spawn inside front door
            playerObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Facing North into Living/Kitchen

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            DualZoneFPSController playerCtrl = playerObj.GetComponent<DualZoneFPSController>();
            if (playerCtrl == null) playerCtrl = playerObj.AddComponent<DualZoneFPSController>();

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

            Debug.Log("[DualZoneSceneBuilder] Dual-Zone Studio Apartment created successfully!");
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
