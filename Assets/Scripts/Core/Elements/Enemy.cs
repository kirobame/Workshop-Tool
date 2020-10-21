using System;
using UnityEngine;

public class Enemy : Scorer
{
    public static event Action OnAllKilled;
    public static event Action OnCountChanged;
    
    public static int Count { get; private set; }

    void OnEnable()
    {
        Count++;
        OnCountChanged?.Invoke();
    }

    public override bool Hit(Bullet bullet, RaycastHit hit)
    {
        if (base.Hit(bullet, hit))
        {
            Count--;
            OnCountChanged?.Invoke();

            if (Count == 0) OnAllKilled?.Invoke();

            return true;
        }

        return false;
    }
}