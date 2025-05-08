#nullable enable
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    public static class TextureUtil
    {
        public static void Resize(ref Texture2D texture, int width, int height)
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

        public static Vector2 Shrink(float aspectWidth, float aspectHeight, float width, float height)
        {
            if (aspectHeight * width > aspectWidth * height)
            {
                return new Vector2(height * aspectWidth / aspectHeight, height);
            }
            else
            {
                return new Vector2(width, width * aspectHeight / aspectWidth);
            }
        }

        public static async Awaitable<string> CaptureScreenshotAsBase64String(int width, int height)
        {
            var size = Shrink(Screen.width, Screen.height, width, height);
            var texture = await CaptureScreenshot((int)size.x, (int)size.y);
            var result = ToBase64(texture);
            Object.Destroy(texture);
            return result;
        }

        public static string ToBase64(Texture2D source)
        {
            var jpg = source.EncodeToJPG();
            return ToBase64(jpg);
        }
        public static string ToBase64(byte[] bytes)
        {
            System.Span<byte> buffer = stackalloc byte[Base64.GetMaxEncodedToUtf8Length(bytes.Length)];
            Base64.EncodeToUtf8(bytes, buffer, out _, out var bytesWritten);
            return System.Text.Encoding.UTF8.GetString(buffer[..bytesWritten]);
        }

        public static int GetMaxByteCount(int base64StringLength)
        {
            var maxByteCount = System.Text.Encoding.UTF8.GetMaxByteCount(base64StringLength);
            return Base64.GetMaxDecodedFromUtf8Length(maxByteCount);
        }

        public static void FromBase64(string base64, System.Span<byte> dest, out int writtenCount)
        {
            System.Span<byte> utf8 = stackalloc byte[System.Text.Encoding.UTF8.GetMaxByteCount(base64.Length)];
            var utf8Length = System.Text.Encoding.UTF8.GetBytes(base64, utf8);
            Base64.DecodeFromUtf8(utf8[..utf8Length], dest, out _, out writtenCount);
        }

        public static void FromBase64(string base64, Texture2D dest)
        {
            System.Span<byte> bytes = stackalloc byte[GetMaxByteCount(base64.Length)];
            FromBase64(base64, bytes, out var writtenCount);
            dest.LoadImage(bytes[..writtenCount], markNonReadable: true);
        }
    }
}
