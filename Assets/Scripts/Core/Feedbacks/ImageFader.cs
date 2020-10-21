using UnityEngine;
using UnityEngine.UI;

public class ImageFader : Fader
{
    [SerializeField] private Image image;

    protected override void Apply(float alpha)
    {
        var color = image.color;
        color.a = alpha;

        image.color = color;
    }
}