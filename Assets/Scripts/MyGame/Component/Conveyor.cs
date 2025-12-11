using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject m_PrefabTunnel;
    [SerializeField] private GameObject m_PrefabArrow;
    [SerializeField] private GameObject m_PrefabSpawn;
    [SerializeField] public List<GameObject> m_spawnPoint;
    [SerializeField] public List<GameObject> m_arrowPoint;
    public Dictionary<int, bool> checkSpawnPoint = new Dictionary<int, bool>();
    

    public void Initialize()
    {
        m_NumberConveyor = LevelRemoteManager.Instance.levelInfo.mapID;
        
        m_meshFilter.mesh = Resources.Load<Mesh>(PathNameResource.PathMeshConveyer + m_NumberConveyor.ToString());
        
        GameObject pathObj = Resources.Load<GameObject>(PathNameResource.PathSpline + m_NumberConveyor.ToString());
        m_SplineContainer = pathObj.GetComponent<SplineContainer>();
        AddArrow();
        AddTunnel();
    }

    void AddArrow()
    {
       
        foreach (GameObject spawn in m_spawnPoint)
        {
            Destroy(spawn.gameObject);
        }
        foreach (GameObject arrow in m_arrowPoint)
        {
            Destroy(arrow.gameObject);
        }
        m_spawnPoint.Clear();
        m_arrowPoint.Clear();
        checkSpawnPoint.Clear();
        int numArrow = Mathf.CeilToInt(m_SplineContainer.CalculateLength() / 5);
        for (int i = 0; i < numArrow; i++)
        {
            var spawnObject = Instantiate(m_PrefabSpawn,transform);
            var arrowObject = Instantiate(m_PrefabArrow,transform);
            checkSpawnPoint.Add(i, false);
            SplineAnimate splineAnimate = spawnObject.GetComponent<SplineAnimate>();
            SplineAnimate arrowAnimate = arrowObject.GetComponent<SplineAnimate>();
            splineAnimate.Container = m_SplineContainer;
            splineAnimate.StartOffset = 1 - (1.0f / numArrow) * i;
            splineAnimate.Restart(true);
            arrowAnimate.Container = m_SplineContainer;
            arrowAnimate.StartOffset = 1 - (1.0f / numArrow) * i - 0.1f;
            arrowAnimate.Restart(true);
            m_spawnPoint.Add(spawnObject);
            m_arrowPoint.Add(arrowObject);
        }
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
