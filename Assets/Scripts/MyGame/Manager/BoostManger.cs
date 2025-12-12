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
                var trunkMg = TrunkManager.Instance;
                trunkMg.m_capacity += 3;
                trunkMg.m_maxCapacity += 3;
                Conveyor.Instance.AddArrow(trunkMg.m_maxCapacity);
                m_boostTypes.RemoveAt(0);
                TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
                TigerForge.EventManager.EmitEvent(EventName.UseBoostCapacity);
            }
        }
    }
}