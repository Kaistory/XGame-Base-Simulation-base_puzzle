using System.Collections.Generic;
using DG.Tweening;
using MyBox;
using MyGame.Data;
using UnityEngine;

namespace MyGame.Manager
{
    public class TrunkManager : Singleton<TrunkManager>
    {
        //Capacity
        [SerializeField] public int m_capacity;
        [SerializeField] public int m_maxCapacity;
        
        //
        [SerializeField] private int numTrunk;

        public void Initialize()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            numTrunk = LevelManager.Instance.levelInfo.trunks.Length;
            m_maxCapacity = 4;
            m_capacity = 4;
            for (int i = 0; i< numTrunk; i++)
            {
                GameObject truckPrefab = Resources.Load<GameObject>(PathNameResource.PathTruck + "Trunk");
                if (truckPrefab != null)
                {
                    GameObject truckClone = Instantiate(truckPrefab, transform);
                    truckClone.GetComponent<BigTrunk>().Initialize(i);
                    truckClone.name = "Trunk_" + i.ToString();
                }
            }
        }

        public void CheckIcebreak()
        {
            foreach (Transform child in transform)
            {
                var bigTruck = child.GetComponent<BigTrunk>();
                if (bigTruck.TrunkData.isFrozen && !bigTruck.TrunkData.isBlock)
                {
                    bigTruck.BreakIce();
                }
            }
        }
        public void CheckUnlockKey()
        {
            foreach (Transform child in transform)
            {
                var bigTruck = child.GetComponent<BigTrunk>();
                if (bigTruck.TrunkData.hasLock && !bigTruck.TrunkData.isBlock)
                {
                    foreach (Transform childUnlock in transform)
                    {
                        var bigTruck2 = childUnlock.GetComponent<BigTrunk>();
                        if (bigTruck2.TrunkData.isChained && !bigTruck2.TrunkData.isBlock)
                        {
                            bigTruck.UnlockKey(childUnlock);
                            bigTruck2.BreakChain();
                        }
                    }
                }
            }
        }

        public bool CheckWin()
        {
            Debug.Log("Check win " + transform.childCount);
            return transform.childCount == 0;
        }
    }
}