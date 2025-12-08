using System;
using MyBox;
using UnityEngine;

namespace MyGame.Manager
{
    public class BgMapManager : Singleton<BgMapManager>
    {

        private SpriteRenderer m_spriteRenderer;

        private String[] m_colorBackgrounds =
        {
            "#FFD1DC",
            "#E6E6FA",
            "#967BB6",
            "#008080",
            "#FFFDD0",
            "#98FF98",
        };
        private void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize()
        {
            int lvl = LevelManager.Instance.levelInfo.level;
            Color newColor;
            if (ColorUtility.TryParseHtmlString(m_colorBackgrounds[lvl % 5], out newColor))
            {
                m_spriteRenderer.color = newColor;
            }
            else
            {
                Debug.LogError("Mã HEX không hợp lệ!");
            }
        }
    }
}