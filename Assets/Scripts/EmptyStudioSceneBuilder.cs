using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.EmptyShell
{
    public class EmptyStudioSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Empty Studio Architectural Shell")]
        public static void BuildEmptyStudioMenu()
        {
            GameObject builderObj = new GameObject("EmptyStudioSceneBuilder");
            EmptyStudioSceneBuilder builder = builderObj.AddComponent<EmptyStudioSceneBuilder>();
            builder.BuildEmptyShellScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Empty Studio Architectural Shell");
        }
#endif

        [ContextMenu("Build Empty Studio Shell Scene")]
        public void BuildEmptyShellScene()
        {
            Debug.Log("[EmptyStudioSceneBuilder] Constructing Photorealistic Empty Studio Architectural Shell scene...");

            // 1. Natural Window Daylight
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Natural Window Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(30f, -50f, 0f);
            sunLight.color = new Color(1.0f, 0.96f, 0.90f);
            sunLight.intensity = 1.4f;
            sunLight.shadows = LightShadows.Soft;

            // Ambient Trilight
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.90f, 0.96f);
            RenderSettings.ambientEquatorColor = new Color(0.90f, 0.86f, 0.80f);
            RenderSettings.ambientGroundColor = new Color(0.40f, 0.35f, 0.28f);
            RenderSettings.ambientIntensity = 1.2f;

            // 2. Instantiate Empty Shell Generator
            GameObject genObj = GameObject.Find("EmptyStudioShellGenerator");
            if (genObj == null) genObj = new GameObject("EmptyStudioShellGenerator");

            EmptyStudioShellGenerator generator = genObj.GetComponent<EmptyStudioShellGenerator>();
            if (generator == null) generator = genObj.AddComponent<EmptyStudioShellGenerator>();

            generator.GenerateEmptyStudioShell();

            // 3. Spawn Player at Front Entrance Door facing Zone A
            GameObject playerObj = GameObject.Find("EmptyStudioFPSPlayer");
            if (playerObj == null) playerObj = new GameObject("EmptyStudioFPSPlayer");

            playerObj.transform.position = new Vector3(-2.2f, 0f, -2.0f);
            playerObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            EmptyStudioFPSController playerCtrl = playerObj.GetComponent<EmptyStudioFPSController>();
            if (playerCtrl == null) playerCtrl = playerObj.AddComponent<EmptyStudioFPSController>();

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

            Debug.Log("[EmptyStudioSceneBuilder] Empty Studio Architectural Shell created successfully!");
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
