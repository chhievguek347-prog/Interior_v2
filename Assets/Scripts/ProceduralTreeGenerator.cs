using System.Collections.Generic;
using UnityEngine;

namespace Interior.Environment
{
    public enum TreePreset
    {
        OakLush,
        CherryBlossom,
        AutumnGold,
        PineEvergreen
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class ProceduralTreeGenerator : MonoBehaviour
    {
        [Header("Tree Preset & Style")]
        public TreePreset preset = TreePreset.OakLush;
        public int randomSeed = 12345;
        public bool generateOnStart = true;

        [Header("Trunk Parameters")]
        [Range(2f, 15f)] public float trunkHeight = 6f;
        [Range(0.2f, 2f)] public float trunkRadius = 0.8f;
        [Range(0.1f, 0.9f)] public float radiusTaper = 0.65f;
        [Range(4, 16)] public int radialSegments = 8;
        [Range(0f, 0.5f)] public float trunkCurvature = 0.15f;

        [Header("Branching Parameters")]
        [Range(2, 6)] public int branchRecursionDepth = 4;
        [Range(2, 5)] public int branchesPerNode = 3;
        [Range(15f, 60f)] public float branchAngle = 35f;
        [Range(0.4f, 0.85f)] public float lengthDecay = 0.7f;
        [Range(0.4f, 0.85f)] public float radiusDecay = 0.6f;

        [Header("Foliage Canopy")]
        public bool generateLeaves = true;
        [Range(0.5f, 3f)] public float leafClusterScale = 1.4f;
        [Range(2, 12)] public int leavesPerTerminalBranch = 6;
        public Color leafColorPrimary = new Color(0.18f, 0.55f, 0.15f);
        public Color leafColorSecondary = new Color(0.35f, 0.72f, 0.22f);

        [Header("Materials")]
        public Material barkMaterial;
        public Material leafMaterial;

        // Internal data
        private MeshFilter m_MeshFilter;
        private MeshRenderer m_MeshRenderer;
        private GameObject m_LeavesContainer;

        private void Awake()
        {
            m_MeshFilter = GetComponent<MeshFilter>();
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateTree();
            }
        }

        public void ApplyPreset(TreePreset selectedPreset)
        {
            preset = selectedPreset;
            switch (preset)
            {
                case TreePreset.OakLush:
                    trunkHeight = 7f;
                    trunkRadius = 0.9f;
                    branchRecursionDepth = 4;
                    branchAngle = 38f;
                    leafColorPrimary = new Color(0.15f, 0.52f, 0.15f);
                    leafColorSecondary = new Color(0.28f, 0.68f, 0.20f);
                    break;
                case TreePreset.CherryBlossom:
                    trunkHeight = 6f;
                    trunkRadius = 0.75f;
                    branchRecursionDepth = 5;
                    branchAngle = 42f;
                    leafColorPrimary = new Color(0.95f, 0.55f, 0.72f);
                    leafColorSecondary = new Color(1.0f, 0.78f, 0.88f);
                    break;
                case TreePreset.AutumnGold:
                    trunkHeight = 6.5f;
                    trunkRadius = 0.8f;
                    branchRecursionDepth = 4;
                    branchAngle = 35f;
                    leafColorPrimary = new Color(0.85f, 0.32f, 0.08f);
                    leafColorSecondary = new Color(0.95f, 0.65f, 0.12f);
                    break;
                case TreePreset.PineEvergreen:
                    trunkHeight = 10f;
                    trunkRadius = 0.6f;
                    branchRecursionDepth = 3;
                    branchAngle = 55f;
                    leafColorPrimary = new Color(0.08f, 0.32f, 0.18f);
                    leafColorSecondary = new Color(0.14f, 0.45f, 0.24f);
                    break;
            }
        }

        [ContextMenu("Generate Tree")]
        public void GenerateTree()
        {
            if (m_MeshFilter == null) m_MeshFilter = GetComponent<MeshFilter>();
            if (m_MeshRenderer == null) m_MeshRenderer = GetComponent<MeshRenderer>();

            Random.InitState(randomSeed);

            // Clear old leaf container child if exists
            Transform existingLeaves = transform.Find("LeafCanopy");
            if (existingLeaves != null)
            {
                if (Application.isPlaying) Destroy(existingLeaves.gameObject);
                else DestroyImmediate(existingLeaves.gameObject);
            }

            m_LeavesContainer = new GameObject("LeafCanopy");
            m_LeavesContainer.transform.SetParent(transform, false);

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();

            List<Vector3> leafPositions = new List<Vector3>();

            // Generate Trunk & Branch hierarchy recursively
            Vector3 startPos = Vector3.zero;
            Vector3 startDir = Vector3.up;
            GenerateBranchSegment(startPos, startDir, trunkHeight, trunkRadius, 0, vertices, triangles, uvs, normals, leafPositions);

            // Assign Bark Mesh
            Mesh barkMesh = new Mesh();
            barkMesh.name = "ProceduralTreeBark";
            barkMesh.vertices = vertices.ToArray();
            barkMesh.triangles = triangles.ToArray();
            barkMesh.uv = uvs.ToArray();
            barkMesh.normals = normals.ToArray();
            barkMesh.RecalculateBounds();

            m_MeshFilter.sharedMesh = barkMesh;

            // Create default materials if not set
            EnsureMaterials();
            if (barkMaterial != null) m_MeshRenderer.sharedMaterial = barkMaterial;

            // Generate Leaf Canopy
            if (generateLeaves && leafPositions.Count > 0)
            {
                GenerateLeafCanopy(leafPositions);
            }
        }

        private void GenerateBranchSegment(
            Vector3 startPos,
            Vector3 dir,
            float length,
            float radius,
            int depth,
            List<Vector3> verts,
            List<int> tris,
            List<Vector2> uvs,
            List<Vector3> normals,
            List<Vector3> leafPositions)
        {
            dir.Normalize();

            // Calculate end position with slight organic curvature
            Vector3 side = Vector3.Cross(dir, Vector3.up);
            if (side.sqrMagnitude < 0.001f) side = Vector3.Cross(dir, Vector3.right);
            side.Normalize();

            Vector3 curveOffset = (side * Random.Range(-1f, 1f) + Vector3.Cross(dir, side) * Random.Range(-1f, 1f)) * (length * trunkCurvature);
            Vector3 endPos = startPos + (dir * length) + curveOffset;

            float endRadius = radius * radiusTaper;
            int ringVertsCount = radialSegments + 1;

            int baseIndex = verts.Count;

            // Create ring at start and end
            for (int i = 0; i <= radialSegments; i++)
            {
                float u = (float)i / radialSegments;
                float angle = u * Mathf.PI * 2f;

                Vector3 localRad = (Mathf.Cos(angle) * side + Mathf.Sin(angle) * Vector3.Cross(dir, side)).normalized;

                // Start Ring Vertex
                Vector3 vStart = startPos + localRad * radius;
                verts.Add(vStart);
                normals.Add(localRad);
                uvs.Add(new Vector2(u, 0f));

                // End Ring Vertex
                Vector3 vEnd = endPos + localRad * endRadius;
                verts.Add(vEnd);
                normals.Add(localRad);
                uvs.Add(new Vector2(u, 1f));
            }

            // Create side quad triangles
            for (int i = 0; i < radialSegments; i++)
            {
                int currentStart = baseIndex + (i * 2);
                int currentEnd = currentStart + 1;
                int nextStart = baseIndex + ((i + 1) * 2);
                int nextEnd = nextStart + 1;

                // Triangle 1
                tris.Add(currentStart);
                tris.Add(currentEnd);
                tris.Add(nextEnd);

                // Triangle 2
                tris.Add(currentStart);
                tris.Add(nextEnd);
                tris.Add(nextStart);
            }

            // If terminal branch reached, mark for leaf placement
            if (depth >= branchRecursionDepth)
            {
                leafPositions.Add(endPos);
                return;
            }

            // Otherwise spawn child branches
            int nextDepth = depth + 1;
            int childCount = (depth == 0) ? branchesPerNode + 1 : branchesPerNode;

            float angleStep = 360f / childCount;
            float baseAngleOffset = Random.Range(0f, 360f);

            for (int b = 0; b < childCount; b++)
            {
                float branchRotAngle = baseAngleOffset + (b * angleStep) + Random.Range(-15f, 15f);
                Quaternion branchRot = Quaternion.AngleAxis(branchRotAngle, dir);
                Vector3 outDir = Quaternion.AngleAxis(branchAngle + Random.Range(-8f, 8f), side) * dir;
                outDir = branchRot * outDir;

                float childLength = length * lengthDecay * Random.Range(0.85f, 1.15f);
                float childRadius = endRadius * radiusDecay;

                // Branch position slightly offset along end of segment
                Vector3 childStart = endPos - dir * (radius * 0.5f);

                GenerateBranchSegment(
                    childStart,
                    outDir,
                    childLength,
                    childRadius,
                    nextDepth,
                    verts,
                    tris,
                    uvs,
                    normals,
                    leafPositions);
            }
        }

        private void GenerateLeafCanopy(List<Vector3> terminalPositions)
        {
            Mesh leafMesh = CreateLeafClusterMesh();

            foreach (Vector3 pos in terminalPositions)
            {
                for (int i = 0; i < leavesPerTerminalBranch; i++)
                {
                    GameObject leafObj = new GameObject("LeafCluster");
                    leafObj.transform.SetParent(m_LeavesContainer.transform, false);

                    Vector3 offset = Random.insideUnitSphere * (leafClusterScale * 0.8f);
                    leafObj.transform.localPosition = pos + offset;
                    leafObj.transform.localRotation = Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(0f, 360f), Random.Range(-30f, 30f));
                    float scale = leafClusterScale * Random.Range(0.8f, 1.2f);
                    leafObj.transform.localScale = new Vector3(scale, scale, scale);

                    MeshFilter mf = leafObj.AddComponent<MeshFilter>();
                    MeshRenderer mr = leafObj.AddComponent<MeshRenderer>();

                    mf.sharedMesh = leafMesh;
                    mr.sharedMaterial = leafMaterial;

                    // Tint leaf color using MaterialPropertyBlock for dynamic variation
                    MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
                    Color clusterColor = Color.Lerp(leafColorPrimary, leafColorSecondary, Random.value);
                    propBlock.SetColor("_BaseColor", clusterColor);
                    propBlock.SetColor("_Color", clusterColor);
                    mr.SetPropertyBlock(propBlock);
                }
            }
        }

        private Mesh CreateLeafClusterMesh()
        {
            // Creates a low-poly stylized cross-quad leaf cluster mesh
            Mesh mesh = new Mesh();
            mesh.name = "LeafClusterMesh";

            float s = 0.5f;
            Vector3[] verts = new Vector3[]
            {
                // Quad 1 (Facing Z)
                new Vector3(-s, -s, 0), new Vector3(s, -s, 0), new Vector3(s, s, 0), new Vector3(-s, s, 0),
                // Quad 2 (Facing X)
                new Vector3(0, -s, -s), new Vector3(0, -s, s), new Vector3(0, s, s), new Vector3(0, s, -s)
            };

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
            };

            int[] tris = new int[]
            {
                // Quad 1 Double-sided
                0, 1, 2, 0, 2, 3,  2, 1, 0, 3, 2, 0,
                // Quad 2 Double-sided
                4, 5, 6, 4, 6, 7,  6, 5, 4, 7, 6, 4
            };

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private void EnsureMaterials()
        {
            Shader defaultLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (defaultLitShader == null) defaultLitShader = Shader.Find("Standard");

            if (barkMaterial == null)
            {
                barkMaterial = new Material(defaultLitShader);
                barkMaterial.name = "Mat_ProceduralBark";
                Color barkColor = new Color(0.32f, 0.20f, 0.12f);
                barkMaterial.SetColor("_BaseColor", barkColor);
                barkMaterial.SetColor("_Color", barkColor);
            }

            if (leafMaterial == null)
            {
                leafMaterial = new Material(defaultLitShader);
                leafMaterial.name = "Mat_ProceduralLeaf";
                leafMaterial.SetColor("_BaseColor", leafColorPrimary);
                leafMaterial.SetColor("_Color", leafColorPrimary);
            }
        }
    }
}
