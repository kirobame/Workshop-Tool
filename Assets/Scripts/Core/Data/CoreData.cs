using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScorerData", menuName = "Custom/Generation/Data/Scorer")]
public class CoreData : CSVRecipient
{
    public event Action OnRuntimeSheetReady;
    
    public bool AreSheetsReady { get; private set; }
    public IReadOnlyList<RuntimeSheet> RuntimeSheets => registry.Values.ToArray();
    
    private Dictionary<string, RuntimeSheet> registry = new Dictionary<string, RuntimeSheet>();

    public RuntimeSheet this[string key] => registry[key];
    
    public void SetRuntimeSheets(RuntimeSheet[] runtimeSheets)
    {
        foreach (var runtimeSheet in runtimeSheets) registry.Add(runtimeSheet.Source.Name, runtimeSheet);
        
        OnRuntimeSheetReady?.Invoke();
        AreSheetsReady = true;
    }
}