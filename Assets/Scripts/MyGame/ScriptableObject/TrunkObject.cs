using System;
using UnityEngine;
using System.Collections.Generic;
using MyGame;
using MyGame.Data;

public class TrunkObject : MonoBehaviour
{
    [Header("Color Settings")]
    public AllColor[] colorLayers = new AllColor[3]; 

    [Header("Status Settings")]
    public bool hasLock;
    public bool isChained;
    public bool isFrozen;
    public bool isBlock;

    [Header("Visibility")]
    [Range(1, 3)]
    public int visibleLayerCount = 3;

    [SerializeField] private BigTrunk m_Bigtrunk;
    public int amountUpTrunk; 

    private void Awake()
    {
        m_Bigtrunk = GetComponent<BigTrunk>();
    }

    void Update()
    {
        isChained =  m_Bigtrunk.TrunkData.isChained;
        isFrozen =  m_Bigtrunk.TrunkData.isFrozen;
        isBlock =  m_Bigtrunk.TrunkData.isBlock;
        hasLock =  m_Bigtrunk.TrunkData.hasLock;
        visibleLayerCount = m_Bigtrunk.TrunkData.visibleLayerCount;
        colorLayers = m_Bigtrunk.TrunkData.colorLayers; 
    }

    // Vẽ Gizmos để dễ nhìn trong Scene
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
        // Hiển thị tên màu trên đầu (nếu cần debug)
    }
}