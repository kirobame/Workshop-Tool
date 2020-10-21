using System;

public static class Score
{
    public static event Action<int> OnModified;
    
    public static int Value { get; private set; }
    
    public static void ModifyBy(int value)
    {
        Value += value;
        OnModified?.Invoke(value);
    }
}