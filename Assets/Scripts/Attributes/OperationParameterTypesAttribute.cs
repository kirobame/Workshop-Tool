using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationParameterTypesAttribute : Attribute
{
    public OperationParameterTypesAttribute(params Type[] types) => this.types = types;

    public IReadOnlyList<Type> Types => types;
    private Type[] types;
}