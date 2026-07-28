using System.Collections.Generic;
using UnityEngine;

namespace Interior.EmptyShell
{
    public class EmptyStudioShellGenerator : MonoBehaviour
    {
        [Header("Architectural Shell Dimensions")]
        public float roomWidth = 8.0f;    // X axis (-4.0 to +4.0)
        public float roomLength = 5.0f;   // Z axis (-2.5 to +2.5)
        public float roomHeight = 2.7f;   // Y axis (0 to 2.7)
        public float wallThickness = 0.15f; // ~15cm realistic wall thickness
        public float baseboardHeight = 0.12f;
        public float baseboardThickness = 0.02f;

        [Header("PBR Material Slots")]
        public Material matDrywallPlaster;
        public Material matWoodPlankReal;
        public Material matTileGrout;
        public Material matTrimBaseboard;
        public Material matDoorWoodPanel;
        public Material matMetalHandle;
        public Material matGlassPane;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_ShellContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateEmptyStudioShell();
            }
        }

        [ContextMenu("Generate Empty Studio Architectural Shell")]
        public void GenerateEmptyStudioShell()
        {
            Transform existing = transform.Find("EmptyStudioArchitecturalShell");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_ShellContainer = new GameObject("EmptyStudioArchitecturalShell");
            m_ShellContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. Continuous Flooring Planes ---
            GameObject floorGroup = CreateSubContainer("1_Flooring");

            // Main Wood Plank Flooring (Living Room & Bedroom Areas)
            CreateBox("Floor_MainWoodPlanks", floorGroup.transform, new Vector3(-0.9f, -0.05f, 0), new Vector3(6.2f, 0.1f, roomLength), matWoodPlankReal);

            // Kitchen Ceramic Tile Flooring (Zone A Kitchen Corner: X = -4.0 to -2.0, Z = 0.5 to 2.5)
            CreateBox("Floor_KitchenTile", floorGroup.transform, new Vector3(-3.0f, 0.01f, 1.4f), new Vector3(1.98f, 0.02f, 2.18f), matTileGrout);

            // Bathroom Ceramic Tile Flooring (Zone B Bathroom: X = +2.2 to +4.0, Z = 0.2 to 2.5)
            CreateBox("Floor_BathroomTile", floorGroup.transform, new Vector3(3.1f, 0.01f, 1.35f), new Vector3(1.78f, 0.02f, 2.28f), matTileGrout);

            // Ceiling (2.7m height)
            CreateBox("Ceiling_MainPlaster", floorGroup.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matDrywallPlaster);

            // --- 2. 15cm Drywall Walls & Windows ---
            GameObject wallGroup = CreateSubContainer("2_WallsAndWindows");

            // West Exterior Wall (Left of Zone A: X = -halfW) with Living Room Window Cutout
            float winW = 2.0f;
            float winSillH = 0.8f;
            float winTopH = 2.4f;
            float winH = winTopH - winSillH;
            float westSideL = (roomLength - winW) * 0.5f;

            CreateBox("Wall_West_South", wallGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, -halfL + westSideL * 0.5f), new Vector3(wallThickness, roomHeight, westSideL), matDrywallPlaster);
            CreateBox("Wall_West_North", wallGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, halfL - westSideL * 0.5f), new Vector3(wallThickness, roomHeight, westSideL), matDrywallPlaster);
            CreateBox("Wall_West_Sill", wallGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, winW), matDrywallPlaster);
            CreateBox("Wall_West_Header", wallGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, winW), matDrywallPlaster);
            // Window Assembly
            CreateBox("LivingWindow_Frame", wallGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(wallThickness + 0.04f, winH, winW), matMetalHandle, false);
            CreateBox("LivingWindow_GlassPane", wallGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.02f, winH - 0.08f, winW - 0.08f), matGlassPane, false);

            // East Exterior Wall (Right of Zone B: X = +halfW) with Bedroom Window Cutout
            float eastSideL = (roomLength - winW) * 0.5f;
            CreateBox("Wall_East_South", wallGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, -halfL + eastSideL * 0.5f), new Vector3(wallThickness, roomHeight, eastSideL), matDrywallPlaster);
            CreateBox("Wall_East_North", wallGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, halfL - eastSideL * 0.5f), new Vector3(wallThickness, roomHeight, eastSideL), matDrywallPlaster);
            CreateBox("Wall_East_Sill", wallGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, winW), matDrywallPlaster);
            CreateBox("Wall_East_Header", wallGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, winW), matDrywallPlaster);
            // Window Assembly
            CreateBox("BedroomWindow_Frame", wallGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(wallThickness + 0.04f, winH, winW), matMetalHandle, false);
            CreateBox("BedroomWindow_GlassPane", wallGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.02f, winH - 0.08f, winW - 0.08f), matGlassPane, false);

            // North Solid Exterior Wall (Z = +halfL)
            CreateBox("Wall_North", wallGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), matDrywallPlaster);

            // South Exterior Wall with Front Entrance Door Cutout (Z = -halfL)
            float frontDoorW = 1.0f;
            float frontDoorH = 2.1f;
            float frontDoorX = -2.2f;
            float frontLeftW = (frontDoorX - (-halfW)) - frontDoorW * 0.5f;
            float frontRightW = (halfW - frontDoorX) - frontDoorW * 0.5f;

            CreateBox("Wall_South_Left", wallGroup.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matDrywallPlaster);
            CreateBox("Wall_South_Right", wallGroup.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matDrywallPlaster);
            CreateBox("Wall_South_DoorHeader", wallGroup.transform, new Vector3(frontDoorX, frontDoorH + (roomHeight - frontDoorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontDoorW, roomHeight - frontDoorH, wallThickness), matDrywallPlaster);

            // --- 3. Single Interior Dividing Wall & Bathroom Partition ---
            // Center Dividing Wall (X = 0) with Connecting Doorway
            float divDoorW = 1.0f;
            float divDoorH = 2.1f;
            float divDoorZ = 0.0f;
            float divSouthL = (divDoorZ - (-halfL)) - divDoorW * 0.5f;
            float divNorthL = (halfL - divDoorZ) - divDoorW * 0.5f;

            CreateBox("DividingWall_South", wallGroup.transform, new Vector3(0, roomHeight * 0.5f, -halfL + divSouthL * 0.5f), new Vector3(wallThickness, roomHeight, divSouthL), matDrywallPlaster);
            CreateBox("DividingWall_North", wallGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL - divNorthL * 0.5f), new Vector3(wallThickness, roomHeight, divNorthL), matDrywallPlaster);
            CreateBox("DividingWall_DoorHeader", wallGroup.transform, new Vector3(0, divDoorH + (roomHeight - divDoorH) * 0.5f, divDoorZ), new Vector3(wallThickness, roomHeight - divDoorH, divDoorW), matDrywallPlaster);

            // Bathroom Enclosure Partitions (X = 2.2, Z = 0.2 to 2.5 and Z = 0.2, X = 2.2 to 4.0)
            float bathDoorW = 0.9f;
            float bathDoorH = 2.1f;
            CreateBox("Bath_Partition_West", wallGroup.transform, new Vector3(2.2f, roomHeight * 0.5f, 1.75f), new Vector3(wallThickness, roomHeight, 1.5f), matDrywallPlaster);
            CreateBox("Bath_Partition_South", wallGroup.transform, new Vector3(3.1f, roomHeight * 0.5f, 0.2f), new Vector3(1.8f, roomHeight, wallThickness), matDrywallPlaster);
            CreateBox("Bath_DoorHeader", wallGroup.transform, new Vector3(2.2f, bathDoorH + (roomHeight - bathDoorH) * 0.5f, 0.65f), new Vector3(wallThickness, roomHeight - bathDoorH, bathDoorW), matDrywallPlaster);

            // --- 4. Detailed Door Assemblies (Frames, Panels, Handles & Hinges) ---
            GameObject doorGroup = CreateSubContainer("3_DoorAssemblies");

            // 1. Front Entrance Door Assembly
            CreateDoorAssembly("FrontEntranceDoor", doorGroup.transform, new Vector3(frontDoorX, 0, -halfL), frontDoorW, frontDoorH, 0.08f, matDoorWoodPanel, matMetalHandle);

            // 2. Center Dividing Wall Door Assembly
            CreateDoorAssembly("DividingWallDoor", doorGroup.transform, new Vector3(0, 0, divDoorZ), divDoorW, divDoorH, 0.08f, matDoorWoodPanel, matMetalHandle);

            // 3. Bathroom Door Assembly
            CreateDoorAssembly("BathroomDoor", doorGroup.transform, new Vector3(2.2f, 0, 0.65f), bathDoorW, bathDoorH, 0.08f, matDoorWoodPanel, matMetalHandle);

            // --- 5. Baseboard Trim Moldings along all wall bases ---
            GameObject trimGroup = CreateSubContainer("4_BaseboardTrim");
            float bH = baseboardHeight;
            float bT = baseboardThickness;

            // North Wall Baseboard
            CreateBox("Trim_NorthWall", trimGroup.transform, new Vector3(0, bH * 0.5f, halfL - bT * 0.5f), new Vector3(roomWidth, bH, bT), matTrimBaseboard, false);
            // West Wall Baseboards (Living/Kitchen)
            CreateBox("Trim_WestWall_South", trimGroup.transform, new Vector3(-halfW + bT * 0.5f, bH * 0.5f, -halfL + westSideL * 0.5f), new Vector3(bT, bH, westSideL), matTrimBaseboard, false);
            CreateBox("Trim_WestWall_North", trimGroup.transform, new Vector3(-halfW + bT * 0.5f, bH * 0.5f, halfL - westSideL * 0.5f), new Vector3(bT, bH, westSideL), matTrimBaseboard, false);
            // East Wall Baseboards (Bedroom/Bathroom)
            CreateBox("Trim_EastWall_South", trimGroup.transform, new Vector3(halfW - bT * 0.5f, bH * 0.5f, -halfL + eastSideL * 0.5f), new Vector3(bT, bH, eastSideL), matTrimBaseboard, false);
            CreateBox("Trim_EastWall_North", trimGroup.transform, new Vector3(halfW - bT * 0.5f, bH * 0.5f, halfL - eastSideL * 0.5f), new Vector3(bT, bH, eastSideL), matTrimBaseboard, false);
            // Dividing Wall Baseboards
            CreateBox("Trim_DividingWall_West", trimGroup.transform, new Vector3(-wallThickness * 0.5f - bT * 0.5f, bH * 0.5f, -halfL + divSouthL * 0.5f), new Vector3(bT, bH, divSouthL), matTrimBaseboard, false);
            CreateBox("Trim_DividingWall_East", trimGroup.transform, new Vector3(wallThickness * 0.5f + bT * 0.5f, bH * 0.5f, halfL - divNorthL * 0.5f), new Vector3(bT, bH, divNorthL), matTrimBaseboard, false);

            Debug.Log("[EmptyStudioShellGenerator] Photorealistic Empty Studio Architectural Shell generated successfully!");
        }

        private void CreateDoorAssembly(string name, Transform parent, Vector3 pos, float width, float height, float thickness, Material doorMat, Material metalMat)
        {
            GameObject doorRoot = new GameObject(name);
            doorRoot.transform.SetParent(parent, false);
            doorRoot.transform.localPosition = pos;

            // Frame
            CreateBox(name + "_FrameLeft", doorRoot.transform, new Vector3(-width * 0.5f + 0.04f, height * 0.5f, 0), new Vector3(0.08f, height, thickness + 0.04f), doorMat, false);
            CreateBox(name + "_FrameRight", doorRoot.transform, new Vector3(width * 0.5f - 0.04f, height * 0.5f, 0), new Vector3(0.08f, height, thickness + 0.04f), doorMat, false);
            CreateBox(name + "_FrameTop", doorRoot.transform, new Vector3(0, height - 0.04f, 0), new Vector3(width, 0.08f, thickness + 0.04f), doorMat, false);

            // Door Panel
            CreateBox(name + "_Panel", doorRoot.transform, new Vector3(0, height * 0.5f, 0), new Vector3(width - 0.08f, height - 0.08f, 0.04f), doorMat, false);

            // Metallic Handle Lever
            CreateBox(name + "_HandleFront", doorRoot.transform, new Vector3(width * 0.35f, height * 0.5f, 0.05f), new Vector3(0.14f, 0.04f, 0.05f), metalMat, false);
            CreateBox(name + "_HandleBack", doorRoot.transform, new Vector3(width * 0.35f, height * 0.5f, -0.05f), new Vector3(0.14f, 0.04f, 0.05f), metalMat, false);
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_ShellContainer.transform, false);
            return c;
        }

        private GameObject CreateBox(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool addCol = true)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = pos;
            obj.transform.localScale = scale;

            if (mat != null)
            {
                MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                if (mr != null) mr.sharedMaterial = mat;
            }

            if (!addCol || !addPhysicsColliders)
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

            if (matDrywallPlaster == null) matDrywallPlaster = CreateMat(litShader, "Mat_Drywall_Plaster", new Color(0.92f, 0.91f, 0.88f), 0.85f, 0.0f);
            if (matWoodPlankReal == null) matWoodPlankReal = CreateMat(litShader, "Mat_WoodPlank_Real", new Color(0.72f, 0.54f, 0.36f), 0.45f, 0.05f);
            if (matTileGrout == null) matTileGrout = CreateMat(litShader, "Mat_Tile_Grout", new Color(0.68f, 0.70f, 0.72f), 0.4f, 0.05f);
            if (matTrimBaseboard == null) matTrimBaseboard = CreateMat(litShader, "Mat_Trim_Baseboard", new Color(0.96f, 0.96f, 0.96f), 0.3f, 0.0f);
            if (matDoorWoodPanel == null) matDoorWoodPanel = CreateMat(litShader, "Mat_Door_WoodPanel", new Color(0.62f, 0.42f, 0.25f), 0.45f, 0.05f);
            if (matMetalHandle == null) matMetalHandle = CreateMat(litShader, "Mat_Metal_Handle", new Color(0.25f, 0.25f, 0.28f), 0.35f, 0.7f);
            if (matGlassPane == null) matGlassPane = CreateMat(litShader, "Mat_Glass_Pane", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
        }

        private Material CreateMat(Shader shader, string name, Color color, float smoothness, float metallic)
        {
            Material mat = new Material(shader);
            mat.name = name;
            mat.SetColor("_BaseColor", color);
            mat.SetColor("_Color", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            return mat;
        }
    }
}
