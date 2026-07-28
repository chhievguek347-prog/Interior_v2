using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace Interior.Studio
{
    public class OrganizedStudioSceneBuilder : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Build Organized Spacious Studio", false, 0)]
        public static void BuildOrganizedStudioMenu()
        {
            GameObject builderObj = new GameObject("OrganizedStudioSceneBuilder");
            OrganizedStudioSceneBuilder builder = builderObj.AddComponent<OrganizedStudioSceneBuilder>();
            builder.BuildOrganizedScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Organized Spacious Studio");
        }
#endif

        [ContextMenu("Build Organized Scene")]
        public void BuildOrganizedScene()
        {
            Debug.Log("[OrganizedStudioSceneBuilder] Constructing Organized Studio Scene...");

            GameObject genObj = GameObject.Find("OrganizedStudio_Generator");
            if (genObj == null)
            {
                genObj = new GameObject("OrganizedStudio_Generator");
            }
            OrganizedStudioGenerator generator = genObj.GetComponent<OrganizedStudioGenerator>();
            if (generator == null) generator = genObj.AddComponent<OrganizedStudioGenerator>();

            generator.GenerateOrganizedStudio();
        }
    }
}
