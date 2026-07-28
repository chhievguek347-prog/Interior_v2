using System.Collections.Generic;
using UnityEngine;

namespace Interior.DualZone
{
    public class DualZoneStudioGenerator : MonoBehaviour
    {
        [Header("Dual-Zone Room Dimensions")]
        public float roomWidth = 8.0f;   // X axis (-4 to +4)
        public float roomLength = 5.0f;  // Z axis (-2.5 to +2.5)
        public float roomHeight = 2.7f;  // Y axis (0 to 2.7)
        public float wallThickness = 0.18f;

        [Header("PBR Materials")]
        public Material matOakFlooring;
        public Material matKitchenTile;
        public Material matBathroomTile;
        public Material matWallPlaster;
        public Material matDividingWall;
        public Material matSofaFabric;
        public Material matMetalBlack;
        public Material matWhiteCeramic;
        public Material matGlassClear;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_StudioContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateDualZoneStudio();
            }
        }

        [ContextMenu("Generate Dual-Zone Studio Apartment")]
        public void GenerateDualZoneStudio()
        {
            Transform existing = transform.Find("DualZoneStudio");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_StudioContainer = new GameObject("DualZoneStudio");
            m_StudioContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. Continuous Flooring Planes ---
            GameObject archRoot = CreateSubContainer("1_Architecture");

            // Main Oak Wood Flooring (Living Room & Bedroom Areas: X = -4 to +2.2)
            CreateBox("Floor_MainOak", archRoot.transform, new Vector3(-0.9f, -0.05f, 0), new Vector3(6.2f, 0.1f, roomLength), matOakFlooring);

            // Kitchen Tile Flooring (Zone A Kitchen Corner: X = -4 to -2, Z = 0.5 to 2.5)
            CreateBox("Floor_KitchenTile", archRoot.transform, new Vector3(-3.0f, 0.01f, 1.4f), new Vector3(1.98f, 0.02f, 2.18f), matKitchenTile);

            // Bathroom Ceramic Tile Flooring (Zone B Bathroom: X = +2.2 to +4.0, Z = 0.2 to 2.5)
            CreateBox("Floor_BathroomTile", archRoot.transform, new Vector3(3.1f, 0.01f, 1.35f), new Vector3(1.78f, 0.02f, 2.28f), matBathroomTile);

            // Ceiling (2.7m height)
            CreateBox("Ceiling_Main", archRoot.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWallPlaster);

            // --- 2. Outer Exterior Walls & Windows ---
            // West Wall (Left side of Zone A: X = -halfW) with Living Room Window Cutout
            float winW = 2.0f;
            float winSillH = 0.8f;
            float winTopH = 2.4f;
            float winH = winTopH - winSillH;
            float westSideL = (roomLength - winW) * 0.5f;
            CreateBox("Wall_West_South", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, -halfL + westSideL * 0.5f), new Vector3(wallThickness, roomHeight, westSideL), matWallPlaster);
            CreateBox("Wall_West_North", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, halfL - westSideL * 0.5f), new Vector3(wallThickness, roomHeight, westSideL), matWallPlaster);
            CreateBox("Wall_West_Sill", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, winW), matWallPlaster);
            CreateBox("Wall_West_Header", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, winW), matWallPlaster);
            CreateBox("Living_WindowGlass", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.04f, winH, winW - 0.1f), matGlassClear, false);

            // East Wall (Right side of Zone B: X = +halfW) with Bedroom Window Cutout
            float eastSideL = (roomLength - winW) * 0.5f;
            CreateBox("Wall_East_South", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, -halfL + eastSideL * 0.5f), new Vector3(wallThickness, roomHeight, eastSideL), matWallPlaster);
            CreateBox("Wall_East_North", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, halfL - eastSideL * 0.5f), new Vector3(wallThickness, roomHeight, eastSideL), matWallPlaster);
            CreateBox("Wall_East_Sill", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, winW), matWallPlaster);
            CreateBox("Wall_East_Header", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, winW), matWallPlaster);
            CreateBox("Bedroom_WindowGlass", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.04f, winH, winW - 0.1f), matGlassClear, false);

            // North Solid Outer Wall (Z = +halfL)
            CreateBox("Wall_North", archRoot.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), matWallPlaster);

            // South Outer Wall with Front Entrance Door Cutout (Z = -halfL)
            float frontDoorW = 1.1f;
            float frontDoorH = 2.1f;
            float frontDoorX = -2.2f; // Front door leads into Zone A living room
            float frontLeftW = (frontDoorX - (-halfW)) - frontDoorW * 0.5f;
            float frontRightW = (halfW - frontDoorX) - frontDoorW * 0.5f;

            CreateBox("Wall_South_Left", archRoot.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matWallPlaster);
            CreateBox("Wall_South_Right", archRoot.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matWallPlaster);
            CreateBox("Wall_South_DoorHeader", archRoot.transform, new Vector3(frontDoorX, frontDoorH + (roomHeight - frontDoorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontDoorW, roomHeight - frontDoorH, wallThickness), matWallPlaster);
            // Front Entrance Door Frame
            CreateBox("FrontEntrance_DoorFrame", archRoot.transform, new Vector3(frontDoorX, frontDoorH * 0.5f, -halfL), new Vector3(frontDoorW, frontDoorH, 0.08f), matMetalBlack, false);

            // --- 3. Single Interior Dividing Wall (Separating Zone A & Zone B at X = 0) ---
            float divDoorW = 1.1f;
            float divDoorH = 2.1f;
            float divDoorZ = 0.0f; // Connecting doorway in the center of dividing wall
            float divSouthL = (divDoorZ - (-halfL)) - divDoorW * 0.5f;
            float divNorthL = (halfL - divDoorZ) - divDoorW * 0.5f;

            CreateBox("DividingWall_SouthSeg", archRoot.transform, new Vector3(0, roomHeight * 0.5f, -halfL + divSouthL * 0.5f), new Vector3(wallThickness, roomHeight, divSouthL), matDividingWall);
            CreateBox("DividingWall_NorthSeg", archRoot.transform, new Vector3(0, roomHeight * 0.5f, halfL - divNorthL * 0.5f), new Vector3(wallThickness, roomHeight, divNorthL), matDividingWall);
            CreateBox("DividingWall_DoorHeader", archRoot.transform, new Vector3(0, divDoorH + (roomHeight - divDoorH) * 0.5f, divDoorZ), new Vector3(wallThickness, roomHeight - divDoorH, divDoorW), matDividingWall);
            CreateBox("ConnectingDoorway_Frame", archRoot.transform, new Vector3(0, divDoorH * 0.5f, divDoorZ), new Vector3(wallThickness + 0.04f, divDoorH, divDoorW), matOakFlooring, false);

            // --- 4. Zone A: Open-Plan Living & Kitchen (X = -4.0 to 0.0) ---
            GameObject zoneAGroup = CreateSubContainer("2_ZoneA_LivingAndKitchen");

            // Living Room Area (X = -2.5 to -0.3, Z = -2.2 to 0.0)
            CreateBox("Living_SofaMain", zoneAGroup.transform, new Vector3(-1.8f, 0.38f, -1.2f), new Vector3(1.85f, 0.42f, 0.85f), matSofaFabric);
            CreateBox("Living_SofaBackrest", zoneAGroup.transform, new Vector3(-1.8f, 0.62f, -0.82f), new Vector3(1.85f, 0.52f, 0.22f), matSofaFabric);
            CreateBox("Living_CoffeeTable", zoneAGroup.transform, new Vector3(-1.8f, 0.22f, -1.9f), new Vector3(1.1f, 0.38f, 0.6f), matOakFlooring);
            CreateBox("Living_MediaConsole", zoneAGroup.transform, new Vector3(-3.78f, 0.25f, -1.2f), new Vector3(0.38f, 0.35f, 1.6f), matOakFlooring);
            CreateBox("Living_MountedTV", zoneAGroup.transform, new Vector3(-3.92f, 1.35f, -1.2f), new Vector3(0.06f, 0.78f, 1.35f), matMetalBlack);

            // Kitchen Counter / Island (Transitioning directly into living area with NO separating wall)
            CreateBox("Kitchen_IslandCounter", zoneAGroup.transform, new Vector3(-2.65f, 0.45f, 1.4f), new Vector3(0.68f, 0.9f, 2.0f), matOakFlooring);
            CreateBox("Kitchen_StoneTop", zoneAGroup.transform, new Vector3(-2.65f, 0.92f, 1.4f), new Vector3(0.72f, 0.06f, 2.05f), matKitchenTile);
            CreateBox("Kitchen_Sink", zoneAGroup.transform, new Vector3(-2.65f, 0.95f, 0.8f), new Vector3(0.45f, 0.02f, 0.55f), matMetalBlack);
            CreateBox("Kitchen_InductionCooktop", zoneAGroup.transform, new Vector3(-2.65f, 0.95f, 1.9f), new Vector3(0.42f, 0.02f, 0.65f), matMetalBlack);

            // --- 5. Zone B: Private Bedroom & Ensuite Bathroom (X = 0.0 to +4.0) ---
            GameObject zoneBGroup = CreateSubContainer("3_ZoneB_BedroomAndBathroom");

            // Bedroom Area (X = 0.2 to +2.2, Z = -2.2 to +2.2)
            CreateBox("Bedroom_QueenBedFrame", zoneBGroup.transform, new Vector3(1.15f, 0.2f, -1.0f), new Vector3(1.65f, 0.35f, 2.15f), matOakFlooring);
            CreateBox("Bedroom_Headboard", zoneBGroup.transform, new Vector3(1.15f, 0.65f, -2.02f), new Vector3(1.65f, 0.95f, 0.15f), matSofaFabric);
            CreateBox("Bedroom_Duvet", zoneBGroup.transform, new Vector3(1.15f, 0.48f, -0.9f), new Vector3(1.55f, 0.28f, 1.95f), matWallPlaster);
            CreateBox("Bedroom_NightstandLeft", zoneBGroup.transform, new Vector3(0.18f, 0.25f, -1.95f), new Vector3(0.38f, 0.45f, 0.42f), matOakFlooring);
            CreateBox("Bedroom_NightstandRight", zoneBGroup.transform, new Vector3(2.12f, 0.25f, -1.95f), new Vector3(0.38f, 0.45f, 0.42f), matOakFlooring);

            // Ensuite Bathroom Interior Partition Wall (X = 2.2, Z = 0.2 to 2.5 and Z = 0.2, X = 2.2 to 4.0)
            float bathDoorW = 0.9f;
            float bathDoorH = 2.1f;
            float bathWallWestL = (2.3f - bathDoorW); // Wall segment next to bathroom door
            CreateBox("Bath_Partition_West", zoneBGroup.transform, new Vector3(2.2f, roomHeight * 0.5f, 1.75f), new Vector3(wallThickness, roomHeight, 1.5f), matWallPlaster);
            CreateBox("Bath_Partition_South", zoneBGroup.transform, new Vector3(3.1f, roomHeight * 0.5f, 0.2f), new Vector3(1.8f, roomHeight, wallThickness), matWallPlaster);
            CreateBox("Bath_DoorHeader", zoneBGroup.transform, new Vector3(2.2f, bathDoorH + (roomHeight - bathDoorH) * 0.5f, 0.65f), new Vector3(wallThickness, roomHeight - bathDoorH, bathDoorW), matWallPlaster);
            CreateBox("Bathroom_DoorFrame", zoneBGroup.transform, new Vector3(2.2f, bathDoorH * 0.5f, 0.65f), new Vector3(wallThickness + 0.04f, bathDoorH, bathDoorW), matOakFlooring, false);

            // Bathroom Fixtures inside Ensuite Bathroom
            CreateBox("Bathroom_Vanity", zoneBGroup.transform, new Vector3(3.1f, 0.45f, 0.65f), new Vector3(0.5f, 0.45f, 0.85f), matOakFlooring);
            CreateBox("Bathroom_Sink", zoneBGroup.transform, new Vector3(3.1f, 0.72f, 0.65f), new Vector3(0.42f, 0.12f, 0.65f), matWhiteCeramic);
            CreateBox("Bathroom_Toilet", zoneBGroup.transform, new Vector3(3.55f, 0.25f, 1.4f), new Vector3(0.42f, 0.45f, 0.65f), matWhiteCeramic);
            CreateBox("Bathroom_WalkInShowerGlass", zoneBGroup.transform, new Vector3(3.1f, 1.15f, 2.05f), new Vector3(1.6f, 2.3f, 0.04f), matGlassClear);

            Debug.Log("[DualZoneStudioGenerator] Dual-Zone Studio Apartment successfully generated!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_StudioContainer.transform, false);
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

            if (matOakFlooring == null) matOakFlooring = CreateMat(litShader, "Mat_Oak_Flooring", new Color(0.72f, 0.54f, 0.36f), 0.45f, 0.05f);
            if (matKitchenTile == null) matKitchenTile = CreateMat(litShader, "Mat_Tile_Kitchen", new Color(0.52f, 0.55f, 0.58f), 0.4f, 0.1f);
            if (matBathroomTile == null) matBathroomTile = CreateMat(litShader, "Mat_Tile_Bathroom", new Color(0.75f, 0.78f, 0.80f), 0.35f, 0.05f);
            if (matWallPlaster == null) matWallPlaster = CreateMat(litShader, "Mat_Wall_Plaster", new Color(0.92f, 0.92f, 0.90f), 0.85f, 0.0f);
            if (matDividingWall == null) matDividingWall = CreateMat(litShader, "Mat_DividingWall", new Color(0.88f, 0.86f, 0.82f), 0.85f, 0.0f);
            if (matSofaFabric == null) matSofaFabric = CreateMat(litShader, "Mat_Sofa_Fabric", new Color(0.32f, 0.35f, 0.38f), 0.9f, 0.0f);
            if (matMetalBlack == null) matMetalBlack = CreateMat(litShader, "Mat_Metal_Black", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
            if (matWhiteCeramic == null) matWhiteCeramic = CreateMat(litShader, "Mat_White_Ceramic", new Color(0.96f, 0.96f, 0.96f), 0.15f, 0.1f);
            if (matGlassClear == null) matGlassClear = CreateMat(litShader, "Mat_Glass_Clear", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
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
