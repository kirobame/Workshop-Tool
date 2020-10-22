using System;
using UnityEngine;

public class Enemy : Scorer
{
    public static event Action OnAllKilled;
    public static event Action OnCountChanged;
    
    public static int Count { get; private set; }
    public static void SetCount(int value)
    {
        Count = value;
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

    public override void Reset()
    {
        base.Reset();
        
        Count++;
        OnCountChanged?.Invoke();
    }
}