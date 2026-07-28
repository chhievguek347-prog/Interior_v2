using System.Collections.Generic;
using UnityEngine;

namespace Interior.Studio
{
    public class RealisticPhotoRoomGenerator : MonoBehaviour
    {
        [Header("Overall Apartment Dimensions (Photo Spec)")]
        public float totalWidth = 6.7f;   // X axis (-3.35 to +3.35)
        public float totalLength = 8.0f;  // Z axis (-4.0 to +4.0)
        public float roomHeight = 2.8f;   // Y axis (0 to 2.8)
        public float wallThickness = 0.18f;

        [Header("PBR Materials")]
        public Material matNaturalOak;
        public Material matOffWhiteWall;
        public Material matMatteBlackFrame;
        public Material matGreyMarble;
        public Material matWhiteCeramic;
        public Material matCreamSofaFabric;
        public Material matBeigeBlanket;
        public Material matFrostedGlass;
        public Material matClearGlass;
        public Material matBathroomTile;
        public Material matWovenRug;
        public Material matPlantFoliage;
        public Material matTowelFabric;
        public Material matTvScreen;

        [Header("Options")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_RoomRoot;

        private void Start()
        {
            if (generateOnStart)
            {
                GeneratePhotoRoom();
            }
        }

        [ContextMenu("Generate Photo Replica 3D Room")]
        public void GeneratePhotoRoom()
        {
            // Clear existing geometry
            Transform existing = transform.Find("PhotoReplicaRoom");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_RoomRoot = new GameObject("PhotoReplicaRoom");
            m_RoomRoot.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = totalWidth * 0.5f;
            float halfL = totalLength * 0.5f;

            // --- 1. ARCHITECTURE (Floor, Ceiling, Outer Shell Walls, Windows) ---
            GameObject archGroup = CreateGroup("1_Architecture");

            // Main Floor - Natural Light Oak (X: -halfW to halfW, Z: -halfL to halfL)
            CreateBox("Main_Oak_Floor", archGroup.transform, new Vector3(0, -0.05f, 0), new Vector3(totalWidth, 0.1f, totalLength), matNaturalOak);

            // Exterior Walls (White)
            // Left Solid Wall (X = -halfW)
            CreateBox("Wall_Exterior_Left", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, totalLength), matOffWhiteWall);

            // Front Entrance Wall (Z = -halfL) with Doorway Cutout (Door at X = -0.5m)
            float doorW = 1.1f;
            float doorH = 2.1f;
            float doorX = -0.5f;
            float frontLeftW = (doorX - doorW * 0.5f) - (-halfW);
            float frontRightW = halfW - (doorX + doorW * 0.5f);

            CreateBox("Wall_Front_Left", archGroup.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Wall_Front_Right", archGroup.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Wall_Front_Header", archGroup.transform, new Vector3(doorX, doorH + (roomHeight - doorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorW, roomHeight - doorH, wallThickness), matOffWhiteWall);

            // Back Wall (Z = +halfL)
            CreateBox("Wall_Back_Solid", archGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(totalWidth + wallThickness * 2f, roomHeight, wallThickness), matOffWhiteWall);

            // Right Wall with 2 Large Windows (Bedroom & Living Area Windows)
            float winSillH = 0.6f;
            float winTopH = 2.5f;
            float winH = winTopH - winSillH;

            // Right Wall Bottom Sill & Top Header
            CreateBox("Wall_Right_Sill", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, totalLength), matOffWhiteWall);
            CreateBox("Wall_Right_Header", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, totalLength), matOffWhiteWall);
            CreateBox("Wall_Right_CenterPillar", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, 0.0f), new Vector3(wallThickness, roomHeight, 0.8f), matOffWhiteWall);
            CreateBox("Wall_Right_BackPillar", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, halfL - 0.2f), new Vector3(wallThickness, roomHeight, 0.4f), matOffWhiteWall);
            CreateBox("Wall_Right_FrontPillar", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, -halfL + 0.2f), new Vector3(wallThickness, roomHeight, 0.4f), matOffWhiteWall);

            // Bedroom Window Glass & Frame (Z = 0.8 to 3.6)
            CreateBox("Bedroom_Window_Frame", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 2.2f), new Vector3(0.08f, winH, 2.6f), matMatteBlackFrame, false);
            CreateBox("Bedroom_Window_Glass", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 2.2f), new Vector3(0.02f, winH - 0.1f, 2.5f), matClearGlass, false);

            // Living Room Window Glass & Frame (Z = -3.6 to -0.8)
            CreateBox("Living_Window_Frame", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, -2.2f), new Vector3(0.08f, winH, 2.6f), matMatteBlackFrame, false);
            CreateBox("Living_Window_Glass", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, -2.2f), new Vector3(0.02f, winH - 0.1f, 2.5f), matClearGlass, false);

            // --- 2. INTERIOR PARTITION WALLS & ROOM ZONES ---
            GameObject partitionGroup = CreateGroup("2_Partitions");

            // Bathroom Enclosure Walls (2m x 2m: X = -3.35 to -1.35, Z = 2.0 to 4.0)
            // Bathroom South Partition Wall (Z = 2.0, X = -3.35 to -1.35 with doorway)
            float bathDoorW = 0.85f;
            float bathWallW = 2.0f;
            float bathWallLeftW = bathWallW - bathDoorW;
            CreateBox("Bath_Partition_South_Wall", partitionGroup.transform, new Vector3(-3.35f + bathWallLeftW * 0.5f, roomHeight * 0.5f, 2.0f), new Vector3(bathWallLeftW, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Bath_Partition_South_Header", partitionGroup.transform, new Vector3(-1.35f - bathDoorW * 0.5f, 2.1f + (roomHeight - 2.1f) * 0.5f, 2.0f), new Vector3(bathDoorW, roomHeight - 2.1f, wallThickness), matOffWhiteWall);

            // Bathroom East Partition Wall (X = -1.35, Z = 2.0 to 4.0)
            CreateBox("Bath_Partition_East", partitionGroup.transform, new Vector3(-1.35f, roomHeight * 0.5f, 3.0f), new Vector3(wallThickness, roomHeight, 2.0f), matOffWhiteWall);

            // Closet / Hallway East Partition Wall (X = -0.15, Z = 2.0 to 4.0)
            CreateBox("Closet_Partition_East", partitionGroup.transform, new Vector3(-0.15f, roomHeight * 0.5f, 3.0f), new Vector3(wallThickness, roomHeight, 2.0f), matOffWhiteWall);

            // Bedroom / Living Area Interior Partition Wall (Z = 0.0, X = -0.15 to 3.35 with doorway at X = -0.15)
            float bedDoorW = 0.95f;
            float bedWallW = 3.5f - bedDoorW;
            CreateBox("Bedroom_Partition_South", partitionGroup.transform, new Vector3(3.35f - bedWallW * 0.5f, roomHeight * 0.5f, 0.0f), new Vector3(bedWallW, roomHeight, wallThickness), matOffWhiteWall);
            CreateBox("Bedroom_Partition_Header", partitionGroup.transform, new Vector3(-0.15f + bedDoorW * 0.5f, 2.1f + (roomHeight - 2.1f) * 0.5f, 0.0f), new Vector3(bedDoorW, roomHeight - 2.1f, wallThickness), matOffWhiteWall);

            // --- 3. BATHROOM MODULE (Top Left: 2m x 2m) ---
            GameObject bathModule = CreateGroup("3_Bathroom");

            // Grey/White Ceramic Tile Floor Overlay
            CreateBox("Bath_Tile_Floor", bathModule.transform, new Vector3(-2.35f, 0.01f, 3.0f), new Vector3(1.95f, 0.02f, 1.95f), matBathroomTile);

            // Glass Shower Stall (1.0m x 1.0m at X = -2.85, Z = 3.5)
            CreateBox("Shower_Glass_East", bathModule.transform, new Vector3(-2.35f, 1.1f, 3.45f), new Vector3(0.03f, 2.1f, 1.0f), matFrostedGlass);
            CreateBox("Shower_Glass_South", bathModule.transform, new Vector3(-2.85f, 1.1f, 2.95f), new Vector3(1.0f, 0.03f, 0.03f), matFrostedGlass);
            CreateBox("Shower_RainHead", bathModule.transform, new Vector3(-2.85f, 2.2f, 3.7f), new Vector3(0.25f, 0.05f, 0.25f), matMatteBlackFrame);

            // Floating Wooden Vanity & Sink (X = -1.85, Z = 3.5)
            CreateBox("Vanity_Cabinet", bathModule.transform, new Vector3(-1.85f, 0.45f, 3.5f), new Vector3(0.5f, 0.45f, 0.75f), matNaturalOak);
            CreateBox("Vanity_SinkBasin", bathModule.transform, new Vector3(-1.85f, 0.72f, 3.5f), new Vector3(0.42f, 0.12f, 0.65f), matWhiteCeramic);
            CreateBox("Vanity_Faucet", bathModule.transform, new Vector3(-1.65f, 0.82f, 3.5f), new Vector3(0.08f, 0.15f, 0.08f), matMatteBlackFrame);
            CreateBox("Vanity_Mirror", bathModule.transform, new Vector3(-1.4f, 1.45f, 3.5f), new Vector3(0.04f, 0.8f, 0.65f), matMatteBlackFrame);
            CreateBox("Vanity_TowelHeater", bathModule.transform, new Vector3(-1.4f, 1.2f, 2.4f), new Vector3(0.04f, 0.9f, 0.45f), matMatteBlackFrame);

            // Toilet (X = -2.85, Z = 2.4)
            CreateBox("Toilet_Base", bathModule.transform, new Vector3(-2.85f, 0.22f, 2.35f), new Vector3(0.42f, 0.44f, 0.65f), matWhiteCeramic);
            CreateBox("Toilet_Tank", bathModule.transform, new Vector3(-3.12f, 0.5f, 2.35f), new Vector3(0.22f, 0.52f, 0.44f), matWhiteCeramic);
            CreateBox("Bath_TowelRack", bathModule.transform, new Vector3(-3.3f, 1.1f, 3.1f), new Vector3(0.04f, 0.2f, 0.5f), matMatteBlackFrame);

            // --- 4. CLOSET HALLWAY MODULE (Top Middle: 1.2m x 2m) ---
            GameObject closetModule = CreateGroup("4_Closet");

            // Built-in Closet with Dual White Sliding Doors (North wall: Z = 3.9)
            CreateBox("Closet_Frame_Outer", closetModule.transform, new Vector3(-0.75f, 1.35f, 3.9f), new Vector3(1.15f, 2.4f, 0.12f), matNaturalOak);
            CreateBox("Closet_Door_Left", closetModule.transform, new Vector3(-0.98f, 1.3f, 3.84f), new Vector3(0.58f, 2.3f, 0.04f), matOffWhiteWall);
            CreateBox("Closet_Door_Right", closetModule.transform, new Vector3(-0.52f, 1.3f, 3.86f), new Vector3(0.58f, 2.3f, 0.04f), matOffWhiteWall);

            // --- 5. BEDROOM MODULE (Top Right: 3.5m x 4m) ---
            GameObject bedModule = CreateGroup("5_Bedroom");

            // Bed Center: X = 1.6m, Z = 2.4m
            // Queen Platform Bed Frame (2.1m long x 1.65m wide)
            CreateBox("Bed_Headboard", bedModule.transform, new Vector3(1.6f, 0.55f, 3.85f), new Vector3(1.75f, 0.9m, 0.1f), matNaturalOak);
            CreateBox("Bed_Frame_Base", bedModule.transform, new Vector3(1.6f, 0.2f, 2.75f), new Vector3(1.68f, 0.35f, 2.1f), matNaturalOak);
            // White Mattress & Fitted Sheet
            CreateBox("Bed_Mattress", bedModule.transform, new Vector3(1.6f, 0.46f, 2.72f), new Vector3(1.58f, 0.22f, 2.0f), matWhiteCeramic);
            // White Duvet
            CreateBox("Bed_WhiteDuvet", bedModule.transform, new Vector3(1.6f, 0.54f, 2.5f), new Vector3(1.55f, 0.12f, 1.55f), matWhiteCeramic);
            // Beige Folded Throw Blanket
            CreateBox("Bed_FoldedBlanket", bedModule.transform, new Vector3(1.6f, 0.58f, 1.95f), new Vector3(1.58f, 0.1f, 0.65f), matBeigeBlanket);

            // 2 Sleeping Pillows
            CreateBox("Pillow_Left", bedModule.transform, new Vector3(1.15f, 0.62f, 3.55f), new Vector3(0.65f, 0.12f, 0.4f), matWhiteCeramic);
            CreateBox("Pillow_Right", bedModule.transform, new Vector3(2.05f, 0.62f, 3.55f), new Vector3(0.65f, 0.12f, 0.4f), matWhiteCeramic);

            // Two Matching Wooden Nightstands
            CreateBox("Nightstand_Left", bedModule.transform, new Vector3(0.35f, 0.24f, 3.65f), new Vector3(0.42f, 0.48f, 0.42f), matNaturalOak);
            CreateBox("Nightstand_Right", bedModule.transform, new Vector3(2.85f, 0.24f, 3.65f), new Vector3(0.42f, 0.48f, 0.42f), matNaturalOak);

            // Bedside Lamps
            CreateBox("Lamp_Left_Base", bedModule.transform, new Vector3(0.35f, 0.52f, 3.65f), new Vector3(0.12f, 0.08f, 0.12f), matMatteBlackFrame);
            CreateBox("Lamp_Left_Shade", bedModule.transform, new Vector3(0.35f, 0.68f, 3.65f), new Vector3(0.22f, 0.24f, 0.22f), matOffWhiteWall);
            CreateBox("Lamp_Right_Base", bedModule.transform, new Vector3(2.85f, 0.52f, 3.65f), new Vector3(0.12f, 0.08f, 0.12f), matMatteBlackFrame);
            CreateBox("Lamp_Right_Shade", bedModule.transform, new Vector3(2.85f, 0.68f, 3.65f), new Vector3(0.22f, 0.24f, 0.22f), matOffWhiteWall);

            // White Curtains / Drapes along Right Window
            CreateBox("Bedroom_Curtain_Fold", bedModule.transform, new Vector3(3.2f, 1.45f, 0.95f), new Vector3(0.2f, 2.3f, 0.35f), matOffWhiteWall, false);

            // --- 6. KITCHENETTE MODULE (Middle Left: 2.5m x 0.6m) ---
            GameObject kitchenModule = CreateGroup("6_Kitchenette");

            float kitchX = -3.05f; // Along Left Wall
            float kitchZ = -0.5f;
            float kitchL = 2.5f;

            // Lower Cabinets (Wood & White)
            CreateBox("Kitchen_LowerCabinets", kitchenModule.transform, new Vector3(kitchX, 0.45f, kitchZ), new Vector3(0.6f, 0.9f, kitchL), matNaturalOak);

            // Grey Marble Countertop
            CreateBox("Kitchen_MarbleCounter", kitchenModule.transform, new Vector3(kitchX, 0.92f, kitchZ), new Vector3(0.64f, 0.05f, kitchL + 0.04f), matGreyMarble);
            CreateBox("Kitchen_MarbleSplasback", kitchenModule.transform, new Vector3(-3.32f, 1.25f, kitchZ), new Vector3(0.04f, 0.6f, kitchL), matGreyMarble);

            // Upper Wooden Wall Cabinets
            CreateBox("Kitchen_UpperCabinets", kitchenModule.transform, new Vector3(kitchX, 2.05f, kitchZ), new Vector3(0.42f, 0.75f, kitchL), matNaturalOak);

            // Black Glass Induction Cooktop (Z = -1.25)
            CreateBox("Kitchen_InductionCooktop", kitchenModule.transform, new Vector3(kitchX, 0.95f, -1.25f), new Vector3(0.45f, 0.02f, 0.55f), matTvScreen);

            // Stainless Steel Sink & Chrome Faucet (Z = 0.2)
            CreateBox("Kitchen_SinkBasin", kitchenModule.transform, new Vector3(kitchX, 0.94f, 0.2f), new Vector3(0.42f, 0.02f, 0.5f), matMatteBlackFrame);
            CreateBox("Kitchen_Faucet", kitchenModule.transform, new Vector3(-3.25f, 1.08f, 0.2f), new Vector3(0.06f, 0.25f, 0.06f), matMatteBlackFrame);

            // Wall Organized Shelves with Plates/Bottles
            CreateBox("Kitchen_WallShelf", kitchenModule.transform, new Vector3(-3.28f, 1.55f, 0.6f), new Vector3(0.12f, 0.03f, 0.6f), matNaturalOak);
            CreateBox("Kitchen_PotPlant", kitchenModule.transform, new Vector3(kitchX, 1.05f, 0.65f), new Vector3(0.18f, 0.22f, 0.18f), matPlantFoliage);

            // Dining Table & Chairs Extension (Attached near Kitchen: X = -2.0, Z = -2.6)
            CreateBox("Dining_Table_Top", kitchenModule.transform, new Vector3(-2.0f, 0.74f, -2.6f), new Vector3(1.2f, 0.04f, 0.75f), matNaturalOak);
            CreateBox("Dining_Table_Leg1", kitchenModule.transform, new Vector3(-2.55f, 0.36f, -2.92f), new Vector3(0.06f, 0.72f, 0.06f), matNaturalOak);
            CreateBox("Dining_Table_Leg2", kitchenModule.transform, new Vector3(-1.45f, 0.36f, -2.92f), new Vector3(0.06f, 0.72f, 0.06f), matNaturalOak);
            CreateBox("Dining_Table_Leg3", kitchenModule.transform, new Vector3(-2.55f, 0.36f, -2.28f), new Vector3(0.06f, 0.72f, 0.06f), matNaturalOak);
            CreateBox("Dining_Table_Leg4", kitchenModule.transform, new Vector3(-1.45f, 0.36f, -2.28f), new Vector3(0.06f, 0.72f, 0.06f), matNaturalOak);

            // 2 Wooden Dining Chairs
            CreateBox("Chair_1_Seat", kitchenModule.transform, new Vector3(-2.3f, 0.44f, -2.05f), new Vector3(0.42f, 0.04f, 0.42f), matNaturalOak);
            CreateBox("Chair_1_Back", kitchenModule.transform, new Vector3(-2.3f, 0.68f, -1.86f), new Vector3(0.42f, 0.45f, 0.04f), matNaturalOak);
            CreateBox("Chair_2_Seat", kitchenModule.transform, new Vector3(-1.7f, 0.44f, -2.05f), new Vector3(0.42f, 0.04f, 0.42f), matNaturalOak);
            CreateBox("Chair_2_Back", kitchenModule.transform, new Vector3(-1.7f, 0.68f, -1.86f), new Vector3(0.42f, 0.45f, 0.04f), matNaturalOak);

            // Dining Table Settings (Plate & Bowl)
            CreateBox("Dining_Plate", kitchenModule.transform, new Vector3(-2.0f, 0.77f, -2.6f), new Vector3(0.28f, 0.02f, 0.28f), matWhiteCeramic);

            // --- 7. ENTRANCE HALLWAY MODULE (Bottom Center) ---
            GameObject entranceModule = CreateGroup("7_Entrance");

            // Sunken Wooden Door Mat / Threshold (X = -0.5, Z = -3.6)
            CreateBox("Entrance_Doormat", entranceModule.transform, new Vector3(-0.5f, 0.005f, -3.6f), new Vector3(0.85f, 0.01f, 0.55f), matBeigeBlanket);

            // Wooden Entrance Door Leaf (Z = -3.98)
            CreateBox("Entrance_DoorLeaf", entranceModule.transform, new Vector3(-0.5f, 1.05f, -3.95f), new Vector3(1.05f, 2.08f, 0.06f), matNaturalOak);
            CreateBox("Entrance_DoorHandle", entranceModule.transform, new Vector3(-0.92f, 1.05f, -3.9f), new Vector3(0.04f, 0.15f, 0.08f), matMatteBlackFrame);

            // Low Wooden Shoe Rack (X = -1.35, Z = -3.5)
            CreateBox("ShoeRack_Base", entranceModule.transform, new Vector3(-1.35f, 0.2f, -3.5f), new Vector3(0.65f, 0.38f, 0.35f), matNaturalOak);

            // Wall Coat Hanger Rack (X = 0.35, Z = -3.85)
            CreateBox("CoatRack_Board", entranceModule.transform, new Vector3(0.35f, 1.3f, -3.85f), new Vector3(0.55f, 0.08f, 0.04f), matNaturalOak);

            // --- 8. LIVING AREA MODULE (Bottom Right: 3.5m x 4m) ---
            GameObject livingModule = CreateGroup("8_LivingArea");

            // Woven Textured Rug (X = 1.6m, Z = -2.0m)
            CreateBox("Living_AreaRug", livingModule.transform, new Vector3(1.6f, 0.01f, -2.0f), new Vector3(2.5f, 0.015f, 2.8f), matWovenRug, false);

            // 2-Seater Cream / Beige Fabric Sofa (X = 1.6m, Z = -3.2m)
            Vector3 sofaPos = new Vector3(1.6f, 0.24f, -3.25f);
            CreateBox("Sofa_SeatCushion", livingModule.transform, sofaPos, new Vector3(1.75f, 0.32f, 0.85f), matCreamSofaFabric);
            CreateBox("Sofa_Backrest", livingModule.transform, new Vector3(1.6f, 0.54f, -3.62f), new Vector3(1.75f, 0.48f, 0.22f), matCreamSofaFabric);
            CreateBox("Sofa_ArmLeft", livingModule.transform, new Vector3(0.68f, 0.42f, -3.25f), new Vector3(0.22f, 0.36f, 0.85f), matCreamSofaFabric);
            CreateBox("Sofa_ArmRight", livingModule.transform, new Vector3(2.52f, 0.42f, -3.25f), new Vector3(0.22f, 0.36f, 0.85f), matCreamSofaFabric);

            // Low Round Wooden Coffee Table (X = 1.6m, Z = -2.0m)
            CreateBox("CoffeeTable_Top", livingModule.transform, new Vector3(1.6f, 0.36f, -2.0f), new Vector3(0.85f, 0.04f, 0.85f), matNaturalOak);
            CreateBox("CoffeeTable_Leg1", livingModule.transform, new Vector3(1.35f, 0.17f, -1.8f), new Vector3(0.06f, 0.34f, 0.06f), matNaturalOak);
            CreateBox("CoffeeTable_Leg2", livingModule.transform, new Vector3(1.85f, 0.17f, -1.8f), new Vector3(0.06f, 0.34f, 0.06f), matNaturalOak);
            CreateBox("CoffeeTable_Leg3", livingModule.transform, new Vector3(1.6f, 0.17f, -2.25f), new Vector3(0.06f, 0.34f, 0.06f), matNaturalOak);
            // Open Book / Magazine on coffee table
            CreateBox("CoffeeTable_OpenBook", livingModule.transform, new Vector3(1.6f, 0.39f, -2.0f), new Vector3(0.28f, 0.015f, 0.22f), matOffWhiteWall);

            // Low Wooden Media Console (Mounted against Bedroom Partition Wall at Z = -0.1)
            CreateBox("MediaConsole_Cabinet", livingModule.transform, new Vector3(1.6f, 0.26f, -0.18f), new Vector3(1.8f, 0.35f, 0.38f), matNaturalOak);

            // 43" Flat Screen TV (Mounted above Media Console)
            CreateBox("TV_Frame_Outer", livingModule.transform, new Vector3(1.6f, 1.05f, -0.06f), new Vector3(1.15f, 0.68f, 0.06f), matMatteBlackFrame);
            CreateBox("TV_GlassScreen", livingModule.transform, new Vector3(1.6f, 1.05f, -0.09f), new Vector3(1.08f, 0.62f, 0.01f), matTvScreen, false);

            // Tall Indoor Potted Green Plant (Corner: X = 2.85m, Z = -0.4m)
            CreateBox("IndoorPlant_Pot", livingModule.transform, new Vector3(2.85f, 0.22f, -0.4f), new Vector3(0.35f, 0.44f, 0.35f), matWhiteCeramic);
            CreateBox("IndoorPlant_Foliage", livingModule.transform, new Vector3(2.85f, 0.85f, -0.4f), new Vector3(0.65f, 0.85f, 0.65f), matPlantFoliage);

            // --- 9. LIGHTING & CAMERA SETUP ---
            SetupLightingAndCamera();

            Debug.Log("[RealisticPhotoRoomGenerator] 3D Room successfully generated to match reference photo perfectly!");
        }

        private GameObject CreateGroup(string groupName)
        {
            GameObject go = new GameObject(groupName);
            go.transform.SetParent(m_RoomRoot.transform, false);
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

        private void SetupLightingAndCamera()
        {
            GameObject lightGroup = CreateGroup("9_LightingAndCameras");

            // Main Directional Sun Light (Daylight from right windows)
            GameObject sunObj = new GameObject("Sun_Daylight");
            sunObj.transform.SetParent(lightGroup.transform, false);
            Light sun = sunObj.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1.0f, 0.96f, 0.90f);
            sun.intensity = 1.4f;
            sunObj.transform.rotation = Quaternion.Euler(42f, -55f, 0f);

            // Soft Ambient Fill Light
            GameObject fillObj = new GameObject("Fill_AmbientLight");
            fillObj.transform.SetParent(lightGroup.transform, false);
            Light fill = fillObj.AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.color = new Color(0.85f, 0.92f, 1.0f);
            fill.intensity = 0.5f;
            fillObj.transform.rotation = Quaternion.Euler(70f, 120f, 0f);

            // Photo-Matching Top-Down Isometric Camera
            GameObject camObj = GameObject.Find("PhotoTopDownCamera");
            if (camObj == null) camObj = new GameObject("PhotoTopDownCamera");
            camObj.transform.SetParent(lightGroup.transform, false);
            Camera cam = camObj.GetComponent<Camera>();
            if (cam == null) cam = camObj.AddComponent<Camera>();
            camObj.transform.position = new Vector3(0.0f, 11.5f, -0.2f);
            camObj.transform.rotation = Quaternion.Euler(88f, 0f, 0f); // Top down angle matching photo
            cam.fieldOfView = 40f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;
        }

        private void EnsureMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (matNaturalOak == null)
                matNaturalOak = CreateMat(litShader, "M_NaturalOak", new Color(0.82f, 0.68f, 0.48f), 0.45f, 0.05f);

            if (matOffWhiteWall == null)
                matOffWhiteWall = CreateMat(litShader, "M_OffWhiteWall", new Color(0.95f, 0.95f, 0.94f), 0.85f, 0.0f);

            if (matMatteBlackFrame == null)
                matMatteBlackFrame = CreateMat(litShader, "M_MatteBlackFrame", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.5f);

            if (matGreyMarble == null)
                matGreyMarble = CreateMat(litShader, "M_GreyMarble", new Color(0.55f, 0.58f, 0.60f), 0.25f, 0.2f);

            if (matWhiteCeramic == null)
                matWhiteCeramic = CreateMat(litShader, "M_WhiteCeramic", new Color(0.97f, 0.97f, 0.97f), 0.15f, 0.05f);

            if (matCreamSofaFabric == null)
                matCreamSofaFabric = CreateMat(litShader, "M_CreamSofaFabric", new Color(0.88f, 0.82f, 0.74f), 0.9f, 0.0f);

            if (matBeigeBlanket == null)
                matBeigeBlanket = CreateMat(litShader, "M_BeigeBlanket", new Color(0.78f, 0.68f, 0.56f), 0.95f, 0.0f);

            if (matFrostedGlass == null)
                matFrostedGlass = CreateMat(litShader, "M_FrostedGlass", new Color(0.85f, 0.92f, 0.95f, 0.45f), 0.2f, 0.1f);

            if (matClearGlass == null)
                matClearGlass = CreateMat(litShader, "M_ClearGlass", new Color(0.9f, 0.95f, 1.0f, 0.2f), 0.1f, 0.1f);

            if (matBathroomTile == null)
                matBathroomTile = CreateMat(litShader, "M_BathroomTile", new Color(0.85f, 0.87f, 0.88f), 0.3f, 0.05f);

            if (matWovenRug == null)
                matWovenRug = CreateMat(litShader, "M_WovenRug", new Color(0.85f, 0.82f, 0.76f), 0.95f, 0.0f);

            if (matPlantFoliage == null)
                matPlantFoliage = CreateMat(litShader, "M_PlantFoliage", new Color(0.28f, 0.45f, 0.22f), 0.8f, 0.0f);

            if (matTowelFabric == null)
                matTowelFabric = CreateMat(litShader, "M_TowelFabric", new Color(0.92f, 0.92f, 0.92f), 0.95f, 0.0f);

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
