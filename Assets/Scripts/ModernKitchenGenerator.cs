using System.Collections.Generic;
using UnityEngine;

namespace Interior.ModernKitchen
{
    public class ModernKitchenGenerator : MonoBehaviour
    {
        [Header("Room Dimensions (Real-World Scale)")]
        public float roomWidth = 3.5f;   // X axis (-1.75 to +1.75)
        public float roomLength = 4.0f;  // Z axis (-2.0 to +2.0)
        public float roomHeight = 2.7f;  // Y axis (0 to 2.7)
        public float wallThickness = 0.18f;

        [Header("PBR Material Slots")]
        public Material matKitchenFloorTile;
        public Material matBacksplashTile;
        public Material matStoneCountertop;
        public Material matCabinetWood;
        public Material matMetalStainless;
        public Material matMetalBlack;
        public Material matWallPlaster;
        public Material matGlassClear;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_KitchenContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateModernKitchen();
            }
        }

        [ContextMenu("Generate Modern Kitchen")]
        public void GenerateModernKitchen()
        {
            Transform existing = transform.Find("ModernKitchenInterior");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_KitchenContainer = new GameObject("ModernKitchenInterior");
            m_KitchenContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. Architecture (Floor, Ceiling, Walls, Window, Entrance) ---
            GameObject archGroup = CreateSubContainer("1_Architecture");

            // Ceramic Tile Flooring (3.5m x 4.0m)
            CreateBox("Floor_CeramicTile", archGroup.transform, new Vector3(0, -0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matKitchenFloorTile);

            // Ceiling (2.7m height)
            CreateBox("Ceiling_Main", archGroup.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWallPlaster);

            // Back Wall (Z = +halfL = 2.0)
            CreateBox("Wall_Back", archGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), matWallPlaster);

            // Left Wall (X = -halfW = -1.75)
            CreateBox("Wall_Left", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matWallPlaster);

            // Front Entrance Wall with Doorway Cutout (Z = -halfL = -2.0)
            float doorW = 1.1f;
            float doorH = 2.1f;
            float doorX = 0.3f;
            float frontLeftW = (doorX - (-halfW)) - doorW * 0.5f;
            float frontRightW = (halfW - doorX) - doorW * 0.5f;

            CreateBox("Wall_Front_Left", archGroup.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matWallPlaster);
            CreateBox("Wall_Front_Right", archGroup.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matWallPlaster);
            CreateBox("Wall_Front_DoorHeader", archGroup.transform, new Vector3(doorX, doorH + (roomHeight - doorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorW, roomHeight - doorH, wallThickness), matWallPlaster);

            // Right Wall with Framed Glass Window Cutout (X = +halfW = 1.75)
            float winW = 1.8f;
            float winSillH = 0.85f;
            float winTopH = 2.45f;
            float winH = winTopH - winSillH;
            float rightSideL = (roomLength - winW) * 0.5f;

            CreateBox("Wall_Right_South", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, -halfL + rightSideL * 0.5f), new Vector3(wallThickness, roomHeight, rightSideL), matWallPlaster);
            CreateBox("Wall_Right_North", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, halfL - rightSideL * 0.5f), new Vector3(wallThickness, roomHeight, rightSideL), matWallPlaster);
            CreateBox("Wall_Right_Sill", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, winW), matWallPlaster);
            CreateBox("Wall_Right_Header", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, winW), matWallPlaster);
            CreateBox("Kitchen_WindowFrame", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.08f, winH, winW), matMetalBlack, false);
            CreateBox("Kitchen_WindowGlass", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.02f, winH - 0.08f, winW - 0.08f), matGlassClear, false);

            // Tiled Backsplash Wall along left and back counters
            CreateBox("Backsplash_Left", archGroup.transform, new Vector3(-halfW + 0.02f, 1.35f, 0.2f), new Vector3(0.04f, 0.8f, 2.6f), matBacksplashTile, false);
            CreateBox("Backsplash_Back", archGroup.transform, new Vector3(-0.5f, 1.35f, halfL - 0.02f), new Vector3(2.4f, 0.8f, 0.04f), matBacksplashTile, false);

            // --- 2. L-Shaped Kitchen Counter, Cabinets & Appliances ---
            GameObject kitchenGroup = CreateSubContainer("2_KitchenCounterAndAppliances");

            // L-Shaped Lower Cabinets (Left Wall & Back Wall)
            CreateBox("LowerCabinets_LeftRun", kitchenGroup.transform, new Vector3(-1.42f, 0.45f, 0.1f), new Vector3(0.62f, 0.9f, 2.8f), matCabinetWood);
            CreateBox("LowerCabinets_BackRun", kitchenGroup.transform, new Vector3(-0.5f, 0.45f, 1.68f), new Vector3(1.4f, 0.9f, 0.62f), matCabinetWood);

            // L-Shaped Stone Countertop
            CreateBox("Countertop_LeftRun", kitchenGroup.transform, new Vector3(-1.42f, 0.92f, 0.1f), new Vector3(0.66f, 0.06f, 2.85f), matStoneCountertop);
            CreateBox("Countertop_BackRun", kitchenGroup.transform, new Vector3(-0.5f, 0.92f, 1.68f), new Vector3(1.45f, 0.06f, 0.66f), matStoneCountertop);

            // Undermount Sink & Metallic Curved Faucet (on Left counter run)
            CreateBox("Undermount_Sink", kitchenGroup.transform, new Vector3(-1.42f, 0.95f, 0.4f), new Vector3(0.45f, 0.02f, 0.58f), matMetalStainless);
            CreateBox("Curved_Faucet", kitchenGroup.transform, new Vector3(-1.65f, 1.12f, 0.4f), new Vector3(0.06f, 0.28f, 0.06f), matMetalStainless, false);

            // Black Induction Cooktop & Stainless Range Hood (on Back counter run)
            CreateBox("Induction_Cooktop", kitchenGroup.transform, new Vector3(-0.5f, 0.95f, 1.68f), new Vector3(0.45f, 0.02f, 0.62f), matMetalBlack);
            CreateBox("Range_Hood", kitchenGroup.transform, new Vector3(-0.5f, 1.9f, 1.65f), new Vector3(0.52f, 0.45f, 0.52f), matMetalStainless);

            // Upper Overhead Cabinets (Left & Back walls)
            CreateBox("UpperCabinets_Left", kitchenGroup.transform, new Vector3(-1.48f, 2.1f, 0.1f), new Vector3(0.42f, 0.75f, 2.4f), matCabinetWood);
            CreateBox("UpperCabinets_Back", kitchenGroup.transform, new Vector3(-0.5f, 2.1f, 1.72f), new Vector3(1.3f, 0.75f, 0.42f), matCabinetWood);

            // Full-Size Modern Refrigerator
            CreateBox("Refrigerator_Unit", kitchenGroup.transform, new Vector3(-1.42f, 0.95f, -1.35f), new Vector3(0.65f, 1.9f, 0.72f), matMetalStainless);

            // --- 3. Dining Nook (Table & 4 Chairs) ---
            GameObject diningGroup = CreateSubContainer("3_DiningNook");

            // Small Wooden Dining Table (X = 0.75, Z = -0.5)
            CreateBox("Dining_Table", diningGroup.transform, new Vector3(0.75f, 0.38f, -0.5f), new Vector3(0.95f, 0.75f, 0.95f), matCabinetWood);
            // 4 Dining Chairs
            CreateBox("Dining_Chair1", diningGroup.transform, new Vector3(0.75f, 0.25f, -1.15f), new Vector3(0.45f, 0.82f, 0.45f), matCabinetWood);
            CreateBox("Dining_Chair2", diningGroup.transform, new Vector3(0.75f, 0.25f, 0.15f), new Vector3(0.45f, 0.82f, 0.45f), matCabinetWood);
            CreateBox("Dining_Chair3", diningGroup.transform, new Vector3(0.1f, 0.25f, -0.5f), new Vector3(0.45f, 0.82f, 0.45f), matCabinetWood);
            CreateBox("Dining_Chair4", diningGroup.transform, new Vector3(1.4f, 0.25f, -0.5f), new Vector3(0.45f, 0.82f, 0.45f), matCabinetWood);

            // --- 4. Lighting & Realistic Countertop Clutter ---
            GameObject detailsGroup = CreateSubContainer("4_DetailsAndLighting");

            // Pendant Lights (over dining table and over counter)
            CreateBox("PendantLight_Dining", detailsGroup.transform, new Vector3(0.75f, 2.15f, -0.5f), new Vector3(0.25f, 0.35f, 0.25f), matMetalBlack, false);
            CreateBox("PendantLight_Counter", detailsGroup.transform, new Vector3(-0.8f, 2.15f, 0.4f), new Vector3(0.25f, 0.35f, 0.25f), matMetalBlack, false);

            // Countertop Clutter (Utensil Holder, Wooden Cutting Board, Herb Plant)
            CreateBox("Utensil_Holder", detailsGroup.transform, new Vector3(-1.42f, 1.05f, 1.1f), new Vector3(0.15f, 0.22f, 0.15f), matMetalStainless, false);
            CreateBox("Cutting_Board", detailsGroup.transform, new Vector3(-1.42f, 0.96f, -0.2f), new Vector3(0.35f, 0.02f, 0.25f), matCabinetWood, false);
            CreateBox("Potted_HerbPlant", detailsGroup.transform, new Vector3(0.15f, 1.05f, 1.68f), new Vector3(0.18f, 0.24f, 0.18f), matCabinetWood, false);

            Debug.Log("[ModernKitchenGenerator] 3.5m x 4.0m Modern Kitchen successfully generated!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_KitchenContainer.transform, false);
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

            if (matKitchenFloorTile == null) matKitchenFloorTile = CreateMat(litShader, "Mat_KitchenFloor_Tile", new Color(0.62f, 0.65f, 0.68f), 0.4f, 0.05f);
            if (matBacksplashTile == null) matBacksplashTile = CreateMat(litShader, "Mat_Backsplash_Tile", new Color(0.92f, 0.94f, 0.95f), 0.35f, 0.05f);
            if (matStoneCountertop == null) matStoneCountertop = CreateMat(litShader, "Mat_Stone_Countertop", new Color(0.35f, 0.38f, 0.40f), 0.3f, 0.15f);
            if (matCabinetWood == null) matCabinetWood = CreateMat(litShader, "Mat_Cabinet_Wood", new Color(0.68f, 0.52f, 0.36f), 0.45f, 0.05f);
            if (matMetalStainless == null) matMetalStainless = CreateMat(litShader, "Mat_Metal_Stainless", new Color(0.82f, 0.85f, 0.88f), 0.25f, 0.8f);
            if (matMetalBlack == null) matMetalBlack = CreateMat(litShader, "Mat_Metal_Black", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
            if (matWallPlaster == null) matWallPlaster = CreateMat(litShader, "Mat_Wall_Plaster", new Color(0.94f, 0.92f, 0.88f), 0.85f, 0.0f);
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
