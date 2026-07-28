using System.Collections.Generic;
using UnityEngine;

namespace Interior.Scandi
{
    public class ScandiStudioGenerator : MonoBehaviour
    {
        [Header("Organized Studio Room Dimensions (Spacious 85m² Layout)")]
        public float roomWidth = 8.5f;   // X axis (-4.25 to +4.25)
        public float roomLength = 10.0f; // Z axis (-5.0 to +5.0)
        public float roomHeight = 3.0f;  // Y axis (0 to 3.0)
        public float wallThickness = 0.2f;

        [Header("Scandinavian & Luxury PBR Materials")]
        public Material matLightOak;
        public Material matWhiteWall;
        public Material matAccentWall;
        public Material matMatteBlack;
        public Material matGrayStone;
        public Material matCeramicWhite;
        public Material matBeigeFabric;
        public Material matOliveGreenFabric;
        public Material matFrostedGlass;
        public Material matGrayTile;
        public Material matRugFabric;
        public Material matClearGlass;
        public Material matTvScreen;

        [Header("Generation Options")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_ApartmentContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateStudioApartment();
            }
        }

        [ContextMenu("Generate Organized Spacious Studio")]
        public void GenerateStudioApartment()
        {
            // Clear existing geometry
            Transform existing = transform.Find("ScandinavianStudio");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_ApartmentContainer = new GameObject("ScandinavianStudio");
            m_ApartmentContainer.transform.SetParent(transform, false);

            EnsurePBRMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. ARCHITECTURE (Floor, Ceiling, Outer Shell Walls, Windows) ---
            GameObject archRoot = CreateSubContainer("1_Architecture");

            // Main Floor (Natural Wood Planks)
            CreateMeshBox("Floor_WoodPlank", archRoot.transform, new Vector3(0, -0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matLightOak);

            // Ceiling (Clean Off-White)
            CreateMeshBox("Ceiling_White", archRoot.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWhiteWall);

            // Exterior Walls
            // Left Exterior Wall (Solid White)
            CreateMeshBox("Wall_Left", archRoot.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matWhiteWall);

            // Back Wall with Feature Accent Wood Slat Panel behind Bed
            CreateMeshBox("Wall_Back", archRoot.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), matWhiteWall);
            CreateMeshBox("Wall_Back_AccentPanel", archRoot.transform, new Vector3(1.8f, roomHeight * 0.5f, halfL - 0.02f), new Vector3(4.2f, roomHeight, 0.04f), matAccentWall);

            // Front Entrance Wall (Z = -halfL) with Doorway Cutout
            float doorW = 1.2f;
            float doorH = 2.2f;
            float doorX = -1.5f;
            float frontLeftW = (doorX - doorW * 0.5f) - (-halfW);
            float frontRightW = halfW - (doorX + doorW * 0.5f);
            CreateMeshBox("Wall_Front_Left", archRoot.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matWhiteWall);
            CreateMeshBox("Wall_Front_Right", archRoot.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matWhiteWall);
            CreateMeshBox("Wall_Front_DoorHeader", archRoot.transform, new Vector3(doorX, doorH + (roomHeight - doorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorW, roomHeight - doorH, wallThickness), matWhiteWall);

            // Right Wall with Panoramic Windows
            float winSillH = 0.5f;
            float winTopH = 2.7f;
            float winH = winTopH - winSillH;
            CreateMeshBox("Wall_Right_Sill", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, roomLength), matWhiteWall);
            CreateMeshBox("Wall_Right_Header", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, roomLength), matWhiteWall);
            CreateMeshBox("Wall_Right_Frame", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.08f, winH, roomLength - 0.8f), matMatteBlack, false);
            CreateMeshBox("Wall_Right_Glass", archRoot.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.02f, winH - 0.1f, roomLength - 0.9f), matClearGlass, false);

            // --- 2. ZONED INTERIOR PARTITIONS ---
            GameObject partitionRoot = CreateSubContainer("2_Partitions");
            // Bathroom Enclosure (Top Left: 2.4m x 2.4m)
            CreateMeshBox("Bath_Partition_South", partitionRoot.transform, new Vector3(-3.05f, roomHeight * 0.5f, 2.6f), new Vector3(2.4f, roomHeight, wallThickness), matWhiteWall);
            CreateMeshBox("Bath_Partition_East", partitionRoot.transform, new Vector3(-1.85f, roomHeight * 0.5f, 3.8f), new Vector3(wallThickness, roomHeight, 2.4f), matWhiteWall);

            // Closet Partition Wall (Top Middle)
            CreateMeshBox("Closet_BackWall", partitionRoot.transform, new Vector3(-0.9f, roomHeight * 0.5f, 4.9f), new Vector3(1.8f, roomHeight, wallThickness), matWhiteWall);
            CreateMeshBox("Closet_Divider_Right", partitionRoot.transform, new Vector3(0.0f, roomHeight * 0.5f, 3.8f), new Vector3(wallThickness, roomHeight, 2.4f), matAccentWall);

            // Acoustic Wood Slat Wall between Bedroom & Living Area (Z = 1.0)
            CreateMeshBox("Partition_BedroomSlatWall", partitionRoot.transform, new Vector3(2.35f, 1.3f, 1.0f), new Vector3(3.7f, 2.6f, 0.12f), matAccentWall);

            // --- 3. SPA BATHROOM MODULE (Top Left: 2.4m x 2.4m) ---
            GameObject bathRoot = CreateSubContainer("3_Bathroom");
            CreateMeshBox("Bath_FloorTiles", bathRoot.transform, new Vector3(-3.05f, 0.01f, 3.8f), new Vector3(2.35f, 0.02f, 2.35f), matGrayTile);
            CreateMeshBox("Shower_GlassWall_E", bathRoot.transform, new Vector3(-3.05f, 1.15f, 4.9f), new Vector3(0.04f, 2.2f, 1.1f), matFrostedGlass);
            CreateMeshBox("Shower_GlassWall_S", bathRoot.transform, new Vector3(-3.6f, 1.15f, 4.35f), new Vector3(1.1f, 0.04f, 0.04f), matFrostedGlass);
            CreateMeshBox("Shower_RainHead", bathRoot.transform, new Vector3(-3.6f, 2.3f, 4.6f), new Vector3(0.3f, 0.05f, 0.3f), matMatteBlack);
            CreateMeshBox("Vanity_Cabinet", bathRoot.transform, new Vector3(-2.3f, 0.45f, 3.0f), new Vector3(0.55f, 0.45f, 0.9f), matLightOak);
            CreateMeshBox("Vanity_Sink", bathRoot.transform, new Vector3(-2.3f, 0.72f, 3.0f), new Vector3(0.45f, 0.12f, 0.75f), matCeramicWhite);
            CreateMeshBox("Vanity_MirrorLED", bathRoot.transform, new Vector3(-1.9f, 1.45f, 3.0f), new Vector3(0.04f, 0.85f, 0.8f), matMatteBlack);
            CreateMeshBox("Toilet_Base", bathRoot.transform, new Vector3(-3.7f, 0.24f, 3.0f), new Vector3(0.44f, 0.45f, 0.68f), matCeramicWhite);
            CreateMeshBox("Toilet_Tank", bathRoot.transform, new Vector3(-4.0f, 0.52f, 3.0f), new Vector3(0.22f, 0.55f, 0.45f), matCeramicWhite);

            // --- 4. CLOSET SUITE MODULE (Top Center: 1.8m x 2.4m) ---
            GameObject closetRoot = CreateSubContainer("4_Closet");
            CreateMeshBox("Closet_CabinetFrame", closetRoot.transform, new Vector3(-0.9f, 1.35f, 4.75f), new Vector3(1.7f, 2.5f, 0.45f), matLightOak);
            CreateMeshBox("Closet_SlidingDoor_L", closetRoot.transform, new Vector3(-1.3f, 1.3f, 4.5f), new Vector3(0.85f, 2.4f, 0.04f), matFrostedGlass);
            CreateMeshBox("Closet_SlidingDoor_R", closetRoot.transform, new Vector3(-0.5f, 1.3f, 4.54f), new Vector3(0.85f, 2.4f, 0.04f), matFrostedGlass);
            CreateMeshBox("Closet_DressingMirror", closetRoot.transform, new Vector3(-0.05f, 1.35f, 3.6f), new Vector3(0.04f, 1.8f, 0.65f), matMatteBlack);

            // --- 5. MASTER BEDROOM SUITE MODULE (Top Right: 4.25m x 4.0m) ---
            GameObject bedRoot = CreateSubContainer("5_Bedroom");
            CreateMeshBox("Bed_Headboard", bedRoot.transform, new Vector3(2.1f, 0.65f, 4.85f), new Vector3(2.1f, 1.1f, 0.15f), matAccentWall);
            CreateMeshBox("Bed_PlatformBase", bedRoot.transform, new Vector3(2.1f, 0.22f, 3.7f), new Vector3(1.9m, 0.38f, 2.2m), matLightOak);
            CreateMeshBox("Bed_Mattress", bedRoot.transform, new Vector3(2.1f, 0.48f, 3.65f), new Vector3(1.8m, 0.24f, 2.1m), matCeramicWhite);
            CreateMeshBox("Bed_WhiteDuvet", bedRoot.transform, new Vector3(2.1f, 0.56f, 3.35f), new Vector3(1.78m, 0.14f, 1.6m), matCeramicWhite);
            CreateMeshBox("Bed_BeigeBlanket", bedRoot.transform, new Vector3(2.1f, 0.61f, 2.75f), new Vector3(1.8m, 0.08f, 0.75f), matBeigeFabric);
            CreateMeshBox("Pillow_1", bedRoot.transform, new Vector3(1.55f, 0.65f, 4.5f), new Vector3(0.7f, 0.14f, 0.45f), matCeramicWhite);
            CreateMeshBox("Pillow_2", bedRoot.transform, new Vector3(2.65f, 0.65f, 4.5f), new Vector3(0.7f, 0.14f, 0.45f), matCeramicWhite);
            CreateMeshBox("Nightstand_Left", bedRoot.transform, new Vector3(0.75f, 0.25f, 4.65f), new Vector3(0.48f, 0.48f, 0.48f), matLightOak);
            CreateMeshBox("Nightstand_Right", bedRoot.transform, new Vector3(3.45f, 0.25f, 4.65f), new Vector3(0.48f, 0.48f, 0.48f), matLightOak);
            CreateMeshBox("PendantLamp_Left", bedRoot.transform, new Vector3(0.75f, 1.6f, 4.65f), new Vector3(0.18f, 0.32f, 0.18f), matMatteBlack);
            CreateMeshBox("PendantLamp_Right", bedRoot.transform, new Vector3(3.45f, 1.6f, 4.65f), new Vector3(0.18f, 0.32f, 0.18f), matMatteBlack);
            CreateMeshBox("Bed_AreaRug", bedRoot.transform, new Vector3(2.1f, 0.01f, 3.2f), new Vector3(2.6m, 0.015f, 2.8m), matRugFabric, false);

            // --- 6. GOURMET KITCHENETTE MODULE (Middle Left: 3.2m x 0.65m + Island) ---
            GameObject kitchenRoot = CreateSubContainer("6_Kitchenette");
            float kitchX = -3.9f;
            float kitchZ = -0.5f;
            float kitchL = 3.2f;
            CreateMeshBox("Kitchen_BaseCabinets", kitchenRoot.transform, new Vector3(kitchX, 0.45f, kitchZ), new Vector3(0.65f, 0.9f, kitchL), matLightOak);
            CreateMeshBox("Kitchen_MarbleCounter", kitchenRoot.transform, new Vector3(kitchX, 0.92f, kitchZ), new Vector3(0.68f, 0.05f, kitchL + 0.05f), matGrayStone);
            CreateMeshBox("Kitchen_MarbleBacksplash", kitchenRoot.transform, new Vector3(-4.2f, 1.3f, kitchZ), new Vector3(0.04f, 0.71f, kitchL), matGrayStone);
            CreateMeshBox("Kitchen_UpperCabinets", kitchenRoot.transform, new Vector3(kitchX, 2.1f, kitchZ), new Vector3(0.45f, 0.8f, kitchL), matLightOak);
            CreateMeshBox("Kitchen_InductionCooktop", kitchenRoot.transform, new Vector3(kitchX, 0.95f, -1.4f), new Vector3(0.48f, 0.02f, 0.65f), matTvScreen);
            CreateMeshBox("Kitchen_BuiltInOven", kitchenRoot.transform, new Vector3(kitchX, 0.45f, -1.4f), new Vector3(0.62f, 0.55f, 0.6m), matMatteBlack);
            CreateMeshBox("Kitchen_StainlessSink", kitchenRoot.transform, new Vector3(kitchX, 0.94f, 0.4f), new Vector3(0.45f, 0.02f, 0.55f), matMatteBlack);
            CreateMeshBox("Kitchen_Faucet", kitchenRoot.transform, new Vector3(-4.15f, 1.1f, 0.4f), new Vector3(0.06f, 0.28f, 0.06f), matMatteBlack);
            CreateMeshBox("Kitchen_TallFridge", kitchenRoot.transform, new Vector3(kitchX, 1.05f, 0.95f), new Vector3(0.65f, 2.1f, 0.75f), matMatteBlack);

            // Kitchen Breakfast Island Bar
            CreateMeshBox("Kitchen_Island_Base", kitchenRoot.transform, new Vector3(-2.2f, 0.45f, -0.5f), new Vector3(0.85f, 0.9f, 1.8f), matLightOak);
            CreateMeshBox("Kitchen_Island_MarbleTop", kitchenRoot.transform, new Vector3(-2.2f, 0.92f, -0.5f), new Vector3(0.95f, 0.06f, 1.9f), matGrayStone);
            for (int i = 0; i < 3; i++)
            {
                float stoolZ = -1.1f + i * 0.6f;
                CreateMeshBox($"BarStool_{i+1}_Seat", kitchenRoot.transform, new Vector3(-1.45f, 0.65f, stoolZ), new Vector3(0.38f, 0.06f, 0.38f), matAccentWall);
                CreateMeshBox($"BarStool_{i+1}_Legs", kitchenRoot.transform, new Vector3(-1.45f, 0.32f, stoolZ), new Vector3(0.32f, 0.62f, 0.32f), matMatteBlack);
            }

            // --- 7. ENTRANCE & HALLWAY MODULE ---
            GameObject entranceRoot = CreateSubContainer("7_Entrance");
            CreateMeshBox("Entrance_Doormat", entranceRoot.transform, new Vector3(-1.5f, 0.005f, -4.6f), new Vector3(0.95f, 0.01f, 0.65f), matBeigeFabric);
            CreateMeshBox("Entrance_DoorLeaf", entranceRoot.transform, new Vector3(-1.5f, 1.1f, -4.95f), new Vector3(1.15f, 2.18f, 0.06f), matLightOak);
            CreateMeshBox("Entrance_ShoeBench", entranceRoot.transform, new Vector3(-2.6f, 0.25f, -4.5f), new Vector3(0.85f, 0.45f, 0.38f), matLightOak);
            CreateMeshBox("Entrance_CoatHanger", entranceRoot.transform, new Vector3(-0.4f, 1.4f, -4.85f), new Vector3(0.65f, 0.1f, 0.04f), matLightOak);

            // --- 8. EXECUTIVE LIVING ROOM MODULE (Bottom Right: 4.25m x 5.0m) ---
            GameObject livingRoot = CreateSubContainer("8_LivingRoom");
            CreateMeshBox("Living_AreaRug", livingRoot.transform, new Vector3(1.8f, 0.01f, -2.2f), new Vector3(3.2m, 0.015f, 3.5m), matRugFabric, false);
            CreateMeshBox("Sofa_MainSeat", livingRoot.transform, new Vector3(1.8f, 0.25f, -3.8f), new Vector3(2.4m, 0.35f, 0.9m), matBeigeFabric);
            CreateMeshBox("Sofa_ChaiseLounge", livingRoot.transform, new Vector3(2.65f, 0.25f, -2.85f), new Vector3(0.9m, 0.35f, 1.0m), matBeigeFabric);
            CreateMeshBox("Sofa_Backrest", livingRoot.transform, new Vector3(1.8f, 0.58f, -4.2f), new Vector3(2.4m, 0.52f, 0.25f), matBeigeFabric);
            CreateMeshBox("Sofa_ArmLeft", livingRoot.transform, new Vector3(0.55f, 0.45f, -3.8f), new Vector3(0.25f, 0.38f, 0.9m), matBeigeFabric);
            CreateMeshBox("CoffeeTable_Top", livingRoot.transform, new Vector3(1.6f, 0.38f, -2.2f), new Vector3(0.95f, 0.04f, 0.95f), matLightOak);
            CreateMeshBox("CoffeeTable_Base", livingRoot.transform, new Vector3(1.6f, 0.18f, -2.2f), new Vector3(0.55f, 0.36f, 0.55f), matMatteBlack);
            CreateMeshBox("CoffeeTable_BookSet", livingRoot.transform, new Vector3(1.6f, 0.41f, -2.2f), new Vector3(0.32f, 0.03f, 0.24f), matWhiteWall);
            CreateMeshBox("TVConsole_Cabinet", livingRoot.transform, new Vector3(2.1f, 0.28f, 0.8m), new Vector3(2.2m, 0.36f, 0.4f), matLightOak);
            CreateMeshBox("TV_OuterFrame", livingRoot.transform, new Vector3(2.1f, 1.15f, 0.95m), new Vector3(1.4m, 0.82f, 0.06f), matMatteBlack);
            CreateMeshBox("TV_GlassScreen", livingRoot.transform, new Vector3(2.1f, 1.15f, 0.91m), new Vector3(1.32m, 0.74f, 0.01f), matTvScreen, false);
            CreateMeshBox("FloorLamp_Pole", livingRoot.transform, new Vector3(3.45f, 0.9f, -4.0f), new Vector3(0.06f, 1.8f, 0.06f), matMatteBlack);
            CreateMeshBox("FloorLamp_Shade", livingRoot.transform, new Vector3(3.45f, 1.7f, -4.0f), new Vector3(0.38f, 0.32f, 0.38f), matWhiteWall);
            CreateMeshBox("PottedPlant_Base", livingRoot.transform, new Vector3(3.45f, 0.25f, -0.6f), new Vector3(0.42f, 0.48f, 0.42f), matCeramicWhite);
            CreateMeshBox("PottedPlant_Foliage", livingRoot.transform, new Vector3(3.45f, 0.95f, -0.6f), new Vector3(0.75f, 0.95f, 0.75f), matOliveGreenFabric);

            Debug.Log("[ScandiStudioGenerator] Organized 85m² Studio generated successfully!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject container = new GameObject(name);
            container.transform.SetParent(m_ApartmentContainer.transform, false);
            return container;
        }

        private GameObject CreateMeshBox(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool addCol = true)
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

        private void EnsurePBRMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (matLightOak == null)
                matLightOak = CreateMat(litShader, "M_LightOak", new Color(0.78f, 0.62f, 0.44f), 0.45f, 0.05f);

            if (matWhiteWall == null)
                matWhiteWall = CreateMat(litShader, "M_WhiteWall", new Color(0.94f, 0.94f, 0.93f), 0.85f, 0.0f);

            if (matAccentWall == null)
                matAccentWall = CreateMat(litShader, "M_AccentWall", new Color(0.35f, 0.28f, 0.22f), 0.5f, 0.0f);

            if (matMatteBlack == null)
                matMatteBlack = CreateMat(litShader, "M_MatteBlack", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.5f);

            if (matGrayStone == null)
                matGrayStone = CreateMat(litShader, "M_GrayStone", new Color(0.88f, 0.90f, 0.92f), 0.15f, 0.1f);

            if (matCeramicWhite == null)
                matCeramicWhite = CreateMat(litShader, "M_CeramicWhite", new Color(0.97f, 0.97f, 0.97f), 0.15f, 0.05f);

            if (matBeigeFabric == null)
                matBeigeFabric = CreateMat(litShader, "M_BeigeFabric", new Color(0.82f, 0.75f, 0.68f), 0.9f, 0.0f);

            if (matOliveGreenFabric == null)
                matOliveGreenFabric = CreateMat(litShader, "M_OliveGreenFabric", new Color(0.25f, 0.42f, 0.20f), 0.8f, 0.0f);

            if (matFrostedGlass == null)
                matFrostedGlass = CreateMat(litShader, "M_FrostedGlass", new Color(0.85f, 0.92f, 0.95f, 0.45f), 0.2f, 0.1f);

            if (matGrayTile == null)
                matGrayTile = CreateMat(litShader, "M_GrayTile", new Color(0.82f, 0.85f, 0.88f), 0.3f, 0.05f);

            if (matRugFabric == null)
                matRugFabric = CreateMat(litShader, "M_RugFabric", new Color(0.86f, 0.83f, 0.78f), 0.95f, 0.0f);

            if (matClearGlass == null)
                matClearGlass = CreateMat(litShader, "M_ClearGlass", new Color(0.9f, 0.95f, 1.0f, 0.2f), 0.1f, 0.1f);

            if (matTvScreen == null)
                matTvScreen = CreateMat(litShader, "M_TvScreen", new Color(0.05f, 0.05f, 0.06f), 0.1f, 0.8f);
        }

        private Material CreateMat(Shader shader, string matName, Color color, float smoothness, float metallic)
        {
            Material mat = new Material(shader);
            mat.name = matName;
            mat.SetColor("_BaseColor", color);
            mat.SetColor("_Color", color);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            return mat;
        }
    }
}
