using System;
using UnityEngine;

namespace GamePlugins
{
	public class SingletonScreen<T> : BaseScreen where T : BaseScreen
	{
		protected static T _instance;

		public static T Instance => _instance;

		protected override void Awake()
		{
			_instance = this as T;
		}
	}
}
