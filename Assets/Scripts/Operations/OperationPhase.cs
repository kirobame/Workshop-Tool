using System;

[Flags]
public enum OperationPhase
{
    None = 0,
    
    Beginning = 1,
    During = 2,
    End = 4
}