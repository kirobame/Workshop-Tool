using UnityEngine;

public class Decal : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private ParticleSystem effect;

    public void Place(RaycastHit hit)
    {
        target.SetParent(null);
        
        target.position = hit.point + hit.normal * Mathf.Epsilon;
        target.rotation = Quaternion.LookRotation(hit.normal);
        
        effect.Play();
    }
}