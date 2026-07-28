using System.Collections.Generic;
using UnityEngine;

namespace Interior.Studio
{
    public class StudioRoomGenerator : MonoBehaviour
    {
        [Header("Room Overall Dimensions")]
        public float roomWidth = 10f;   // X axis (-5 to +5)
        public float roomLength = 8f;   // Z axis (-4 to +4)
        public float roomHeight = 3.2f; // Y axis (0 to 3.2)
        public float wallThickness = 0.2f;

        [Header("Materials")]
        public Material floorMaterial;
        public Material wallMaterial;
        public Material partitionMaterial;
        public Material glassMaterial;

        [Header("Generation Options")]
        public bool generateOnStart = true;
        public bool addWallColliders = true;

        private GameObject m_StructureContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateStudioLayout();
            }
        }

        [ContextMenu("Generate Studio Room Layout")]
        public void GenerateStudioLayout()
        {
            // Clear existing geometry
            Transform existing = transform.Find("StudioStructure");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_StructureContainer = new GameObject("StudioStructure");
            m_StructureContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // 1. Floor
            CreateBoxSegment("Floor", new Vector3(0, -0.1f, 0), new Vector3(roomWidth, 0.2f, roomLength), floorMaterial);

            // 2. Ceiling
            CreateBoxSegment("Ceiling", new Vector3(0, roomHeight + 0.1f, 0), new Vector3(roomWidth, 0.2f, roomLength), wallMaterial);

            // 3. Exterior Walls
            // Back Wall (Z = +halfL)
            CreateBoxSegment("BackWall", new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), wallMaterial);

            // Front Wall with Entrance Doorway Cutout (Z = -halfL)
            // Left segment
            float doorWidth = 1.2f;
            float doorHeight = 2.2f;
            float leftWallW = (roomWidth - doorWidth) * 0.5f;
            CreateBoxSegment("FrontWall_Left", new Vector3(-halfW + leftWallW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(leftWallW, roomHeight, wallThickness), wallMaterial);
            // Right segment
            CreateBoxSegment("FrontWall_Right", new Vector3(halfW - leftWallW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(leftWallW, roomHeight, wallThickness), wallMaterial);
            // Lintels above door
            float lintelH = roomHeight - doorHeight;
            CreateBoxSegment("FrontWall_DoorHeader", new Vector3(0, doorHeight + lintelH * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorWidth, lintelH, wallThickness), wallMaterial);

            // Right Solid Wall (X = +halfW)
            CreateBoxSegment("RightWall", new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), wallMaterial);

            // Left Wall with Large Studio Windows (X = -halfW)
            // Bottom sill
            float sillHeight = 0.6f;
            CreateBoxSegment("LeftWall_Sill", new Vector3(-halfW - wallThickness * 0.5f, sillHeight * 0.5f, 0), new Vector3(wallThickness, sillHeight, roomLength), wallMaterial);
            // Top lintel
            float windowTopH = 2.8f;
            float topHeaderH = roomHeight - windowTopH;
            CreateBoxSegment("LeftWall_Header", new Vector3(-halfW - wallThickness * 0.5f, windowTopH + topHeaderH * 0.5f, 0), new Vector3(wallThickness, topHeaderH, roomLength), wallMaterial);
            // Window Glass Pane
            float windowH = windowTopH - sillHeight;
            CreateBoxSegment("LeftWall_Glass", new Vector3(-halfW - wallThickness * 0.5f, sillHeight + windowH * 0.5f, 0), new Vector3(0.05f, windowH, roomLength - 1f), glassMaterial, false);

            // 4. Interior Partition Zones

            // Zone A: Kitchenette Bar Counter Partition (X = -2 to +0.5, Z = -1.8, Height = 1.1m)
            CreateBoxSegment("Partition_KitchenBar", new Vector3(-0.75f, 0.55f, -1.8f), new Vector3(2.8f, 1.1f, 0.3f), partitionMaterial);

            // Zone B: Sleeping Alcove Feature Partition (X = 2.2, Z = 0.5 to 3.8, Height = 2.4m)
            CreateBoxSegment("Partition_BedAlcove", new Vector3(2.2f, 1.2f, 2.15f), new Vector3(0.15f, 2.4f, 3.3f), partitionMaterial);

            // Zone C: Bathroom Enclosed Partition Walls (X = -4.8 to -2.0, Z = -3.8 to -1.8)
            // Bath East Wall (with doorway)
            float bathDoorW = 0.9f;
            float bathWallL = 2.0f;
            float bathWallSeg = (bathWallL - bathDoorW);
            CreateBoxSegment("Partition_BathWallEast_A", new Vector3(-2.0f, roomHeight * 0.5f, -3.8f + bathWallSeg * 0.5f), new Vector3(0.15f, roomHeight, bathWallSeg), wallMaterial);
            CreateBoxSegment("Partition_BathHeader", new Vector3(-2.0f, 2.2f + (roomHeight - 2.2f) * 0.5f, -3.8f + bathWallL - bathDoorW * 0.5f), new Vector3(0.15f, roomHeight - 2.2f, bathDoorW), wallMaterial);

            // Bath North Wall
            CreateBoxSegment("Partition_BathWallNorth", new Vector3(-3.4f, roomHeight * 0.5f, -1.8f), new Vector3(2.8f, roomHeight, 0.15f), wallMaterial);

            // Zone D: Workspace / Desk Corner Accent Divider (X = -4.8 to -3.2, Z = 1.2, Height = 1.5m)
            CreateBoxSegment("Partition_WorkspaceDivider", new Vector3(-4.0f, 0.75f, 1.2f), new Vector3(1.6f, 1.5f, 0.12f), partitionMaterial);

            Debug.Log("[StudioRoomGenerator] Studio Room layout successfully generated!");
        }

        private GameObject CreateBoxSegment(string segName, Vector3 localPos, Vector3 scale, Material mat, bool addCollider = true)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = segName;
            obj.transform.SetParent(m_StructureContainer.transform, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = scale;

            if (mat != null)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null) mr.sharedMaterial = mat;
            }

            if (!addCollider || !addWallColliders)
            {
                BoxCollider col = obj.GetComponent<BoxCollider>();
                if (col != null) DestroyImmediate(col);
            }

            return obj;
        }

        private void EnsureMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (floorMaterial == null)
            {
                floorMaterial = new Material(litShader);
                floorMaterial.name = "M_StudioFloor";
                Color c = new Color(0.42f, 0.28f, 0.18f); // Warm hardwood timber
                floorMaterial.SetColor("_BaseColor", c);
                floorMaterial.SetColor("_Color", c);
            }

            if (wallMaterial == null)
            {
                wallMaterial = new Material(litShader);
                wallMaterial.name = "M_StudioWall";
                Color c = new Color(0.92f, 0.92f, 0.90f); // Clean off-white studio
                wallMaterial.SetColor("_BaseColor", c);
                wallMaterial.SetColor("_Color", c);
            }

            if (partitionMaterial == null)
            {
                partitionMaterial = new Material(litShader);
                partitionMaterial.name = "M_StudioPartition";
                Color c = new Color(0.24f, 0.28f, 0.32f); // Accent slate dark
                partitionMaterial.SetColor("_BaseColor", c);
                partitionMaterial.SetColor("_Color", c);
            }

            if (glassMaterial == null)
            {
                glassMaterial = new Material(litShader);
                glassMaterial.name = "M_StudioGlass";
                Color c = new Color(0.7f, 0.85f, 0.95f, 0.35f);
                glassMaterial.SetColor("_BaseColor", c);
                glassMaterial.SetColor("_Color", c);
            }
        }
    }
}
