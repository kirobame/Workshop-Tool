using System.Collections.Generic;
using System.Linq;
using System.Text;

// Runtime class allowing the processing of a data sheet into rows & columns of data.
public class RuntimeSheet
{
    public Sheet Source => source;

    public IReadOnlyDictionary<string, List<string>> Rows => rows;
    private Dictionary<string, List<string>> rows = new Dictionary<string, List<string>>();
    
    public IReadOnlyDictionary<string, List<string>> Columns => columns;
    private Dictionary<string, List<string>> columns = new Dictionary<string, List<string>>();

    private Sheet source;
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Allows access of data at specific Row/Column intersection. 
    public string this[string rowKey, string columnKey]
    {
        get
        {
            if (!rows.TryGetValue(rowKey, out var row) || !columns.TryGetValue(columnKey, out var column)) return string.Empty;
            return row.Find(rowItem => column.Any(columnItem => rowItem == columnItem));
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Identifies all rows & columns in the given sheet. 
    public void Process(Sheet sheet)
    {
        source = sheet;
        
        for (var x = 0; x < sheet.Size.x; x++)
        {
            for (var y = 0; y < sheet.Size.y; y++)
            {
                var item = sheet[x,y];
                if (item == string.Empty) continue;
                
                if (item[0] == 'Ⓒ')
                {
                    var column = new List<string>();
                    
                    for (var i = y + 1; i < sheet.Size.y; i++)
                    {
                        var subItem = sheet[x,i];
                        if (subItem.Contains('Ⓒ') || subItem == string.Empty) break;

                        subItem = subItem.Replace("Ⓡ", string.Empty);
                        subItem = subItem.Replace(" ", string.Empty);
                        column.Add(subItem);
                    }
                                        
                    item = item.Replace("Ⓒ", string.Empty);
                    item = item.Replace(" ", string.Empty);
                    columns.Add(item, column);
                }
                else if (item[0] == 'Ⓡ')
                {
                    var row = new List<string>();
 
                    for (var i = x + 1; i < sheet.Size.x; i++)
                    {
                        var subItem = sheet[i,y];
                        if (subItem.Contains('Ⓡ') || subItem == string.Empty) break;
                        
                        subItem = subItem.Replace("Ⓒ", string.Empty);
                        subItem = subItem.Replace(" ", string.Empty);
                        row.Add(subItem);
                    }
                    
                    item = item.Replace("Ⓡ", string.Empty);
                    item = item.Replace(" ", string.Empty);
                    rows.Add(item, row);
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public override string ToString()
    {
        var builder = new StringBuilder();
        
        builder.AppendLine("Columns :");
        foreach (var column in columns)
        {
            var value = $"{column.Key} :";
            for (var i = 0; i < column.Value.Count - 1; i++) value += $" {column.Value[i]} |";
            value += $" {column.Value.Last()};";

            builder.AppendLine(value);
        }

        builder.AppendLine();
        builder.AppendLine("Rows :");
        foreach (var row in rows)
        {
            var value = $"{row.Key} :";
            for (var i = 0; i < row.Value.Count - 1; i++) value += $" {row.Value[i]} |";
            value += $" {row.Value.Last()};";

            builder.AppendLine(value);
        }

        return builder.ToString();
    }
}