using UnityEngine;

public class PaletteChanger : MonoBehaviour
{
    private static readonly int Palette = Shader.PropertyToID("_Palette");
    
    [SerializeField] private Material target;
    [SerializeField] private Texture[] palletes;

    private int index = -1;
    
    public void Next()
    {
        index++;
        target.SetTexture(Palette, palletes[index]);
    }
}