using System;

[Serializable]
public struct Null
{
    public static Null Default => instance;
    private static Null instance = new Null();
}