using System;
using MyGame;
using UnityEngine;

public class CutterObject : MonoBehaviour
{
    [Tooltip("Góc xoay (0, 90, 180, 270 hoặc index tuỳ logic game)")]
    public float rot;

    private CutterMachine m_cutterMachine;

    void Awake()
    {
        m_cutterMachine = GetComponent<CutterMachine>();
    }

    private void Update()
    {
        rot = m_cutterMachine.transform.eulerAngles.y;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 1f);
    }
}