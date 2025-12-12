using System;
using System.Collections.Generic;
using DG.Tweening;
using MyGame.Data;
using MyGame.Manager;
using mygame.sdk;
using UnityEngine;
using UnityEngine.Splines;

namespace MyGame
{
    public class CutterMachine : MonoBehaviour
    {
        //Cutter
        [SerializeField] private Transform[] positionCutters = new Transform[3];
        [SerializeField] private Cutter cutterPefab;
        [SerializeField] private GameObject cuttersManager;
        
        // Conveyor Ani
        [SerializeField] private ConveyorBelt3D m_conveyBelt;
        public void Initialize(Vector3 pos, float rot)
        {
            SpawnCutters();
            transform.position = pos;
            transform.rotation = Quaternion.Euler(0, rot, 0);
        }

        
        void SpawnCutters()
        {
            for (int i = 0; i <= 2; i++)
                    {
                        Cutter cuttersGameObject = Instantiate(cutterPefab, positionCutters[i].position, positionCutters[i].rotation,
                            cuttersManager.transform);
                        cuttersGameObject.Init(this);
                        if(i == 0) 
                            cuttersGameObject.MBoxCollider.enabled = true;
                    }
        }
        
        public void ResetCutters()
        {
                DOVirtual.DelayedCall(0.5f , () =>
                {
                    m_conveyBelt.ActivateBelt();
                    foreach (Transform cutterTF in cuttersManager.transform)
                    {
                        cutterTF.DOMove(cutterTF.position + transform.forward * 1.25f, 1f);
                        if (cutterTF == cuttersManager.transform.GetChild(0))
                            cutterTF.GetComponent<Cutter>().MBoxCollider.enabled = true;
                    }
                }).OnComplete(() =>
                {
                    if (CutterMachineManager.Instance.spawnsColor.Count != 0)
                    {   
                        Cutter newCutter= Instantiate(cutterPefab, positionCutters[2].position, positionCutters[2].rotation,
                            cuttersManager.transform);
                        newCutter.Init(this);
                    }
                });
        }
    }
}