using System;
using System.Collections;
using _JigblockPuzzle;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoneyFX : MonoBehaviour
{
    public float range;
    public Transform endPos;
    [SerializeField] private float spreadDuration = 0.75f;
    [SerializeField] private float returnDuration = 0.75f;
    [SerializeField] private float scaleFactor = 1f;

    private void OnDisable()
    {
        CheckStopCoroutineFX();
    }

    private void CheckStopCoroutineFX()
    {
        if (coroutineFX != null)
        {
            StopCoroutine(coroutineFX);
        }
    }

    private Coroutine coroutineFX;

    public void PlayFx(System.Action callBack, int idx, Transform PosStart)
    {
        CheckStopCoroutineFX();
        coroutineFX = StartCoroutine(DelayPlayFx(callBack, idx, PosStart));
    }

    IEnumerator DelayPlayFx(System.Action callBack, int idx, Transform PosStart)
    {
        yield return new WaitForSeconds(idx);
        // AudioManager.Instance.PlaySFX(AudioName.SFX_Item_Jump_End, 1);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform curChild = transform.GetChild(i);
            curChild.gameObject.SetActive(true);
            float ranNumX = Random.Range(-range, range) + PosStart.localPosition.x;
            float ranNumY = Random.Range(-range, range) + PosStart.localPosition.y;

            var rd = Random.Range(1f, 1.25f);
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
            Vector3 spreadPosition = PosStart.transform.position + randomDirection * Random.Range(1f, 7f);
            curChild.localScale = Vector3.zero;
            curChild.localPosition = Vector3.zero;

            curChild.transform.DOScale(scaleFactor, spreadDuration * rd);
            curChild.transform.DOMove(spreadPosition, spreadDuration * rd).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                var rd = Random.Range(0.5f, 1f);
                curChild.transform.DOScale(scaleFactor * 1.25f, returnDuration * rd);
                curChild.transform.DOMove(endPos.position, returnDuration * rd).SetEase(Ease.InCubic)
                    .OnComplete(() =>
                    {
                        curChild.gameObject.SetActive(false);
                        callBack.Invoke();
                    });
            });
        }
    }
}