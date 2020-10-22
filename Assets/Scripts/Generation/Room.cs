using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Sheet Sheet => sheet;
    
    [SerializeField] private Token playerToken;
    
    private List<IResetable> resetables = new List<IResetable>();
    
    private Sheet sheet;
    private Vector3 spawn;

    public void Initialize(Sheet sheet) => this.sheet = sheet;
    public void Activate()
    {
        var player = Repository.GetFirst<Player>(playerToken);
        player.Place(spawn);
        
        gameObject.SetActive(true);

        foreach (var resetable in resetables) resetable.Reset();
    }

    public void AddResetable(IResetable resetable) => resetables.Add(resetable);
    public void SetSpawn(Vector3 spawn) => this.spawn = spawn;
}