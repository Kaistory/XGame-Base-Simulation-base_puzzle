using UnityEngine;

namespace MyGame
{
    public class GlassManager : MonoBehaviour
    {
        [Header("Object con glass")] 
        [SerializeField] private GameObject[] m_glass = new GameObject[2];
        
        [SerializeField] private int m_idxGlassPlate;

        private void OnValidate()
        {
            SetUpGlassPlate(m_idxGlassPlate);
        }

        void SetUpGlassPlate(int num)
        {
            m_glass[0].SetActive(false);
            m_glass[1].SetActive(false);
            switch (num)
            {
                case 1:
                    m_glass[0].SetActive(true);
                    break;
                case 2:    
                    m_glass[1].SetActive(true);
                    break;
            }
        }
    }
}