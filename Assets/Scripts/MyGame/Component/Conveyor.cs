using System;
using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using MyGame.Data;
using MyGame.Manager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Conveyor : Singleton<Conveyor>
{
    [SerializeField] private MeshFilter m_meshFilter;
    [SerializeField] private int m_NumberConveyor;
    [SerializeField] private SplineContainer m_SplineContainer;
    [SerializeField] private GameObject[] m_Tunners = new GameObject[2]; 
    [SerializeField] private GameObject m_PrefabArrow;
    [SerializeField] private GameObject m_PrefabSpawn;
    [SerializeField] public List<GameObject> m_spawnPoint;
    public Dictionary<int, bool> checkSpawnPoint = new Dictionary<int, bool>();
    private int numBase = 4;

    public void Initialize()
    {
        m_NumberConveyor = LevelRemoteManager.Instance.levelInfo.mapID;
        
        m_meshFilter.mesh = Resources.Load<Mesh>(PathNameResource.PathMeshConveyer + m_NumberConveyor.ToString());
        
        GameObject pathObj = Resources.Load<GameObject>(PathNameResource.PathSpline + m_NumberConveyor.ToString());
        m_SplineContainer = pathObj.GetComponent<SplineContainer>();
        AddArrow(numBase);
        AddTunnel();
    }

    public void AddArrow(int numArrow)
    {
       
        foreach (GameObject spawn in m_spawnPoint)
        {
            Destroy(spawn.gameObject);
        }
        m_spawnPoint.Clear();
        if(numArrow == numBase)
            checkSpawnPoint.Clear();
        // int numArrow = Mathf.CeilToInt(m_SplineContainer.CalculateLength() / 5);
        for (int i = 0; i < numArrow; i++)
                {
                    var spawnObject = Instantiate(m_PrefabSpawn,transform);
                    var arrowObject = Instantiate(m_PrefabArrow,spawnObject.transform);
                    if(!checkSpawnPoint.ContainsKey(i))
                        checkSpawnPoint.Add(i, false);
                    SplineAnimate splineAnimate = spawnObject.GetComponent<SplineAnimate>();
                    SplineAnimate arrowAnimate = arrowObject.GetComponent<SplineAnimate>();
                    splineAnimate.Container = m_SplineContainer;
                    splineAnimate.StartOffset = 1 - (1.0f / numArrow) * i - 0.07f;
                    splineAnimate.Restart(true);
                    arrowAnimate.Container = m_SplineContainer;
                    arrowAnimate.StartOffset = 1 - (1.0f / numArrow) * i;
                    arrowAnimate.Restart(true);
                    m_spawnPoint.Add(spawnObject);
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
            Vector3 offSet = new Vector3(0,1,0);
            m_Tunners[0].transform.position -= offSet;
            m_Tunners[1].transform.SetPositionAndRotation(
                containerTf.TransformPoint(endKnot.Position) , 
                endKnot.Rotation * Quaternion.Euler(0, 180, 0)
            );
            m_Tunners[1].transform.position -= offSet;
        }
    }
}
