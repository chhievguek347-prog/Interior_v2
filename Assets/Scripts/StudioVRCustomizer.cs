using System.Collections.Generic;
using UnityEngine;

namespace Interior.VRCustomizer
{
    public class StudioVRCustomizer : MonoBehaviour
    {
        [Header("Target Mesh Renderers")]
        public List<MeshRenderer> mainWallRenderers = new List<MeshRenderer>();
        public List<MeshRenderer> sofaRenderers = new List<MeshRenderer>();
        public List<MeshRenderer> bedDuvetRenderers = new List<MeshRenderer>();
        public GameObject upholsteredHeadboardObj;
        public GameObject woodenHeadboardObj;

        [Header("Wall Color Presets")]
        public Color[] wallColors = new Color[]
        {
            new Color(0.92f, 0.88f, 0.82f), // Warm Sand
            new Color(0.95f, 0.95f, 0.94f), // Crisp White
            new Color(0.72f, 0.74f, 0.76f), // Modern Concrete
            new Color(0.78f, 0.82f, 0.75f)  // Sage Tint
        };

        [Header("Sofa Fabric Presets")]
        public Color[] sofaColors = new Color[]
        {
            new Color(0.85f, 0.80f, 0.72f), // Beige Cream
            new Color(0.24f, 0.26f, 0.28f), // Charcoal Grey
            new Color(0.78f, 0.42f, 0.32f), // Terracotta Warm
            new Color(0.48f, 0.58f, 0.46f)  // Sage Green
        };

        [Header("Bed Duvet Presets")]
        public Color[] duvetColors = new Color[]
        {
            new Color(0.95f, 0.95f, 0.95f), // White Linen
            new Color(0.38f, 0.42f, 0.46f), // Slate Grey
            new Color(0.82f, 0.62f, 0.65f), // Dusty Rose
            new Color(0.85f, 0.68f, 0.32f)  // Mustard Gold
        };

        private int m_CurrentWallIndex = 0;
        private int m_CurrentSofaIndex = 0;
        private int m_CurrentDuvetIndex = 0;
        private bool m_IsUpholsteredHeadboard = true;

        private void Start()
        {
            FindRenderersIfEmpty();
        }

        public void SetWallPreset(int index)
        {
            if (index < 0 || index >= wallColors.Length) return;
            m_CurrentWallIndex = index;
            Color c = wallColors[index];

            foreach (MeshRenderer mr in mainWallRenderers)
            {
                if (mr != null)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    mr.GetPropertyBlock(block);
                    block.SetColor("_BaseColor", c);
                    block.SetColor("_Color", c);
                    mr.SetPropertyBlock(block);
                }
            }
            Debug.Log($"[StudioVRCustomizer] Swapped Main Wall Color to Preset #{index}");
        }

        public void SetSofaPreset(int index)
        {
            if (index < 0 || index >= sofaColors.Length) return;
            m_CurrentSofaIndex = index;
            Color c = sofaColors[index];

            foreach (MeshRenderer mr in sofaRenderers)
            {
                if (mr != null)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    mr.GetPropertyBlock(block);
                    block.SetColor("_BaseColor", c);
                    block.SetColor("_Color", c);
                    mr.SetPropertyBlock(block);
                }
            }
            Debug.Log($"[StudioVRCustomizer] Swapped Sofa Fabric to Preset #{index}");
        }

        public void SetDuvetPreset(int index)
        {
            if (index < 0 || index >= duvetColors.Length) return;
            m_CurrentDuvetIndex = index;
            Color c = duvetColors[index];

            foreach (MeshRenderer mr in bedDuvetRenderers)
            {
                if (mr != null)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    mr.GetPropertyBlock(block);
                    block.SetColor("_BaseColor", c);
                    block.SetColor("_Color", c);
                    mr.SetPropertyBlock(block);
                }
            }
            Debug.Log($"[StudioVRCustomizer] Swapped Bed Duvet to Preset #{index}");
        }

        public void ToggleBedFrameStyle()
        {
            m_IsUpholsteredHeadboard = !m_IsUpholsteredHeadboard;
            if (upholsteredHeadboardObj != null) upholsteredHeadboardObj.SetActive(m_IsUpholsteredHeadboard);
            if (woodenHeadboardObj != null) woodenHeadboardObj.SetActive(!m_IsUpholsteredHeadboard);
            Debug.Log($"[StudioVRCustomizer] Toggled Bed Frame Style (Upholstered: {m_IsUpholsteredHeadboard})");
        }

        private void FindRenderersIfEmpty()
        {
            GameObject root = GameObject.Find("VRStudioApartment");
            if (root == null) return;

            if (mainWallRenderers.Count == 0)
            {
                foreach (MeshRenderer mr in root.GetComponentsInChildren<MeshRenderer>())
                {
                    if (mr.gameObject.name.StartsWith("Wall_") && !mr.gameObject.name.Contains("Glass"))
                    {
                        mainWallRenderers.Add(mr);
                    }
                }
            }

            if (sofaRenderers.Count == 0)
            {
                foreach (MeshRenderer mr in root.GetComponentsInChildren<MeshRenderer>())
                {
                    if (mr.gameObject.name.StartsWith("Sofa_"))
                    {
                        sofaRenderers.Add(mr);
                    }
                }
            }

            if (bedDuvetRenderers.Count == 0)
            {
                foreach (MeshRenderer mr in root.GetComponentsInChildren<MeshRenderer>())
                {
                    if (mr.gameObject.name.Contains("Duvet") || mr.gameObject.name.Contains("Pillow"))
                    {
                        bedDuvetRenderers.Add(mr);
                    }
                }
            }
        }
    }
}
