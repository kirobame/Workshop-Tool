using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interpreter : ScriptableObject
{
    public string Target => target;
    [SerializeField] private string target;
    
    public abstract void Interpret(string info, Sheet source, Room room, Vector2Int index, Vector3 position);
}