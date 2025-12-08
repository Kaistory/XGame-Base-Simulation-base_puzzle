using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immortal;
using System;

public static class HelperImmortal
{
    public static ImmortalObject Object
    {
        get
        {
            if (_obj == null)
            {
                _obj = new GameObject("ImmortalObject").AddComponent<ImmortalObject>();
                _obj.OnUpdate += UpdateLogic;
            }

            return _obj;
        }
    }

    private static ImmortalObject _obj;
    private static List<Counter> counters = new List<Counter>();

    private static void UpdateLogic(float delta)
    {
        for (int i = 0; i < counters.Count; i++)
        {
            counters[i].UpdateCounter(delta);
        }
    }

    public static void StartCoroutine(IEnumerator ienumerator)
    {
        Object.StartCoroutine(ienumerator);
    }

    public static void Delay(float duration, Action callBack)
    {
        Object.StartCoroutine(IEDelay());

        IEnumerator IEDelay()
        {
            float timer = duration;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            callBack?.Invoke();
        }
    }

    public static Counter CreateCounter(float duration, Action complete)
    {
        Counter counter = new Counter(duration, complete);
        counters.Add(counter);
        counter.OnComplete += () => RemoveCounter(counter);
        return counter;
    }

    public static void RemoveCounter(Counter counter)
    {
        if (counter == null) return;
        if (!counters.Contains(counter)) return;
        counters.Remove(counter);
    }

    public static string ConvertMoneyToString(long money)
    {
        if (money >= 1000000000)
        {
            return (money / 1000000000).ToString() + "B";
        }

        if (money >= 1000000)
        {
            return (money / 1000000).ToString() + "M";
        }

        if (money >= 10000)
        {
            return (money / 1000).ToString() + "K";
        }

        return money.ToString();
    }

    public class Counter
    {
        public Counter(float duration, Action complete)
        {
            this.OnComplete = complete;
            IsPause = false;
            if (duration <= 0)
            {
                duration = 0.000001f;
            }

            Timer = duration;
        }

        public float Timer { get; private set; }
        public bool IsPause { get; set; }
        public event Action OnComplete;

        public void UpdateCounter(float delta)
        {
            if (IsPause || Timer <= 0) return;
            Timer -= delta;
            if (Timer <= 0)
            {
                OnComplete?.Invoke();
                OnComplete = null;
            }
        }

        public void Cancel()
        {
            Timer = 0;
            RemoveCounter(this);
        }

        public static implicit operator bool(Counter exists)
        {
            return exists != null;
        }
    }
}