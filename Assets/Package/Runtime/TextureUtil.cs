using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    public static class TextureUtil
    {
        static public void Resize(ref Texture2D texture, int width, int height)
        {
            var rt = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(texture, rt);
            RenderTexture.active = rt;

            texture.Resize(width, height);
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = null;
        }
    }
}
