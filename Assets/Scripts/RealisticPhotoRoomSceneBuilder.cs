using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace Interior.Studio
{
    public class RealisticPhotoRoomSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Realistic 3D Photo Room", false, 1)]
        public static void BuildPhotoRoomSceneMenu()
        {
            GameObject builderObj = new GameObject("RealisticPhotoRoomSceneBuilder");
            RealisticPhotoRoomSceneBuilder builder = builderObj.AddComponent<RealisticPhotoRoomSceneBuilder>();
            builder.BuildPhotoRoomScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Realistic 3D Photo Room");
        }
#endif

        [ContextMenu("Build Photo Room Scene")]
        public void BuildPhotoRoomScene()
        {
            Debug.Log("[RealisticPhotoRoomSceneBuilder] Constructing Realistic Photo Room Scene...");

            GameObject genObj = GameObject.Find("PhotoRoom_Generator");
            if (genObj == null)
            {
                genObj = new GameObject("PhotoRoom_Generator");
            }
            RealisticPhotoRoomGenerator generator = genObj.GetComponent<RealisticPhotoRoomGenerator>();
            if (generator == null) generator = genObj.AddComponent<RealisticPhotoRoomGenerator>();

            generator.GeneratePhotoRoom();
        }
    }
}

