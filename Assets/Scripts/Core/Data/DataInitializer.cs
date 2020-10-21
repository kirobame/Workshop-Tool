using UnityEngine;

public class DataInitializer : MonoBehaviour
{
    [SerializeField] private CoreData[] datas;

    void Awake()
    {
        foreach (var data in datas) StartCoroutine(data.Download(sheets => Convert(data, sheets)));
    }

    private void Convert(CoreData data, Sheet[] sheets)
    {
        var runtimeSheets = new RuntimeSheet[sheets.Length];
        for (var i = 0; i < sheets.Length; i++)
        {
            var runtimeSheet = new RuntimeSheet();
            runtimeSheet.Process(sheets[i]);

            runtimeSheets[i] = runtimeSheet;
        }

        data.SetRuntimeSheets(runtimeSheets);
    }
}