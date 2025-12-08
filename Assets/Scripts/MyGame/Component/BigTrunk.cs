using System;
using DG.Tweening;
using MyGame.Data;
using MyGame.Manager;
using mygame.sdk;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using Sequence = DG.Tweening.Sequence;

namespace MyGame
{
    public class BigTrunk : MonoBehaviour, IPointerDownHandler
    {
        [Header("Object Reference")]
        [SerializeField] private GameObject m_chainObject;
        [SerializeField] private GameObject m_keyObject;
        [SerializeField] private GameObject m_blockObject;
        [SerializeField] private GameObject[] m_iceObject = new GameObject[2];
        [SerializeField] private GameObject[] m_truckObjects = new GameObject[3];

        [Header("Movement Settings")]
        [SerializeField] public SplineAnimate m_SplineAnimate;
        [SerializeField] private float speed;
        [SerializeField] private int m_NumberConveyor;

        [Header("Truck Configuration")]
        [Range(1, 3)]
        [SerializeField] private int m_truck;
        [SerializeField] private AllColor[] m_trunkColor = new AllColor[3];

        [Header("State Flags")]
        [SerializeField] private bool m_isBlock;
        [SerializeField] private bool m_hasKey;
        [SerializeField] private bool m_hasChain;
        [SerializeField] private bool m_hasIce;
        [SerializeField] private int m_idx;
        [SerializeField] private int m_numIce;
        [SerializeField] private int m_amountUpTrunk;

        private bool isClicked;

        public GameObject MBlockObject
        {
            get => m_blockObject;
            set => m_blockObject = value;
        }

        public int MTruck
        {
            get => m_truck;
            set => m_truck = value;
        }

        public AllColor[] MTrunkColor => m_trunkColor;

        public bool MIsBlock
        {
            get => m_isBlock;
            set => m_isBlock = value;
        }

        public bool MHasKey => m_hasKey;
        public bool MHasChain => m_hasChain;
        public bool MHasIce => m_hasIce;
        public int MAmountUpTrunk => m_amountUpTrunk;

        private void Awake()
        {
            m_SplineAnimate = GetComponent<SplineAnimate>();
            isClicked = false;
        }

        private void OnDisable()
        {
            TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var boostMgr = BoostManger.Instance;
            if (m_isBlock || m_hasIce || m_hasChain || isClicked)
                return;

            if (boostMgr.isRemoveOuterTrunkBoost)
            {
                isClicked = true;
                ProcessRemoveOuterLayer();
                boostMgr.SetActiveBoost(BoostType.None);
                TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
                return;
            }

            if (boostMgr.isRemoveBoost)
            {
                isClicked = true;
                RemoveFromConveyor();
                boostMgr.SetActiveBoost(BoostType.None);
                TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
                return;
            }

            if (BoostManger.Instance.m_bosstType == BoostType.None && !m_SplineAnimate.IsPlaying)
            {
                isClicked = true;
                TrySpawnOnConveyor();
                TruckManager.Instance.CheckIcebreak();
                TruckManager.Instance.CheckUnlockKey();
                TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            BigTrunk trunk = other.GetComponent<BigTrunk>();
            if (other.CompareTag("Trunk") && trunk != null)
            {
                m_amountUpTrunk--;
                if (m_amountUpTrunk <= 0)
                {
                    trunk.MIsBlock = false;
                    trunk.m_blockObject.SetActive(false);
                }
            }
        }

        private void OnValidate()
        {
            SetUpObject();
            AddAllColor(m_trunkColor);
            UpdateTruckVisual(immediate: true);
        }

        public void Initialize(int index)
        {
            m_idx = index;
            var trunk = LevelManager.Instance.levelInfo.trunks;
            m_hasKey = trunk[m_idx].hasLock;
            m_hasChain = trunk[m_idx].isChained;
            m_hasIce = trunk[m_idx].isFrozen;
            m_truck = trunk[m_idx].visibleLayerCount;
            m_isBlock = trunk[m_idx].isBlock;
            m_amountUpTrunk = trunk[m_idx].amountUpTrunk;

            SetUpObject();
            transform.position = trunk[m_idx].position;
            ChangeSpeed(speed);
            UpdateTruckVisual(immediate: true);
            SetPath();
            AddAllColor(trunk[m_idx].colorLayers);
        }

        public void SetUpObject()
        {
            m_blockObject.SetActive(m_isBlock);
            m_chainObject.SetActive(m_hasChain);
            m_keyObject.SetActive(m_hasKey);
            
            if (m_hasIce)
            {
                m_numIce = 2;
            }
            else
            {
                m_numIce = 0;
            }
            ChangeIce(m_numIce);
        }

        public void UpdateTruckVisual(bool immediate = false)
        {
            if (m_truckObjects == null || m_truckObjects.Length < 3) return;
            HandlePartState(0, m_truck >= 1, immediate);
            HandlePartState(1, m_truck >= 2, immediate);
            HandlePartState(2, m_truck >= 3, immediate);
        }

        public void AddAllColor(AllColor[] color)
        {
            m_trunkColor = color;
            for (int i = 0; i < 3; i++)
            {
                m_truckObjects[i].GetComponent<Truck>().ChangeColor(m_trunkColor[i]);
            }
        }

        public void BreakIce()
        {
            if (m_isBlock)
                return;
            m_numIce -= 1;
            if (m_numIce < 0)
                return;
            m_hasIce = (m_numIce != 0);
            m_iceObject[m_numIce].transform.DOShakePosition(0.5f, strength: 0.5f, vibrato: 20, randomness: 90).OnComplete(() =>
            {
                m_iceObject[m_numIce].SetActive(false);
                AudioManager.Instance.PlaySFX(AudioName.SFX_Ice);
            });
        }

        public void BreakChain()
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Join(m_chainObject.transform.DOMoveY(transform.position.y - 2f, 1f).SetEase(Ease.InQuad));
            mySequence.OnComplete(() =>
            {
                m_hasChain = false;
                m_chainObject.SetActive(false);
            });
        }

        public void UnlockKey(Transform tfDestination)
        {
            m_keyObject.transform.DOMove(tfDestination.position, 0.5f).OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX(AudioName.SFX_Unlock);
                m_hasKey = false;
                m_keyObject.SetActive(false);
            });
        }

        public void OnHitCutter()
        {
            m_truck -= 1;
            GameHelper.Instance.Vibrate(Type_vibreate.Vib_Medium);
            UpdateTruckVisual();
            if (m_truck == 0)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                TruckManager.Instance.m_capacity += 1;
                Destroy(gameObject);
            }
        }

        public AllColor GetColorOuter()
        {
            if (m_truck - 1 < 0)
                return AllColor.None;
            return m_truckObjects[m_truck - 1].GetComponent<Truck>().MAllcolor;
        }

        private void ProcessRemoveOuterLayer()
        {
            m_truck--;
            UpdateTruckVisual();
            if (m_truck == 0)
            {
                Destroy(gameObject);
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    if (TruckManager.Instance.CheckWin())
                    {
                        LevelManager.Instance.CurrentLevel.CompleteLevel();
                    }
                    isClicked = false;
                });
                TruckManager.Instance.m_capacity += 1;
            }
        }

        private void TrySpawnOnConveyor()
        {
            var truckMgr = TruckManager.Instance;
            if (truckMgr.m_capacity > 0)
            {
                truckMgr.m_capacity--;
                SpawnOnConveyor();
            }
        }

        private void HandlePartState(int index, bool shouldBeActive, bool immediate)
        {
            GameObject part = m_truckObjects[index];
            if (part == null) return;

            if (shouldBeActive)
            {
                part.SetActive(true);
                part.transform.DOKill();

                if (immediate)
                {
                    part.transform.localScale = Vector3.one;
                }
                else
                {
                    part.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                }
            }
            else
            {
                if (part.activeSelf)
                {
                    part.transform.DOKill();

                    if (immediate || !Application.isPlaying)
                    {
                        part.SetActive(false);
                        part.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        part.transform.DOScale(new Vector3(0.5f, 1, 0.5f), 0.5f)
                            .OnComplete(() =>
                            {
                                part.SetActive(false);
                                part.transform.localScale = Vector3.one;
                            }).SetEase(Ease.Linear);
                    }
                }
            }
        }

        private void ChangeSpeed(float newSpeed)
        {
            if (m_SplineAnimate != null) m_SplineAnimate.MaxSpeed = newSpeed;
        }

        private void SetPath()
        {
            m_NumberConveyor = LevelRemoteManager.Instance.levelInfo.mapID;
            GameObject pathObj = Resources.Load<GameObject>(PathNameResource.PathSpline + m_NumberConveyor.ToString());
            if (pathObj != null && m_SplineAnimate != null)
            {
                m_SplineAnimate.Container = pathObj.GetComponent<SplineContainer>();
            }
        }

        private void SpawnOnConveyor()
        {
            if (m_SplineAnimate != null)
            {
                Spline spline = m_SplineAnimate.Container.Spline;
                float3 localPos = spline.EvaluatePosition(m_SplineAnimate.StartOffset);
                Vector3 spawnPosWorld = m_SplineAnimate.Container.transform.TransformPoint(localPos);
                
                transform.DOMove(new Vector3(transform.position.x, 3, transform.position.z), 0.1f).OnComplete(() =>
                {
                    transform.DOMove(spawnPosWorld, 0.5f).SetEase(Ease.InQuart).OnComplete(() =>
                    {
                        isClicked = false;
                        m_SplineAnimate.Restart(true);
                    });
                });
            }
        }

        private void ChangeIce(int num)
        {
            m_iceObject[0].SetActive(true);
            m_iceObject[1].SetActive(true);
            switch (num)
            {
                case 2:
                    break;
                case 1:
                    m_iceObject[1].SetActive(false);
                    break;
                case 0:
                    m_iceObject[1].SetActive(false);
                    m_iceObject[0].SetActive(false);
                    m_hasIce = false;
                    break;
            }
        }

        private void RemoveFromConveyor()
        {
            if (!m_SplineAnimate.IsPlaying)
            {
                isClicked = false;
                return;
            }

            Vector3 removePos = LevelManager.Instance.levelInfo.trunks[m_idx].position;
            m_SplineAnimate.Pause();
            transform.DOMove(removePos, 0.5f).SetEase(Ease.InQuart).OnComplete(() =>
            {
                isClicked = false;
            });
            TruckManager.Instance.m_capacity++;
        }
    }
}