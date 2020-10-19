using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    #region Encapsulated Types

    [Serializable]
    public struct MapLink
    {
        public MapLink(string name)
        {
            this.name = name;
            state = false;
        }

        public string Name => name;
        public bool State => state;
        
        [SerializeField] private string name;
        [SerializeField] private bool state;
    }
    #endregion
    
    [SerializeField] private InputActionAsset asset;
    [SerializeField] private MapLink[] links;

    void OnEnable()
    {
        asset.Enable();
        foreach (var link in links)
        {
            if (!link.State) asset.FindActionMap(link.Name).Disable();
            else asset.FindActionMap(link.Name).Enable();
        }
    }
    void OnDisable()
    {
        asset.Disable();
        foreach (var link in links) asset.FindActionMap(link.Name).Disable();
    }
}