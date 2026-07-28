using System.Collections.Generic;
using UnityEngine;

namespace Interior.VRCustomizer
{
    public class StudioVRLayoutGenerator : MonoBehaviour
    {
        [Header("Studio Room Dimensions (6m x 6m footprint)")]
        public float roomWidth = 6.0f;   // X axis (-3 to +3)
        public float roomLength = 6.0f;  // Z axis (-3 to +3)
        public float roomHeight = 2.7f;  // Y axis (0 to 2.7)
        public float wallThickness = 0.18f;

        [Header("PBR Material Slots")]
        public Material matWallMain;
        public Material matSofaFabric;
        public Material matBedDuvet;
        public Material matWoodOak;
        public Material matBathroomTile;
        public Material matKitchenCounter;
        public Material matGlassIndustrial;
        public Material matMetalBlack;
        public Material matWhiteCeramic;
        public Material matRugJute;
        public Material matGlassClear;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addBoundaryColliders = true;

        private GameObject m_RootContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateVRStudioLayout();
            }
        }

        [ContextMenu("Generate VR Studio Layout")]
        public void GenerateVRStudioLayout()
        {
            Transform existing = transform.Find("VRStudioApartment");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_RootContainer = new GameObject("VRStudioApartment");
            m_RootContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. Architecture & Flooring ---
            GameObject archGroup = CreateSubContainer("1_Architecture");

            // Main Warm Oak Wood Flooring (6m x 6m)
            CreateBox("Floor_WarmOak", archGroup.transform, new Vector3(0, -0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWoodOak);

            // Bathroom Light Gray Tile Flooring Overlay (Upper Right: X = 1.2 to 3.0, Z = 1.2 to 3.0)
            CreateBox("Floor_BathroomTile", archGroup.transform, new Vector3(2.1f, 0.01f, 2.1f), new Vector3(1.78f, 0.02f, 1.78f), matBathroomTile);

            // Ceiling (2.7m height)
            CreateBox("Ceiling_Main", archGroup.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWallMain);

            // Outer Solid Walls
            // Back Wall (Z = +halfL = 3.0)
            CreateBox("Wall_Back", archGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), matWallMain);
            // Right Wall (X = +halfW = 3.0)
            CreateBox("Wall_Right", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matWallMain);
            // Front Entry Wall (Z = -halfL = -3.0) with Door cutout
            float entryDoorW = 1.1f;
            float sideW = (roomWidth - entryDoorW) * 0.5f;
            CreateBox("Wall_Front_Left", archGroup.transform, new Vector3(-halfW + sideW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(sideW, roomHeight, wallThickness), matWallMain);
            CreateBox("Wall_Front_Right", archGroup.transform, new Vector3(halfW - sideW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(sideW, roomHeight, wallThickness), matWallMain);
            CreateBox("Wall_Front_Header", archGroup.transform, new Vector3(0, 2.1f + (roomHeight - 2.1f) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(entryDoorW, roomHeight - 2.1f, wallThickness), matWallMain);

            // Left Wall with Balcony Sliding Glass Door Opening (X = -halfW = -3.0)
            float balconyDoorW = 2.4f;
            float leftWallSide = (roomLength - balconyDoorW) * 0.5f;
            CreateBox("Wall_Left_South", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, -halfL + leftWallSide * 0.5f), new Vector3(wallThickness, roomHeight, leftWallSide), matWallMain);
            CreateBox("Wall_Left_North", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, halfL - leftWallSide * 0.5f), new Vector3(wallThickness, roomHeight, leftWallSide), matWallMain);
            CreateBox("Wall_Left_Header", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, 2.3f + (roomHeight - 2.3f) * 0.5f, 0.4f), new Vector3(wallThickness, roomHeight - 2.3f, balconyDoorW), matWallMain);

            // Sliding Glass Balcony Door Frame & Glass
            CreateBox("Balcony_DoorFrame", archGroup.transform, new Vector3(-halfW, 1.15f, 0.4f), new Vector3(0.08f, 2.3f, balconyDoorW), matMetalBlack, false);
            CreateBox("Balcony_DoorGlass", archGroup.transform, new Vector3(-halfW, 1.15f, 0.4f), new Vector3(0.02f, 2.2f, balconyDoorW - 0.1f), matGlassClear, false);

            // Balcony Extension (Outside Sliding Door: X = -3.8, Z = 0.4, Size = 1.6m x 2.4m)
            CreateBox("Balcony_Floor", archGroup.transform, new Vector3(-3.8f, -0.05f, 0.4f), new Vector3(1.6f, 0.1f, 2.4f), matBathroomTile);
            CreateBox("Balcony_Railing_West", archGroup.transform, new Vector3(-4.55f, 0.55f, 0.4f), new Vector3(0.06f, 1.1f, 2.4f), matMetalBlack);
            CreateBox("Balcony_Railing_South", archGroup.transform, new Vector3(-3.8f, 0.55f, -0.75f), new Vector3(1.5f, 1.1f, 0.06f), matMetalBlack);
            CreateBox("Balcony_Railing_North", archGroup.transform, new Vector3(-3.8f, 0.55f, 1.55f), new Vector3(1.5f, 1.1f, 0.06f), matMetalBlack);
            // Bistro Table & 2 Chairs on Balcony
            CreateBox("Balcony_BistroTable", archGroup.transform, new Vector3(-3.8f, 0.38f, 0.4f), new Vector3(0.65f, 0.72f, 0.65f), matMetalBlack);
            CreateBox("Balcony_Chair1", archGroup.transform, new Vector3(-3.8f, 0.25f, -0.1f), new Vector3(0.4f, 0.8f, 0.4f), matMetalBlack);
            CreateBox("Balcony_Chair2", archGroup.transform, new Vector3(-3.8f, 0.25f, 0.9f), new Vector3(0.4f, 0.8f, 0.4f), matMetalBlack);

            // --- 2. Zone 1: Entry / Utility Alcove (Z = -3.0, X = -3.0 to -1.2) ---
            GameObject entryGroup = CreateSubContainer("2_EntryUtilityNook");
            // Stacked Washer / Dryer in alcove
            CreateBox("WasherDryer_LowerUnit", entryGroup.transform, new Vector3(-2.45f, 0.45f, -2.45f), new Vector3(0.68f, 0.88f, 0.68f), matMetalBlack);
            CreateBox("WasherDryer_UpperUnit", entryGroup.transform, new Vector3(-2.45f, 1.35f, -2.45f), new Vector3(0.68f, 0.88f, 0.68f), matMetalBlack);
            CreateBox("Utility_SlidingDoorFrame", entryGroup.transform, new Vector3(-2.45f, 1.0f, -2.08f), new Vector3(0.75f, 2.0f, 0.04f), matWoodOak);
            // Slim Storage Cabinet beside alcove
            CreateBox("Entry_SlimCabinet", entryGroup.transform, new Vector3(-1.65f, 0.95f, -2.45f), new Vector3(0.45f, 1.9f, 0.45f), matWoodOak);

            // --- 3. Zone 2: Kitchen & Dining Nook (X = -3.0 to -0.8, Z = -1.2 to 1.5) ---
            GameObject kitchenGroup = CreateSubContainer("3_KitchenAndDining");
            // L-Shaped Kitchen Counter (Along left wall and extending inward)
            CreateBox("Kitchen_LowerCabinets_Main", kitchenGroup.transform, new Vector3(-2.65f, 0.45f, -0.3f), new Vector3(0.65f, 0.9f, 2.2f), matWoodOak);
            CreateBox("Kitchen_LowerCabinets_LReturn", kitchenGroup.transform, new Vector3(-2.0f, 0.45f, -1.2f), new Vector3(0.8f, 0.9f, 0.65f), matWoodOak);
            CreateBox("Kitchen_Countertop_L", kitchenGroup.transform, new Vector3(-2.4f, 0.92f, -0.3f), new Vector3(1.15f, 0.06f, 2.25f), matKitchenCounter);
            // Undermount Sink & Induction Cooktop
            CreateBox("Kitchen_Sink", kitchenGroup.transform, new Vector3(-2.65f, 0.95f, -0.6f), new Vector3(0.45f, 0.02f, 0.55f), matMetalBlack);
            CreateBox("Kitchen_InductionCooktop", kitchenGroup.transform, new Vector3(-2.65f, 0.95f, 0.4f), new Vector3(0.42f, 0.02f, 0.65f), matMetalBlack);
            // Upper Cabinets & Open Shelving
            CreateBox("Kitchen_UpperCabinets", kitchenGroup.transform, new Vector3(-2.68f, 2.1f, -0.3f), new Vector3(0.4f, 0.75f, 1.8f), matWoodOak);
            CreateBox("Kitchen_OpenShelf", kitchenGroup.transform, new Vector3(-2.65f, 1.5f, 0.4f), new Vector3(0.35f, 0.04f, 0.8f), matWoodOak);
            // Tiled Backsplash
            CreateBox("Kitchen_Backsplash", kitchenGroup.transform, new Vector3(-2.92f, 1.35f, -0.3f), new Vector3(0.04f, 0.8f, 2.2f), matBathroomTile, false);
            // Island / Counter Pendant Light
            CreateBox("Kitchen_PendantLight", kitchenGroup.transform, new Vector3(-1.95f, 2.1f, -0.3f), new Vector3(0.22f, 0.35f, 0.22f), matMetalBlack, false);

            // Dining Nook (Near balcony door)
            CreateBox("Dining_WoodTable", kitchenGroup.transform, new Vector3(-1.65f, 0.38f, 0.4f), new Vector3(0.85f, 0.74f, 0.85f), matWoodOak);
            CreateBox("Dining_RattanChair1", kitchenGroup.transform, new Vector3(-1.65f, 0.25f, -0.15f), new Vector3(0.45f, 0.82f, 0.45f), matWoodOak);
            CreateBox("Dining_RattanChair2", kitchenGroup.transform, new Vector3(-1.65f, 0.25f, 0.95f), new Vector3(0.45f, 0.82f, 0.45f), matWoodOak);

            // --- 4. Zone 3: Living Area (Lower Right: X = 0.0 to 3.0, Z = -3.0 to 0.0) ---
            GameObject livingGroup = CreateSubContainer("4_LivingArea");
            // Round Jute Rug
            CreateBox("Living_RoundJuteRug", livingGroup.transform, new Vector3(1.5f, 0.01f, -1.5f), new Vector3(2.4f, 0.02f, 2.4f), matRugJute, false);
            // L-Shaped Sofa (Main swappable furniture: matSofaFabric)
            CreateBox("Sofa_MainBench", livingGroup.transform, new Vector3(1.5f, 0.35f, -0.6f), new Vector3(2.0f, 0.42f, 0.85f), matSofaFabric);
            CreateBox("Sofa_LReturnBench", livingGroup.transform, new Vector3(2.1f, 0.35f, -1.25f), new Vector3(0.8f, 0.42f, 0.85f), matSofaFabric);
            CreateBox("Sofa_Backrest", livingGroup.transform, new Vector3(1.5f, 0.62f, -0.22f), new Vector3(2.0f, 0.52f, 0.22f), matSofaFabric);
            // Round Wood Coffee Table
            CreateBox("Living_RoundCoffeeTable", livingGroup.transform, new Vector3(1.3f, 0.22f, -1.5f), new Vector3(0.75f, 0.38f, 0.75f), matWoodOak);
            // Low Media Console with Mounted TV along Front Right Wall
            CreateBox("Living_MediaConsole", livingGroup.transform, new Vector3(1.5f, 0.25f, -2.68f), new Vector3(1.6f, 0.38f, 0.42f), matWoodOak);
            CreateBox("Living_MountedTVFrame", livingGroup.transform, new Vector3(1.5f, 1.35f, -2.92f), new Vector3(1.3f, 0.75f, 0.06f), matMetalBlack);
            // Potted Corner Plants
            CreateBox("Living_CornerPlant", livingGroup.transform, new Vector3(2.6f, 0.4f, -2.6f), new Vector3(0.38f, 0.85f, 0.38f), matWoodOak);

            // --- 5. Zone 4: Bedroom & Industrial Glass Bathroom (Upper Right: X = 0.0 to 3.0, Z = 0.0 to 3.0) ---
            GameObject bedGroup = CreateSubContainer("5_BedroomAndEnsuite");
            // Queen Bed with Upholstered Headboard
            Vector3 bedPos = new Vector3(0.85f, 0.22f, 2.05f);
            CreateBox("QueenBed_PlatformFrame", bedGroup.transform, bedPos, new Vector3(1.65f, 0.35f, 2.15f), matWoodOak);
            CreateBox("QueenBed_UpholsteredHeadboard", bedGroup.transform, new Vector3(0.85f, 0.65f, 3.02f), new Vector3(1.65f, 0.95f, 0.15f), matSofaFabric);
            CreateBox("QueenBed_Duvet", bedGroup.transform, new Vector3(0.85f, 0.48f, 1.95f), new Vector3(1.55f, 0.28f, 1.95f), matBedDuvet);
            CreateBox("QueenBed_LayeredPillows", bedGroup.transform, new Vector3(0.85f, 0.64f, 2.75f), new Vector3(1.45f, 0.16f, 0.45f), matBedDuvet);
            // Storage Bench at foot of bed
            CreateBox("QueenBed_FootBench", bedGroup.transform, new Vector3(0.85f, 0.22f, 0.75f), new Vector3(1.45f, 0.35f, 0.42f), matWoodOak);
            // Nightstands & Hanging Pendant Lights
            CreateBox("Nightstand_Left", bedGroup.transform, new Vector3(-0.15f, 0.25f, 2.85f), new Vector3(0.38f, 0.45f, 0.42f), matWoodOak);
            CreateBox("Nightstand_Right", bedGroup.transform, new Vector3(1.85f, 0.25f, 2.85f), new Vector3(0.38f, 0.45f, 0.42f), matWoodOak);
            CreateBox("BedPendantLight_L", bedGroup.transform, new Vector3(-0.15f, 1.8f, 2.85f), new Vector3(0.16f, 0.3f, 0.16f), matMetalBlack, false);
            CreateBox("BedPendantLight_R", bedGroup.transform, new Vector3(1.85f, 1.8f, 2.85f), new Vector3(0.16f, 0.3f, 0.16f), matMetalBlack, false);

            // Ensuite Bathroom separated by Black-Framed Industrial Glass Partition Walls
            // Glass Partition Wall West (X = 1.2, Z = 1.2 to 3.0)
            CreateBox("Ensuite_GlassWall_West", bedGroup.transform, new Vector3(1.2f, roomHeight * 0.5f, 2.1f), new Vector3(0.06f, roomHeight, 1.8f), matGlassIndustrial);
            // Glass Partition Wall South (Z = 1.2, X = 1.2 to 3.0)
            CreateBox("Ensuite_GlassWall_South", bedGroup.transform, new Vector3(2.1f, roomHeight * 0.5f, 1.2f), new Vector3(1.8f, roomHeight, 0.06f), matGlassIndustrial);
            // Walk-in Shower Enclosure & Floating Vanity inside Bathroom
            CreateBox("Bathroom_WalkInShowerGlass", bedGroup.transform, new Vector3(2.1f, 1.15f, 2.75f), new Vector3(1.6f, 2.3f, 0.04f), matGlassClear);
            CreateBox("Bathroom_FloatingVanity", bedGroup.transform, new Vector3(2.45f, 0.45f, 1.65f), new Vector3(0.48f, 0.45f, 0.85f), matWoodOak);
            CreateBox("Bathroom_Sink", bedGroup.transform, new Vector3(2.45f, 0.72f, 1.65f), new Vector3(0.42f, 0.12f, 0.65f), matWhiteCeramic);
            CreateBox("Bathroom_Mirror", bedGroup.transform, new Vector3(2.92f, 1.45f, 1.65f), new Vector3(0.04f, 0.8f, 0.7f), matMetalBlack);

            Debug.Log("[StudioVRLayoutGenerator] VR Studio Apartment (6m x 6m) successfully generated!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_RootContainer.transform, false);
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

            if (!addCol || !addBoundaryColliders)
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

            if (matWallMain == null) matWallMain = CreateMat(litShader, "Mat_Wall_Main", new Color(0.92f, 0.88f, 0.82f), 0.85f, 0.0f); // Soft Warm Sand
            if (matSofaFabric == null) matSofaFabric = CreateMat(litShader, "Mat_Sofa_Fabric", new Color(0.85f, 0.80f, 0.72f), 0.9f, 0.0f); // Beige Cream
            if (matBedDuvet == null) matBedDuvet = CreateMat(litShader, "Mat_Bed_Duvet", new Color(0.95f, 0.95f, 0.95f), 0.9f, 0.0f); // White Linen
            if (matWoodOak == null) matWoodOak = CreateMat(litShader, "Mat_Wood_Oak", new Color(0.72f, 0.54f, 0.36f), 0.45f, 0.05f); // Warm Oak
            if (matBathroomTile == null) matBathroomTile = CreateMat(litShader, "Mat_Bathroom_Tile", new Color(0.75f, 0.78f, 0.80f), 0.35f, 0.05f); // Light Gray Tile
            if (matKitchenCounter == null) matKitchenCounter = CreateMat(litShader, "Mat_Kitchen_Counter", new Color(0.38f, 0.40f, 0.42f), 0.35f, 0.15f);
            if (matGlassIndustrial == null) matGlassIndustrial = CreateMat(litShader, "Mat_Glass_Industrial", new Color(0.12f, 0.12f, 0.14f, 0.85f), 0.2f, 0.8f);
            if (matMetalBlack == null) matMetalBlack = CreateMat(litShader, "Mat_Metal_Black", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
            if (matWhiteCeramic == null) matWhiteCeramic = CreateMat(litShader, "Mat_White_Ceramic", new Color(0.96f, 0.96f, 0.96f), 0.15f, 0.1f);
            if (matRugJute == null) matRugJute = CreateMat(litShader, "Mat_Rug_Jute", new Color(0.78f, 0.70f, 0.58f), 0.95f, 0.0f);
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
