using UnityEngine;

public class Room : MonoBehaviour
{
    public Sheet Sheet => sheet;
    
    [SerializeField] private Token playerToken;
    
    private Sheet sheet;
    private Vector3 spawn;

    public void Initialize(Sheet sheet) => this.sheet = sheet;
    public void Activate()
    {
        var player = Repository.GetFirst<Player>(playerToken);
        player.Place(spawn);
        
        gameObject.SetActive(true);
    }
    
    public void SetSpawn(Vector3 spawn) => this.spawn = spawn;
}