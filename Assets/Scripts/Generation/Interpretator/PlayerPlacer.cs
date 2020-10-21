using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerPlacer", menuName = "Custom/Generation/Interpreters/Player Placer")]
public class PlayerPlacer : Interpreter
{
    public override void Interpret(string info, Sheet source, Room room, Vector2Int index, Vector3 position)
    {
        room.SetSpawn(position);
    }
}