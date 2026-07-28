using System.Collections.Generic;
using UnityEngine;

namespace Interior.KitchenFurniture
{
    public class KitchenFurnitureSetGenerator : MonoBehaviour
    {
        [Header("PBR Material Slots")]
        public Material matOakCabinet;
        public Material matStoneQuartz;
        public Material matChromeReflect;
        public Material matStainlessSteel;
        public Material matGlassPendant;
        public Material matCeramicWhite;
        public Material matMetalBlack;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_KitchenContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateHighPolyKitchenFurnitureSet();
            }
        }

        [ContextMenu("Generate High-Poly Kitchen Furniture Set")]
        public void GenerateHighPolyKitchenFurnitureSet()
        {
            Transform existing = transform.Find("HighPolyKitchenFurnitureSet");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_KitchenContainer = new GameObject("HighPolyKitchenFurnitureSet");
            m_KitchenContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            // --- 1. L-Shaped Countertop & Cabinets ---
            GameObject counterGroup = CreateSubContainer("1_L_CountertopAndCabinets");
            // Lower Cabinets
            CreateSubBox("LowerCabinets_LeftRun", counterGroup.transform, new Vector3(-1.42f, 0.45f, 0.1f), new Vector3(0.62f, 0.90f, 2.80f), matOakCabinet);
            CreateSubBox("LowerCabinets_BackRun", counterGroup.transform, new Vector3(-0.50f, 0.45f, 1.68f), new Vector3(1.40f, 0.90f, 0.62f), matOakCabinet);
            // Cabinet Handles
            CreateSubBox("Lower_Handle1", counterGroup.transform, new Vector3(-1.10f, 0.65f, -0.6f), new Vector3(0.03f, 0.14f, 0.03f), matChromeReflect, false);
            CreateSubBox("Lower_Handle2", counterGroup.transform, new Vector3(-1.10f, 0.65f, 0.8f), new Vector3(0.03f, 0.14f, 0.03f), matChromeReflect, false);

            // Quartz Stone Countertop
            CreateSubBox("Countertop_LeftStone", counterGroup.transform, new Vector3(-1.42f, 0.92f, 0.1f), new Vector3(0.66f, 0.06f, 2.85f), matStoneQuartz);
            CreateSubBox("Countertop_BackStone", counterGroup.transform, new Vector3(-0.50f, 0.92f, 1.68f), new Vector3(1.45f, 0.06f, 0.66f), matStoneQuartz);

            // Upper Cabinets & Hinges
            CreateSubBox("UpperCabinets_Left", counterGroup.transform, new Vector3(-1.48f, 2.10f, 0.1f), new Vector3(0.42f, 0.75f, 2.40f), matOakCabinet);
            CreateSubBox("UpperCabinets_Back", counterGroup.transform, new Vector3(-0.50f, 2.10f, 1.72f), new Vector3(1.30f, 0.75f, 0.42f), matOakCabinet);
            CreateSubBox("Upper_Handle1", counterGroup.transform, new Vector3(-1.26f, 1.85f, -0.5f), new Vector3(0.03f, 0.14f, 0.03f), matChromeReflect, false);

            // --- 2. Built-in Sink & Curved Metallic Chrome Faucet ---
            GameObject sinkGroup = CreateSubContainer("2_SinkAndChromeFaucet");
            CreateSubBox("Undermount_SinkBasin", sinkGroup.transform, new Vector3(-1.42f, 0.95f, 0.4f), new Vector3(0.45f, 0.02f, 0.58f), matStainlessSteel);
            CreateSubBox("Faucet_Base", sinkGroup.transform, new Vector3(-1.65f, 0.98f, 0.4f), new Vector3(0.08f, 0.06f, 0.08f), matChromeReflect, false);
            CreateSubBox("Faucet_CurvedSpout", sinkGroup.transform, new Vector3(-1.65f, 1.15f, 0.4f), new Vector3(0.06f, 0.28f, 0.06f), matChromeReflect, false);
            CreateSubBox("Faucet_LeverHandle", sinkGroup.transform, new Vector3(-1.65f, 1.05f, 0.48f), new Vector3(0.04f, 0.08f, 0.12f), matChromeReflect, false);

            // --- 3. Induction Cooktop & Range Hood ---
            GameObject cookGroup = CreateSubContainer("3_CooktopAndRangeHood");
            CreateSubBox("Induction_CooktopGlass", cookGroup.transform, new Vector3(-0.50f, 0.95f, 1.68f), new Vector3(0.45f, 0.02f, 0.62f), matMetalBlack);
            CreateSubBox("Range_HoodBody", cookGroup.transform, new Vector3(-0.50f, 1.90f, 1.65f), new Vector3(0.52f, 0.45f, 0.52f), matStainlessSteel);
            CreateSubBox("Range_HoodChimney", cookGroup.transform, new Vector3(-0.50f, 2.45f, 1.65f), new Vector3(0.28f, 0.65f, 0.28f), matStainlessSteel, false);

            // --- 4. Stainless Steel Refrigerator ---
            GameObject fridgeGroup = CreateSubContainer("4_StainlessRefrigerator");
            Vector3 fPos = new Vector3(-1.42f, 0.95f, -1.35f);
            CreateSubBox("Fridge_Body", fridgeGroup.transform, fPos, new Vector3(0.65f, 1.90f, 0.72f), matStainlessSteel);
            CreateSubBox("Fridge_DoorLine", fridgeGroup.transform, fPos + new Vector3(0.33f, 0f, 0f), new Vector3(0.02f, 1.85f, 0.02f), matMetalBlack, false);
            CreateSubBox("Fridge_HandleLeft", fridgeGroup.transform, fPos + new Vector3(0.34f, 0.25f, -0.15f), new Vector3(0.04f, 0.65f, 0.04f), matChromeReflect, false);
            CreateSubBox("Fridge_HandleRight", fridgeGroup.transform, fPos + new Vector3(0.34f, 0.25f, 0.15f), new Vector3(0.04f, 0.65f, 0.04f), matChromeReflect, false);

            // --- 5. Small Dining Table & 4 Chairs ---
            GameObject diningGroup = CreateSubContainer("5_DiningTableAndChairs");
            Vector3 dPos = new Vector3(0.75f, 0.38f, -0.5f);
            CreateSubBox("Dining_TableTop", diningGroup.transform, dPos + new Vector3(0f, 0.35f, 0f), new Vector3(0.95f, 0.05f, 0.95f), matOakCabinet);
            CreateSubBox("Dining_TableLeg1", diningGroup.transform, dPos + new Vector3(-0.40f, 0f, -0.40f), new Vector3(0.06f, 0.70f, 0.06f), matOakCabinet);
            CreateSubBox("Dining_TableLeg2", diningGroup.transform, dPos + new Vector3(0.40f, 0f, -0.40f), new Vector3(0.06f, 0.70f, 0.06f), matOakCabinet);
            CreateSubBox("Dining_TableLeg3", diningGroup.transform, dPos + new Vector3(-0.40f, 0f, 0.40f), new Vector3(0.06f, 0.70f, 0.06f), matOakCabinet);
            CreateSubBox("Dining_TableLeg4", diningGroup.transform, dPos + new Vector3(0.40f, 0f, 0.40f), new Vector3(0.06f, 0.70f, 0.06f), matOakCabinet);

            // 4 Matching Chairs
            Vector3[] chairPositions = new Vector3[]
            {
                new Vector3(0.75f, 0.25f, -1.15f),
                new Vector3(0.75f, 0.25f, 0.15f),
                new Vector3(0.10f, 0.25f, -0.50f),
                new Vector3(1.40f, 0.25f, -0.50f)
            };
            for (int i = 0; i < chairPositions.Length; i++)
            {
                CreateSubBox("Chair_" + (i + 1) + "_Seat", diningGroup.transform, chairPositions[i] + new Vector3(0f, 0.20f, 0f), new Vector3(0.45f, 0.06f, 0.45f), matOakCabinet);
                CreateSubBox("Chair_" + (i + 1) + "_Back", diningGroup.transform, chairPositions[i] + new Vector3(0f, 0.48f, -0.18f), new Vector3(0.45f, 0.50f, 0.05f), matOakCabinet);
            }

            // --- 6. Open Shelving & Pantry (Jars & Plates) ---
            GameObject shelfGroup = CreateSubContainer("6_OpenShelvingAndPantry");
            Vector3 sPos = new Vector3(0.75f, 1.6f, 1.72f);
            CreateSubBox("Shelf_Plank1", shelfGroup.transform, sPos, new Vector3(0.95f, 0.04f, 0.28f), matOakCabinet, false);
            CreateSubBox("Shelf_Plank2", shelfGroup.transform, sPos + new Vector3(0f, 0.45f, 0f), new Vector3(0.95f, 0.04f, 0.28f), matOakCabinet, false);

            // Glass Jars & Ceramic Plates
            CreateSubBox("Glass_StorageJar1", shelfGroup.transform, sPos + new Vector3(-0.28f, 0.15f, 0f), new Vector3(0.14f, 0.22f, 0.14f), matGlassPendant, false);
            CreateSubBox("Glass_StorageJar2", shelfGroup.transform, sPos + new Vector3(-0.10f, 0.15f, 0f), new Vector3(0.14f, 0.22f, 0.14f), matGlassPendant, false);
            CreateSubBox("Ceramic_PlatesStack", shelfGroup.transform, sPos + new Vector3(0.25f, 0.12f, 0f), new Vector3(0.22f, 0.16f, 0.22f), matCeramicWhite, false);

            // --- 7. Countertop Clutter (Kettle, Toaster, Cutting Board, Utensils, Fruit Bowl) ---
            GameObject clutterGroup = CreateSubContainer("7_CountertopClutter");
            // Electric Kettle
            CreateSubBox("Electric_Kettle", clutterGroup.transform, new Vector3(-1.42f, 1.08f, -0.5f), new Vector3(0.18f, 0.26f, 0.18f), matStainlessSteel, false);
            // Two-Slice Toaster
            CreateSubBox("Toaster_Appliance", clutterGroup.transform, new Vector3(-1.42f, 1.05f, 1.3f), new Vector3(0.22f, 0.18f, 0.16f), matCeramicWhite, false);
            // Wooden Cutting Board
            CreateSubBox("Cutting_BoardWood", clutterGroup.transform, new Vector3(-1.42f, 0.96f, -0.1f), new Vector3(0.35f, 0.02f, 0.25f), matOakCabinet, false);
            // Utensil Holder
            CreateSubBox("Utensil_HolderMetal", clutterGroup.transform, new Vector3(-1.42f, 1.05f, 1.0f), new Vector3(0.15f, 0.22f, 0.15f), matStainlessSteel, false);
            // Fruit Bowl with Fruits
            CreateSubBox("Fruit_BowlCeramic", clutterGroup.transform, new Vector3(0.75f, 0.81f, -0.5f), new Vector3(0.28f, 0.08f, 0.28f), matCeramicWhite, false);

            // --- 8. Glass & Metal Pendant Lights ---
            GameObject pendantGroup = CreateSubContainer("8_GlassPendantLights");
            CreateSubBox("Pendant_DiningGlass", pendantGroup.transform, new Vector3(0.75f, 2.15f, -0.5f), new Vector3(0.25f, 0.35f, 0.25f), matGlassPendant, false);
            CreateSubBox("Pendant_CounterGlass", pendantGroup.transform, new Vector3(-0.8f, 2.15f, 0.4f), new Vector3(0.25f, 0.35f, 0.25f), matGlassPendant, false);

            Debug.Log("[KitchenFurnitureSetGenerator] High-Poly 3D Kitchen Furniture & Fixtures Set successfully generated!");
        }

        private GameObject CreateSubContainer(string name)
        {
            GameObject c = new GameObject(name);
            c.transform.SetParent(m_KitchenContainer.transform, false);
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

            if (matOakCabinet == null) matOakCabinet = CreateMat(litShader, "Mat_Oak_Cabinet", new Color(0.68f, 0.52f, 0.36f), 0.45f, 0.05f);
            if (matStoneQuartz == null) matStoneQuartz = CreateMat(litShader, "Mat_Stone_Quartz", new Color(0.38f, 0.42f, 0.45f), 0.3f, 0.15f);
            if (matChromeReflect == null) matChromeReflect = CreateMat(litShader, "Mat_Chrome_Reflect", new Color(0.85f, 0.88f, 0.90f), 0.15f, 0.9f);
            if (matStainlessSteel == null) matStainlessSteel = CreateMat(litShader, "Mat_Stainless_Steel", new Color(0.78f, 0.80f, 0.82f), 0.25f, 0.8f);
            if (matGlassPendant == null) matGlassPendant = CreateMat(litShader, "Mat_Glass_Pendant", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
            if (matCeramicWhite == null) matCeramicWhite = CreateMat(litShader, "Mat_Ceramic_White", new Color(0.96f, 0.96f, 0.96f), 0.15f, 0.1f);
            if (matMetalBlack == null) matMetalBlack = CreateMat(litShader, "Mat_Metal_Black", new Color(0.12f, 0.12f, 0.14f), 0.35f, 0.6f);
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
