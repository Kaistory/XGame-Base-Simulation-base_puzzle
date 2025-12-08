using System;
using MyGame.Data;
using UnityEngine;

namespace MyGame
{
    public class Truck : MonoBehaviour
    {
        // Color
        [SerializeField] private MeshRenderer truck_meshRenderer;
        [SerializeField] private AllColor m_Allcolor;

        public AllColor MAllcolor
        {
            get => m_Allcolor;
            set => m_Allcolor = value;
        }

        [SerializeField] private Material[] basicMaterial = new Material[1];

        void Awake()
        {
            truck_meshRenderer = GetComponent<MeshRenderer>();
        }

        public void ChangeColor(AllColor color)
        {
            m_Allcolor = color;
            basicMaterial[0] = (Resources.Load<Material>(PathNameResource.PathMaterial + "m_mainColor_"+ m_Allcolor.ToString()));
            truck_meshRenderer.materials = basicMaterial;
        }
        
    }
}

