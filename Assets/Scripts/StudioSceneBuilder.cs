using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.Studio
{
    public class StudioSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Studio Room Layout")]
        public static void BuildStudioRoomMenu()
        {
            GameObject builderObj = new GameObject("StudioSceneBuilder");
            StudioSceneBuilder builder = builderObj.AddComponent<StudioSceneBuilder>();
            builder.BuildStudioScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Studio Room Layout");
        }
#endif

        [ContextMenu("Build Studio Room Scene")]
        public void BuildStudioScene()
        {
            Debug.Log("[StudioSceneBuilder] Constructing Studio Room Walkthrough Scene...");

            // 1. Setup Sun Light & Ambient Lighting
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Directional Sunlight");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            // Angle sun so light streams through the left studio windows (X = -5)
            sunLight.transform.rotation = Quaternion.Euler(25f, -65f, 0f);
            sunLight.color = new Color(1.0f, 0.94f, 0.86f);
            sunLight.intensity = 1.3f;
            sunLight.shadows = LightShadows.Soft;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.75f, 0.85f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.85f, 0.82f, 0.78f);
            RenderSettings.ambientGroundColor = new Color(0.35f, 0.28f, 0.22f);

            // 2. Instantiate Studio Room Generator
            GameObject roomObj = GameObject.Find("StudioRoomGenerator");
            if (roomObj == null)
            {
                roomObj = new GameObject("StudioRoomGenerator");
            }
            StudioRoomGenerator generator = roomObj.GetComponent<StudioRoomGenerator>();
            if (generator == null) generator = roomObj.AddComponent<StudioRoomGenerator>();

            generator.GenerateStudioLayout();

            // 3. Instantiate Recessed Ceiling Spotlights
            CreateCeilingSpotlights(generator.roomWidth, generator.roomLength, generator.roomHeight);

            // 4. Instantiate FPS Walkthrough Player
            GameObject playerObj = GameObject.Find("StudioFPSPlayer");
            if (playerObj == null)
            {
                playerObj = new GameObject("StudioFPSPlayer");
            }
            playerObj.transform.position = new Vector3(0f, 0f, -3.2f); // Spawn near front entrance
            playerObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Facing forward into studio

            CharacterController cc = playerObj.GetComponent<CharacterController>();
            if (cc == null) cc = playerObj.AddComponent<CharacterController>();

            StudioFPSController fpsCtrl = playerObj.GetComponent<StudioFPSController>();
            if (fpsCtrl == null) fpsCtrl = playerObj.AddComponent<StudioFPSController>();

            // Setup Camera on Player
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

            Debug.Log("[StudioSceneBuilder] Studio Room Walkthrough Scene created successfully!");
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

        private void CreateCeilingSpotlights(float width, float length, float height)
        {
            GameObject spotsContainer = GameObject.Find("CeilingSpotlights");
            if (spotsContainer != null) DestroyImmediate(spotsContainer);

            spotsContainer = new GameObject("CeilingSpotlights");

            // Define spotlight locations over functional zones
            Vector3[] spotLocations = new Vector3[]
            {
                new Vector3(-3.0f, height - 0.2f, 2.2f),   // Workspace zone spot
                new Vector3(2.5f, height - 0.2f, 2.2f),    // Sleeping alcove spot
                new Vector3(-0.75f, height - 0.2f, -1.8f), // Kitchen counter spot
                new Vector3(0.0f, height - 0.2f, 1.0f)     // Living area spot
            };

            foreach (Vector3 loc in spotLocations)
            {
                GameObject spotObj = new GameObject("InteriorSpotLight");
                spotObj.transform.SetParent(spotsContainer.transform, false);
                spotObj.transform.position = loc;
                spotObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                Light spot = spotObj.AddComponent<Light>();
                spot.type = LightType.Spot;
                spot.spotAngle = 75f;
                spot.range = 6.0f;
                spot.intensity = 1.8f;
                spot.color = new Color(1.0f, 0.92f, 0.80f); // Warm interior lighting
                spot.shadows = LightShadows.Soft;
            }
        }
    }
}
