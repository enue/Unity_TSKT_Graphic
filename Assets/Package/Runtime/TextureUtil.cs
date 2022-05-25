#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TSKT_GRAPHIC_SUPPORT_UNITASK
using Cysharp.Threading.Tasks;
#endif

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

        public static IEnumerator CaptureScreenshot(int width, int height, System.Action<Texture2D> callback)
        {
            // UniTaskのWaitForEndOfFrameはCaptureScreenshotAsTextureに非対応なのでコルーチンを使う
            yield return new WaitForEndOfFrame();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();

            if (texture.width != width || texture.height != height)
            {
                Resize(ref texture, width, height);
            }
            callback(texture);
        }

#if TSKT_GRAPHIC_SUPPORT_UNITASK
        public static UniTask<Texture2D> CaptureScreenshot(int width, int height, MonoBehaviour monoBehaviour)
        {
            var completion = new UniTaskCompletionSource<Texture2D>();
            monoBehaviour.StartCoroutine(CaptureScreenshot(width, height, _ =>
            {
                completion.TrySetResult(_);
            }));

            return completion.Task;
        }
#endif
    }
}
