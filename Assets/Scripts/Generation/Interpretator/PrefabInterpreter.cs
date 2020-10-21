using UnityEngine;

[CreateAssetMenu(fileName = "NewPrefabInterpreter", menuName = "Custom/Generation/Interpreters/Prefab")]
public class PrefabInterpreter : Interpreter
{
    [SerializeField] private GameObject prefab;
    
    public override void Interpret(string info, Sheet source, Room room, Vector2Int index, Vector3 position)
    {
        Instantiate(prefab, position, Quaternion.identity, room.transform);
    }
}