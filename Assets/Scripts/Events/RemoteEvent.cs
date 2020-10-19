using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRemoteEvent", menuName = "Custom/Remote Events/Void")]
public class RemoteEvent : ScriptableObject
{
    public event Action action;

    public void Invoke() => action?.Invoke();
}