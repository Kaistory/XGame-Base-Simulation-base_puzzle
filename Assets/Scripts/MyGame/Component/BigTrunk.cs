using System;
using DG.Tweening;
using MyGame.Data;
using MyGame.Manager;
using mygame.sdk;
using TigerForge;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;
using Sequence = DG.Tweening.Sequence;

namespace MyGame
{
    public class BigTrunk : MonoBehaviour, IPointerDownHandler
    {
        #region Header
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
        [SerializeField] private int m_spawnNum;
        [SerializeField] private bool m_isRun;

        [Header("Truck Configuration")]
        
        [SerializeField] private LevelConfig.TrunkData trunkData;
        
        public LevelConfig.TrunkData TrunkData
        {
            get => trunkData;
        }
        

        private bool isClicked;

        [SerializeField] private int m_numIce;
        
        #endregion

        #region MonoBehaviour

        private void OnValidate()
        {
            SetUpObject();
            SetAllColor(trunkData.colorLayers);
            UpdateTruckVisual(immediate: true);
        }
        private void Awake()
        {
            m_SplineAnimate = GetComponent<SplineAnimate>();
            isClicked = false;
            m_isRun = false;
        }

        void Update()
        {
                if (m_isRun)
                {
                    var spawnPoint = Conveyor.Instance.m_spawnPoint[m_spawnNum];
                    transform.position = spawnPoint.transform.position;
                }
        }

        private void OnDisable()
        {
            EventManager.EmitEvent(EventName.UpdateCapacity);
            Conveyor.Instance.checkSpawnPoint[m_spawnNum] = false;
        }

        #endregion
        
        #region  Handle Click

        public void OnPointerDown(PointerEventData eventData)
        {
            var boostMgr = BoostManger.Instance;
            if (trunkData.isBlock || trunkData.isFrozen || trunkData.isChained || isClicked)
                return;
            
            
            if (boostMgr.m_boostTypes.Count == 0 && !m_SplineAnimate.IsPlaying && TrunkManager.Instance.m_capacity > 0)
            {
                isClicked = true;
                SpawnOnConveyor();
                TrunkManager.Instance.CheckIcebreak();
                TrunkManager.Instance.CheckUnlockKey();
            }
            else
            {
                if (boostMgr.m_boostTypes.Contains(RES_type.BOOSTER_1))
                {
                    isClicked = true;
                    RemoveFromConveyor();
                }
                else
                {
                    if (boostMgr.m_boostTypes.Contains(RES_type.BOOSTER_3))
                    {
                        isClicked = true;
                        ProcessRemoveOuterLayer();
                    }
                }
            }
            TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
        }

        private void ProcessRemoveOuterLayer()
        {
            trunkData.visibleLayerCount--;
            UpdateTruckVisual();
            if ( trunkData.visibleLayerCount == 0)
            {
                TigerForge.EventManager.EmitEvent(EventName.UpdateCapacity);
                Destroy(gameObject);
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    if (TrunkManager.Instance.CheckWin())
                    {
                        LevelManager.Instance.CurrentLevel.CompleteLevel();
                    }
                });
                TrunkManager.Instance.m_capacity += 1;
            }
            isClicked = false;
            BoostManger.Instance.m_boostTypes.Remove(RES_type.BOOSTER_3);
        }
        
        
        private void RemoveFromConveyor()
        {
            if (!m_SplineAnimate.IsPlaying)
            {
                isClicked = false;
                BoostManger.Instance.m_boostTypes.Remove(RES_type.BOOSTER_1);
                return;
            }

            Vector3 removePos = trunkData.position;
            m_SplineAnimate.Pause();
            transform.DOMove(removePos, 0.5f).SetEase(Ease.InQuart).OnComplete(() =>
            {
                isClicked = false;
                BoostManger.Instance.m_boostTypes.Remove(RES_type.BOOSTER_1);
            });
            TrunkManager.Instance.m_capacity++;
        }
        private void SpawnOnConveyor()
        {
            var truckMgr = TrunkManager.Instance;
            truckMgr.m_capacity--;
            if (m_SplineAnimate != null)
            {
                foreach (var key in Conveyor.Instance.checkSpawnPoint)
                {
                    if (key.Value == false)
                    {
                        m_spawnNum = key.Key;
                        Conveyor.Instance.checkSpawnPoint[m_spawnNum] = true;
                        break;
                    }
                }
                
                transform.DOMove(new Vector3(transform.position.x, 3, transform.position.z), 0.1f).OnComplete(() =>
                {
                    var spawnPoint = Conveyor.Instance.m_spawnPoint[m_spawnNum];
                    transform.DOMove(spawnPoint.transform.position, 0.2f).SetEase(Ease.InQuart).OnComplete(() =>
                    {
                        isClicked = false;
                        m_isRun = true;
                        // m_SplineAnimate.StartOffset = spawnPoint.GetComponent<SplineAnimate>().StartOffset;
                        // m_SplineAnimate.Restart(true);
                    });
                });
            }
        }
        #endregion
        
        #region Setup

        public void Initialize(int index)
        {
            var trunks = LevelManager.Instance.levelInfo.trunks;
            trunkData = trunks[index].Clone();
            SetUpObject();
            transform.position = trunkData.position;
            SetAllColor(trunkData.colorLayers);
            
            UpdateTruckVisual(immediate: true);
            m_SplineAnimate.MaxSpeed = speed;
            SetPath();
        }

        public void SetUpObject()
        {
            m_blockObject.SetActive(trunkData.isBlock);
            m_chainObject.SetActive(trunkData.isChained);
            m_keyObject.SetActive(trunkData.hasLock);
            m_numIce = trunkData.isFrozen ? 2 : 0;
            ChangeIce(m_numIce);
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
        void SetAllColor(AllColor[] color)
        {
            trunkData.colorLayers = color;
            for (int i = 0; i < 3; i++)
            {
                m_truckObjects[i].GetComponent<Truck>().ChangeColor(trunkData.colorLayers[i]);
            }
        }
        void ChangeIce(int num)
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
                    trunkData.isFrozen = false;
                    break;
            }
        }
        #endregion
        
        #region Activities

        public void BreakIce()
        {
            if (trunkData.isBlock)
                return;
            m_numIce -= 1;
            if (m_numIce < 0)
                return;
            trunkData.isFrozen = (m_numIce != 0);
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
                trunkData.isChained = false;
                m_chainObject.SetActive(false);
            });
        }

        public void UnlockKey(Transform tfDestination)
        {
            DOTween.Sequence().Append(m_keyObject.transform.DOScale(1.6f , 0.4f).SetEase(Ease.OutBack))
                .Join(m_keyObject.transform.DOMove( m_keyObject.transform.position + Vector3.up , 0.4f).SetEase(Ease.OutBack))
                .Append(m_keyObject.transform.DOMove(tfDestination.position - Vector3.forward *0.5f, 0.5f) )
                .Join(m_keyObject.transform.DOLocalRotate( new Vector3(90f, 0f,-90) , 0.5f))
                .Join(m_keyObject.transform.DOScale(1 , 0.5f))
                .AppendCallback(() => Destroy(m_keyObject.gameObject) ) .OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX(AudioName.SFX_Unlock);
                trunkData.hasLock = false;
                m_keyObject.SetActive(false);
            });;
        }

        public void OnHitCutter()
        {
            trunkData.visibleLayerCount -= 1;
            GameHelper.Instance.Vibrate(Type_vibreate.Vib_Medium);
            UpdateTruckVisual();
            if ( trunkData.visibleLayerCount == 0)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                TrunkManager.Instance.m_capacity += 1;
                Destroy(gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            BigTrunk trunk = other.GetComponent<BigTrunk>();
            if (other.CompareTag("Trunk") && trunk != null)
            {
                trunkData.amountUpTrunk--;
                if (trunkData.amountUpTrunk <= 0)
                {
                    trunkData.isBlock = false;
                    trunk.m_blockObject.SetActive(false);
                }
            }
        }
        #endregion

        #region Visual Color

        public void UpdateTruckVisual(bool immediate = false)
        {
            if (m_truckObjects == null || m_truckObjects.Length < 3) return;
            HandlePartState(0,  trunkData.visibleLayerCount >= 1, immediate);
            HandlePartState(1,  trunkData.visibleLayerCount >= 2, immediate);
            HandlePartState(2,  trunkData.visibleLayerCount >= 3, immediate);
        }
        

        public AllColor GetColorOuter()
        {
            if ( trunkData.visibleLayerCount - 1 < 0)
                return AllColor.None;
            return m_truckObjects[ trunkData.visibleLayerCount - 1].GetComponent<Truck>().MAllcolor;
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

        #endregion
    }
}