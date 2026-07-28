using System.Collections.Generic;
using UnityEngine;

namespace Interior.LivingFurniture
{
    public class LivingFurnitureSetGenerator : MonoBehaviour
    {
        [Header("PBR Material Slots")]
        public Material matOakWarm;
        public Material matFabricSofa;
        public Material matFabricArmchair;
        public Material matStoneTop;
        public Material matMetalBlack;
        public Material matRugSoftPile;
        public Material matGlassReflect;
        public Material matLampshadeFabric;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_FurnitureContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateHighPolyLivingFurnitureSet();
            }
        }

        [ContextMenu("Generate High-Poly Living Furniture Set")]
        public void GenerateHighPolyLivingFurnitureSet()
        {
            Transform existing = transform.Find("HighPolyLivingFurnitureSet");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_FurnitureContainer = new GameObject("HighPolyLivingFurnitureSet");
            m_FurnitureContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            // --- 1. 3-Seater Sofa with Plush Cushions & Natural Wrinkles ---
            GameObject sofaGroup = CreateSubContainer("1_Sofa3Seater_Detailed");
            Vector3 sofaCenter = new Vector3(1.75f, 0f, -0.8f);

            // Tapered Wooden Leg Supports
            CreateSubBox("Sofa_Leg_FL", sofaGroup.transform, sofaCenter + new Vector3(-0.85f, 0.08f, -0.38f), new Vector3(0.08f, 0.16f, 0.08f), matOakWarm);
            CreateSubBox("Sofa_Leg_FR", sofaGroup.transform, sofaCenter + new Vector3(0.85f, 0.08f, -0.38f), new Vector3(0.08f, 0.16f, 0.08f), matOakWarm);
            CreateSubBox("Sofa_Leg_BL", sofaGroup.transform, sofaCenter + new Vector3(-0.85f, 0.08f, 0.38f), new Vector3(0.08f, 0.16f, 0.08f), matOakWarm);
            CreateSubBox("Sofa_Leg_BR", sofaGroup.transform, sofaCenter + new Vector3(0.85f, 0.08f, 0.38f), new Vector3(0.08f, 0.16f, 0.08f), matOakWarm);

            // Main Base Frame
            CreateSubBox("Sofa_BaseFrame", sofaGroup.transform, sofaCenter + new Vector3(0f, 0.22f, 0f), new Vector3(1.85f, 0.14f, 0.85f), matOakWarm);
            // Armrests Left & Right
            CreateSubBox("Sofa_Armrest_L", sofaGroup.transform, sofaCenter + new Vector3(-0.86f, 0.48f, 0f), new Vector3(0.16f, 0.38f, 0.85f), matFabricSofa);
            CreateSubBox("Sofa_Armrest_R", sofaGroup.transform, sofaCenter + new Vector3(0.86f, 0.48f, 0f), new Vector3(0.16f, 0.38f, 0.85f), matFabricSofa);

            // 3 Plush Seat Cushions with Natural Wrinkles/Sagging
            CreateSubBox("Sofa_SeatCushion1", sofaGroup.transform, sofaCenter + new Vector3(-0.55f, 0.36f, -0.02f), new Vector3(0.56f, 0.18f, 0.72f), matFabricSofa);
            CreateSubBox("Sofa_SeatCushion2", sofaGroup.transform, sofaCenter + new Vector3(0.0f, 0.36f, -0.02f), new Vector3(0.56f, 0.18f, 0.72f), matFabricSofa);
            CreateSubBox("Sofa_SeatCushion3", sofaGroup.transform, sofaCenter + new Vector3(0.55f, 0.36f, -0.02f), new Vector3(0.56f, 0.18f, 0.72f), matFabricSofa);

            // 3 Backrest Cushions
            CreateSubBox("Sofa_BackCushion1", sofaGroup.transform, sofaCenter + new Vector3(-0.55f, 0.62f, 0.32f), new Vector3(0.56f, 0.48f, 0.22f), matFabricSofa);
            CreateSubBox("Sofa_BackCushion2", sofaGroup.transform, sofaCenter + new Vector3(0.0f, 0.62f, 0.32f), new Vector3(0.56f, 0.48f, 0.22f), matFabricSofa);
            CreateSubBox("Sofa_BackCushion3", sofaGroup.transform, sofaCenter + new Vector3(0.55f, 0.62f, 0.32f), new Vector3(0.56f, 0.48f, 0.22f), matFabricSofa);

            // --- 2. Coffee Table (Oak Frame & Stone Top) ---
            GameObject tableGroup = CreateSubContainer("2_CoffeeTable_StoneTop");
            Vector3 tableCenter = new Vector3(1.75f, 0f, -1.8f);
            CreateSubBox("Table_OakLeg1", tableGroup.transform, tableCenter + new Vector3(-0.48f, 0.18f, -0.28f), new Vector3(0.08f, 0.36f, 0.08f), matOakWarm);
            CreateSubBox("Table_OakLeg2", tableGroup.transform, tableCenter + new Vector3(0.48f, 0.18f, -0.28f), new Vector3(0.08f, 0.36f, 0.08f), matOakWarm);
            CreateSubBox("Table_OakLeg3", tableGroup.transform, tableCenter + new Vector3(-0.48f, 0.18f, 0.28f), new Vector3(0.08f, 0.36f, 0.08f), matOakWarm);
            CreateSubBox("Table_OakLeg4", tableGroup.transform, tableCenter + new Vector3(0.48f, 0.18f, 0.28f), new Vector3(0.08f, 0.36f, 0.08f), matOakWarm);
            CreateSubBox("Table_OakFrame", tableGroup.transform, tableCenter + new Vector3(0f, 0.34f, 0f), new Vector3(1.10f, 0.06f, 0.65f), matOakWarm);
            CreateSubBox("Table_StoneTopSlab", tableGroup.transform, tableCenter + new Vector3(0f, 0.38f, 0f), new Vector3(1.12f, 0.04f, 0.67f), matStoneTop);

            // --- 3. TV Media Console with Flat-Screen TV & Cable/Remote Clutter ---
            GameObject tvGroup = CreateSubContainer("3_TVMediaConsoleAndClutter");
            Vector3 tvCenter = new Vector3(2.78f, 0f, -2.6f);
            CreateSubBox("TV_MediaConsoleBody", tvGroup.transform, tvCenter + new Vector3(0f, 0.22f, 0f), new Vector3(0.38f, 0.35f, 1.80f), matOakWarm);
            CreateSubBox("TV_ConsoleDrawer1", tvGroup.transform, tvCenter + new Vector3(-0.02f, 0.22f, -0.45f), new Vector3(0.36f, 0.28f, 0.78f), matOakWarm, false);
            CreateSubBox("TV_ConsoleDrawer2", tvGroup.transform, tvCenter + new Vector3(-0.02f, 0.22f, 0.45f), new Vector3(0.36f, 0.28f, 0.78f), matOakWarm, false);
            CreateSubBox("TV_DrawerHandle1", tvGroup.transform, tvCenter + new Vector3(-0.21f, 0.22f, -0.45f), new Vector3(0.03f, 0.14f, 0.03f), matMetalBlack, false);
            CreateSubBox("TV_DrawerHandle2", tvGroup.transform, tvCenter + new Vector3(-0.21f, 0.22f, 0.45f), new Vector3(0.03f, 0.14f, 0.03f), matMetalBlack, false);

            // Widescreen Flat-Screen TV
            CreateSubBox("TV_StandBase", tvGroup.transform, tvCenter + new Vector3(0.05f, 0.41f, 0f), new Vector3(0.22f, 0.03f, 0.45f), matMetalBlack, false);
            CreateSubBox("TV_StandStem", tvGroup.transform, tvCenter + new Vector3(0.05f, 0.52f, 0f), new Vector3(0.06f, 0.20f, 0.12f), matMetalBlack, false);
            CreateSubBox("TV_OuterFrame", tvGroup.transform, tvCenter + new Vector3(0.12f, 1.15f, 0f), new Vector3(0.06f, 0.82f, 1.45f), matMetalBlack);
            CreateSubBox("TV_GlassScreen", tvGroup.transform, tvCenter + new Vector3(0.08f, 1.15f, 0f), new Vector3(0.02f, 0.76f, 1.38f), matMetalBlack, false);

            // Cable & Remote Control Clutter
            CreateSubBox("TV_RemoteControl", tvGroup.transform, tvCenter + new Vector3(-0.10f, 0.41f, -0.3f), new Vector3(0.16f, 0.02f, 0.06f), matMetalBlack, false);
            CreateSubBox("TV_CableBox", tvGroup.transform, tvCenter + new Vector3(-0.05f, 0.43f, 0.4f), new Vector3(0.24f, 0.05f, 0.32f), matMetalBlack, false);

            // --- 4. Accent Armchair & Side Table ---
            GameObject chairGroup = CreateSubContainer("4_AccentArmchairAndSideTable");
            Vector3 chairCenter = new Vector3(-0.1f, 0f, -1.8f);
            CreateSubBox("Armchair_BaseFrame", chairGroup.transform, chairCenter + new Vector3(0f, 0.22f, 0f), new Vector3(0.85f, 0.14f, 0.85f), matOakWarm);
            CreateSubBox("Armchair_SeatCushion", chairGroup.transform, chairCenter + new Vector3(0f, 0.36f, 0f), new Vector3(0.78f, 0.18f, 0.78f), matFabricArmchair);
            CreateSubBox("Armchair_Backrest", chairGroup.transform, chairCenter + new Vector3(-0.30f, 0.62f, 0f), new Vector3(0.22f, 0.48f, 0.78f), matFabricArmchair);
            CreateSubBox("Armchair_Leg1", chairGroup.transform, chairCenter + new Vector3(-0.35f, 0.08f, -0.35f), new Vector3(0.06f, 0.16f, 0.06f), matOakWarm);
            CreateSubBox("Armchair_Leg2", chairGroup.transform, chairCenter + new Vector3(0.35f, 0.08f, -0.35f), new Vector3(0.06f, 0.16f, 0.06f), matOakWarm);
            CreateSubBox("Armchair_Leg3", chairGroup.transform, chairCenter + new Vector3(-0.35f, 0.08f, 0.35f), new Vector3(0.06f, 0.16f, 0.06f), matOakWarm);
            CreateSubBox("Armchair_Leg4", chairGroup.transform, chairCenter + new Vector3(0.35f, 0.08f, 0.35f), new Vector3(0.06f, 0.16f, 0.06f), matOakWarm);

            // Side Table
            Vector3 sideTablePos = chairCenter + new Vector3(0f, 0f, 0.85f);
            CreateSubBox("SideTable_Top", chairGroup.transform, sideTablePos + new Vector3(0f, 0.50f, 0f), new Vector3(0.45f, 0.04f, 0.45f), matOakWarm);
            CreateSubBox("SideTable_Leg1", chairGroup.transform, sideTablePos + new Vector3(-0.18f, 0.24f, -0.18f), new Vector3(0.04f, 0.48f, 0.04f), matMetalBlack);
            CreateSubBox("SideTable_Leg2", chairGroup.transform, sideTablePos + new Vector3(0.18f, 0.24f, -0.18f), new Vector3(0.04f, 0.48f, 0.04f), matMetalBlack);
            CreateSubBox("SideTable_Leg3", chairGroup.transform, sideTablePos + new Vector3(-0.18f, 0.24f, 0.18f), new Vector3(0.04f, 0.48f, 0.04f), matMetalBlack);
            CreateSubBox("SideTable_Leg4", chairGroup.transform, sideTablePos + new Vector3(0.18f, 0.24f, 0.18f), new Vector3(0.04f, 0.48f, 0.04f), matMetalBlack);

            // --- 5. Open Bookshelf Unit with Books, Decor & Plants ---
            GameObject shelfGroup = CreateSubContainer("5_OpenBookshelfUnit");
            Vector3 shelfCenter = new Vector3(-0.1f, 0f, 0.5f);
            CreateSubBox("Bookshelf_FrameOuter", shelfGroup.transform, shelfCenter + new Vector3(0f, 0.90f, 0f), new Vector3(0.38f, 1.80f, 0.95f), matOakWarm);
            CreateSubBox("Shelf_Tier1", shelfGroup.transform, shelfCenter + new Vector3(0f, 0.45f, 0f), new Vector3(0.36f, 0.04f, 0.91f), matOakWarm, false);
            CreateSubBox("Shelf_Tier2", shelfGroup.transform, shelfCenter + new Vector3(0f, 0.90f, 0f), new Vector3(0.36f, 0.04f, 0.91f), matOakWarm, false);
            CreateSubBox("Shelf_Tier3", shelfGroup.transform, shelfCenter + new Vector3(0f, 1.35f, 0f), new Vector3(0.36f, 0.04f, 0.91f), matOakWarm, false);

            // Books, Decor & Plant Clutter on Shelves
            CreateSubBox("Book_Row1", shelfGroup.transform, shelfCenter + new Vector3(0f, 0.60f, -0.22f), new Vector3(0.24f, 0.22f, 0.35f), matFabricArmchair, false);
            CreateSubBox("Book_Row2", shelfGroup.transform, shelfCenter + new Vector3(0f, 1.05f, 0.22f), new Vector3(0.24f, 0.22f, 0.35f), matFabricSofa, false);
            CreateSubBox("Ceramic_DecorVase", shelfGroup.transform, shelfCenter + new Vector3(0f, 1.48f, -0.25f), new Vector3(0.18f, 0.22f, 0.18f), matStoneTop, false);
            CreateSubBox("Potted_ShelfPlant", shelfGroup.transform, shelfCenter + new Vector3(0f, 1.95f, 0.22f), new Vector3(0.22f, 0.25f, 0.22f), matOakWarm, false);

            // --- 6. Tall Floor Lamp ---
            GameObject lampGroup = CreateSubContainer("6_TallFloorLamp");
            Vector3 lampCenter = new Vector3(2.6f, 0f, 0.4f);
            CreateSubBox("FloorLamp_Base", lampGroup.transform, lampCenter + new Vector3(0f, 0.02f, 0f), new Vector3(0.35f, 0.04f, 0.35f), matMetalBlack, false);
            CreateSubBox("FloorLamp_Stem", lampGroup.transform, lampCenter + new Vector3(0f, 0.88f, 0f), new Vector3(0.04f, 1.70f, 0.04f), matMetalBlack, false);
            CreateSubBox("FloorLamp_Shade", lampGroup.transform, lampCenter + new Vector3(0f, 1.62f, 0f), new Vector3(0.38f, 0.32f, 0.38f), matLampshadeFabric, false);

            // --- 7. Soft Pile Area Rug ---
            GameObject rugGroup = CreateSubContainer("7_SoftPileAreaRug");
            CreateSubBox("SoftPile_AreaRug", rugGroup.transform, new Vector3(1.75f, 0.01f, -1.8f), new Vector3(2.20f, 0.02f, 2.80f), matRugSoftPile, false);

            // --- 8. Framed Wall Decor ---
            GameObject artGroup = CreateSubContainer("8_WallDecor");
            Vector3 artPos = new Vector3(1.75f, 1.6f, 0.38f);
            CreateSubBox("WallArt_Frame", artGroup.transform, artPos, new Vector3(1.35f, 0.85f, 0.04f), matOakWarm, false);
            CreateSubBox("WallArt_Glass", artGroup.transform, artPos + new Vector3(0f, 0f, -0.02f), new Vector3(1.26f, 0.76f, 0.01f), matGlassReflect, false);

            Debug.Log("[LivingFurnitureSetGenerator] High-Poly 3D Living Room Furniture Set successfully generated!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_FurnitureContainer.transform, false);
            return c;
        }

        private GameObject CreateSubBox(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool addCol = true)
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

            if (matOakWarm == null) matOakWarm = CreateMat(litShader, "Mat_Oak_Warm", new Color(0.72f, 0.54f, 0.36f), 0.45f, 0.05f);
            if (matFabricSofa == null) matFabricSofa = CreateMat(litShader, "Mat_Fabric_Sofa", new Color(0.85f, 0.82f, 0.78f), 0.9f, 0.0f);
            if (matFabricArmchair == null) matFabricArmchair = CreateMat(litShader, "Mat_Fabric_Armchair", new Color(0.38f, 0.42f, 0.46f), 0.85f, 0.0f);
            if (matStoneTop == null) matStoneTop = CreateMat(litShader, "Mat_Stone_Top", new Color(0.48f, 0.50f, 0.52f), 0.35f, 0.15f);
            if (matMetalBlack == null) matMetalBlack = CreateMat(litShader, "Mat_Metal_Black", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
            if (matRugSoftPile == null) matRugSoftPile = CreateMat(litShader, "Mat_Rug_SoftPile", new Color(0.88f, 0.85f, 0.80f), 0.95f, 0.0f);
            if (matGlassReflect == null) matGlassReflect = CreateMat(litShader, "Mat_Glass_Reflect", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
            if (matLampshadeFabric == null) matLampshadeFabric = CreateMat(litShader, "Mat_Lampshade_Fabric", new Color(0.96f, 0.92f, 0.84f), 0.85f, 0.0f);
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
