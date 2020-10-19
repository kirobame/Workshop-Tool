using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class Referencer : MonoBehaviour
{
    [SerializeField] private Token token;
    [SerializeField] private Object[] values;

    void Awake() => Repository.RegisterMany(token, values);
    void OnDestroy() => Repository.UnregisterMany(token, values);
}