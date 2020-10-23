using System.Collections.Generic;
using UnityEngine;

public class OperationHandler : MonoBehaviour
{
    [SerializeField] private InputLink[] links;
    
    private List<IUpdatable> updatables = new List<IUpdatable>();

    void Awake()
    {
        foreach (var link in links)
        {
            link.Initialize();
            foreach (var operation in link.Operations) operation.SetSource(this);
            
            if (link.Activator is IUpdatable updatable) updatables.Add(updatable);
        }
    }
    
    void OnEnable() { foreach (var link in links) link.Enable(); }
    void OnDisable() { foreach (var link in links) link.Disable(); }
    
    void Update() { foreach (var updatable in updatables) updatable.Update(); }
}