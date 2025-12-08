using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Twisted
{
    public class VisualResHeart : VisualResUIBase
    {
        [SerializeField] private Image bg;
        [SerializeField] private Image icon;
        public override Image BG { get => bg; set => bg = value; }
        public override Image Icon { get => icon; set => icon = value; }

        public override void Init(DataResource type, Sprite bgDf = null, Sprite iconDf = null, byte star = 0)
        {
            
        }
    }
}
