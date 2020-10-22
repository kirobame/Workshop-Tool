using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomHandler : MonoBehaviour
{
    [SerializeField] private RoomCollection collection;
    [SerializeField] private Transform ground;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Room roomPrefab;
    [SerializeField] private float activationDelay;
    [SerializeField] private Interpreter[] interpreters;

    [SerializeField] private UnityEvent onEnd;
    [SerializeField] private UnityEvent onDeactivation;
    [SerializeField] private UnityEvent onActivation;

    private Room[] rooms;
    private Dictionary<string,Interpreter> registry = new Dictionary<string,Interpreter>();

    private int advancement = -1;
    
    void Awake()
    {
        StartCoroutine(collection.Download(Generate));
        foreach (var interpreter in interpreters) registry.Add(interpreter.Target, interpreter);
    }

    private void Generate(Sheet[] sheets)
    {
        rooms = new Room[sheets.Length];
        for (var i = 0; i < sheets.Length; i++)
        {
            var sheet = sheets[i];

            rooms[i] = Instantiate(roomPrefab);
            rooms[i].Initialize(sheet);

            for (var x = 0; x < sheet.Size.x; x++)
            {
                for (var y = sheet.Size.y - 1; y >= 0; y--)
                {
                    var index = new Vector2Int(x,y);
                    var item = sheet[index];

                    if (item == string.Empty) continue;
                
                    var identifier = item.Contains("=") ? item.Split('=')[0] : item;
                    var info = item.Length > 2 ? item.Substring(2) : string.Empty;

                    var inversedIndex = new Vector2Int(x, sheet.Size.y - 1 - y);
                    var position = new Vector3(inversedIndex.x * cellSize.x, 0f, inversedIndex.y * cellSize.y);

                    if (registry.TryGetValue(identifier, out var interpreter)) interpreter.Interpret(info, sheet, rooms[i], new Vector2Int(x,y), position);
                }
            }
        }

        ActivateNext();
    }

    public void ActivateSpecific(int index) => StartCoroutine(ActivationRoutine(index));
    public void ActivateNext() => StartCoroutine(ActivationRoutine(advancement + 1));
    
    private IEnumerator ActivationRoutine(int newAdvancement)
    {
        if (advancement >= 0 && advancement <= rooms.Length - 1) onDeactivation.Invoke();
        yield return new WaitForSeconds(activationDelay);

        if (advancement >= 0 && advancement < rooms.Length) rooms[advancement].gameObject.SetActive(false);

        if (newAdvancement >= rooms.Length)
        {
            advancement = newAdvancement;
            
            onEnd.Invoke();
            yield break;
        }
        
        var room = rooms[newAdvancement];
        room.Activate();
        
        ground.localScale = new Vector3(room.Sheet.Size.x * cellSize.x, 100, room.Sheet.Size.y * cellSize.y);
        ground.position = new Vector3(-cellSize.x * 0.5f,-100,-cellSize.y * 0.5f);

        onActivation.Invoke();
        advancement = newAdvancement;
    }
}