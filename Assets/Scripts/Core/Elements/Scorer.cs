﻿using System;
using UnityEngine;
using UnityEngine.Events;

public class Scorer : MonoBehaviour, IHittable
{
    public bool HasAlreadyBeenHit => hasAlreadyBeenHit;

    [SerializeField] private string key;
    [SerializeField] private CoreData data;
    
    [Space, SerializeField] private Token acceptedToken;
    [SerializeField] private UnityEvent onScored;
    
    protected bool hasAlreadyBeenHit;
    private int scoreValue;

    void Awake()
    {
        if (data.AreSheetsReady) SetScoreValue();
        else data.OnRuntimeSheetReady += SetScoreValue;
    }

    private void SetScoreValue()
    {
        scoreValue = Convert.ToInt32(data["Scorers"][key, "Score"]);
        Debug.Log($"For {key} : Score is {scoreValue}");
    }

    public virtual bool Hit(Bullet bullet, RaycastHit hit)
    {
        if (hasAlreadyBeenHit) return false;

        if (bullet.TryGetComponent<PoolableBullet>(out var poolable) && poolable.Tag == acceptedToken)
        {
            Score.ModifyBy(scoreValue);
            onScored.Invoke();

            hasAlreadyBeenHit = true;
            return true;
        }

        return false;
    }
}