using System;
using UnityEditor;
using UnityEngine;

public class Helper : MonoBehaviour
{
    [SerializeField] private Shape shape;

    void Awake() => shape.GenerateRuntimeData();
}