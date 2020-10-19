using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class HitEvent : UnityEvent<RaycastHit> { }