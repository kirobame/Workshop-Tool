using UnityEngine;

public class PaletteChanger : MonoBehaviour
{
    private static readonly int Palette = Shader.PropertyToID("_Palette");
    
    [SerializeField] private Material target;
    [SerializeField] private Texture[] palettes;

    private int index = -1;
    
    public void Next()
    {
        if (index + 1 == palettes.Length) index = 0;
        else index++;
       
        target.SetTexture(Palette, palettes[index]);
    }
}