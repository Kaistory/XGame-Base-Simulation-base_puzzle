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
       
        [SerializeField] public List<RES_type> m_boostTypes;

        public void SetActiveBoost(RES_type type)
        {
            m_boostTypes.Insert(0,type);
            if (type == RES_type.BOOSTER_2)
            {
                TruckManager.Instance.m_capacity += 3;
                TruckManager.Instance.m_maxCapacity += 3;
                m_boostTypes.RemoveAt(0);
                TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
            }
        }
    }
}