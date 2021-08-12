using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TSKT
{
    public class CaptureScreenshotSample : MonoBehaviour
    {
        IEnumerator Start()
        {
            Texture2D texture = null;
            yield return TextureUtil.CaptureScreenshot(100, 100, _ => texture = _);
            var bytes = texture.EncodeToJPG();
            System.IO.File.WriteAllBytes("ss.jpg", bytes);
            A();
        }

        async void A()
        {
            var texture = await TextureUtil.CaptureScreenshot(100, 100, this);
            var bytes = texture.EncodeToJPG();
            System.IO.File.WriteAllBytes("ss2.jpg", bytes);
        }
    }
}
