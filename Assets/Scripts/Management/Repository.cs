using System.Collections.Generic;
using UnityEngine;

public static class Repository
{
    private static Dictionary<Token, List<object>> registry = new Dictionary<Token, List<object>>();

    public static void Register(Token token, object value)
    {
        if (registry.TryGetValue(token, out var list)) list.Add(value);
        else registry.Add(token, new List<object>() {value});
    }
    public static void RegisterMany(Token token, IEnumerable<object> values)
    {
        if (registry.TryGetValue(token, out var list)) list.AddRange(values);
        else
        {
            var newList = new List<object>();
            foreach (var value in values) newList.Add(value);
            
            registry.Add(token, newList);
        }
    }
    
    public static void Unregister(Token token, object value)
    {
        if (!registry.TryGetValue(token, out var list)) return;
        list.Remove(value);
    }
    public static void UnregisterMany(Token token, IEnumerable<object> values)
    {
        if (!registry.TryGetValue(token, out var list)) return;
        foreach (var value in values) list.Remove(value);
    }

    public static T GetFirst<T>(Token token) => Get<T>(token, 0);
    public static T Get<T>(Token token, int index) => (T)registry[token][index];
}