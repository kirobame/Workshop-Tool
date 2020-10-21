using UnityEngine;

public class Helper : MonoBehaviour
{
    [SerializeField] private RoomCollection collection;

    void Awake() => StartCoroutine(collection.Download(Process));

    void Process(Sheet[] sheets)
    {
        var runtimeSheets = new RuntimeSheet[sheets.Length];
        for (var i = 0; i < sheets.Length; i++)
        {
            var runtimeSheet = new RuntimeSheet();
            runtimeSheet.Process(sheets[i]);

            runtimeSheets[i] = runtimeSheet;
        }

        Debug.Log(runtimeSheets[0]["Mary","Age"]);
        Debug.Log(runtimeSheets[1]["Klaus","Age"]);
    }
}