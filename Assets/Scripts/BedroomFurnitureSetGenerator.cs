using System.Collections.Generic;
using UnityEngine;

namespace Interior.BedroomFurniture
{
    public class BedroomFurnitureSetGenerator : MonoBehaviour
    {
        [Header("PBR Material Slots")]
        public Material matOakGrain;
        public Material matFabricBedding;
        public Material matFabricBlanket;
        public Material matMetalBrushed;
        public Material matLampshadeFabric;
        public Material matRugTexture;
        public Material matArtGlass;

        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool addPhysicsColliders = true;

        private GameObject m_FurnitureContainer;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateHighPolyFurnitureSet();
            }
        }

        [ContextMenu("Generate High-Poly Bedroom Furniture Set")]
        public void GenerateHighPolyFurnitureSet()
        {
            Transform existing = transform.Find("HighPolyBedroomFurnitureSet");
            if (existing != null)
            {
                if (Application.isPlaying) Destroy(existing.gameObject);
                else DestroyImmediate(existing.gameObject);
            }

            m_FurnitureContainer = new GameObject("HighPolyBedroomFurnitureSet");
            m_FurnitureContainer.transform.SetParent(transform, false);

            EnsureMaterials();

            // --- 1. Queen Bed with Detailed Wrinkled Bedding & Pillows ---
            GameObject bedGroup = CreateSubContainer("1_QueenBed_Detailed");
            Vector3 bedCenter = new Vector3(0.5f, 0f, 0.9f);

            // Tapered Wooden Leg Supports
            CreateSubBox("Bed_Leg_FL", bedGroup.transform, bedCenter + new Vector3(-0.75f, 0.08f, -0.95f), new Vector3(0.08f, 0.16f, 0.08f), matOakGrain);
            CreateSubBox("Bed_Leg_FR", bedGroup.transform, bedCenter + new Vector3(0.75f, 0.08f, -0.95f), new Vector3(0.08f, 0.16f, 0.08f), matOakGrain);
            CreateSubBox("Bed_Leg_BL", bedGroup.transform, bedCenter + new Vector3(-0.75f, 0.08f, 0.95f), new Vector3(0.08f, 0.16f, 0.08f), matOakGrain);
            CreateSubBox("Bed_Leg_BR", bedGroup.transform, bedCenter + new Vector3(0.75f, 0.08f, 0.95f), new Vector3(0.08f, 0.16f, 0.08f), matOakGrain);

            // Wood Platform Frame
            CreateSubBox("Bed_PlatformFrame", bedGroup.transform, bedCenter + new Vector3(0f, 0.22f, 0f), new Vector3(1.65f, 0.18f, 2.15f), matOakGrain);
            // Detailed Headboard with Trim
            CreateSubBox("Bed_HeadboardMain", bedGroup.transform, bedCenter + new Vector3(0f, 0.65f, 1.02f), new Vector3(1.65f, 0.95f, 0.12f), matOakGrain);
            CreateSubBox("Bed_HeadboardCap", bedGroup.transform, bedCenter + new Vector3(0f, 1.15f, 1.02f), new Vector3(1.72f, 0.06f, 0.16f), matOakGrain);

            // Mattress
            CreateSubBox("Bed_Mattress", bedGroup.transform, bedCenter + new Vector3(0f, 0.38f, -0.05f), new Vector3(1.55f, 0.22f, 2.05f), matFabricBedding);
            // Wrinkled Duvet Cover
            CreateSubBox("Bed_DuvetMain", bedGroup.transform, bedCenter + new Vector3(0f, 0.52f, -0.1f), new Vector3(1.58f, 0.14f, 1.95f), matFabricBedding);
            CreateSubBox("Bed_DuvetSkirt_L", bedGroup.transform, bedCenter + new Vector3(-0.78f, 0.42f, -0.1f), new Vector3(0.06f, 0.28f, 1.95f), matFabricBedding, false);
            CreateSubBox("Bed_DuvetSkirt_R", bedGroup.transform, bedCenter + new Vector3(0.78f, 0.42f, -0.1f), new Vector3(0.06f, 0.28f, 1.95f), matFabricBedding, false);

            // Folded Throw Blanket with Creases
            CreateSubBox("Bed_ThrowBlanket", bedGroup.transform, bedCenter + new Vector3(0f, 0.58f, -0.6f), new Vector3(1.62f, 0.08f, 0.85f), matFabricBlanket);

            // 4 Layered Pillows
            CreateSubBox("Pillow_BackLeft", bedGroup.transform, bedCenter + new Vector3(-0.4f, 0.62f, 0.72f), new Vector3(0.68f, 0.16f, 0.42f), matFabricBedding);
            CreateSubBox("Pillow_BackRight", bedGroup.transform, bedCenter + new Vector3(0.4f, 0.62f, 0.72f), new Vector3(0.68f, 0.16f, 0.42f), matFabricBedding);
            CreateSubBox("Pillow_FrontLeft", bedGroup.transform, bedCenter + new Vector3(-0.4f, 0.65f, 0.52f), new Vector3(0.64f, 0.14f, 0.38f), matFabricBlanket);
            CreateSubBox("Pillow_FrontRight", bedGroup.transform, bedCenter + new Vector3(0.4f, 0.65f, 0.52f), new Vector3(0.64f, 0.14f, 0.38f), matFabricBlanket);

            // --- 2. Two Matching Nightstands with Table Lamps ---
            GameObject nightstandGroup = CreateSubContainer("2_NightstandsAndLamps");

            // Left Nightstand
            Vector3 nsLeftPos = new Vector3(-0.55f, 0.26f, 1.7f);
            CreateSubBox("Nightstand_L_Body", nightstandGroup.transform, nsLeftPos, new Vector3(0.45f, 0.52f, 0.42f), matOakGrain);
            CreateSubBox("Nightstand_L_Drawer1", nightstandGroup.transform, nsLeftPos + new Vector3(0f, 0.12f, 0.02f), new Vector3(0.40f, 0.20f, 0.38f), matOakGrain, false);
            CreateSubBox("Nightstand_L_Drawer2", nightstandGroup.transform, nsLeftPos + new Vector3(0f, -0.12f, 0.02f), new Vector3(0.40f, 0.20f, 0.38f), matOakGrain, false);
            CreateSubBox("Nightstand_L_Handle1", nightstandGroup.transform, nsLeftPos + new Vector3(0f, 0.12f, -0.20f), new Vector3(0.14f, 0.03f, 0.03f), matMetalBrushed, false);
            CreateSubBox("Nightstand_L_Handle2", nightstandGroup.transform, nsLeftPos + new Vector3(0f, -0.12f, -0.20f), new Vector3(0.14f, 0.03f, 0.03f), matMetalBrushed, false);
            // Lamp Left
            CreateSubBox("Lamp_L_Base", nightstandGroup.transform, nsLeftPos + new Vector3(0f, 0.30f, 0f), new Vector3(0.12f, 0.10f, 0.12f), matMetalBrushed, false);
            CreateSubBox("Lamp_L_Stem", nightstandGroup.transform, nsLeftPos + new Vector3(0f, 0.42f, 0f), new Vector3(0.03f, 0.24f, 0.03f), matMetalBrushed, false);
            CreateSubBox("Lamp_L_Shade", nightstandGroup.transform, nsLeftPos + new Vector3(0f, 0.62f, 0f), new Vector3(0.24f, 0.22f, 0.24f), matLampshadeFabric, false);

            // Right Nightstand
            Vector3 nsRightPos = new Vector3(1.55f, 0.26f, 1.7f);
            CreateSubBox("Nightstand_R_Body", nightstandGroup.transform, nsRightPos, new Vector3(0.45f, 0.52f, 0.42f), matOakGrain);
            CreateSubBox("Nightstand_R_Drawer1", nightstandGroup.transform, nsRightPos + new Vector3(0f, 0.12f, 0.02f), new Vector3(0.40f, 0.20f, 0.38f), matOakGrain, false);
            CreateSubBox("Nightstand_R_Drawer2", nightstandGroup.transform, nsRightPos + new Vector3(0f, -0.12f, 0.02f), new Vector3(0.40f, 0.20f, 0.38f), matOakGrain, false);
            CreateSubBox("Nightstand_R_Handle1", nightstandGroup.transform, nsRightPos + new Vector3(0f, 0.12f, -0.20f), new Vector3(0.14f, 0.03f, 0.03f), matMetalBrushed, false);
            CreateSubBox("Nightstand_R_Handle2", nightstandGroup.transform, nsRightPos + new Vector3(0f, -0.12f, -0.20f), new Vector3(0.14f, 0.03f, 0.03f), matMetalBrushed, false);
            // Lamp Right
            CreateSubBox("Lamp_R_Base", nightstandGroup.transform, nsRightPos + new Vector3(0f, 0.30f, 0f), new Vector3(0.12f, 0.10f, 0.12f), matMetalBrushed, false);
            CreateSubBox("Lamp_R_Stem", nightstandGroup.transform, nsRightPos + new Vector3(0f, 0.42f, 0f), new Vector3(0.03f, 0.24f, 0.03f), matMetalBrushed, false);
            CreateSubBox("Lamp_R_Shade", nightstandGroup.transform, nsRightPos + new Vector3(0f, 0.62f, 0f), new Vector3(0.24f, 0.22f, 0.24f), matLampshadeFabric, false);

            // --- 3. Full-Height Wardrobe Closet with Door Handles & Hinges ---
            GameObject wardrobeGroup = CreateSubContainer("3_WardrobeCloset");
            Vector3 wPos = new Vector3(-1.65f, 1.15f, 0.8f);
            CreateSubBox("Wardrobe_CabinetBody", wardrobeGroup.transform, wPos, new Vector3(0.60f, 2.30f, 1.60f), matOakGrain);
            CreateSubBox("Wardrobe_DoorLeft", wardrobeGroup.transform, wPos + new Vector3(0.28f, 0f, -0.38f), new Vector3(0.04f, 2.22f, 0.78f), matOakGrain, false);
            CreateSubBox("Wardrobe_DoorRight", wardrobeGroup.transform, wPos + new Vector3(0.28f, 0f, 0.38f), new Vector3(0.04f, 2.22f, 0.78f), matOakGrain, false);
            // Door Handles & Hinges
            CreateSubBox("Wardrobe_HandleL", wardrobeGroup.transform, wPos + new Vector3(0.32f, 0f, -0.04f), new Vector3(0.03f, 0.28f, 0.03f), matMetalBrushed, false);
            CreateSubBox("Wardrobe_HandleR", wardrobeGroup.transform, wPos + new Vector3(0.32f, 0f, 0.04f), new Vector3(0.03f, 0.28f, 0.03f), matMetalBrushed, false);

            // --- 4. Study Desk / Vanity & Chair ---
            GameObject deskGroup = CreateSubContainer("4_StudyDeskAndChair");
            Vector3 dPos = new Vector3(-1.2f, 0.38f, -1.2f);
            CreateSubBox("Desk_Surface", deskGroup.transform, dPos + new Vector3(0f, 0.35f, 0f), new Vector3(1.20f, 0.06f, 0.55f), matOakGrain);
            CreateSubBox("Desk_LegLeft", deskGroup.transform, dPos + new Vector3(-0.55f, 0f, 0f), new Vector3(0.06f, 0.68f, 0.52f), matOakGrain);
            CreateSubBox("Desk_DrawerCabinet", deskGroup.transform, dPos + new Vector3(0.35f, -0.05f, 0f), new Vector3(0.42f, 0.58f, 0.52f), matOakGrain);
            CreateSubBox("Desk_DrawerHandle1", deskGroup.transform, dPos + new Vector3(0.35f, 0.12f, 0.28f), new Vector3(0.14f, 0.03f, 0.03f), matMetalBrushed, false);
            CreateSubBox("Desk_DrawerHandle2", deskGroup.transform, dPos + new Vector3(0.35f, -0.12f, 0.28f), new Vector3(0.14f, 0.03f, 0.03f), matMetalBrushed, false);

            // Matching Chair
            Vector3 chairPos = dPos + new Vector3(0f, -0.13f, 0.55f);
            CreateSubBox("Chair_SeatCushion", deskGroup.transform, chairPos + new Vector3(0f, 0.25f, 0f), new Vector3(0.45f, 0.08f, 0.45f), matFabricBlanket);
            CreateSubBox("Chair_Backrest", deskGroup.transform, chairPos + new Vector3(0f, 0.52f, 0.18f), new Vector3(0.45f, 0.48f, 0.06f), matOakGrain);
            CreateSubBox("Chair_LegFL", deskGroup.transform, chairPos + new Vector3(-0.18f, 0f, -0.18f), new Vector3(0.05f, 0.46f, 0.05f), matMetalBrushed);
            CreateSubBox("Chair_LegFR", deskGroup.transform, chairPos + new Vector3(0.18f, 0f, -0.18f), new Vector3(0.05f, 0.46f, 0.05f), matMetalBrushed);
            CreateSubBox("Chair_LegBL", deskGroup.transform, chairPos + new Vector3(-0.18f, 0f, 0.18f), new Vector3(0.05f, 0.46f, 0.05f), matMetalBrushed);
            CreateSubBox("Chair_LegBR", deskGroup.transform, chairPos + new Vector3(0.18f, 0f, 0.18f), new Vector3(0.05f, 0.46f, 0.05f), matMetalBrushed);

            // --- 5. Soft Area Rug ---
            GameObject rugGroup = CreateSubContainer("5_SoftAreaRug");
            CreateSubBox("Soft_AreaRug", rugGroup.transform, new Vector3(0.5f, 0.01f, 0.4f), new Vector3(2.20f, 0.02f, 2.60f), matRugTexture, false);

            // --- 6. Wall Decor (Framed Artwork with Glass Pane) ---
            GameObject artGroup = CreateSubContainer("6_WallDecor");
            Vector3 artPos = new Vector3(0.5f, 1.6f, 2.18f);
            CreateSubBox("Art_WoodenFrame", artGroup.transform, artPos, new Vector3(1.20f, 0.80f, 0.04f), matOakGrain, false);
            CreateSubBox("Art_GlassPane", artGroup.transform, artPos + new Vector3(0f, 0f, -0.02f), new Vector3(1.12f, 0.72f, 0.01f), matArtGlass, false);

            Debug.Log("[BedroomFurnitureSetGenerator] High-Poly 3D Bedroom Furniture Set successfully generated!");
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

            if (matOakGrain == null) matOakGrain = CreateMat(litShader, "Mat_Oak_Grain", new Color(0.68f, 0.50f, 0.32f), 0.45f, 0.05f);
            if (matFabricBedding == null) matFabricBedding = CreateMat(litShader, "Mat_Fabric_Bedding", new Color(0.95f, 0.94f, 0.92f), 0.9f, 0.0f);
            if (matFabricBlanket == null) matFabricBlanket = CreateMat(litShader, "Mat_Fabric_Blanket", new Color(0.78f, 0.74f, 0.68f), 0.95f, 0.0f);
            if (matMetalBrushed == null) matMetalBrushed = CreateMat(litShader, "Mat_Metal_Brushed", new Color(0.22f, 0.22f, 0.25f), 0.35f, 0.7f);
            if (matLampshadeFabric == null) matLampshadeFabric = CreateMat(litShader, "Mat_Lampshade_Fabric", new Color(0.96f, 0.92f, 0.84f), 0.85f, 0.0f);
            if (matRugTexture == null) matRugTexture = CreateMat(litShader, "Mat_Rug_Texture", new Color(0.85f, 0.82f, 0.75f), 0.95f, 0.0f);
            if (matArtGlass == null) matArtGlass = CreateMat(litShader, "Mat_Art_Glass", new Color(0.9f, 0.95f, 1.0f, 0.25f), 0.1f, 0.1f);
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
