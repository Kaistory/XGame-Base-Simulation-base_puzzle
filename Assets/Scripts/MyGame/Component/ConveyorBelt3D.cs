using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt3D : MonoBehaviour
{
    [SerializeField]
    private float conveyorSpeed;

    private Material material;
    
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        ActivateBelt();
    }
    IEnumerator RunConveyor(float duration)
    {
        float timer = 0f;
        
        while (timer < duration)
        {
            material.mainTextureOffset += new Vector2(-1, 0) * conveyorSpeed * Time.deltaTime;
            
            timer += Time.deltaTime;
            
            yield return null;
        }
        Debug.Log("Đã dừng băng chuyền!");
    }
    
    public void ActivateBelt()
    {
        StartCoroutine(RunConveyor(0.5f));
    }

}