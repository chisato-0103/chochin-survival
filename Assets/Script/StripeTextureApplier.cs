using UnityEngine;

public class StripeTextureApplier : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int stripeCount = 8;           // 縞の本数（片側色の数）
    public bool vertical = true;          // true=縦縞, false=横縞
    public Color colorA = Color.red;
    public Color colorB = Color.white;

    void Start()
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point; // くっきりさせたいならPoint、滑らかならBilinear

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float t = vertical ? (float)x / width : (float)y / height;
            int band = Mathf.FloorToInt(t * stripeCount * 2); // *2でA/B交互
            bool isA = (band % 2 == 0);
            tex.SetPixel(x, y, isA ? colorA : colorB);
        }

        tex.Apply();

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = tex;
        }
    }
}
