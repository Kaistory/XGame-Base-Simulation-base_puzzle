using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyGame;
using MyGame.Data;
using MyGame.Manager;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    //Color
    [SerializeField] private MeshRenderer cutter_meshRenderer;
    [SerializeField] private BoxCollider m_boxCollider;
    [SerializeField] private bool cutted;
    
    public BoxCollider MBoxCollider
    {
        get => m_boxCollider;
        set => m_boxCollider = value;
    }

    [SerializeField] private Material[] basicMaterial = new Material[4];
    [SerializeField] private AllColor m_Allcolor;

    // add cutter machine
    private CutterMachine cutterMachine;
    
    //Effect
    [SerializeField] private ParticleSystem hitTrunkParticle;
    
    public void Init(CutterMachine cutterMachine)
    {
        this.cutterMachine = cutterMachine;
    }


    private void OnValidate()
    {
        basicMaterial[1] = (Resources.Load<Material>(PathNameResource.PathMaterial + "m_mainColor_"+ m_Allcolor.ToString() +"_Cutter"));
        cutter_meshRenderer.materials = basicMaterial;
    }

    void Start()
    {
        int lengthColor = CutterMachineManager.Instance.spawnsColor.Count;
        cutted = false;
        transform.name = "Cutter_" + lengthColor.ToString();
        if (lengthColor != 0)
        {
            m_Allcolor = CutterMachineManager.Instance.spawnsColor[lengthColor - 1];
            CutterMachineManager.Instance.spawnsColor.RemoveAt(lengthColor - 1);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        basicMaterial[1] = (Resources.Load<Material>(PathNameResource.PathMaterial + "m_mainColor_"+ m_Allcolor.ToString() +"_Cutter"));
        cutter_meshRenderer.materials = basicMaterial;

        hitTrunkParticle.startColor = Color.white; 
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(m_Allcolor.ToString(), out parsedColor))
        {
            hitTrunkParticle.startColor = parsedColor;
        }
    }
    
    private void OnTriggerEnter(Collider other)
        {
             if (other.CompareTag("Trunk") && !cutted)
             {
                 BigTrunk bigTrunk= other.GetComponent<BigTrunk>();
                 if(bigTrunk.GetColorOuter() != m_Allcolor)
                        return;
                 cutted = true;
                 bigTrunk.OnHitCutter();
                 PlayParticle();
                 AudioManager.Instance.PlaySFX(AudioName.SFX_WoodCutting);
                 transform.DOMove(transform.position + cutterMachine.transform.forward, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                  {
                      
                      transform.DOScale(0, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                          {
                              cutterMachine.ResetCutters();
                              DOVirtual.DelayedCall(0.2f, () =>
                              {
                                  CutterMachineManager.Instance.removeColor.Insert(0, m_Allcolor);
                                  Destroy(gameObject);
                                  if (TrunkManager.Instance.CheckWin())
                                  {
                                      LevelManager.Instance.CurrentLevel.CompleteLevel();
                                  }
                              });
            
                          }
                      );
                  });
             }
        }
    public void PlayParticle()
    {
        hitTrunkParticle.gameObject.SetActive(true);
        hitTrunkParticle.Play();
        DOVirtual.DelayedCall(0.5f, () => hitTrunkParticle.Stop());
    }
    
}
