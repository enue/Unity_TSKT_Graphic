#nullable enable
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

            texture.Reinitialize(width, height);
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = null;
        }

        public static async Awaitable<Texture2D> CaptureScreenshot(int width, int height)
        {
            await Awaitable.EndOfFrameAsync();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();

            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
		        var newScreenShot = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
		        newScreenShot.SetPixels32(texture.GetPixels32());
		        newScreenShot.Apply();

                Object.Destroy(texture);
                texture = newScreenShot;
            }

            if (texture.width != width || texture.height != height)
            {
                Resize(ref texture, width, height);
            }

            return texture;
        }
    }
}
