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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            IncreaseSpeed();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ReduceSpeed();
        }
    }

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
            setPositionAndRotation(0, spline);
            setPositionAndRotation(1, spline);
        }
    }

    void setPositionAndRotation(int indexTunel, Spline spline)
    {
        var Knot = (indexTunel == 1) ? spline[^1] : spline[0];
        Quaternion offsetRotation = (indexTunel == 1) ? Quaternion.Euler(0, 180, 0)
             : Quaternion.Euler(0, 0, 0);
        Transform containerTf = m_SplineContainer.transform;
        m_Tunners[indexTunel].transform.SetPositionAndRotation(
            containerTf.TransformPoint(Knot.Position), 
            (Knot.Rotation * offsetRotation)
        );
        Vector3 offSet = new Vector3(0,0,1f);
        m_Tunners[indexTunel].transform.position -= offSet;
    }

    public void StopConveyor()
    {
        foreach (GameObject point in m_spawnPoint)
        {
            point.GetComponent<SplineAnimate>().Pause();
            point.transform.GetChild(point.transform.childCount - 1).GetComponent<SplineAnimate>().Pause();
        }
    }
    public void ContinuteConveyor()
    {
        foreach (GameObject point in m_spawnPoint)
        {
            point.GetComponent<SplineAnimate>().Play();
            point.transform.GetChild(point.transform.childCount - 1).GetComponent<SplineAnimate>().Play();
        }
    }

    public void IncreaseSpeed()
    {
        foreach (GameObject point in m_spawnPoint)
        {
            point.GetComponent<SplineAnimate>().MaxSpeed *= 2;
            point.transform.GetChild(point.transform.childCount - 1).GetComponent<SplineAnimate>().MaxSpeed *= 2;
        }
    }
    public void ReduceSpeed()
    {
        foreach (GameObject point in m_spawnPoint)
        {
            point.GetComponent<SplineAnimate>().MaxSpeed /= 2;
            point.transform.GetChild(point.transform.childCount - 1).GetComponent<SplineAnimate>().MaxSpeed /= 2;
        }
    }
}
