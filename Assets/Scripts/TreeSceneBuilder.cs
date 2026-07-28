using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Interior.Environment
{
    public class TreeSceneBuilder : MonoBehaviour
    {
        [Header("Environment Settings")]
        public float groundRadius = 15f;
        public Color grassColor = new Color(0.22f, 0.58f, 0.20f);
        public Color skyColor = new Color(0.45f, 0.72f, 0.95f);

        [Header("Preset Override")]
        public TreePreset selectedPreset = TreePreset.OakLush;

#if UNITY_EDITOR
        [MenuItem("Tools/Build Tree Scene")]
        public static void BuildTreeSceneMenu()
        {
            GameObject builderObj = new GameObject("TreeSceneBuilder");
            TreeSceneBuilder builder = builderObj.AddComponent<TreeSceneBuilder>();
            builder.BuildCompleteScene();
            Undo.RegisterCreatedObjectUndo(builderObj, "Build Tree Scene");
        }
#endif

        [ContextMenu("Build Scene Environment")]
        public void BuildCompleteScene()
        {
            Debug.Log("[TreeSceneBuilder] Constructing complete 3D Tree Scene...");

            // 1. Setup Camera
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }
            mainCam.backgroundColor = skyColor;
            mainCam.clearFlags = CameraClearFlags.SolidColor;

            TreeOrbitCamera orbitCam = mainCam.gameObject.GetComponent<TreeOrbitCamera>();
            if (orbitCam == null) orbitCam = mainCam.gameObject.AddComponent<TreeOrbitCamera>();

            // 2. Setup Sun & Ambient Lighting
            Light sunLight = FindSunLight();
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("Directional Sun Light");
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
            }
            sunLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            sunLight.color = new Color(1f, 0.96f, 0.85f);
            sunLight.intensity = 1.25f;
            sunLight.shadows = LightShadows.Soft;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = new Color(0.5f, 0.7f, 0.5f);
            RenderSettings.ambientGroundColor = new Color(0.2f, 0.35f, 0.15f);

            // 3. Create Ground Terrain Island
            GameObject groundObj = GameObject.Find("TreeGroundIsland");
            if (groundObj == null)
            {
                groundObj = new GameObject("TreeGroundIsland");
            }
            MeshFilter gmf = groundObj.GetComponent<MeshFilter>();
            if (gmf == null) gmf = groundObj.AddComponent<MeshFilter>();
            MeshRenderer gmr = groundObj.GetComponent<MeshRenderer>();
            if (gmr == null) gmr = groundObj.AddComponent<MeshRenderer>();

            gmf.sharedMesh = CreateGroundIslandMesh(groundRadius);

            Shader defaultLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (defaultLitShader == null) defaultLitShader = Shader.Find("Standard");

            Material groundMat = new Material(defaultLitShader);
            groundMat.name = "M_GroundGrass";
            groundMat.SetColor("_BaseColor", grassColor);
            groundMat.SetColor("_Color", grassColor);
            gmr.sharedMaterial = groundMat;

            // 4. Instantiate Procedural Tree Generator
            GameObject treeObj = GameObject.Find("ProceduralTree");
            if (treeObj == null)
            {
                treeObj = new GameObject("ProceduralTree");
            }
            treeObj.transform.position = Vector3.zero;

            ProceduralTreeGenerator treeGen = treeObj.GetComponent<ProceduralTreeGenerator>();
            if (treeGen == null) treeGen = treeObj.AddComponent<ProceduralTreeGenerator>();

            treeGen.ApplyPreset(selectedPreset);
            treeGen.GenerateTree();

            // Set camera target to tree
            orbitCam.target = treeObj.transform;
            orbitCam.targetOffset = new Vector3(0f, treeGen.trunkHeight * 0.5f, 0f);

            // 5. Create Ambient Falling Leaves Particle System
            CreateLeafParticleSystem(treeObj.transform, treeGen.trunkHeight);

            Debug.Log("[TreeSceneBuilder] 3D Tree Scene build successfully completed!");
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

        private Mesh CreateGroundIslandMesh(float radius)
        {
            Mesh mesh = new Mesh();
            mesh.name = "GroundIslandMesh";

            int segments = 32;
            int rings = 8;
            int vertCount = (segments + 1) * (rings + 1);

            Vector3[] verts = new Vector3[vertCount];
            Vector2[] uvs = new Vector2[vertCount];
            Vector3[] normals = new Vector3[vertCount];

            int v = 0;
            for (int r = 0; r <= rings; r++)
            {
                float ringPercent = (float)r / rings;
                float currentRad = ringPercent * radius;

                // Subtle organic height drop off towards the island edges
                float height = (1f - Mathf.Pow(ringPercent, 2f)) * 0.6f + Mathf.Sin(ringPercent * Mathf.PI * 3f) * 0.1f;

                for (int s = 0; s <= segments; s++)
                {
                    float segPercent = (float)s / segments;
                    float angle = segPercent * Mathf.PI * 2f;

                    float x = Mathf.Cos(angle) * currentRad;
                    float z = Mathf.Sin(angle) * currentRad;

                    verts[v] = new Vector3(x, height, z);
                    uvs[v] = new Vector2(x / radius * 0.5f + 0.5f, z / radius * 0.5f + 0.5f);
                    normals[v] = Vector3.up;
                    v++;
                }
            }

            int[] tris = new int[rings * segments * 6];
            int t = 0;

            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < segments; s++)
                {
                    int current = r * (segments + 1) + s;
                    int next = current + segments + 1;

                    tris[t++] = current;
                    tris[t++] = next;
                    tris[t++] = current + 1;

                    tris[t++] = current + 1;
                    tris[t++] = next;
                    tris[t++] = next + 1;
                }
            }

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private void CreateLeafParticleSystem(Transform treeTransform, float treeHeight)
        {
            GameObject psObj = GameObject.Find("FallingLeavesParticles");
            if (psObj == null)
            {
                psObj = new GameObject("FallingLeavesParticles");
            }
            psObj.transform.SetParent(treeTransform, false);
            psObj.transform.localPosition = new Vector3(0f, treeHeight * 0.8f, 0f);

            ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
            if (ps == null) ps = psObj.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 5f;
            main.loop = true;
            main.startLifetime = 6f;
            main.startSpeed = 0.5f;
            main.startSize = 0.35f;
            main.gravityModifier = 0.15f;
            main.maxParticles = 80;

            var emitter = ps.emission;
            emitter.rateOverTime = 8f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 4f;

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.x = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);
            vel.y = new ParticleSystem.MinMaxCurve(-0.5f, -0.1f);
            vel.z = new ParticleSystem.MinMaxCurve(-0.8f, 0.8f);

            var rot = ps.rotationOverLifetime;
            rot.enabled = true;
            rot.z = new ParticleSystem.MinMaxCurve(-180f, 180f);
        }
    }
}
