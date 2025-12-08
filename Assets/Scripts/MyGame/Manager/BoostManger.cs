using System;
using System.Collections.Generic;
using MyBox;
using MyGame.Data;
using mygame.sdk;
using UnityEngine;

namespace MyGame.Manager
{
    public class BoostManger : Singleton<BoostManger>
    {
        // check valriables boost
        [SerializeField] public bool isRemoveBoost;
        [SerializeField] public bool isRemoveOuterTrunkBoost;
        [SerializeField] public List<BoostType> m_backBoost;
        
        [SerializeField] public BoostType m_bosstType;

        public void SetActiveBoost(BoostType boostType)
        {
            if(m_bosstType  != BoostType.None &&  boostType != BoostType.None)
                m_backBoost.Add(m_bosstType);
            m_bosstType = boostType;
            String boostName = m_bosstType.ToString();
       
            isRemoveBoost = false;
            isRemoveOuterTrunkBoost = false;
            switch (boostName)
            {
                case "Remove":
                    if (DataManager.GetResources(RES_type.BOOSTER_1) > 0)
                    {
                        isRemoveBoost = true;
                    }
                    break;
                case "AddCapacity":
                    if (DataManager.GetResources(RES_type.BOOSTER_2) > 0)
                    {
                        TruckManager.Instance.m_capacity += 3;
                        TruckManager.Instance.m_maxCapacity += 3;
                        if(m_backBoost.Count == 0)
                            m_bosstType = BoostType.None;
                        else
                        {
                            m_bosstType = m_backBoost[0];
                            m_backBoost.RemoveAt(0);
                        }
                        TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
                    }
                    break;
                case "RemoveOuterTrunk":
                    if (DataManager.GetResources(RES_type.BOOSTER_3) > 0)
                    {
                        isRemoveOuterTrunkBoost = true;
                    }
                    break;
            }
        }
    }
}