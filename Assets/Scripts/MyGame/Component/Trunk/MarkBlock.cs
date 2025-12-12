using System;
using UnityEngine;

namespace MyGame
{
    public class MarkBlock : MonoBehaviour
    {
        [SerializeField] private BigTrunk m_bigTrunk;

        private void OnEnable()
        {
            switch (m_bigTrunk.TrunkData.visibleLayerCount)
            {
                case 3:
                    transform.localScale = new Vector3(0.3f,0.3f,0.3f);
                    break;
                case 2:
                    transform.localScale = new Vector3(0.2f,0.2f,0.2f);;
                    break;
                case 1:
                    transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                    
                    break;
            }
            
        }
    }
}