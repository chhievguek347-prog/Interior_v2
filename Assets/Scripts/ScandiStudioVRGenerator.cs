using System.Collections.Generic;
using UnityEngine;

namespace Interior.ScandiVR
{
    public class ScandiStudioVRGenerator : MonoBehaviour
    {
        [Header("Studio Room Dimensions (Real-World Scale)")]
        public float roomWidth = 6.0f;   // X axis (-3 to +3)
        public float roomLength = 8.0f;  // Z axis (-4 to +4)
        public float roomHeight = 2.8f;  // Y axis (0 to 2.8)
        public float wallThickness = 0.18f;

        [Header("Scandinavian PBR Material Slots")]
        public Material matLightOak;
        public Material matWhiteWall;
        public Material matMatteBlack;
        public Material matGrayStone;
        public Material matCeramicWhite;
        public Material matBeigeFabric;
        public Material matOliveGreen;
        public Material matFrostedGlass;
        public Material matGrayTile;
        public Material matRugFabric;
        public Material matClearGlass;

        [Header("Generation Options")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_RootContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateGameReadyScandiStudio();
            }
        }

        [ContextMenu("Generate Game-Ready Scandi VR Studio")]
        public void GenerateGameReadyScandiStudio()
        {
            Transform existing = transform.Find("GameReadyScandiStudio");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_RootContainer = new GameObject("GameReadyScandiStudio");
            m_RootContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. Architecture (Floor, Ceiling, Walls, Bedroom Window) ---
            GameObject archRoot = CreateSubContainer("1_Architecture");

            // Main Floor (Light Oak Wood)
            CreateModularBox("Floor_LightOak", archRoot.transform, new Vector3(0, -0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matLightOak);

            // Ceiling (Clean White)
            CreateModularBox("Ceiling_White", archRoot.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWhiteWall);

            // Exterior Walls
            CreateModularBox("Wall_Left", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matWhiteWall);
            CreateModularBox("Wall_Right", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matWhiteWall);

            // Front Entrance Wall (Z = -halfL = -4.0) with Entrance Doorway Cutout
            float doorW = 1.1f;
            float doorH = 2.1f;
            float frontSideW = (roomWidth - doorW) * 0.5f;
            CreateModularBox("Wall_Front_Left", archRoot.transform, new Vector3(-halfW + frontSideW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontSideW, roomHeight, wallThickness), matWhiteWall);
            CreateModularBox("Wall_Front_Right", archRoot.transform, new Vector3(halfW - frontSideW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontSideW, roomHeight, wallThickness), matWhiteWall);
            CreateModularBox("Wall_Front_DoorHeader", archRoot.transform, new Vector3(0, doorH + (roomHeight - doorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorW, roomHeight - doorH, wallThickness), matWhiteWall);

            // Back Bedroom Wall with Large Window Above Bed (Z = +halfL = +4.0)
            float winW = 3.2f;
            float winSillH = 1.0f;
            float winTopH = 2.6f;
            float winH = winTopH - winSillH;
            float backSideW = (roomWidth - winW) * 0.5f;
            CreateModularBox("Wall_Back_Left", archRoot.transform, new Vector3(-halfW + backSideW * 0.5f, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(backSideW, roomHeight, wallThickness), matWhiteWall);
            CreateModularBox("Wall_Back_Right", archRoot.transform, new Vector3(halfW - backSideW * 0.5f, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(backSideW, roomHeight, wallThickness), matWhiteWall);
            CreateModularBox("Wall_Back_Sill", archRoot.transform, new Vector3(0, winSillH * 0.5f, halfL + wallThickness * 0.5f), new Vector3(winW, winSillH, wallThickness), matWhiteWall);
            CreateModularBox("Wall_Back_Header", archRoot.transform, new Vector3(0, winTopH + (roomHeight - winTopH) * 0.5f, halfL + wallThickness * 0.5f), new Vector3(winW, roomHeight - winTopH, wallThickness), matWhiteWall);
            // Bedroom Window Frame & Glass
            CreateModularBox("Bedroom_WindowFrame", archRoot.transform, new Vector3(0, winSillH + winH * 0.5f, halfL + wallThickness * 0.5f), new Vector3(winW, winH, 0.08f), matMatteBlack, false);
            CreateModularBox("Bedroom_WindowGlass", archRoot.transform, new Vector3(0, winSillH + winH * 0.5f, halfL + wallThickness * 0.5f), new Vector3(winW - 0.1f, winH - 0.1f, 0.02f), matClearGlass, false);

            // --- 2. Bathroom Module (Upper Left: X = -3.0 to -1.0, Z = 1.0 to 4.0) ---
            GameObject bathRoot = CreateSubContainer("2_Bathroom_Module");
            CreateModularBox("Bath_Partition_East", bathRoot.transform, new Vector3(-1.0f, roomHeight * 0.5f, 2.5f), new Vector3(wallThickness, roomHeight, 3.0f), matWhiteWall);
            CreateModularBox("Bath_Partition_South", bathRoot.transform, new Vector3(-2.0f, roomHeight * 0.5f, 1.0f), new Vector3(2.0f, roomHeight, wallThickness), matWhiteWall);
            CreateModularBox("Bath_FloorTiles", bathRoot.transform, new Vector3(-2.0f, 0.01f, 2.5f), new Vector3(1.95f, 0.02f, 2.95f), matGrayTile);
            // Frosted Glass Shower Enclosure with Rain Shower Head
            CreateModularBox("Shower_GlassEnclosure", bathRoot.transform, new Vector3(-1.95f, 1.1f, 3.2f), new Vector3(0.04f, 2.2f, 1.2f), matFrostedGlass);
            CreateModularBox("Shower_RainHead", bathRoot.transform, new Vector3(-2.45f, 2.35f, 3.3f), new Vector3(0.28f, 0.05f, 0.28f), matMatteBlack);
            // White Ceramic Toilet
            CreateModularBox("Toilet_CeramicBase", bathRoot.transform, new Vector3(-2.45f, 0.22f, 1.4f), new Vector3(0.42f, 0.44f, 0.65f), matCeramicWhite);
            CreateModularBox("Toilet_CeramicTank", bathRoot.transform, new Vector3(-2.45f, 0.5f, 1.12f), new Vector3(0.44f, 0.52f, 0.22f), matCeramicWhite);
            // Floating Wooden Vanity & White Sink
            CreateModularBox("Floating_WoodenVanity", bathRoot.transform, new Vector3(-1.35f, 0.45f, 1.55f), new Vector3(0.5f, 0.45f, 0.8f), matLightOak);
            CreateModularBox("Vanity_WhiteSink", bathRoot.transform, new Vector3(-1.35f, 0.72f, 1.55f), new Vector3(0.42f, 0.12f, 0.65f), matCeramicWhite);
            CreateModularBox("Vanity_Mirror", bathRoot.transform, new Vector3(-1.08f, 1.45f, 1.55f), new Vector3(0.04f, 0.8f, 0.7f), matMatteBlack);

            // --- 3. Kitchen Module (Single-Wall Left: X = -3.0 to -1.0, Z = -3.0 to 0.8) ---
            GameObject kitchenRoot = CreateSubContainer("3_Kitchen_Module");
            float kitchZ = -1.25f;
            float kitchL = 3.2f;
            CreateModularBox("Kitchen_LowerCabinets", kitchenRoot.transform, new Vector3(-2.68f, 0.45f, kitchZ), new Vector3(0.64f, 0.9f, kitchL), matLightOak);
            CreateModularBox("Kitchen_GrayStoneCounter", kitchenRoot.transform, new Vector3(-2.68f, 0.92f, kitchZ), new Vector3(0.68f, 0.06f, kitchL + 0.05f), matGrayStone);
            CreateModularBox("Kitchen_StainlessSink", kitchenRoot.transform, new Vector3(-2.68f, 0.95f, -0.4f), new Vector3(0.45f, 0.02f, 0.55f), matMatteBlack);
            CreateModularBox("Kitchen_InductionCooktop", kitchenRoot.transform, new Vector3(-2.68f, 0.95f, -2.1f), new Vector3(0.42f, 0.02f, 0.65f), matMatteBlack);
            CreateModularBox("Kitchen_CompactFridge", kitchenRoot.transform, new Vector3(-2.65f, 0.42f, 0.05f), new Vector3(0.6f, 0.85f, 0.55f), matMatteBlack);
            CreateModularBox("Kitchen_Backsplash", kitchenRoot.transform, new Vector3(-2.95f, 1.35f, kitchZ), new Vector3(0.04f, 0.8f, kitchL), matGrayTile, false);
            CreateModularBox("Kitchen_CountertopPlant", kitchenRoot.transform, new Vector3(-2.65f, 1.05f, 0.8f), new Vector3(0.18f, 0.22f, 0.18f), matOliveGreen);

            // --- 4. Open Wardrobe Closet (Between Bathroom & Bedroom: X = -0.8 to 0.4, Z = 2.4 to 4.0) ---
            GameObject closetRoot = CreateSubContainer("4_OpenCloset_Module");
            CreateModularBox("Wardrobe_Frame_Back", closetRoot.transform, new Vector3(-0.25f, 1.25f, 3.88f), new Vector3(1.1f, 2.3f, 0.08f), matLightOak);
            CreateModularBox("Wardrobe_Frame_Left", closetRoot.transform, new Vector3(-0.78f, 1.25f, 3.25f), new Vector3(0.08f, 2.3f, 1.2f), matLightOak);
            CreateModularBox("Wardrobe_Frame_Right", closetRoot.transform, new Vector3(0.28f, 1.25f, 3.25f), new Vector3(0.08f, 2.3f, 1.2f), matLightOak);
            CreateModularBox("Wardrobe_LowerDrawers", closetRoot.transform, new Vector3(-0.25f, 0.25f, 3.25f), new Vector3(0.98f, 0.45f, 1.15f), matLightOak);
            CreateModularBox("Wardrobe_StorageShelves", closetRoot.transform, new Vector3(-0.25f, 1.6f, 3.25f), new Vector3(0.98f, 0.06f, 1.15f), matLightOak);
            CreateModularBox("Wardrobe_HangingShirts", closetRoot.transform, new Vector3(-0.25f, 1.1f, 3.25f), new Vector3(0.85f, 0.75f, 0.45f), matBeigeFabric, false);
            CreateModularBox("Wardrobe_TopShelfPlant", closetRoot.transform, new Vector3(-0.25f, 2.38f, 3.25f), new Vector3(0.22f, 0.28f, 0.22f), matOliveGreen);

            // --- 5. Bedroom Module (Upper Right: X = 0.5 to 3.0, Z = 0.8 to 4.0) ---
            GameObject bedRoot = CreateSubContainer("5_Bedroom_Module");
            // Queen Platform Bed Frame (Light Oak)
            Vector3 bedPos = new Vector3(1.75f, 0.2f, 2.85f);
            CreateModularBox("Queen_PlatformBed_Frame", bedRoot.transform, bedPos, new Vector3(1.65f, 0.35f, 2.15f), matLightOak);
            CreateModularBox("Queen_WhiteDuvet", bedRoot.transform, new Vector3(1.75f, 0.48f, 2.75f), new Vector3(1.55f, 0.28f, 1.95f), matWhiteWall);
            CreateModularBox("Queen_BeigeBlanket", bedRoot.transform, new Vector3(1.75f, 0.52f, 2.15f), new Vector3(1.58f, 0.16f, 0.85f), matBeigeFabric);
            CreateModularBox("Pillow_1", bedRoot.transform, new Vector3(1.35f, 0.64f, 3.65f), new Vector3(0.65f, 0.14f, 0.38f), matWhiteWall);
            CreateModularBox("Pillow_2", bedRoot.transform, new Vector3(2.15f, 0.64f, 3.65f), new Vector3(0.65f, 0.14f, 0.38f), matWhiteWall);
            CreateModularBox("Pillow_3", bedRoot.transform, new Vector3(1.35f, 0.66f, 3.42f), new Vector3(0.62f, 0.12f, 0.35f), matBeigeFabric);
            CreateModularBox("Pillow_4", bedRoot.transform, new Vector3(2.15f, 0.66f, 3.42f), new Vector3(0.62f, 0.12f, 0.35f), matBeigeFabric);
            CreateModularBox("OliveGreen_AccentCushion", bedRoot.transform, new Vector3(1.75f, 0.68f, 3.35f), new Vector3(0.35f, 0.12f, 0.35f), matOliveGreen);
            // Nightstands & Minimalist Lamps
            CreateModularBox("Nightstand_Left", bedRoot.transform, new Vector3(0.72f, 0.25f, 3.6f), new Vector3(0.42f, 0.48f, 0.45f), matLightOak);
            CreateModularBox("Nightstand_Right", bedRoot.transform, new Vector3(2.78f, 0.25f, 3.6f), new Vector3(0.42f, 0.48f, 0.45f), matLightOak);
            CreateModularBox("BedsideLamp_Left", bedRoot.transform, new Vector3(0.72f, 0.62f, 3.6f), new Vector3(0.16f, 0.28f, 0.16f), matMatteBlack, false);
            CreateModularBox("BedsideLamp_Right", bedRoot.transform, new Vector3(2.78f, 0.62f, 3.6f), new Vector3(0.16f, 0.28f, 0.16f), matMatteBlack, false);

            // --- 6. Living Room Module (Lower Right: X = 0.5 to 3.0, Z = -4.0 to 0.5) ---
            GameObject livingRoot = CreateSubContainer("6_LivingRoom_Module");
            CreateModularBox("Area_Rug", livingRoot.transform, new Vector3(1.75f, 0.01f, -1.8f), new Vector3(2.2f, 0.02f, 2.8f), matRugFabric, false);
            // Two-Seat Beige Fabric Sofa
            Vector3 sofaPos = new Vector3(1.75f, 0.38f, -0.8f);
            CreateModularBox("Sofa_BaseSeat", livingRoot.transform, sofaPos, new Vector3(1.85f, 0.42f, 0.85f), matBeigeFabric);
            CreateModularBox("Sofa_Backrest", livingRoot.transform, new Vector3(1.75f, 0.62f, -0.42f), new Vector3(1.85f, 0.52f, 0.22f), matBeigeFabric);
            // Wooden Coffee Table
            CreateModularBox("Wooden_CoffeeTable", livingRoot.transform, new Vector3(1.75f, 0.22f, -1.8f), new Vector3(1.1f, 0.38f, 0.65f), matLightOak);
            // Floating TV Console with Wall-Mounted Flat-Screen TV
            CreateModularBox("Floating_TVConsole", livingRoot.transform, new Vector3(2.78f, 0.45f, -2.6f), new Vector3(0.38f, 0.35f, 1.8f), matLightOak);
            CreateModularBox("TV_Frame", livingRoot.transform, new Vector3(2.92f, 1.45f, -2.6f), new Vector3(0.06f, 0.82f, 1.45f), matMatteBlack);
            CreateModularBox("TV_Screen", livingRoot.transform, new Vector3(2.88f, 1.45f, -2.6f), new Vector3(0.02f, 0.76f, 1.38f), matMatteBlack, false);
            // Indoor Plants
            CreateModularBox("Indoor_PottedPlant", livingRoot.transform, new Vector3(2.6f, 0.4f, -3.5f), new Vector3(0.35f, 0.8f, 0.35f), matOliveGreen);

            Debug.Log("[ScandiStudioVRGenerator] Game-ready Scandinavian VR Studio Apartment generated successfully!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_RootContainer.transform, false);
            return c;
        }

        private GameObject CreateModularBox(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool addCol = true)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = pos;
            cube.transform.localScale = scale;

            if (mat != null)
            {
                MeshRenderer mr = cube.GetComponent<MeshRenderer>();
                if (mr != null) mr.sharedMaterial = mat;
            }

            if (!addCol || !addPhysicsColliders)
            {
                BoxCollider col = cube.GetComponent<BoxCollider>();
                if (col != null) DestroyImmediate(col);
            }

            return cube;
        }

        private void EnsureMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (matLightOak == null) matLightOak = CreateMat(litShader, "Mat_LightOak", new Color(0.76f, 0.60f, 0.42f), 0.45f, 0.05f);
            if (matWhiteWall == null) matWhiteWall = CreateMat(litShader, "Mat_WhiteWall", new Color(0.94f, 0.94f, 0.92f), 0.85f, 0.0f);
            if (matMatteBlack == null) matMatteBlack = CreateMat(litShader, "Mat_MatteBlack", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
            if (matGrayStone == null) matGrayStone = CreateMat(litShader, "Mat_GrayStone", new Color(0.45f, 0.48f, 0.50f), 0.35f, 0.15f);
            if (matCeramicWhite == null) matCeramicWhite = CreateMat(litShader, "Mat_CeramicWhite", new Color(0.96f, 0.96f, 0.96f), 0.15f, 0.1f);
            if (matBeigeFabric == null) matBeigeFabric = CreateMat(litShader, "Mat_BeigeFabric", new Color(0.82f, 0.76f, 0.68f), 0.9f, 0.0f);
            if (matOliveGreen == null) matOliveGreen = CreateMat(litShader, "Mat_OliveGreen", new Color(0.32f, 0.42f, 0.25f), 0.85f, 0.0f);
            if (matFrostedGlass == null) matFrostedGlass = CreateMat(litShader, "Mat_FrostedGlass", new Color(0.85f, 0.92f, 0.95f, 0.45f), 0.2f, 0.1f);
            if (matGrayTile == null) matGrayTile = CreateMat(litShader, "Mat_GrayTile", new Color(0.65f, 0.68f, 0.70f), 0.4f, 0.05f);
            if (matRugFabric == null) matRugFabric = CreateMat(litShader, "Mat_RugFabric", new Color(0.88f, 0.85f, 0.80f), 0.95f, 0.0f);
            if (matClearGlass == null) matClearGlass = CreateMat(litShader, "Mat_ClearGlass", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
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
