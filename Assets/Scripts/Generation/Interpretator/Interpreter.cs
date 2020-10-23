using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class allowing the execution of a specific behaviour based on the content of a cell.
// For this specific implementation, the behaviour is directly coupled to the room system. 
public abstract class Interpreter : ScriptableObject
{
    // The cell key. 
    public string Target => target;
    [SerializeField] private string target;

    public abstract void Interpret(string info, Sheet source, Room room, Vector2Int index, Vector3 position);
}