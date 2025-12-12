using System;
using UnityEngine;

namespace MyGame
{
    public class BlockKey : MonoBehaviour
    {
        [SerializeField] private BigTrunk m_bigTrunk;
        private Animator m_animator;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (m_bigTrunk.TrunkData.isBlock)
            {
                m_animator.enabled = false;
            }
        }
    }
}