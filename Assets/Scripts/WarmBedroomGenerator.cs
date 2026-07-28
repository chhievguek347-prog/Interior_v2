using System.Collections.Generic;
using UnityEngine;

namespace Interior.WarmBedroom
{
    public class WarmBedroomGenerator : MonoBehaviour
    {
        [Header("Room Dimensions (Real-World Scale)")]
        public float roomWidth = 4.0f;   // X axis (-2.0 to +2.0)
        public float roomLength = 4.5f;  // Z axis (-2.25 to +2.25)
        public float roomHeight = 2.7f;  // Y axis (0 to 2.7)
        public float wallThickness = 0.18f;

        [Header("PBR Material Slots")]
        public Material matWoodPlankFloor;
        public Material matWarmWall;
        public Material matBedding;
        public Material matWoodWarm;
        public Material matCurtainSheer;
        public Material matMetalBlack;
        public Material matGlassClear;
        public Material matRugSoft;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_RoomContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateWarmBedroom();
            }
        }

        [ContextMenu("Generate Warm Modern Bedroom")]
        public void GenerateWarmBedroom()
        {
            Transform existing = transform.Find("WarmModernBedroom");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_RoomContainer = new GameObject("WarmModernBedroom");
            m_RoomContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            float halfW = roomWidth * 0.5f;
            float halfL = roomLength * 0.5f;

            // --- 1. Architecture (Floor, Ceiling, Walls, Window, Door) ---
            GameObject archGroup = CreateSubContainer("1_Architecture");

            // Wood-Plank Flooring (4m x 4.5m)
            CreateBox("Floor_WoodPlanks", archGroup.transform, new Vector3(0, -0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWoodPlankFloor);

            // Ceiling (2.7m height)
            CreateBox("Ceiling_Main", archGroup.transform, new Vector3(0, roomHeight + 0.05f, 0), new Vector3(roomWidth, 0.1f, roomLength), matWarmWall);

            // Back Wall (Z = +halfL = 2.25)
            CreateBox("Wall_Back", archGroup.transform, new Vector3(0, roomHeight * 0.5f, halfL + wallThickness * 0.5f), new Vector3(roomWidth + wallThickness * 2, roomHeight, wallThickness), matWarmWall);

            // Left Wall (X = -halfW = -2.0)
            CreateBox("Wall_Left", archGroup.transform, new Vector3(-halfW - wallThickness * 0.5f, roomHeight * 0.5f, 0), new Vector3(wallThickness, roomHeight, roomLength), matWarmWall);

            // Front Wall with Wooden Interior Exit Door Cutout (Z = -halfL = -2.25)
            float doorW = 1.0f;
            float doorH = 2.1f;
            float doorX = -1.1f;
            float frontLeftW = (doorX - (-halfW)) - doorW * 0.5f;
            float frontRightW = (halfW - doorX) - doorW * 0.5f;

            CreateBox("Wall_Front_Left", archGroup.transform, new Vector3(-halfW + frontLeftW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontLeftW, roomHeight, wallThickness), matWarmWall);
            CreateBox("Wall_Front_Right", archGroup.transform, new Vector3(halfW - frontRightW * 0.5f, roomHeight * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(frontRightW, roomHeight, wallThickness), matWarmWall);
            CreateBox("Wall_Front_DoorHeader", archGroup.transform, new Vector3(doorX, doorH + (roomHeight - doorH) * 0.5f, -halfL - wallThickness * 0.5f), new Vector3(doorW, roomHeight - doorH, wallThickness), matWarmWall);
            // Wooden Interior Exit Door & Frame
            CreateBox("Interior_DoorFrame", archGroup.transform, new Vector3(doorX, doorH * 0.5f, -halfL), new Vector3(doorW, doorH, 0.08f), matWoodWarm, false);
            CreateBox("Interior_DoorPanel", archGroup.transform, new Vector3(doorX + 0.05f, doorH * 0.5f, -halfL + 0.02f), new Vector3(doorW - 0.08f, doorH - 0.04f, 0.04f), matWoodWarm, false);

            // Right Wall with Framed Glass Window & Sheer Curtains (X = +halfW = 2.0)
            float winW = 1.8f;
            float winSillH = 0.8f;
            float winTopH = 2.4f;
            float winH = winTopH - winSillH;
            float rightSideL = (roomLength - winW) * 0.5f;

            CreateBox("Wall_Right_South", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, -halfL + rightSideL * 0.5f), new Vector3(wallThickness, roomHeight, rightSideL), matWarmWall);
            CreateBox("Wall_Right_North", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, roomHeight * 0.5f, halfL - rightSideL * 0.5f), new Vector3(wallThickness, roomHeight, rightSideL), matWarmWall);
            CreateBox("Wall_Right_Sill", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH * 0.5f, 0), new Vector3(wallThickness, winSillH, winW), matWarmWall);
            CreateBox("Wall_Right_Header", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winTopH + (roomHeight - winTopH) * 0.5f, 0), new Vector3(wallThickness, roomHeight - winTopH, winW), matWarmWall);

            // Window Frame & Glass Pane
            CreateBox("Window_BlackFrame", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.08f, winH, winW), matMetalBlack, false);
            CreateBox("Window_GlassPane", archGroup.transform, new Vector3(halfW + wallThickness * 0.5f, winSillH + winH * 0.5f, 0), new Vector3(0.02f, winH - 0.08f, winW - 0.08f), matGlassClear, false);
            // Sheer Curtains Rod & Curtains
            CreateBox("Curtain_Rod", archGroup.transform, new Vector3(halfW - 0.06f, winTopH + 0.1f, 0), new Vector3(0.04f, 0.04f, winW + 0.4f), matMetalBlack, false);
            CreateBox("Curtain_SheerLeft", archGroup.transform, new Vector3(halfW - 0.08f, winSillH + winH * 0.5f - 0.2f, -winW * 0.4f), new Vector3(0.02f, winH + 0.4f, 0.45f), matCurtainSheer, false);
            CreateBox("Curtain_SheerRight", archGroup.transform, new Vector3(halfW - 0.08f, winSillH + winH * 0.5f - 0.2f, winW * 0.4f), new Vector3(0.02f, winH + 0.4f, 0.45f), matCurtainSheer, false);

            // --- 2. Furnishings ---
            GameObject furnGroup = CreateSubContainer("2_Furnishings");

            // Queen Bed with Detailed Bedding (X = 0.5, Z = 0.9)
            Vector3 bedCenter = new Vector3(0.5f, 0.2f, 0.9f);
            CreateBox("QueenBed_Frame", furnGroup.transform, bedCenter, new Vector3(1.65f, 0.35f, 2.15f), matWoodWarm);
            CreateBox("QueenBed_Headboard", furnGroup.transform, new Vector3(0.5f, 0.65f, 1.92f), new Vector3(1.65f, 0.95f, 0.15f), matBedding);
            CreateBox("QueenBed_MattressDuvet", furnGroup.transform, new Vector3(0.5f, 0.48f, 0.8f), new Vector3(1.55f, 0.28f, 1.95f), matBedding);
            CreateBox("QueenBed_Pillows", furnGroup.transform, new Vector3(0.5f, 0.64f, 1.6f), new Vector3(1.45f, 0.16f, 0.42f), matBedding);

            // Two Nightstands with Lamps
            CreateBox("Nightstand_Left", furnGroup.transform, new Vector3(-0.55f, 0.25f, 1.7f), new Vector3(0.38f, 0.48f, 0.42f), matWoodWarm);
            CreateBox("Nightstand_Right", furnGroup.transform, new Vector3(1.55f, 0.25f, 1.7f), new Vector3(0.38f, 0.48f, 0.42f), matWoodWarm);
            CreateBox("BedsideLamp_Left", furnGroup.transform, new Vector3(-0.55f, 0.65f, 1.7f), new Vector3(0.18f, 0.32f, 0.18f), matMetalBlack, false);
            CreateBox("BedsideLamp_Right", furnGroup.transform, new Vector3(1.55f, 0.65f, 1.7f), new Vector3(0.18f, 0.32f, 0.18f), matMetalBlack, false);

            // Full-Height Wardrobe Closet (Along Left Wall: X = -1.6, Z = 0.8)
            CreateBox("Wardrobe_Closet", furnGroup.transform, new Vector3(-1.65f, 1.15f, 0.8f), new Vector3(0.55f, 2.3f, 1.6f), matWoodWarm);

            // Desk / Vanity with Chair (X = -1.2, Z = -1.2)
            CreateBox("DeskVanity_Surface", furnGroup.transform, new Vector3(-1.2f, 0.38f, -1.2f), new Vector3(1.2f, 0.74f, 0.55f), matWoodWarm);
            CreateBox("DeskVanity_Mirror", furnGroup.transform, new Vector3(-1.2f, 1.3f, -1.45f), new Vector3(0.6f, 0.75f, 0.04f), matMetalBlack, false);
            CreateBox("Desk_Chair", furnGroup.transform, new Vector3(-1.2f, 0.25f, -0.65f), new Vector3(0.45f, 0.82f, 0.45f), matWoodWarm);

            // Soft Textured Area Rug (Under bed: X = 0.5, Z = 0.4)
            CreateBox("Area_Rug", furnGroup.transform, new Vector3(0.5f, 0.01f, 0.4f), new Vector3(2.2f, 0.02f, 2.6f), matRugSoft, false);

            // Simple Wall Decor (Framed Art on Back Wall)
            CreateBox("WallArt_Frame", furnGroup.transform, new Vector3(0.5f, 1.6f, 2.18f), new Vector3(1.2f, 0.8f, 0.04f), matMetalBlack, false);

            // Ceiling Light Fixture (Center: X = 0, Y = 2.65, Z = 0)
            CreateBox("Ceiling_LightFixture", furnGroup.transform, new Vector3(0f, 2.62f, 0f), new Vector3(0.35f, 0.12f, 0.35f), matMetalBlack, false);

            Debug.Log("[WarmBedroomGenerator] 4m x 4.5m Warm Modern Bedroom successfully generated!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_RoomContainer.transform, false);
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

            if (matWoodPlankFloor == null) matWoodPlankFloor = CreateMat(litShader, "Mat_WoodPlank_Floor", new Color(0.72f, 0.52f, 0.35f), 0.45f, 0.05f);
            if (matWarmWall == null) matWarmWall = CreateMat(litShader, "Mat_WarmWall", new Color(0.92f, 0.90f, 0.86f), 0.85f, 0.0f);
            if (matBedding == null) matBedding = CreateMat(litShader, "Mat_Bedding", new Color(0.95f, 0.94f, 0.92f), 0.9f, 0.0f);
            if (matWoodWarm == null) matWoodWarm = CreateMat(litShader, "Mat_Wood_Warm", new Color(0.65f, 0.45f, 0.28f), 0.45f, 0.05f);
            if (matCurtainSheer == null) matCurtainSheer = CreateMat(litShader, "Mat_Curtain_Sheer", new Color(0.95f, 0.95f, 0.95f, 0.45f), 0.9f, 0.0f);
            if (matMetalBlack == null) matMetalBlack = CreateMat(litShader, "Mat_Metal_Black", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
            if (matGlassClear == null) matGlassClear = CreateMat(litShader, "Mat_Glass_Clear", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
            if (matRugSoft == null) matRugSoft = CreateMat(litShader, "Mat_Rug_Soft", new Color(0.85f, 0.82f, 0.75f), 0.95f, 0.0f);
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
