using UnityEngine;

public class Decal : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private ParticleSystem effect;

    public void Place(RaycastHit hit)
    {
        var hittable = hit.transform.GetComponent<IHittable>();
        if (hittable != null && hittable.HasAlreadyBeenHit) return;
        
        target.SetParent(hit.transform);
        
        target.position = hit.point + hit.normal * Mathf.Epsilon;
        target.rotation = Quaternion.LookRotation(hit.normal);
        
        effect.Play();
    }
}