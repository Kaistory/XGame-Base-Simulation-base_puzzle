using System;
using DG.Tweening;
using MyBox;
using MyGame.Data;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Conveyor : Singleton<Conveyor>
{
    [SerializeField] private MeshFilter m_meshFilter;
    [SerializeField] private int m_NumberConveyor;
    [SerializeField] private SplineContainer m_SplineContainer;
    [SerializeField] private GameObject[] m_Tunners = new GameObject[2];
    [SerializeField] private GameObject m_Prefab;
    
    

    public void Initialize()
    {
        m_NumberConveyor = LevelRemoteManager.Instance.levelInfo.mapID;
        
        m_meshFilter.mesh = Resources.Load<Mesh>(PathNameResource.PathMeshConveyer + m_NumberConveyor.ToString());
        
        GameObject pathObj = Resources.Load<GameObject>(PathNameResource.PathSpline + m_NumberConveyor.ToString());
        m_SplineContainer = pathObj.GetComponent<SplineContainer>();
        
        AddTunnel();
    }

    void AddTunnel()
    {
        var spline = m_SplineContainer.Spline;
        if (spline.Count == 0) return;

        bool isOpen = !spline.Closed;

        m_Tunners[0].SetActive(isOpen);
        m_Tunners[1].SetActive(isOpen);

        if (isOpen)
        {
            Transform containerTf = m_SplineContainer.transform;
        
            var startKnot = spline[0];
            var endKnot = spline[^1];

            m_Tunners[0].transform.SetPositionAndRotation(
                containerTf.TransformPoint(startKnot.Position), 
                startKnot.Rotation
            );

            m_Tunners[1].transform.SetPositionAndRotation(
                containerTf.TransformPoint(endKnot.Position), 
                endKnot.Rotation * Quaternion.Euler(0, 180, 0)
            );
        }
    }
}
