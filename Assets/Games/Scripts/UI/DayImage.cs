using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace _JigblockPuzzle
{
    public class DayImage : MonoBehaviour, IPointerClickHandler
    {
        public static DayImage current;
        [SerializeField] private Image img;
        [SerializeField] private Sprite bg_miss;
        [SerializeField] private Sprite bg_sellect;
        public int ActiveNumber;

        public static event Action<DayImage> OnDaySelected;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (current == this) return;

            foreach (var di in FindObjectsOfType<DayImage>())
            {
                if (di.enabled && di.img != null)
                {
                    di.img.sprite = di.bg_miss;
                }
            }
            img.sprite = bg_sellect;
            current = this;
            OnDaySelected?.Invoke(this);
        }

    }
}

