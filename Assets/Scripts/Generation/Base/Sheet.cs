using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class Sheet
{
    public bool IsInitialized => size != Vector2Int.zero;

    public string Name => name;
    public string Version => version;
    
    public Vector2Int Size => size;

    [SerializeField] private string name;
    [SerializeField] private string version;
    
    [SerializeField] private string[] array;
    [SerializeField] private Vector2Int size;

    public string this[Vector2Int index] => this[index.x, index.y];
    public string this[int x, int y]
    {
        get
        {
            var index = x + y * size.x;
            return array[index];
        }
    }
    
    public bool Process(string data)
    {
        var lines = Regex.Split(data, "\r\n|\r|\n");

        var firstLine = lines.First().Split(',');
        var width = firstLine.Length;

        var indicator = firstLine.First().Split('=');
        name = indicator[0];
        version = indicator[1];
        
        if (!lines.Any() ||lines.Length < 2 || lines.First().Length < 2) return false;

        size = new Vector2Int(width - 1, lines.Length - 1);
        array = new string[size.x * size.y];

        for (var y = 0; y < size.y; y++)
        {
            var items = lines[y + 1].Split(',');
            for (var x = 0; x < size.x; x++)
            {
                var index = x + y * size.x;
                array[index] = items[x + 1];
            }
        }
        
        return true;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        for (var y = 0; y < size.y; y++)
        {
            for (var x = 0; x < size.x; x++)
            {
                var item = this[x, y];

                var value = item == string.Empty ? "Empty" : item;
                var suffix = x == size.x - 1 ? ";" : ",";

                builder.Append($" {value} {suffix}");
            }
            builder.AppendLine();
        }

        return builder.ToString();
    }
}