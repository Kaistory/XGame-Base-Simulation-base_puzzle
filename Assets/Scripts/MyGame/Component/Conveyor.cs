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
        Spline spline = m_SplineContainer.Spline;
        
        if (spline.Count == 0) return;
        
        BezierKnot startKnot = spline[0];
        BezierKnot endKnot = spline[spline.Count - 1];

        
        Vector3 startPosWorld = m_SplineContainer.transform.TransformPoint(startKnot.Position);
        Vector3 endPosWorld = m_SplineContainer.transform.TransformPoint(endKnot.Position);
        

        if(!spline.Closed)
        {
            m_Tunners[0].SetActive(true);
            m_Tunners[1].SetActive(true);
            m_Tunners[0].transform.position = startPosWorld;
            m_Tunners[0].transform.rotation = startKnot.Rotation;
            m_Tunners[1].transform.position = endPosWorld;
            m_Tunners[1].transform.rotation = endKnot.Rotation;
            m_Tunners[1].transform.Rotate(0, 180, 0);
            
        }
        else
        {
            m_Tunners[0].SetActive(false);
            m_Tunners[1].SetActive(false);
        }
    }

}
