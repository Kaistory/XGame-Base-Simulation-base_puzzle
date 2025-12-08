using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Twisted
{
    public class VisualBonusTimeLevel : MonoBehaviour
    {
        public GameObject obj;
        public Text txtBonusTime;
        public void SetBonusTime(int bonusTime, Action onDone)
        {
            gameObject.SetActive(true);
            obj.SetActive(true);
            obj.transform.localScale = Vector3.zero;
            txtBonusTime.text = $"+{bonusTime}";
            var sq = DOTween.Sequence();
            sq.SetId(this);
            sq.Insert(0, obj.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            sq.AppendInterval(1f);
            sq.Append(obj.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
            sq.OnComplete(() =>
            {
                gameObject.SetActive(false);
                onDone?.Invoke();
                onDone = null;
            });
        }
        private void OnDisable()
        {
            this.DOKill();
        }
    }
}
