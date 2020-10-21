using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenTag : MonoBehaviour
{
    public Token Token => token;
    [SerializeField] private Token token;
}
