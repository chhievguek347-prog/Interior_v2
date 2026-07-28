using System.Collections.Generic;
using UnityEngine;

namespace Interior.Studio
{
    public class OrganizedStudioGenerator : MonoBehaviour
    {
        [Header("Room Overall Dimensions (Spacious Layout)")]
        public float roomWidth = 8.5f;   // X axis (-4.25 to +4.25)
        public float roomLength = 10.0f; // Z axis (-5.0 to +5.0)
        public float roomHeight = 3.0f;  // Y axis (0 to 3.0)
        public float wallThickness = 0.2f;

        [Header("PBR Materials")]
        public Material matWoodPlank;
        public Material matOffWhiteWall;
        public Material matAccentWall;
        public Material matMatteBlack;
        public Material matQuartzMarble;
        public Material matCeramicWhite;
        public Material matPlushSofaFabric;
        public Material matBedDuvet;
        public Material matBlanket;
        public Material matFrostedGlass;
        public Material matClearGlass;
        public Material matBathTile;
        public Material matRugTexture;
        public Material matPlantFoliage;
        public Material matTvScreen;

        [Header("Options")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_StudioRoot;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateOrganizedStudio();
            }
        }

        [ContextMenu("Generate Organized Spacious Studio")]
        public void GenerateOrganizedStudio()
        {
            // Clear existing layout
            Transform existing = transform.Find("OrganizedStudioLayout");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_StudioRoot = new GameObject("OrganizedStudioLayout");
            m_StudioRoot.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. ARCHITECTURE & SHELL ---
            GameObject archGroup = CreateGroup("1_Architecture");

            // Main Flooring - Warm Hardwood Planks
            CreateBox("Floor_Hardwood", archGroup.transform, new Vector3(0, -0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWoodPlank);

            // Ceiling - Clean White
            CreateBox("Ceiling_White", archGroup.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matOffWhiteWall);

            // Exterior Walls
            // Left Solid Wall (X = -halfW)
            CreateBox("Wall_Exterior_Left", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matOffWhiteWall);

            // Back Wall (Z = +halfL) with Feature Accent Wall Panel
            CreateBox("Wall_Exterior_Back", archGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2f, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Wall_Back_AccentPanel", archGroup.transform, new Vector3(1.8f, roomHeight * 0.5f, halfL - 0.02f), new Vector3(4.2f, roomHeight, 0.04f), matAccentWall);

            // Front Entrance Wall (Z = -halfL) with Doorway
            float doorW = 1.2f;
            float doorH = 2.2f;
            float doorX = -1.5f;
            float frontLeftW = (doorX - doorW * 0.5f) - (-halfW);
            float frontRightW = halfW - (doorX + doorW * 0.5f);
            CreateBox("Wall_Front_Left", archGroup.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Wall_Front_Right", archGroup.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Wall_Front_Header", archGroup.transform, new Vector3(doorX, doorH + (roomHeight - doorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorW, roomHeight - doorH, wallThickness), matOffWhiteWall);

            // Right Wall with Panoramic Windows
            float winSillH = 0.5f;
            float winTopH = 2.7f;
            float winH = winTopH - winSillH;
            CreateBox("Wall_Right_Sill", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, roomLength), matOffWhiteWall);
            CreateBox("Wall_Right_Header", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, roomLength), matOffWhiteWall);
            CreateBox("Wall_Right_Frame", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.08f, winH, roomLength - 0.8f), matMatteBlack, false);
            CreateBox("Wall_Right_Glass", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.02f, winH - 0.1f, roomLength - 0.9f), matClearGlass, false);

            // --- 2. ZONED INTERIOR PARTITION WALLS ---
            GameObject partitionGroup = CreateGroup("2_Partitions");

            // Zone A: Enclosed Bathroom Partition (Top Left: 2.4m x 2.4m)
            CreateBox("Bath_Wall_South", partitionGroup.transform, new Vector3(-3.05f, roomHeight * 0.5f, 2.6f), new Vector3(2.4f, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Bath_Wall_East", partitionGroup.transform, new Vector3(-1.85f, roomHeight * 0.5f, 3.8f), new Vector3(wallThickness, roomHeight, 2.4f), matOffWhiteWall);

            // Zone B: Closet Partition / Dressing Divider (Top Middle: X = -1.85 to 0.0, Z = 2.6 to 5.0)
            CreateBox("Closet_BackWall", partitionGroup.transform, new Vector3(-0.9f, roomHeight * 0.5f, 4.9m), new Vector3(1.8m, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Closet_Divider_Right", partitionGroup.transform, new Vector3(0.0f, roomHeight * 0.5f, 3.8f), new Vector3(wallThickness, roomHeight, 2.4f), matAccentWall);

            // Zone C: Living / Bedroom Acoustic Wooden Slat Partition (Z = 1.0, X = 0.5 to 4.25)
            CreateBox("Partition_BedroomSlatWall", partitionGroup.transform, new Vector3(2.35f, 1.3f, 1.0f), new Vector3(3.7f, 2.6f, 0.12f), matAccentWall);

            // --- 3. BEDROOM SUITE AREA (Top Right: 4.25m x 4.0m) ---
            GameObject bedGroup = CreateGroup("3_BedroomArea");

            // Platform Bed Frame & Padded Headboard (X = 2.1m, Z = 3.5m)
            CreateBox("Bed_Headboard", bedGroup.transform, new Vector3(2.1f, 0.65f, 4.85f), new Vector3(2.1f, 1.1f, 0.15f), matAccentWall);
            CreateBox("Bed_PlatformBase", bedGroup.transform, new Vector3(2.1f, 0.22f, 3.7f), new Vector3(1.9m, 0.38f, 2.2m), matWoodPlank);
            CreateBox("Bed_Mattress", bedGroup.transform, new Vector3(2.1f, 0.48f, 3.65f), new Vector3(1.8m, 0.24f, 2.1m), matCeramicWhite);
            CreateBox("Bed_Duvet", bedGroup.transform, new Vector3(2.1f, 0.56f, 3.35f), new Vector3(1.78m, 0.14f, 1.6m), matBedDuvet);
            CreateBox("Bed_FoldedBlanket", bedGroup.transform, new Vector3(2.1f, 0.61f, 2.75f), new Vector3(1.8m, 0.08f, 0.75f), matBlanket);

            // Pillows
            CreateBox("Pillow_1", bedGroup.transform, new Vector3(1.55f, 0.65f, 4.5f), new Vector3(0.7f, 0.14f, 0.45f), matCeramicWhite);
            CreateBox("Pillow_2", bedGroup.transform, new Vector3(2.65f, 0.65f, 4.5f), new Vector3(0.7f, 0.14f, 0.45f), matCeramicWhite);

            // Nightstands & Pendant Lamps
            CreateBox("Nightstand_Left", bedGroup.transform, new Vector3(0.75f, 0.25f, 4.65f), new Vector3(0.48f, 0.48f, 0.48f), matWoodPlank);
            CreateBox("Nightstand_Right", bedGroup.transform, new Vector3(3.45f, 0.25f, 4.65f), new Vector3(0.48f, 0.48f, 0.48f), matWoodPlank);
            CreateBox("PendantLamp_Left", bedGroup.transform, new Vector3(0.75f, 1.6f, 4.65f), new Vector3(0.18f, 0.32f, 0.18f), matMatteBlack);
            CreateBox("PendantLamp_Right", bedGroup.transform, new Vector3(3.45f, 1.6f, 4.65f), new Vector3(0.18f, 0.32f, 0.18f), matMatteBlack);

            // Bedroom Rug
            CreateBox("Bed_AreaRug", bedGroup.transform, new Vector3(2.1f, 0.01f, 3.2f), new Vector3(2.6m, 0.015f, 2.8m), matRugTexture, false);

            // --- 4. CLOSET AREA (Top Center: 1.8m x 2.4m) ---
            GameObject closetGroup = CreateGroup("4_ClosetArea");

            // Built-in Wardrobe System with Shelves & Hanging Rail
            CreateBox("Closet_CabinetFrame", closetGroup.transform, new Vector3(-0.9f, 1.35f, 4.75f), new Vector3(1.7m, 2.5f, 0.45f), matWoodPlank);
            CreateBox("Closet_SlidingDoor_L", closetGroup.transform, new Vector3(-1.3f, 1.3f, 4.5f), new Vector3(0.85f, 2.4f, 0.04f), matFrostedGlass);
            CreateBox("Closet_SlidingDoor_R", closetGroup.transform, new Vector3(-0.5f, 1.3f, 4.54f), new Vector3(0.85f, 2.4f, 0.04f), matFrostedGlass);
            CreateBox("Closet_DressingMirror", closetGroup.transform, new Vector3(-0.05f, 1.35f, 3.6f), new Vector3(0.04f, 1.8f, 0.65f), matMatteBlack);

            // --- 5. SPA BATHROOM AREA (Top Left: 2.4m x 2.4m) ---
            GameObject bathGroup = CreateGroup("5_BathroomArea");

            // Ceramic Tile Floor
            CreateBox("Bath_TileFloor", bathGroup.transform, new Vector3(-3.05f, 0.01f, 3.8f), new Vector3(2.35f, 0.02f, 2.35f), matBathTile);

            // Walk-in Glass Shower Stall
            CreateBox("Shower_GlassWall_E", bathGroup.transform, new Vector3(-3.05f, 1.15f, 4.9f), new Vector3(0.04f, 2.2f, 1.1f), matFrostedGlass);
            CreateBox("Shower_GlassWall_S", bathGroup.transform, new Vector3(-3.6f, 1.15f, 4.35f), new Vector3(1.1f, 0.04f, 0.04f), matFrostedGlass);
            CreateBox("Shower_RainHead", bathGroup.transform, new Vector3(-3.6f, 2.3f, 4.6f), new Vector3(0.3f, 0.05f, 0.3f), matMatteBlack);

            // Floating Vanity & Basin
            CreateBox("Vanity_Cabinet", bathGroup.transform, new Vector3(-2.3f, 0.45f, 3.0f), new Vector3(0.55f, 0.45f, 0.9m), matWoodPlank);
            CreateBox("Vanity_SinkBasin", bathGroup.transform, new Vector3(-2.3f, 0.72f, 3.0f), new Vector3(0.45f, 0.12f, 0.75f), matCeramicWhite);
            CreateBox("Vanity_MirrorLED", bathGroup.transform, new Vector3(-1.9f, 1.45f, 3.0f), new Vector3(0.04f, 0.85f, 0.8m), matMatteBlack);

            // Toilet
            CreateBox("Toilet_Base", bathGroup.transform, new Vector3(-3.7f, 0.24f, 3.0f), new Vector3(0.44f, 0.45f, 0.68f), matCeramicWhite);
            CreateBox("Toilet_Tank", bathGroup.transform, new Vector3(-4.0f, 0.52f, 3.0f), new Vector3(0.22f, 0.55f, 0.45f), matCeramicWhite);

            // --- 6. GOURMET KITCHENETTE AREA (Middle Left: 3.2m x 0.65m + Island) ---
            GameObject kitchenGroup = CreateGroup("6_KitchenArea");

            float kitchX = -3.9f;
            float kitchZ = -0.5f;
            float kitchL = 3.2f;

            // Main Counter Base & Upper Cabinets
            CreateBox("Kitchen_BaseCabinets", kitchenGroup.transform, new Vector3(kitchX, 0.45f, kitchZ), new Vector3(0.65f, 0.9f, kitchL), matWoodPlank);
            CreateBox("Kitchen_QuartzCounter", kitchenGroup.transform, new Vector3(kitchX, 0.92f, kitchZ), new Vector3(0.68f, 0.05f, kitchL + 0.05f), matQuartzMarble);
            CreateBox("Kitchen_QuartzBacksplash", kitchenGroup.transform, new Vector3(-4.2f, 1.3f, kitchZ), new Vector3(0.04f, 0.71f, kitchL), matQuartzMarble);
            CreateBox("Kitchen_UpperCabinets", kitchenGroup.transform, new Vector3(kitchX, 2.1f, kitchZ), new Vector3(0.45f, 0.8f, kitchL), matWoodPlank);

            // Cooktop, Oven & Sink
            CreateBox("Kitchen_InductionCooktop", kitchenGroup.transform, new Vector3(kitchX, 0.95f, -1.4f), new Vector3(0.48f, 0.02f, 0.65f), matTvScreen);
            CreateBox("Kitchen_BuiltInOven", kitchenGroup.transform, new Vector3(kitchX, 0.45f, -1.4f), new Vector3(0.62f, 0.55f, 0.6m), matMatteBlack);
            CreateBox("Kitchen_StainlessSink", kitchenGroup.transform, new Vector3(kitchX, 0.94f, 0.4f), new Vector3(0.45f, 0.02f, 0.55f), matMatteBlack);
            CreateBox("Kitchen_GooseneckFaucet", kitchenGroup.transform, new Vector3(-4.15f, 1.1f, 0.4f), new Vector3(0.06f, 0.28f, 0.06f), matMatteBlack);

            // Refrigerator (Built-in Tall Unit at X = -3.9, Z = 0.9)
            CreateBox("Kitchen_Refrigerator", kitchenGroup.transform, new Vector3(kitchX, 1.05f, 0.95f), new Vector3(0.65f, 2.1f, 0.75f), matMatteBlack);

            // Kitchen Breakfast Island Bar (X = -2.2m, Z = -0.5m)
            CreateBox("Kitchen_Island_Base", kitchenGroup.transform, new Vector3(-2.2f, 0.45f, -0.5f), new Vector3(0.85f, 0.9f, 1.8f), matWoodPlank);
            CreateBox("Kitchen_Island_QuartzTop", kitchenGroup.transform, new Vector3(-2.2f, 0.92f, -0.5f), new Vector3(0.95f, 0.06f, 1.9f), matQuartzMarble);

            // 3 Leather Bar Stools
            for (int i = 0; i < 3; i++)
            {
                float stoolZ = -1.1f + i * 0.6f;
                CreateBox($"BarStool_{i+1}_Seat", kitchenGroup.transform, new Vector3(-1.45f, 0.65f, stoolZ), new Vector3(0.38f, 0.06f, 0.38f), matAccentWall);
                CreateBox($"BarStool_{i+1}_Legs", kitchenGroup.transform, new Vector3(-1.45f, 0.32f, stoolZ), new Vector3(0.32f, 0.62f, 0.32f), matMatteBlack);
            }

            // --- 7. EXECUTIVE LIVING ROOM AREA (Bottom Right: 4.25m x 5.0m) ---
            GameObject livingGroup = CreateGroup("7_LivingRoomArea");

            // Woven Textured Area Rug
            CreateBox("Living_AreaRug", livingGroup.transform, new Vector3(1.8f, 0.01f, -2.2f), new Vector3(3.2m, 0.015f, 3.5m), matRugTexture, false);

            // L-Shaped Sectional Plush Sofa (X = 1.8m, Z = -3.5m)
            CreateBox("Sofa_MainSeat", livingGroup.transform, new Vector3(1.8f, 0.25f, -3.8f), new Vector3(2.4m, 0.35f, 0.9m), matPlushSofaFabric);
            CreateBox("Sofa_ChaiseLounge", livingGroup.transform, new Vector3(2.65f, 0.25f, -2.85f), new Vector3(0.9m, 0.35f, 1.0m), matPlushSofaFabric);
            CreateBox("Sofa_Backrest", livingGroup.transform, new Vector3(1.8f, 0.58f, -4.2f), new Vector3(2.4m, 0.52f, 0.25f), matPlushSofaFabric);
            CreateBox("Sofa_ArmLeft", livingGroup.transform, new Vector3(0.55f, 0.45f, -3.8f), new Vector3(0.25f, 0.38f, 0.9m), matPlushSofaFabric);

            // Round Coffee Table & Accessories
            CreateBox("CoffeeTable_Top", livingGroup.transform, new Vector3(1.6f, 0.38f, -2.2f), new Vector3(0.95f, 0.04f, 0.95f), matWoodPlank);
            CreateBox("CoffeeTable_Base", livingGroup.transform, new Vector3(1.6f, 0.18f, -2.2f), new Vector3(0.55f, 0.36f, 0.55f), matMatteBlack);
            CreateBox("CoffeeTable_BookSet", livingGroup.transform, new Vector3(1.6f, 0.41f, -2.2f), new Vector3(0.32f, 0.03f, 0.24f), matOffWhiteWall);

            // Low TV Console Mounted under Acoustic Slat Partition (Z = 0.9m)
            CreateBox("TVConsole_Cabinet", livingGroup.transform, new Vector3(2.1f, 0.28f, 0.8m), new Vector3(2.2m, 0.36f, 0.4f), matWoodPlank);

            // 55" 4K Smart TV
            CreateBox("TV_OuterFrame", livingGroup.transform, new Vector3(2.1f, 1.15f, 0.95m), new Vector3(1.4m, 0.82f, 0.06f), matMatteBlack);
            CreateBox("TV_GlassScreen", livingGroup.transform, new Vector3(2.1f, 1.15f, 0.91m), new Vector3(1.32m, 0.74f, 0.01f), matTvScreen, false);

            // Tall Floor Lamp & Indoor Potted Plant
            CreateBox("FloorLamp_Pole", livingGroup.transform, new Vector3(3.45f, 0.9f, -4.0f), new Vector3(0.06f, 1.8f, 0.06f), matMatteBlack);
            CreateBox("FloorLamp_Shade", livingGroup.transform, new Vector3(3.45f, 1.7f, -4.0f), new Vector3(0.38f, 0.32f, 0.38f), matOffWhiteWall);
            CreateBox("PottedPlant_Base", livingGroup.transform, new Vector3(3.45f, 0.25f, -0.6f), new Vector3(0.42f, 0.48f, 0.42f), matCeramicWhite);
            CreateBox("PottedPlant_Foliage", livingGroup.transform, new Vector3(3.45f, 0.95f, -0.6f), new Vector3(0.75f, 0.95f, 0.75f), matPlantFoliage);

            // --- 8. ENTRANCE & HALLWAY AREA (Bottom Center) ---
            GameObject entranceGroup = CreateGroup("8_EntranceArea");

            CreateBox("Entrance_Doormat", entranceGroup.transform, new Vector3(-1.5f, 0.005f, -4.6f), new Vector3(0.95f, 0.01f, 0.65f), matBlanket);
            CreateBox("Entrance_DoorLeaf", entranceGroup.transform, new Vector3(-1.5f, 1.1f, -4.95f), new Vector3(1.15f, 2.18f, 0.06f), matWoodPlank);
            CreateBox("Entrance_ShoeBench", entranceGroup.transform, new Vector3(-2.6f, 0.25f, -4.5f), new Vector3(0.85f, 0.45f, 0.38f), matWoodPlank);
            CreateBox("Entrance_CoatHanger", entranceGroup.transform, new Vector3(-0.4f, 1.4f, -4.85f), new Vector3(0.65f, 0.1f, 0.04f), matWoodPlank);

            Debug.Log("[OrganizedStudioGenerator] Spacious Organized Studio generated successfully!");
        }

        private GameObject CreateGroup(string groupName)
        {
            GameObject go = new GameObject(groupName);
            go.transform.SetParent(m_StudioRoot.transform, false);
            return go;
        }

        private GameObject CreateBox(string boxName, Transform parent, Vector3 localPos, Vector3 scale, Material mat, bool addCol = true)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = boxName;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
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

            if (matWoodPlank == null)
                matWoodPlank = CreateMat(litShader, "M_WoodPlank", new Color(0.78f, 0.62f, 0.44f), 0.45f, 0.05f);

            if (matOffWhiteWall == null)
                matOffWhiteWall = CreateMat(litShader, "M_OffWhiteWall", new Color(0.94f, 0.94f, 0.93f), 0.85f, 0.0f);

            if (matAccentWall == null)
                matAccentWall = CreateMat(litShader, "M_AccentWall", new Color(0.35f, 0.28f, 0.22f), 0.5f, 0.0f);

            if (matMatteBlack == null)
                matMatteBlack = CreateMat(litShader, "M_MatteBlack", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.5f);

            if (matQuartzMarble == null)
                matQuartzMarble = CreateMat(litShader, "M_QuartzMarble", new Color(0.88f, 0.90f, 0.92f), 0.15f, 0.1f);

            if (matCeramicWhite == null)
                matCeramicWhite = CreateMat(litShader, "M_CeramicWhite", new Color(0.97f, 0.97f, 0.97f), 0.15f, 0.05f);

            if (matPlushSofaFabric == null)
                matPlushSofaFabric = CreateMat(litShader, "M_PlushSofaFabric", new Color(0.82f, 0.75f, 0.68f), 0.9f, 0.0f);

            if (matBedDuvet == null)
                matBedDuvet = CreateMat(litShader, "M_BedDuvet", new Color(0.95f, 0.95f, 0.95f), 0.9f, 0.0f);

            if (matBlanket == null)
                matBlanket = CreateMat(litShader, "M_Blanket", new Color(0.68f, 0.58f, 0.48f), 0.95f, 0.0f);

            if (matFrostedGlass == null)
                matFrostedGlass = CreateMat(litShader, "M_FrostedGlass", new Color(0.85f, 0.92f, 0.95f, 0.45f), 0.2f, 0.1f);

            if (matClearGlass == null)
                matClearGlass = CreateMat(litShader, "M_ClearGlass", new Color(0.9f, 0.95f, 1.0f, 0.2f), 0.1f, 0.1f);

            if (matBathTile == null)
                matBathTile = CreateMat(litShader, "M_BathTile", new Color(0.82f, 0.85f, 0.88f), 0.3f, 0.05f);

            if (matRugTexture == null)
                matRugTexture = CreateMat(litShader, "M_RugTexture", new Color(0.86f, 0.83f, 0.78f), 0.95f, 0.0f);

            if (matPlantFoliage == null)
                matPlantFoliage = CreateMat(litShader, "M_PlantFoliage", new Color(0.25f, 0.42f, 0.20f), 0.8f, 0.0f);

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
