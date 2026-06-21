using UnityEngine;

public static class Stage1Visuals
{
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    public static void SetColor(Renderer renderer, Color color)
    {
        if (renderer == null)
        {
            return;
        }

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        block.SetColor(ColorId, color);
        block.SetColor(BaseColorId, color);
        renderer.SetPropertyBlock(block);
    }
}
