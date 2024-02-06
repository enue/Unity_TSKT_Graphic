#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT
{
    public class CaptureScreenshotSample : MonoBehaviour
    {
        async Awaitable Start()
        {
            var texture = await TextureUtil.CaptureScreenshot(100, 100);
            var bytes = texture.EncodeToJPG();
            System.IO.File.WriteAllBytes("ss.jpg", bytes);
            A();
        }

        async void A()
        {
            var texture = await TextureUtil.CaptureScreenshot(100, 100);
            var bytes = texture.EncodeToJPG();
            System.IO.File.WriteAllBytes("ss2.jpg", bytes);
        }
    }
}
