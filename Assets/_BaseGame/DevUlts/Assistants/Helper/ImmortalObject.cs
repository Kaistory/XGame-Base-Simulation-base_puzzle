using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Immortal
{
    public class ImmortalObject : MonoBehaviour
    {
        public static ImmortalObject Instance;
        public event Action<float> OnUpdate;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Update()
        {
            OnUpdate?.Invoke(Time.deltaTime);
        }
    }
}
