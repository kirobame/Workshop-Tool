using System;
using System.Collections.Generic;

public class OperationParameterNamesAttribute : Attribute
{
    public OperationParameterNamesAttribute(params string[] names) => this.names = names;

    public IReadOnlyList<string> Names => names;
    private string[] names;
}