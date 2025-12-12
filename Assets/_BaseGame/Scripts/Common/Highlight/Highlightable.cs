using System;
using MyBox;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Highlightable : MonoBehaviour
{
	// [SerializeField]
// 	private List<Renderer> m_RenderersToHighlight = new List<Renderer>();
//
// 	public bool ignoreChildren;
// 	private bool m_Highlighted;
//
// 	public List<Renderer> RenderersToHighlight => m_RenderersToHighlight;
//
// 	public bool Highlighted
// 	{
// 		set
// 		{
// 			m_Highlighted = value;
// 		}
// 	}
//
// #if UNITY_EDITOR
// 	public void GetRenderer(GameObject cube)
// 	{
// 		var renderer = cube.GetComponent<Renderer>();
// 		for (int i = 0 ; i < m_RenderersToHighlight.Count ; i++)
// 		{
// 			if( m_RenderersToHighlight.Contains(renderer) ) return;
// 		}
// 		
// 		m_RenderersToHighlight.Add(renderer);
//
// 		EditorUtility.SetDirty(gameObject);
// 	}
// #endif
//
// 	private void Reset()
// 	{
// 		m_RenderersToHighlight = new List<Renderer>();
// 		m_RenderersToHighlight = GetComponentsInChildren<Renderer>().ToList();
// 	}
//
// 	public void AddOrRemoveRenderer(IList<Renderer> renderers, bool add)
// 	{
// 		if (add)
// 		{
// 			m_RenderersToHighlight.AddRange(renderers);
// 			if (m_Highlighted)
// 			{
// 				Singleton<HighlightManager>.Instance.AddOrRemoveRenderer(renderers, add: true);
// 			}
// 			return;
// 		}
// 		foreach (Renderer item in renderers)
// 		{
// 			if (m_RenderersToHighlight.Contains(item))
// 			{
// 				m_RenderersToHighlight.Remove(item);
// 				if (m_Highlighted)
// 				{
// 					Singleton<HighlightManager>.Instance.AddOrRemoveRenderer(renderers, add: false);
// 				}
// 			}
// 		}
// 	}
//
// 	public void RemoveRenderer()
// 	{
// 		foreach (Renderer item in m_RenderersToHighlight)
// 		{
// 			m_RenderersToHighlight.Remove(item);
// 		}
// 	}
}
